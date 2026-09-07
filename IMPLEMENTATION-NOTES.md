# Implementation Notes — Order Service Domain

Implements the domain layer and SQL Server persistence specified by `docs/15-ALL-IN-ONE.md`
(sections 00–08 and 12–13). Scope for this pass: **Domain layer for all three bounded contexts +
first migration + database update**. Application handlers, process managers, seed runner, read
models, and tests are not in this pass.

## What was built

### `AeroTech.Ordering.Domain`

```
SharedKernel/
  Identifiers/     29 Guid v7 strongly-typed ids (readonly record structs)
  Enums/           commercial, governance, fulfillment, consumption enums
  ValueObjects/    Money, CurrencyCode, AirportCode, CarrierCode, CountryCode,
                   OrderReference, ActorContext, SalesContext, BuyerSnapshot,
                   CommercialSource, SettlementArrangement, CapacityCommitmentRef,
                   ResponsibilityAssignment
Ordering/
  Aggregates/      Order (sole aggregate root)
  Entities/        Traveler, IdentityDocument, LoyaltyAccountRef, TravelerAssociation,
                   Contact, Journey, JourneySegment, OrderItem, Entitlement, ChargeLine,
                   ValueAllocation, FulfillmentLink, TimeLimit, ProcessingLock,
                   ExternalReference, ServicingDelegation
  Specifications/  AirTransport, Seat, Baggage, Meal, Lounge, Accommodation, Transfer,
                   Insurance
  StateMachines/   OrderItem, Entitlement, TimeLimit, ProcessingLockPolicy
  ValueObjects/    ProductSnapshot, PriceSnapshot, FxSnapshot, CommercialTermsSnapshot,
                   Applicability, ServicingAuthority, OrderLineage, ItemLineage, TravelerName
  Events/          25 domain events
Fulfillment/
  Aggregates/      ElectronicTicket, ElectronicMiscDocument, SupplierReservation, DocumentStock
  Entities/        TicketCoupon, EmdCoupon, FulfillmentUnit
  StateMachines/   TicketCoupon, EmdCoupon, SupplierReservation
  Events/          16 domain events
Consumption/
  Aggregates/      ConsumptionFact (append-only), ReconciliationCase
```

### `AeroTech.Ordering.Persistence`

Fluent configurations for every entity, NodaTime and strongly-typed-id value converters,
`OrderHistoryEntry` / `OrderSnapshot` (append-only audit, persistence-side only).

## Migration

- `20260807180203_InitialCreate` — **applied** to `DotAirOrderNewV2` on `localhost\SQLEXPRESS`.
- 51 tables: `ordering` 40, `fulfillment` 7, `consumption` 2, `integration` 2.
- Artifacts generated for review: `artifacts/sql/order-service-create.sql`,
  `artifacts/sql/order-service-idempotent.sql`.

Verified directly against the database after apply:

| Check | Result |
|---|---|
| Cross-schema foreign keys | 0 |
| `PaidAmount`/`BalanceDue`/`RefundedAmount`/`TicketStatus`/`CheckedIn`/`Flown` in `ordering` | 0 |
| `rowversion` on aggregate roots | 7 (Orders, 4 fulfillment roots, 2 consumption roots) |
| Check constraints | 6 (baggage allowance shape, occupancy, date ranges, stock range) |
| Unique: `Orders.Reference`, `(SourceSystem,SourceEventId)`, ticket/EMD numbers, coupon numbers, `(OrderItemId,Sequence)`, `(JourneyId,Sequence)` | present |

The three foreign keys with `principalSchema: "fulfillment"` are all **intra**-schema
(coupons → their document, units → their reservation) — aggregate-internal cascades, not
bounded-context crossings.

## Deviations from the specification

1. **Solution layout.** The spec's `src/OrderService.*` tree was not created. The domain was
   placed into the existing `AeroTech.Ordering.Domain` / `.Persistence` projects, per this
   repo's established layout. Module boundaries are preserved by namespace
   (`Ordering`/`Fulfillment`/`Consumption`) and by SQL schema.

2. **Schema names.** Spec says `ordering`/`fulfillment`/`consumption`/`workflow`/`integration`.
   `workflow` is **not created** — no workflow tables exist yet because process managers are
   application-layer and out of scope for this pass. The other four exist.

3. **Single `OrderingDbContext`.** The spec prefers separate contexts per module. One context is
   used, because the outbox and OrderHistory must share the Ordering physical transaction and
   the repo's `CommandDbContext`/`IUnitOfWork` already provides exactly that. Boundaries are
   enforced by schema and the no-cross-schema-FK rule rather than by context separation. Splitting
   later is mechanical.

4. **Base types.** `Order` and the fulfillment/consumption roots derive from the repo's
   `AggregateRoot<TId>`, so they inherit `RowVersion`, `LastUpdateTime`/`LastUpdatedBy`, and
   `Causes(...)` event collection. The spec's `ConcurrencyToken` is that inherited `RowVersion`.
   `AggregateVersion` is a separate domain-maintained `long`, as specified.

5. **Exceptions.** The spec asks for "named domain exceptions or Result error codes; be
   consistent". This repo mandates `ExceptionFactory` (CLAUDE.md), so invariant violations throw
   `BusinessException` via factory methods carrying a numeric code and a REST-accurate
   `HttpStatus`. New codes occupy **2400–2679**, leaving the pre-existing 2001–2323 block intact.

6. **ID-list collections became join entities.** `Contact.TravelerRefs`,
   `TimeLimit.{OrderItem,Entitlement,Traveler}Refs`, `ProcessingLock.{OrderItem,Entitlement}Refs`,
   `Entitlement.BeneficiaryRefs`, and `OrderItem.PredecessorItemIds` are modelled as small record
   types (e.g. `ContactTravelerRef`) rather than `List<StronglyTypedId>`. This is what produces the
   spec's explicit join tables (`ContactTravelers`, `TimeLimitItems`, …) instead of a JSON column,
   and satisfies "explicit child entities/tables for collections". Each exposes a companion
   `…Ids` projection so domain code still reads in terms of ids.

7. **`PriceSnapshot.Total`.** Stored as `TotalAmount` + `Currency` scalars with `Total` as a
   computed `Money`. `Money` is a readonly record struct and EF cannot own a struct navigation.
   Column names still match the catalog (`PriceTotal`, `PriceCurrency`).

8. **Private parameterless constructors on snapshot value objects.** `PriceSnapshot`,
   `ProductSnapshot`, `CommercialTermsSnapshot`, `SalesContext`, `CommercialSource`,
   `CapacityCommitmentRef`, `ServicingAuthority`, `TravelerName`, `OrderLineage` each gained a
   private ctor with private setters. EF cannot bind constructor parameters that are collections
   or owned navigations. Public validating constructors remain the only way application code can
   build them, so invalid state is still unconstructable from outside.

9. **`SourceRuleRefs`, seat `Characteristics`, meal `DietaryAttributes`, delegation
   `AuthorityTypes`** are mapped as EF primitive collections (JSON in `nvarchar(max)`) rather than
   separate tables. The catalog only names tables for `SourceRuleReferences`; these four are flat
   scalar lists never queried or joined independently. `ServicingDelegationAuthorities` from the
   catalog is therefore a JSON column, not a table — **flag this if the authority check is ever
   pushed into SQL.**

10. **NodaTime mapping.** No NodaTime provider exists for EF Core SQL Server, so explicit
    `ValueConverter`s map `Instant`→`datetime2(7)` (UTC), `LocalDate`→`date`,
    `LocalTime`→`time(0)`. Domain stays free of `DateTime`/`DateTimeOffset` as required.

11. **Entitlement specifications use TPT.** A base `EntitlementSpecifications` table holds the
    `EntitlementId` key, with one table per concrete spec, giving the catalog's
    `ordering.BaggageSpecs` etc. keyed 1:1 on `EntitlementId`.

## Contradiction with `CLAUDE.md` — resolved in favour of the docs

`CLAUDE.md` describes a different model: aggregates `Order` + `TrafficDocument` + `Payment` +
`FulfillmentTask` + `ProviderInteraction`, snowflake `long` ids, and schemas
`Order`/`ReadModel`/`ReferenceData` on database `DotAirOrderNew`. The docs specify an Order-native
model, Guid v7 ids, NodaTime, and `ordering`/`fulfillment`/`consumption`.

Confirmed with the user: **follow the docs exactly.** Supporting evidence — the Domain project was
empty apart from `_Shared/Resources`; `AeroTech.Ordering.Domain.csproj` already referenced
NodaTime (only the docs' model needs it); and the configured database is `DotAirOrderNewV2`, not
`DotAirOrderNew`.

Consequences to be aware of:

- The pre-existing `ExceptionFactory` methods (codes 2001–2323) describe the **CLAUDE.md** model
  (`TrafficDocument`, delivery units, seat blocks, tax documents). They are untouched and now
  largely unreferenced. They are not dead-code-safe to delete yet — decide once the servicing
  layer lands.
- `CLAUDE.md` itself is now out of date for the domain-model, ID-strategy, schema, and database
  sections. It should be updated before the next session relies on it.
- Legacy schemas `Ordering`/`Delivery`/`Ops`/`Audit` still exist in the older `DotAirOrderNewV`
  database. Untouched by this work.

## Unresolved assumptions

- **`ChargeType` negatives.** Spec: "negative amounts permitted only for explicitly configured
  discount/credit-like line types". Implemented as `Discount` and `OtherCharge`. `Penalty` is
  positive-only. Confirm against the pricing engine's sign convention.
- **INV-045 capacity exemption.** `Entitlement.Activate` requires a `CapacityCommitmentRef` for
  every `AirTransport` entitlement. The spec allows "unless an explicit policy exempts it"; no
  policy port exists yet, so there is currently no exemption path.
- **INV-046 responsibility.** Enforced only as "if a `ResponsibilityAssignment` is supplied it
  must not be empty". Deciding *when* one is mandatory needs the supplier/partner registry.
- **`ProcessingLockPolicy`.** Encodes the spec's stated rules (Split conflicts with everything;
  Refund/Cancellation/VoluntaryChange/Reaccommodation conflict on overlap; same-type conflicts;
  PaymentCompletion blocks expiry). The full compatibility matrix for the remaining lock types is
  not specified — currently same-type-only.
- **Order closure.** `CloseOrder` refuses while any entitlement is Active or any time limit is
  Active. The spec says only "when commercial closure policy allows".
- **`OrderReference` generation.** The value object validates the 10–12 char unambiguous-Base32
  format but does not generate references; that belongs to the application layer.

## Not enforced automatically

- No architecture tests (tests are paused by standing repo decision). The rules that would be
  test-enforced — Domain not referencing EF/ASP.NET, no cross-context navigations, no public
  mutable collections, no `decimal` money parameters — currently hold by construction and were
  checked by hand.
- The append-only nature of `OrderHistory` is a convention, not a database constraint. No trigger
  or permission blocks an UPDATE.
- `AggregateVersion` increments once per accepted mutation by construction (`BumpVersion()` in each
  public method); nothing at the database level enforces it.

## Build and verification status

- `dotnet build AeroTech.Ordering.sln` — succeeded, 0 errors, 3 pre-existing warnings.
- `dotnet ef migrations add InitialCreate` — succeeded.
- `dotnet ef database update` — succeeded against `DotAirOrderNewV2`.
- Post-apply schema verified by direct SQL query (table counts, FK boundaries, absent financial
  columns, rowversion, check constraints, unique indexes).
- **No tests were written or run** — tests are paused by standing repo decision. The spec's test
  strategy (section 10) is therefore unimplemented, including the invariant, property-based,
  and migration/seed tests it calls for.
