# Architecture Documentation - Hybrid Domain-Driven Design

## Overview

This project implements a **Hybrid Domain-Driven Design (DDD) approach** - a pragmatic balance between anemic domain models and full DDD complexity. This approach was chosen to maximize code quality and maintainability while keeping development velocity high for an exam project timeline.

---

## Architecture Decision: Why Hybrid DDD?

### The Spectrum

```
Anemic Models ←――――――――――――――――― HYBRID ―――――――――――――――→ Full DDD
(Simple CRUD)                  (Pragmatic)              (Complex)
```

### Our Choice: Hybrid Approach

We implement **selective DDD patterns** where they provide the most value:

✅ **What we DO use:**
- Value Objects for critical domain concepts
- Domain Events for important state changes
- Business logic methods in entities
- Validation in domain models

❌ **What we DON'T use (to avoid over-engineering):**
- Aggregate Roots with strict boundaries
- Event Sourcing
- CQRS (Command Query Responsibility Segregation)
- Complex repository patterns with Unit of Work

---

## Value Objects

Value Objects ensure **type safety** and **immutability** for critical domain concepts.

### Implemented Value Objects

#### 1. **Money** (`Models/ValueObjects/Money.cs`)
Represents monetary amounts with currency.

**Why?**
- Prevents mixing currencies (can't add DKK + EUR)
- Ensures amounts are never negative
- Type-safe arithmetic operations

**Example:**
```csharp
var price = Money.Create(100m, "DKK");
var tax = Money.Create(25m, "DKK");
var total = price + tax;  // 125 DKK

// This throws exception:
var invalid = price + Money.Create(10m, "EUR");  // ❌ Can't mix currencies
```

#### 2. **Email** (`Models/ValueObjects/Email.cs`)
Represents a validated email address.

**Why?**
- Email is always valid when created
- Normalized to lowercase
- No need to validate email format multiple times

**Example:**
```csharp
var email = Email.Create("USER@EXAMPLE.COM");
Console.WriteLine(email.Value);  // "user@example.com" (normalized)

Email.Create("invalid");  // ❌ Throws ArgumentException
```

#### 3. **DateRange** (`Models/ValueObjects/DateRange.cs`)
Represents a period between two dates.

**Why?**
- Ensures end date is always after start date
- Provides domain methods (Overlaps, Contains, GetNumberOfNights)
- Prevents invalid booking periods

**Example:**
```csharp
var stay = DateRange.Create(
    new DateTime(2025, 6, 1),
    new DateTime(2025, 6, 10)
);

Console.WriteLine(stay.GetNumberOfNights());  // 9
Console.WriteLine(stay.Contains(new DateTime(2025, 6, 5)));  // true
```

#### 4. **BookingStatus** (`Models/ValueObjects/BookingStatus.cs`)
Represents booking status with transition rules.

**Why?**
- Type-safe status (no string typos like "Confirmedd")
- Enforces valid state transitions
- Compile-time safety

**Example:**
```csharp
var status = BookingStatus.Pending;

if (status.CanTransitionTo(BookingStatus.Confirmed))
{
    status = BookingStatus.Confirmed;  // ✅ Valid transition
}

// This is prevented:
status = BookingStatus.Completed;  // ❌ Can't go from Pending to Completed directly
```

#### 5. **PaymentStatus** (`Models/ValueObjects/PaymentStatus.cs`)
Represents payment status with transition rules.

**Why?**
- Same benefits as BookingStatus
- Prevents invalid payment state transitions

---

## Domain Events

Domain Events represent **something that happened** in the domain that domain experts care about.

### Why Domain Events?

1. **Decoupling** - Event handlers can be added without modifying entities
2. **Kafka Integration** - Events map directly to Kafka messages (Fase 3)
3. **Audit Trail** - Events provide a history of what happened
4. **Testability** - Easy to verify that correct events are raised

### Implemented Domain Events

#### 1. **BookingConfirmedEvent**
Raised when a booking is confirmed.

**Triggers:**
- Send confirmation email to guest
- Update availability
- Notify staff

#### 2. **BookingCancelledEvent**
Raised when a booking is cancelled.

**Triggers:**
- Process refund
- Release accommodation units
- Send cancellation email

#### 3. **PaymentCompletedEvent**
Raised when payment is successfully processed.

**Triggers:**
- Confirm booking
- Generate receipt
- Update accounting system

#### 4. **PaymentRefundedEvent**
Raised when a payment is refunded.

**Triggers:**
- Update booking status
- Send refund confirmation
- Update financial records

---

## Entity Design Pattern

Our entities follow this pattern:

```csharp
public class Booking
{
    // 1. Properties with validation
    private DateTime _checkInDate;
    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if (value < DateTime.UtcNow.Date)
                throw new ArgumentException("CheckInDate cannot be in the past");
            _checkInDate = value;
        }
    }

    // 2. Domain Events collection
    public List<IDomainEvent> DomainEvents { get; } = new();

    // 3. Business logic methods
    public void Confirm()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending bookings can be confirmed");
        
        Status = "Confirmed";
        
        // Raise domain event
        DomainEvents.Add(new BookingConfirmedEvent(BookingId, GuestId, CheckInDate, CheckOutDate));
    }
}
```

---

## Benefits of This Approach

### 1. **Type Safety**
```csharp
// ❌ Anemic approach - runtime errors
booking.Status = "Confirmedd";  // Typo! Only caught at runtime

// ✅ Hybrid approach - compile-time safety
booking.Status = BookingStatus.Confirmed;  // Type-safe!
```

### 2. **Business Logic Encapsulation**
```csharp
// ❌ Anemic approach - logic in services
public class BookingService
{
    public void ConfirmBooking(Booking booking)
    {
        if (booking.Status != "Pending") throw ...
        booking.Status = "Confirmed";
        booking.ConfirmedDate = DateTime.UtcNow;
        // What if someone forgets to set ConfirmedDate?
    }
}

// ✅ Hybrid approach - logic in entity
public class Booking
{
    public void Confirm()
    {
        // All logic is HERE - single source of truth
        if (!Status.CanTransitionTo(BookingStatus.Confirmed))
            throw new InvalidOperationException(...);
        
        Status = BookingStatus.Confirmed;
        ConfirmedDate = DateTime.UtcNow;
        DomainEvents.Add(new BookingConfirmedEvent(...));
    }
}
```

### 3. **Testability**
```csharp
[Fact]
public void Confirm_WhenPending_RaisesBookingConfirmedEvent()
{
    // Arrange
    var booking = new Booking { Status = BookingStatus.Pending, ... };
    
    // Act
    booking.Confirm();
    
    // Assert
    Assert.Contains(booking.DomainEvents, e => e is BookingConfirmedEvent);
    Assert.Equal(BookingStatus.Confirmed, booking.Status);
}
```

---

## Trade-offs

### What We Gain
✅ Type safety with Value Objects  
✅ Business logic encapsulation  
✅ Domain events for Kafka integration  
✅ Better testability  
✅ Clearer domain model  

### What We Sacrifice
❌ More initial setup time  
❌ Slightly more complex than pure CRUD  
❌ Learning curve for Value Objects  

### Why It's Worth It
For an exam project, this demonstrates:
- Understanding of DDD principles
- Pragmatic decision-making
- Balance between theory and practice
- Production-ready code quality

---

## Testing Strategy

### Value Objects: 83 tests
- Test immutability
- Test equality by value
- Test validation rules
- Test business methods

### Entities: 370+ tests
- Test property validation
- Test business logic methods
- Test state transitions
- Test domain events

---

## Future Extensions

This architecture is ready for:

1. **Kafka Integration** (Fase 3)
   - Domain events map directly to Kafka messages
   - Event handlers can publish to Kafka topics

2. **CQRS** (if needed)
   - Commands use entities with business logic
   - Queries use read models (DTOs)

3. **Event Sourcing** (if needed)
   - Domain events already capture state changes
   - Can rebuild state from event stream

---

## Conclusion

This hybrid approach gives us **80% of DDD benefits with 20% of the complexity**. It's the right choice for:
- Exam projects with time constraints
- Teams learning DDD
- Projects that may scale in the future
- Demonstrating architectural understanding

**Result:** Clean, maintainable, testable code that impresses examiners while staying practical.

