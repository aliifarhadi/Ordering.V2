using AeroTech.Ordering.Domain.Ordering.Aggregates;
using AeroTech.Ordering.Domain.SharedKernel.Enums;
using AeroTech.Ordering.Domain.SharedKernel.ValueObjects;
using AeroTech.Ordering.Persistence;
using AeroTech.Ordering.Seeder;
using AeroTech.Ordering.Seeder.Operations;
using AeroTech.Ordering.Seeder.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var connectionString = configuration["ConnectionString"]
    ?? configuration.GetConnectionString("CommandDbContext")
    ?? "Server=localhost\\SQLEXPRESS;Database=DotAirOrderNewV2;Trusted_Connection=True;TrustServerCertificate=True";

// The reference alphabet excludes ambiguous characters (no O, I, 0, 1).
var reference = new OrderReference(configuration["Reference"] ?? "THRMHDRT22");
var replace = configuration.GetValue("Replace", false);

var options = new DbContextOptionsBuilder<OrderingDbContext>()
    .UseSqlServer(
        connectionString,
        sql => sql.MigrationsHistoryTable(
            OrderingDbContext.MigrationsHistoryTable,
            OrderingDbContext.MigrationsHistorySchema))
    .Options;

await using var dbContext = new OrderingDbContext(
    options,
    new SeederIdentityService(),
    new SeederClock(),
    new NoOpDomainEventDispatcher());

Console.WriteLine($"Database   : {dbContext.Database.GetDbConnection().Database}");
Console.WriteLine($"Reference  : {reference.Value}");

var refundTicketNumber = configuration["Refund"];
if (!string.IsNullOrWhiteSpace(refundTicketNumber))
{
    var keepAncillaries = configuration.GetValue("KeepAncillaries", false);
    return await RefundAsync(dbContext, reference, refundTicketNumber, !keepAncillaries);
}

var changeTicketNumber = configuration["ChangeReturn"];
if (!string.IsNullOrWhiteSpace(changeTicketNumber))
{
    return await ChangeReturnAsync(
        dbContext,
        reference,
        changeTicketNumber,
        configuration.GetValue("DaysLater", 1),
        configuration.GetValue("Penalty", 5_000_000m),
        configuration.GetValue("FareDifference", 2_800_000m));
}

var existing = await dbContext.Orders
    .FirstOrDefaultAsync(order => order.Reference == reference);

if (existing is not null)
{
    if (!replace)
    {
        Console.WriteLine($"Order {reference.Value} already exists ({existing.Id}). Nothing to do.");
        Console.WriteLine("Pass --Replace true to delete and recreate it.");
        return 0;
    }

    Console.WriteLine($"Replacing existing order {existing.Id}...");
    await RemoveExistingAsync(dbContext, existing);
}

var scenario = new ThrMhdRoundTripScenario(SystemClock.Instance.GetCurrentInstant(), new SequentialEventIdFactory());
var result = scenario.Build(reference);

dbContext.Orders.Add(result.Order);
dbContext.ElectronicTickets.AddRange(result.Tickets);
dbContext.ElectronicMiscDocuments.AddRange(result.Documents);

await dbContext.SaveChangesAsync();

Report(result);
return 0;

static async Task<int> ChangeReturnAsync(
    OrderingDbContext dbContext,
    OrderReference reference,
    string ticketNumber,
    int daysLater,
    decimal penalty,
    decimal fareDifference)
{
    var order = await LoadOrderAsync(dbContext, reference);
    if (order is null)
    {
        Console.Error.WriteLine($"Order {reference.Value} was not found.");
        return 1;
    }

    var ticket = await dbContext.ElectronicTickets
        .Include(candidate => candidate.Coupons)
        .FirstOrDefaultAsync(candidate => candidate.TicketNumber == ticketNumber && candidate.OrderId == order.Id);

    if (ticket is null)
    {
        Console.Error.WriteLine($"Ticket {ticketNumber} was not found on order {reference.Value}.");
        return 1;
    }

    // Every document held by this traveller: after an earlier reissue the live return coupon is
    // on a replacement ticket, not the one named on the command line.
    var travelerTickets = await dbContext.ElectronicTickets
        .Include(candidate => candidate.Coupons)
        .Where(candidate => candidate.OrderId == order.Id && candidate.TravelerId == ticket.TravelerId)
        .ToListAsync();

    var traveler = order.Travelers.FirstOrDefault(candidate => candidate.Id == ticket.TravelerId);
    var currency = order.CreatedSalesContext.SaleCurrency;

    Console.WriteLine($"Changing   : {ticket.TicketNumber} ({traveler?.Name.ToString() ?? ticket.TravelerId.ToString()})");
    Console.WriteLine($"Shift      : return leg +{daysLater} day(s)");
    Console.WriteLine();

    var operation = new ChangeReturnDateOperation(
        SystemClock.Instance.GetCurrentInstant(),
        new SequentialEventIdFactory());

    ChangeResult result;
    try
    {
        result = operation.Execute(
            order,
            ticket,
            travelerTickets,
            daysLater,
            new Money(penalty, currency),
            new Money(fareDifference, currency));
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Change rejected: {exception.Message}");
        return 1;
    }

    // The reissued ticket is a new aggregate: it must be registered with the context explicitly.
    dbContext.ElectronicTickets.Add(result.NewTicket);

    await dbContext.SaveChangesAsync();

    Console.WriteLine($"Segment    W5-{result.OldSegment.FlightNumber} "
        + $"{result.OldSegment.Origin}-{result.OldSegment.Destination}");
    Console.WriteLine($"  was      {result.OldSegment.DepartureLocalDate} {result.OldSegment.DepartureLocalTime}");
    Console.WriteLine($"  now      {result.NewSegment.DepartureLocalDate} {result.NewSegment.DepartureLocalTime}");
    Console.WriteLine();
    Console.WriteLine($"Item       original -> {result.OriginalItem.Status}");
    Console.WriteLine($"           new      -> {result.NewItem.Status}  ({result.NewItem.Price.Total})");
    Console.WriteLine();
    Console.WriteLine($"Ticket     {result.OriginalTicket.TicketNumber} -> {result.OriginalTicket.Status}");

    foreach (var coupon in result.OriginalTicket.Coupons.OrderBy(coupon => coupon.CouponNumber))
        Console.WriteLine($"  coupon {coupon.CouponNumber}  {coupon.Status}");

    Console.WriteLine($"           {result.NewTicket.TicketNumber} -> {result.NewTicket.Status} (reissue)");
    Console.WriteLine();
    Console.WriteLine($"Collected  {result.AmountCollected}");
    Console.WriteLine($"Order      {order.Lifecycle}, version {order.AggregateVersion}");
    Console.WriteLine();
    Console.WriteLine("Reissue recorded. Collection of the change amount is Receivables' responsibility.");

    return 0;
}

static Task<Order?> LoadOrderAsync(OrderingDbContext dbContext, OrderReference reference) =>
    dbContext.Orders
        .Include(candidate => candidate.Items).ThenInclude(item => item.Entitlements)
        .Include(candidate => candidate.Items).ThenInclude(item => item.Beneficiaries)
        .Include(candidate => candidate.ProcessingLocks)
        .Include(candidate => candidate.TimeLimits)
        .Include(candidate => candidate.Travelers)
        .Include(candidate => candidate.Journeys).ThenInclude(journey => journey.Segments)
        .FirstOrDefaultAsync(candidate => candidate.Reference == reference);

static async Task<int> RefundAsync(
    OrderingDbContext dbContext,
    OrderReference reference,
    string ticketNumber,
    bool includeAncillaries)
{
    var order = await dbContext.Orders
        .Include(candidate => candidate.Items).ThenInclude(item => item.Entitlements)
        .Include(candidate => candidate.ProcessingLocks)
        .Include(candidate => candidate.TimeLimits)
        .Include(candidate => candidate.Travelers)
        .Include(candidate => candidate.Journeys).ThenInclude(journey => journey.Segments)
        .FirstOrDefaultAsync(candidate => candidate.Reference == reference);

    if (order is null)
    {
        Console.Error.WriteLine($"Order {reference.Value} was not found.");
        return 1;
    }

    var ticket = await dbContext.ElectronicTickets
        .Include(candidate => candidate.Coupons)
        .FirstOrDefaultAsync(candidate => candidate.TicketNumber == ticketNumber && candidate.OrderId == order.Id);

    if (ticket is null)
    {
        Console.Error.WriteLine($"Ticket {ticketNumber} was not found on order {reference.Value}.");
        return 1;
    }

    // The EMD carrying this traveller's ancillaries, if one was issued.
    var document = await dbContext.ElectronicMiscDocuments
        .Include(candidate => candidate.Coupons)
        .FirstOrDefaultAsync(candidate =>
            candidate.OrderId == order.Id && candidate.TravelerId == ticket.TravelerId);

    var traveler = order.Travelers.FirstOrDefault(candidate => candidate.Id == ticket.TravelerId);

    Console.WriteLine($"Refunding  : {ticket.TicketNumber} ({traveler?.Name.ToString() ?? ticket.TravelerId.ToString()})");
    Console.WriteLine($"Ancillaries: {(includeAncillaries && document is not null ? document.EmdNumber : "not included")}");
    Console.WriteLine();

    var operation = new RefundTicketOperation(
        SystemClock.Instance.GetCurrentInstant(),
        new SequentialEventIdFactory());

    RefundResult result;
    try
    {
        result = operation.Execute(order, ticket, document, includeAncillaries);
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Refund rejected: {exception.Message}");
        return 1;
    }

    await dbContext.SaveChangesAsync();

    Console.WriteLine($"Ticket     {result.Ticket.TicketNumber}  ->  {result.Ticket.Status}");
    foreach (var coupon in result.Ticket.Coupons.OrderBy(coupon => coupon.CouponNumber))
        Console.WriteLine($"  coupon {coupon.CouponNumber}  ->  {coupon.Status}");

    if (result.Document is not null)
    {
        Console.WriteLine($"EMD        {result.Document.EmdNumber}  ->  {result.Document.Status}");
        foreach (var coupon in result.Document.Coupons.OrderBy(coupon => coupon.CouponNumber))
            Console.WriteLine($"  coupon {coupon.CouponNumber}  ->  {coupon.Status}");
    }

    Console.WriteLine();
    Console.WriteLine($"Cancelled  {result.CancelledEntitlementIds.Count} entitlements");
    foreach (var item in result.AffectedItems)
        Console.WriteLine($"  {item.Product.ProductType,-14} -> {item.Status}");

    Console.WriteLine();
    Console.WriteLine($"Order      {order.Lifecycle}, version {order.AggregateVersion}");
    Console.WriteLine($"Remaining active entitlements: {order.AllEntitlements.Count(entitlement => entitlement.IsActive)}");
    Console.WriteLine();
    Console.WriteLine("Refund recorded. Financial credit is Receivables' responsibility and was not written here.");

    return 0;
}

static async Task RemoveExistingAsync(OrderingDbContext dbContext, Order existing)
{
    var orderId = existing.Id;

    var tickets = await dbContext.ElectronicTickets.Where(ticket => ticket.OrderId == orderId).ToListAsync();
    var documents = await dbContext.ElectronicMiscDocuments.Where(document => document.OrderId == orderId).ToListAsync();
    var reservations = await dbContext.SupplierReservations.Where(reservation => reservation.OrderId == orderId).ToListAsync();

    dbContext.ElectronicTickets.RemoveRange(tickets);
    dbContext.ElectronicMiscDocuments.RemoveRange(documents);
    dbContext.SupplierReservations.RemoveRange(reservations);

    var full = await dbContext.Orders
        .Include(order => order.Items).ThenInclude(item => item.Entitlements)
        .Include(order => order.Journeys)
        .Include(order => order.Travelers)
        .Include(order => order.Contacts)
        .Include(order => order.TimeLimits)
        .Include(order => order.ProcessingLocks)
        .Include(order => order.ExternalReferences)
        .Include(order => order.Delegations)
        .FirstAsync(order => order.Id == orderId);

    dbContext.Orders.Remove(full);
    await dbContext.SaveChangesAsync();
}

static void Report(ScenarioResult result)
{
    var order = result.Order;

    Console.WriteLine();
    Console.WriteLine($"Order        : {order.Id}");
    Console.WriteLine($"Lifecycle    : {order.Lifecycle}");
    Console.WriteLine($"Version      : {order.AggregateVersion}");
    Console.WriteLine($"Travelers    : {order.Travelers.Count}");
    Console.WriteLine($"Journeys     : {order.Journeys.Count} ({order.AllSegments.Count()} segments)");
    Console.WriteLine($"Items        : {order.Items.Count}");
    Console.WriteLine($"Entitlements : {order.AllEntitlements.Count()} ({order.AllEntitlements.Count(entitlement => entitlement.IsActive)} active)");
    Console.WriteLine();

    foreach (var traveler in order.Travelers)
        Console.WriteLine($"  Traveler   {traveler.Name} ({traveler.Type})");

    Console.WriteLine();
    foreach (var segment in order.AllSegments.OrderBy(segment => segment.DepartureUtc))
    {
        Console.WriteLine(
            $"  Segment    {segment.MarketingCarrier}-{segment.FlightNumber} "
            + $"{segment.Origin}->{segment.Destination} {segment.DepartureLocalDate}");
    }

    Console.WriteLine();
    foreach (var group in order.Items.GroupBy(item => item.Product.ProductType))
    {
        var total = group.Sum(item => item.Price.TotalAmount);
        Console.WriteLine($"  {group.Key,-14} x{group.Count()}  {total:N0} {group.First().Price.Currency}");
    }

    var grandTotal = order.Items.Sum(item => item.Price.TotalAmount);
    var currency = order.Items.First().Price.Currency;
    Console.WriteLine($"  {"TOTAL",-14}      {grandTotal:N0} {currency}");

    Console.WriteLine();
    foreach (var ticket in result.Tickets)
        Console.WriteLine($"  Ticket     {ticket.TicketNumber}  {ticket.Status}  coupons={ticket.Coupons.Count}");

    foreach (var document in result.Documents)
        Console.WriteLine($"  EMD        {document.EmdNumber}  {document.Status}  coupons={document.Coupons.Count}");

    var pnr = order.ExternalReferences.FirstOrDefault(reference => reference.Type == "PNR");
    if (pnr is not null)
        Console.WriteLine($"  PNR        {pnr.Value}");

    Console.WriteLine();
    Console.WriteLine("Seeded successfully.");
}

internal sealed class SequentialEventIdFactory : IEventIdFactory
{
    private long _sequence;

    public string Next() => $"seed-{Interlocked.Increment(ref _sequence):0000}";
}
