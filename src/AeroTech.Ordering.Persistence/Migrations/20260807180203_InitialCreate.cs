using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.Ordering.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.EnsureSchema(
                name: "consumption");

            migrationBuilder.EnsureSchema(
                name: "fulfillment");

            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "ConsumptionFacts",
                schema: "consumption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSystem = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    SourceEventId = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FulfillmentUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FactType = table.Column<string>(type: "varchar(64)", nullable: false),
                    QuantityValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    QuantityUnit = table.Column<string>(type: "varchar(16)", maxLength: 256, nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumptionFacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentStocks",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerCarrier = table.Column<string>(type: "varchar(3)", nullable: false),
                    OfficeRef = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    DocumentType = table.Column<string>(type: "varchar(16)", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(8)", maxLength: 256, nullable: true),
                    RangeStart = table.Column<long>(type: "bigint", nullable: false),
                    RangeEnd = table.Column<long>(type: "bigint", nullable: false),
                    NextAvailable = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStocks", x => x.Id);
                    table.CheckConstraint("CK_DocumentStocks_Range", "[RangeEnd] >= [RangeStart]");
                });

            migrationBuilder.CreateTable(
                name: "ElectronicMiscDocuments",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmdNumber = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(16)", nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicMiscDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicTickets",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketNumber = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssuingCarrier = table.Column<string>(type: "varchar(3)", nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "integration",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Consumer = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReceivedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => new { x.MessageId, x.Consumer });
                });

            migrationBuilder.CreateTable(
                name: "OrderHistory",
                schema: "ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNo = table.Column<long>(type: "bigint", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CommandName = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    ActorType = table.Column<string>(type: "varchar(32)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    SellerId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    SellerOfficeId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    Channel = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    CausationId = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ChangeSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ChangeJson = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistory", x => new { x.OrderId, x.SequenceNo });
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "varchar(16)", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    CreatedSellerId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    CreatedSellerOfficeId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    CreatedChannelCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    CreatedPointOfSaleCountry = table.Column<string>(type: "char(2)", nullable: false),
                    CreatedSaleCurrency = table.Column<string>(type: "char(3)", nullable: false),
                    CreatedSoldAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedActorType = table.Column<string>(type: "varchar(32)", nullable: false),
                    CreatedUserId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    BuyerPartyType = table.Column<string>(type: "varchar(32)", nullable: true),
                    BuyerPartyRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    BuyerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BuyerEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    BuyerPhone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BuyerTaxIdentity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AuthorityOwnerSellerId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    AuthorityOwnerOfficeId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    AirlineOverrideAllowed = table.Column<bool>(type: "bit", nullable: false),
                    RootOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SplitFromOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SplitWorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderSnapshots",
                schema: "ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    CapturedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSnapshots", x => new { x.OrderId, x.AggregateVersion });
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "integration",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationCases",
                schema: "consumption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "varchar(40)", nullable: false),
                    Status = table.Column<string>(type: "varchar(32)", nullable: false),
                    ExpectedValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    ObservedValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    Difference = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    ValueUnit = table.Column<string>(type: "varchar(16)", maxLength: 256, nullable: true),
                    Classification = table.Column<string>(type: "varchar(40)", nullable: true),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OpenedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierReservations",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    ProductType = table.Column<string>(type: "varchar(32)", nullable: false),
                    ExternalConfirmationNumber = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmdCoupons",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElectronicMiscDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponNumber = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmdCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmdCoupons_ElectronicMiscDocuments_ElectronicMiscDocumentId",
                        column: x => x.ElectronicMiscDocumentId,
                        principalSchema: "fulfillment",
                        principalTable: "ElectronicMiscDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCoupons",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElectronicTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponNumber = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    ControlHolder = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    ControlAcquiredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCoupons_ElectronicTickets_ElectronicTicketId",
                        column: x => x.ElectronicTicketId,
                        principalSchema: "fulfillment",
                        principalTable: "ElectronicTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactType = table.Column<string>(type: "varchar(16)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Role = table.Column<string>(type: "varchar(32)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalReferences",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<string>(type: "varchar(24)", nullable: false),
                    ScopedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    System = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    Owner = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalReferences_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Journeys",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Origin = table.Column<string>(type: "char(3)", nullable: false),
                    Destination = table.Column<string>(type: "char(3)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journeys_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ProductCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    ProductType = table.Column<string>(type: "varchar(32)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CatalogVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    PriceTotal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    PriceCurrency = table.Column<string>(type: "char(3)", nullable: false),
                    PricedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    PricingEngineVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    RoundingPolicyVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    FxFrom = table.Column<string>(type: "char(3)", nullable: true),
                    FxTo = table.Column<string>(type: "char(3)", nullable: true),
                    FxRate = table.Column<decimal>(type: "decimal(19,8)", precision: 19, scale: 4, nullable: true),
                    FxSource = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    FxCapturedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TermsVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    Refundability = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    Changeability = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    NoShowPolicyCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    ValidFromUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ValidUntilUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    SourceRuleRefs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommercialSourceType = table.Column<string>(type: "varchar(40)", nullable: false),
                    CommercialSourceReference = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    CommercialSourceVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    CommercialSourceAcceptedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ItemSellerId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    ItemSellerOfficeId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    ItemChannelCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    ItemPointOfSaleCountry = table.Column<string>(type: "char(2)", nullable: false),
                    ItemSaleCurrency = table.Column<string>(type: "char(3)", nullable: false),
                    ItemSoldAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ItemActorType = table.Column<string>(type: "varchar(32)", nullable: false),
                    ItemUserId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    SettlementModel = table.Column<string>(type: "varchar(40)", nullable: false),
                    SettlementDebtorPartyRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    SettlementAgreementRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    SettlementCollectionRequired = table.Column<bool>(type: "bit", nullable: false),
                    SettlementReceivableRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "varchar(24)", nullable: false),
                    LineageChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ReplacedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingLocks",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(32)", nullable: false),
                    AcquiredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingLocks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicingDelegations",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DelegatePartyRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    Scope = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    ValidFromUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ValidUntilUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    AuthorityTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicingDelegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicingDelegations_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLimits",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(32)", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    PolicyRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ExtensionCount = table.Column<int>(type: "int", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeLimits_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Travelers",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    TravelerType = table.Column<string>(type: "varchar(8)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "varchar(16)", maxLength: 256, nullable: true),
                    CustomerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    RedressNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    KnownTravelerNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ResidenceCountry = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Travelers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FulfillmentUnits",
                schema: "fulfillment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    UnitType = table.Column<string>(type: "varchar(32)", nullable: false),
                    Status = table.Column<string>(type: "varchar(16)", nullable: false),
                    ExternalReference = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    SupplierReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FulfillmentUnits_SupplierReservations_SupplierReservationId",
                        column: x => x.SupplierReservationId,
                        principalSchema: "fulfillment",
                        principalTable: "SupplierReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactTravelers",
                schema: "ordering",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTravelers", x => new { x.ContactId, x.TravelerId });
                    table.ForeignKey(
                        name: "FK_ContactTravelers_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "ordering",
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneySegments",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JourneyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketingCarrier = table.Column<string>(type: "varchar(3)", nullable: false),
                    OperatingCarrier = table.Column<string>(type: "varchar(3)", nullable: false),
                    FlightNumber = table.Column<string>(type: "varchar(8)", maxLength: 256, nullable: false),
                    Origin = table.Column<string>(type: "char(3)", nullable: false),
                    Destination = table.Column<string>(type: "char(3)", nullable: false),
                    DepartureUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ArrivalUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    DepartureLocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartureLocalTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    OriginTimeZoneId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    ArrivalLocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ArrivalLocalTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    DestinationTimeZoneId = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    AircraftType = table.Column<string>(type: "varchar(16)", maxLength: 256, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneySegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JourneySegments_Journeys_JourneyId",
                        column: x => x.JourneyId,
                        principalSchema: "ordering",
                        principalTable: "Journeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeLines",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ChargeType = table.Column<string>(type: "varchar(32)", nullable: false),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", nullable: false),
                    Refundable = table.Column<bool>(type: "bit", nullable: false),
                    TaxJurisdiction = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargeLines_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialTermRestrictions",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestrictionCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialTermRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommercialTermRestrictions_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entitlements",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "varchar(40)", nullable: false),
                    Status = table.Column<string>(type: "varchar(24)", nullable: false),
                    JourneyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CapacityType = table.Column<string>(type: "varchar(40)", nullable: true),
                    CapacityReference = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    RetailerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    SupplierRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    FulfillmentOwnerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ServicingOwnerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    RefundOwnerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    DisruptionOwnerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    SettlementOwnerRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ExternalProductRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ExternalOrderRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    ExternalServiceRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ReplacedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entitlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entitlements_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLineagePredecessors",
                schema: "ordering",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredecessorOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLineagePredecessors", x => new { x.OrderItemId, x.PredecessorOrderItemId });
                    table.ForeignKey(
                        name: "FK_ItemLineagePredecessors_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemBeneficiaries",
                schema: "ordering",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemBeneficiaries", x => new { x.OrderItemId, x.TravelerId });
                    table.ForeignKey(
                        name: "FK_OrderItemBeneficiaries_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                schema: "ordering",
                columns: table => new
                {
                    AttributeKey = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => new { x.OrderItemId, x.AttributeKey });
                    table.ForeignKey(
                        name: "FK_ProductAttributes_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValueAllocations",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", nullable: false),
                    AllocationVersion = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    Purpose = table.Column<string>(type: "varchar(32)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValueAllocations_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "ordering",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingLockEntitlements",
                schema: "ordering",
                columns: table => new
                {
                    ProcessingLockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingLockEntitlements", x => new { x.ProcessingLockId, x.EntitlementId });
                    table.ForeignKey(
                        name: "FK_ProcessingLockEntitlements_ProcessingLocks_ProcessingLockId",
                        column: x => x.ProcessingLockId,
                        principalSchema: "ordering",
                        principalTable: "ProcessingLocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingLockItems",
                schema: "ordering",
                columns: table => new
                {
                    ProcessingLockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingLockItems", x => new { x.ProcessingLockId, x.OrderItemId });
                    table.ForeignKey(
                        name: "FK_ProcessingLockItems_ProcessingLocks_ProcessingLockId",
                        column: x => x.ProcessingLockId,
                        principalSchema: "ordering",
                        principalTable: "ProcessingLocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLimitEntitlements",
                schema: "ordering",
                columns: table => new
                {
                    TimeLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLimitEntitlements", x => new { x.TimeLimitId, x.EntitlementId });
                    table.ForeignKey(
                        name: "FK_TimeLimitEntitlements_TimeLimits_TimeLimitId",
                        column: x => x.TimeLimitId,
                        principalSchema: "ordering",
                        principalTable: "TimeLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLimitItems",
                schema: "ordering",
                columns: table => new
                {
                    TimeLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLimitItems", x => new { x.TimeLimitId, x.OrderItemId });
                    table.ForeignKey(
                        name: "FK_TimeLimitItems_TimeLimits_TimeLimitId",
                        column: x => x.TimeLimitId,
                        principalSchema: "ordering",
                        principalTable: "TimeLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLimitTravelers",
                schema: "ordering",
                columns: table => new
                {
                    TimeLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLimitTravelers", x => new { x.TimeLimitId, x.TravelerId });
                    table.ForeignKey(
                        name: "FK_TimeLimitTravelers_TimeLimits_TimeLimitId",
                        column: x => x.TimeLimitId,
                        principalSchema: "ordering",
                        principalTable: "TimeLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerAssociations",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedTravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociationType = table.Column<string>(type: "varchar(32)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerAssociations_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalSchema: "ordering",
                        principalTable: "Travelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerIdentityDocuments",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "varchar(32)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IssuingCountry = table.Column<string>(type: "char(2)", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "char(2)", nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerIdentityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerIdentityDocuments_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalSchema: "ordering",
                        principalTable: "Travelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerLoyaltyAccounts",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    AccountRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    TierCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerLoyaltyAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerLoyaltyAccounts_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalSchema: "ordering",
                        principalTable: "Travelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitlementBeneficiaries",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitlementBeneficiaries", x => new { x.EntitlementId, x.TravelerId });
                    table.ForeignKey(
                        name: "FK_EntitlementBeneficiaries_Entitlements_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "Entitlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitlementSegments",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JourneySegmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitlementSegments", x => new { x.EntitlementId, x.JourneySegmentId });
                    table.ForeignKey(
                        name: "FK_EntitlementSegments_Entitlements_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "Entitlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitlementSpecifications",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitlementSpecifications", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_EntitlementSpecifications_Entitlements_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "Entitlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FulfillmentLinks",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FulfillmentType = table.Column<string>(type: "varchar(32)", nullable: false),
                    FulfillmentAggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FulfillmentUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalReference = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: true),
                    LinkedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FulfillmentLinks_Entitlements_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "Entitlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyRef = table.Column<string>(type: "varchar(128)", maxLength: 256, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    BoardBasis = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OccupancyAdults = table.Column<int>(type: "int", nullable: false),
                    OccupancyChildren = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckOutDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RatePlanCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true),
                    CancellationPolicyText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationSpecs", x => x.EntitlementId);
                    table.CheckConstraint("CK_AccommodationSpecs_DateRange", "[CheckOutDate] > [CheckInDate]");
                    table.CheckConstraint("CK_AccommodationSpecs_Occupancy", "[OccupancyAdults] > 0 AND [OccupancyChildren] >= 0");
                    table.ForeignKey(
                        name: "FK_AccommodationSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirTransportSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cabin = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: false),
                    Rbd = table.Column<string>(type: "varchar(4)", maxLength: 256, nullable: true),
                    BrandCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true),
                    FareBasisCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirTransportSpecs", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_AirTransportSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaggageSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowanceKind = table.Column<string>(type: "varchar(16)", nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(9,3)", precision: 19, scale: 4, nullable: true),
                    Pieces = table.Column<int>(type: "int", nullable: true),
                    MaxPieceWeightKg = table.Column<decimal>(type: "decimal(9,3)", precision: 19, scale: 4, nullable: true),
                    DimensionRuleCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageSpecs", x => x.EntitlementId);
                    table.CheckConstraint("CK_BaggageSpecs_Allowance", "([AllowanceKind] = 'Weight' AND [WeightKg] IS NOT NULL AND [WeightKg] > 0) OR ([AllowanceKind] = 'Piece' AND [Pieces] IS NOT NULL AND [Pieces] > 0)");
                    table.ForeignKey(
                        name: "FK_BaggageSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyProductCode = table.Column<string>(type: "varchar(64)", maxLength: 256, nullable: false),
                    CoverageStart = table.Column<DateOnly>(type: "date", nullable: false),
                    CoverageEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    CoverageSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceSpecs", x => x.EntitlementId);
                    table.CheckConstraint("CK_InsuranceSpecs_CoverageRange", "[CoverageEnd] >= [CoverageStart]");
                    table.ForeignKey(
                        name: "FK_InsuranceSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoungeSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoungeCode = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true),
                    AccessCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoungeSpecs", x => x.EntitlementId);
                    table.CheckConstraint("CK_LoungeSpecs_AccessCount", "[AccessCount] > 0");
                    table.ForeignKey(
                        name: "FK_LoungeSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealCode = table.Column<string>(type: "varchar(16)", maxLength: 256, nullable: false),
                    DietaryAttributes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSpecs", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_MealSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeatSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeatNumber = table.Column<string>(type: "varchar(8)", maxLength: 256, nullable: true),
                    Characteristics = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatSpecs", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_SeatSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferSpecs",
                schema: "ordering",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginLocation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DestinationLocation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VehicleClass = table.Column<string>(type: "varchar(32)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferSpecs", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_TransferSpecs_EntitlementSpecifications_EntitlementId",
                        column: x => x.EntitlementId,
                        principalSchema: "ordering",
                        principalTable: "EntitlementSpecifications",
                        principalColumn: "EntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeLines_OrderItemId_Sequence",
                schema: "ordering",
                table: "ChargeLines",
                columns: new[] { "OrderItemId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialTermRestrictions_OrderItemId",
                schema: "ordering",
                table: "CommercialTermRestrictions",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionFacts_EntitlementId",
                schema: "consumption",
                table: "ConsumptionFacts",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionFacts_JourneySegmentId",
                schema: "consumption",
                table: "ConsumptionFacts",
                column: "JourneySegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionFacts_OccurredAtUtc",
                schema: "consumption",
                table: "ConsumptionFacts",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionFacts_OrderId",
                schema: "consumption",
                table: "ConsumptionFacts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionFacts_SourceSystem_SourceEventId",
                schema: "consumption",
                table: "ConsumptionFacts",
                columns: new[] { "SourceSystem", "SourceEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_OrderId",
                schema: "ordering",
                table: "Contacts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStocks_OwnerCarrier_DocumentType_Status",
                schema: "fulfillment",
                table: "DocumentStocks",
                columns: new[] { "OwnerCarrier", "DocumentType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicMiscDocuments_EmdNumber",
                schema: "fulfillment",
                table: "ElectronicMiscDocuments",
                column: "EmdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicMiscDocuments_OrderId",
                schema: "fulfillment",
                table: "ElectronicMiscDocuments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicMiscDocuments_TravelerId",
                schema: "fulfillment",
                table: "ElectronicMiscDocuments",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicTickets_OrderId",
                schema: "fulfillment",
                table: "ElectronicTickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicTickets_TicketNumber",
                schema: "fulfillment",
                table: "ElectronicTickets",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicTickets_TravelerId",
                schema: "fulfillment",
                table: "ElectronicTickets",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmdCoupons_ElectronicMiscDocumentId_CouponNumber",
                schema: "fulfillment",
                table: "EmdCoupons",
                columns: new[] { "ElectronicMiscDocumentId", "CouponNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmdCoupons_EntitlementId",
                schema: "fulfillment",
                table: "EmdCoupons",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlements_OrderId",
                schema: "ordering",
                table: "Entitlements",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Entitlements_OrderItemId",
                schema: "ordering",
                table: "Entitlements",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_OrderId",
                schema: "ordering",
                table: "ExternalReferences",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_System_Type_Value",
                schema: "ordering",
                table: "ExternalReferences",
                columns: new[] { "System", "Type", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLinks_EntitlementId",
                schema: "ordering",
                table: "FulfillmentLinks",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLinks_FulfillmentAggregateId",
                schema: "ordering",
                table: "FulfillmentLinks",
                column: "FulfillmentAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentUnits_EntitlementId",
                schema: "fulfillment",
                table: "FulfillmentUnits",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentUnits_OrderItemId",
                schema: "fulfillment",
                table: "FulfillmentUnits",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentUnits_SupplierReservationId",
                schema: "fulfillment",
                table: "FulfillmentUnits",
                column: "SupplierReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ReceivedOn",
                schema: "integration",
                table: "InboxMessages",
                column: "ReceivedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_OrderId",
                schema: "ordering",
                table: "Journeys",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_JourneySegments_JourneyId_Sequence",
                schema: "ordering",
                table: "JourneySegments",
                columns: new[] { "JourneyId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JourneySegments_MarketingCarrier_FlightNumber_DepartureUtc",
                schema: "ordering",
                table: "JourneySegments",
                columns: new[] { "MarketingCarrier", "FlightNumber", "DepartureUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_JourneySegments_OrderId",
                schema: "ordering",
                table: "JourneySegments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistory_CorrelationId",
                schema: "ordering",
                table: "OrderHistory",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistory_OccurredAtUtc",
                schema: "ordering",
                table: "OrderHistory",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CommercialSourceReference",
                schema: "ordering",
                table: "OrderItems",
                column: "CommercialSourceReference");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemSellerId",
                schema: "ordering",
                table: "OrderItems",
                column: "ItemSellerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                schema: "ordering",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductType",
                schema: "ordering",
                table: "OrderItems",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedSellerId",
                schema: "ordering",
                table: "Orders",
                column: "CreatedSellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Reference",
                schema: "ordering",
                table: "Orders",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RootOrderId",
                schema: "ordering",
                table: "Orders",
                column: "RootOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn",
                schema: "integration",
                table: "OutboxMessages",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLocks_ExpiresAtUtc",
                schema: "ordering",
                table: "ProcessingLocks",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLocks_OrderId",
                schema: "ordering",
                table: "ProcessingLocks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLocks_WorkflowInstanceId",
                schema: "ordering",
                table: "ProcessingLocks",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationCases_OrderId",
                schema: "consumption",
                table: "ReconciliationCases",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationCases_Status_OpenedAtUtc",
                schema: "consumption",
                table: "ReconciliationCases",
                columns: new[] { "Status", "OpenedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ServicingDelegations_OrderId",
                schema: "ordering",
                table: "ServicingDelegations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReservations_ExternalConfirmationNumber",
                schema: "fulfillment",
                table: "SupplierReservations",
                column: "ExternalConfirmationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReservations_OrderId",
                schema: "fulfillment",
                table: "SupplierReservations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierReservations_SupplierId",
                schema: "fulfillment",
                table: "SupplierReservations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCoupons_ElectronicTicketId_CouponNumber",
                schema: "fulfillment",
                table: "TicketCoupons",
                columns: new[] { "ElectronicTicketId", "CouponNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCoupons_EntitlementId",
                schema: "fulfillment",
                table: "TicketCoupons",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCoupons_JourneySegmentId",
                schema: "fulfillment",
                table: "TicketCoupons",
                column: "JourneySegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLimits_OrderId",
                schema: "ordering",
                table: "TimeLimits",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLimits_Status_DueAtUtc",
                schema: "ordering",
                table: "TimeLimits",
                columns: new[] { "Status", "DueAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TravelerAssociations_TravelerId",
                schema: "ordering",
                table: "TravelerAssociations",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerIdentityDocuments_TravelerId",
                schema: "ordering",
                table: "TravelerIdentityDocuments",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerLoyaltyAccounts_ProgramCode_AccountRef",
                schema: "ordering",
                table: "TravelerLoyaltyAccounts",
                columns: new[] { "ProgramCode", "AccountRef" });

            migrationBuilder.CreateIndex(
                name: "IX_TravelerLoyaltyAccounts_TravelerId",
                schema: "ordering",
                table: "TravelerLoyaltyAccounts",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_CustomerRef",
                schema: "ordering",
                table: "Travelers",
                column: "CustomerRef");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_OrderId",
                schema: "ordering",
                table: "Travelers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_Surname",
                schema: "ordering",
                table: "Travelers",
                column: "Surname");

            migrationBuilder.CreateIndex(
                name: "IX_ValueAllocations_EntitlementId",
                schema: "ordering",
                table: "ValueAllocations",
                column: "EntitlementId");

            migrationBuilder.CreateIndex(
                name: "IX_ValueAllocations_OrderItemId",
                schema: "ordering",
                table: "ValueAllocations",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "AirTransportSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "BaggageSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ChargeLines",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "CommercialTermRestrictions",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ConsumptionFacts",
                schema: "consumption");

            migrationBuilder.DropTable(
                name: "ContactTravelers",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "DocumentStocks",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "EmdCoupons",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "EntitlementBeneficiaries",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "EntitlementSegments",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ExternalReferences",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "FulfillmentLinks",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "FulfillmentUnits",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "InsuranceSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ItemLineagePredecessors",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "JourneySegments",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "LoungeSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "MealSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "OrderHistory",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "OrderItemBeneficiaries",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "OrderSnapshots",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "ProcessingLockEntitlements",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ProcessingLockItems",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ProductAttributes",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ReconciliationCases",
                schema: "consumption");

            migrationBuilder.DropTable(
                name: "SeatSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ServicingDelegations",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TicketCoupons",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "TimeLimitEntitlements",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TimeLimitItems",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TimeLimitTravelers",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TransferSpecs",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TravelerAssociations",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TravelerIdentityDocuments",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "TravelerLoyaltyAccounts",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ValueAllocations",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ElectronicMiscDocuments",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "SupplierReservations",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "Journeys",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ProcessingLocks",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "ElectronicTickets",
                schema: "fulfillment");

            migrationBuilder.DropTable(
                name: "TimeLimits",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "EntitlementSpecifications",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "Travelers",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "Entitlements",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "ordering");
        }
    }
}
