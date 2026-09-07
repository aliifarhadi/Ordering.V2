# Ordering Seeder

Builds a complete, issued order by driving the real `Order` aggregate and persisting it through
`OrderingDbContext`. Every domain invariant runs, so this doubles as an end-to-end smoke test of
the domain and the EF mappings — it is not a SQL insert script.

## Run

```bash
cd tools/AeroTech.Ordering.Seeder
dotnet run
```

Options (command line, environment variable, or `appsettings.json`):

| Option | Default | Meaning |
|---|---|---|
| `--Reference` | `THRMHDRT22` | Order reference to create or operate on |
| `--Replace` | `false` | Delete and recreate if the reference already exists |
| `--Refund <TicketNumber>` | — | Refund that ticket instead of seeding |
| `--KeepAncillaries` | `false` | With `--Refund`, leave the traveller's EMD untouched |
| `--ConnectionString` | `DotAirOrderNewV2` on `localhost\SQLEXPRESS` | Target database |

```bash
dotnet run -- --Replace true
dotnet run -- --Reference THRMHDRT23
dotnet run -- --ConnectionString "Server=...;Database=...;Trusted_Connection=True;TrustServerCertificate=True"
```

Re-running without `--Replace` is a no-op, so it is safe in a loop.

## Refund

```bash
dotnet run -- --Refund 217-2400001                      # ticket + that traveller's ancillaries
dotnet run -- --Refund 217-2400001 --KeepAncillaries true  # flight coupons only
```

Refunds one traveller's ticket and, by default, the EMD issued to the same traveller. The order
of operations follows the spec's CancellationProcess/RefundProcess:

1. acquire a `Refund` `ProcessingLock` over the affected entitlements;
2. refund the ticket coupons, then the EMD coupons — **fulfilment is withdrawn before value is
   returned**, per "entitlement must not remain valid after value has been returned";
3. `ApplyCancellation` on the corresponding Ordering entitlements, which recomputes each item's
   commercial status;
4. release the lock, then re-check invariants.

**No money is written.** Ordering is not the financial system of record — Receivables owns the
credit. The command records only the commercial and fulfilment consequences.

Guards: the operation aborts before touching anything if any coupon is not `Open` (already
refunded, flown, or under DCS control), so re-running it is rejected rather than corrupting state.

Refunding one traveller on a two-traveller order leaves the order `Active`, because the other
traveller's entitlements are still live. Each cancelled traveller's items become `Cancelled`
individually — that is `DeriveStatus` working off entitlement states, not a status set by hand.
Refunding a subset of one item's entitlements (say only the return leg) would instead yield
`PartiallyChanged`.

## Change return date (voluntary reissue)

```bash
dotnet run -- --ChangeReturn 217-2400000                        # +1 day, default penalty
dotnet run -- --ChangeReturn 217-2400000 --DaysLater 3
dotnet run -- --ChangeReturn 217-2400000 --Penalty 5000000 --FareDifference 0
```

Moves one traveller's **return** leg to a later date. This is modelled as a **reissue/exchange**,
not a revalidation, and follows `VoluntaryChangeProcess`.

Three distinct industry operations exist and they are not interchangeable:

| Operation | When it applies | Document effect | Our model |
|---|---|---|---|
| **Revalidation** | Date/flight changes with **no** change to price, fare basis or RBD | Same coupon, date updated | `ApplyScheduleSnapshotChange` |
| **Reissue / exchange** | Any price-affecting change; requires a priced ServicingOffer | Old coupon `Exchanged`, new document issued | `ApplyReplacement` / `ApplyPartialChange` |
| **Involuntary reissue** | Airline-caused (delay, cancellation, equipment) | No penalty, no fare difference | `CommercialSource = DisruptionAuthority` |

This command implements the middle row, because a change penalty and fare difference make it
price-affecting. Using revalidation here would lose the audit trail for the money collected.

What it does, in order:

1. acquire a `VoluntaryChange` lock on the return entitlement;
2. add the replacement segment **before** releasing the old one — protected rebooking;
3. create a new `OrderItem` with `CommercialSource = ServicingOffer`, priced as a `Penalty` charge
   line plus an optional `Base` fare-difference line, and `ItemLineage` pointing at the original
   item (INV-103);
4. confirm it, mark the old return coupon `Exchanged`, and `ApplyPartialChange` to mark **only**
   the return entitlement `Replaced`;
5. issue the replacement ticket and attach its `FulfillmentLink`;
6. release the lock and re-check invariants.

The outbound leg is deliberately untouched, so the original item lands in `PartiallyChanged` while
the new item is `Confirmed` — exactly scenario S07 in the spec. The original ticket stays `Issued`
with coupon 1 `Open` and coupon 2 `Exchanged`: commercial price history and fulfilment usage stay
distinct records.

Guards: a terminal item is rejected outright (a cancelled entitlement can never be reinstated,
INV-048), and a return coupon that is not `Open` cannot be exchanged. Refund a ticket and this
command will refuse it.

**`OrderReference` uses an unambiguous Base32 alphabet** — `ABCDEFGHJKLMNPQRSTUVWXYZ23456789`.
No `O`, `I`, `0` or `1`, and 10–12 characters. `ORDTHRMHD01` is rejected; `THRMHDRT22` is valid.

## Scenario: `ThrMhdRoundTripScenario`

THR → MHD → THR round trip, two adults, confirmed and issued.

```
Travelers    Rezaei/Ali (ADT), Mohammadi/Sara (ADT)
Journeys     J1 THR->MHD  segment W5-1084  2026-09-02
             J2 MHD->THR  segment W5-1085  2026-09-09
Items        2 x AirTransport   42,200,000 IRR each  (base 38,000,000 + tax 4,200,000)
             4 x Baggage         3,500,000 IRR each  (10 kg, per traveller per segment)
             2 x Lounge          2,200,000 IRR each  (THR only, location-scoped)
             TOTAL             102,800,000 IRR
Entitlements 10, all Active
Documents    2 tickets (2 coupons each) + 2 EMDs (3 coupons each: 2 baggage + 1 lounge)
```

Modelling choices worth knowing:

- **One air item per traveller**, not one shared item. `AirTransport` entitlements must apply to
  exactly one traveller (INV-041), and the price snapshot is per-item, so a shared item could not
  carry a per-traveller fare.
- **`ValueAllocation` on the air items.** Each covers two segments, so INV-025 requires complete
  allocations before fulfilment; they sum exactly to the item total (INV-024).
- **Lounge is location-scoped, not segment-scoped** — `Applicability(locationRef: "THR")` with no
  segment refs, matching the spec's lounge example.
- **Ancillaries are separate items** because each carries its own price, commercial source, and
  terms. Bundling them into the air item would violate the immutable-price-snapshot rule.
- **One EMD per traveller** carrying that traveller's three ancillary coupons, with
  `FulfillmentLink` rows attaching every document back to its entitlement (10 links total).

## Adding a scenario

Implement a class in `Scenarios/` returning `ScenarioResult`, then wire it in `Program.cs`. Take
`Instant` and `IEventIdFactory` in the constructor rather than calling `SystemClock` or generating
ids inline, so runs stay reproducible.

## Two traps

**`SalesContext` is a `record`.** Passing one instance to both the order and its items makes EF
collapse them under identity resolution and the insert fails with `Cannot insert the value NULL
into column 'ItemSellerId'`. Build a fresh instance per owned location — that is why the scenario
calls `BuildSalesContext(currency)` at each site instead of reusing a local.

**Domain events are discarded.** `NoOpDomainEventDispatcher` drops them: the outbox and the
integration pipeline are application-layer concerns that do not exist yet. Nothing is published to
RabbitMQ, and `integration.OutboxMessages` stays empty. Revisit when the application layer lands.

## Verify

```sql
SELECT o.Reference, o.AggregateVersion, i.ProductType, i.Status, i.PriceTotal, i.PriceCurrency
FROM ordering.Orders o
JOIN ordering.OrderItems i ON i.OrderId = o.Id
WHERE o.Reference = 'THRMHDRT22'
ORDER BY i.ProductType;
```
