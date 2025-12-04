# BookMyHome - Diagram Prompts til Lucidchart

## 📋 **OVERSIGT**

Dette dokument indeholder prompts til at generere alle nødvendige diagrammer til eksamensprojektet.

---

## 1️⃣ **ENTITY RELATIONSHIP DIAGRAM (ERD)**

**Tool:** Lucidchart ERD
**Formål:** Vise database struktur og relationer

**Prompt:**
```
Create an Entity Relationship Diagram (ERD) for a campsite booking system with the following entities:

BOOKING (BookingId PK, GuestId FK, CampsiteId FK, AccommodationTypeId FK, CheckInDate, CheckOutDate, Status, TotalPrice, NumberOfAdults, NumberOfChildren, SpecialRequests, CreatedAt)

GUEST (GuestId PK, Email, FirstName, LastName, Phone, Country, DateOfBirth, CreatedAt)

USER (UserId PK, Email, FirstName, LastName, Phone, Country, PasswordHash, IsActive, JoinedDate)

CAMPSITE (CampsiteId PK, Name, Region, Description, Latitude, Longitude, IsActive)

ACCOMMODATION_TYPE (AccommodationTypeId PK, CampsiteId FK, Name, Description, Capacity, BasePrice, IsActive)

PAYMENT (PaymentId PK, BookingId FK, Amount, Currency, Status, PaymentMethod, TransactionId, ProcessedAt)

REVIEW (ReviewId PK, BookingId FK, GuestId FK, Rating, Comment, CreatedAt)

AMENITY (AmenityId PK, Name, Description, IconName)

CAMPSITE_AMENITY (CampsiteId FK, AmenityId FK)

ACCOMMODATION_TYPE_AMENITY (AccommodationTypeId FK, AmenityId FK)

AVAILABILITY (AvailabilityId PK, AccommodationTypeId FK, Date, AvailableUnits, IsBlocked)

PRICING (PricingId PK, AccommodationTypeId FK, StartDate, EndDate, PricePerNight)

NEWSLETTER_SUBSCRIPTION (SubscriptionId PK, UserId FK, Email, IsActive, SubscribedAt)

NOTIFICATION (NotificationId PK, UserId FK, Type, Title, Message, IsRead, CreatedAt)

Relationships:
- BOOKING many-to-one GUEST
- BOOKING many-to-one CAMPSITE
- BOOKING many-to-one ACCOMMODATION_TYPE
- BOOKING one-to-one PAYMENT
- BOOKING one-to-many REVIEW
- CAMPSITE one-to-many ACCOMMODATION_TYPE
- CAMPSITE many-to-many AMENITY (through CAMPSITE_AMENITY)
- ACCOMMODATION_TYPE many-to-many AMENITY (through ACCOMMODATION_TYPE_AMENITY)
- ACCOMMODATION_TYPE one-to-many AVAILABILITY
- ACCOMMODATION_TYPE one-to-many PRICING

Use crow's foot notation. Show cardinality (1:1, 1:N, N:M).
```

---

## 2️⃣ **USE CASE DIAGRAM**

**Tool:** Lucidchart UML
**Formål:** Vise system funktionalitet og aktører

**Prompt:**
```
Create a UML Use Case Diagram for a campsite booking system:

Actors:
- Guest (primary)
- Registered User (extends Guest)
- Admin (extends User)
- System (for automated tasks)

Use Cases for Guest:
- Search Campsites
- View Campsite Details
- View Accommodation Types
- View Availability
- View Pricing
- Register Account

Use Cases for Registered User:
- Login
- Create Booking
- View My Bookings
- Update Booking (special requests)
- Cancel Booking
- Make Payment
- Write Review
- Subscribe to Newsletter
- View Notifications

Use Cases for Admin:
- Manage Campsites
- Manage Accommodation Types
- Manage Amenities
- Manage Pricing
- View All Bookings
- View Analytics

Use Cases for System:
- Send Booking Confirmation Email (extends Create Booking)
- Send Payment Receipt (extends Make Payment)
- Send Cancellation Email (extends Cancel Booking)
- Process Scheduled Notifications

Include relationships:
- "Login" includes "Authenticate User"
- "Create Booking" includes "Check Availability"
- "Create Booking" includes "Calculate Price"
- "Make Payment" extends "Create Booking"

Use standard UML notation with stick figures for actors and ovals for use cases.
```

---

## 3️⃣ **CLEAN ARCHITECTURE DIAGRAM**

**Tool:** Lucidchart (Custom shapes)
**Formål:** Vise lagdelt arkitektur med dependency rule

**Prompt:**
```
Create a Clean Architecture (Onion Architecture) diagram with concentric circles:

INNERMOST CIRCLE - Domain Layer:
- Entities (Booking, User, Campsite, etc.)
- Value Objects (Email, Money, DateRange)
- Domain Events (BookingCreatedEvent, UserCreatedEvent)
- Repository Interfaces (IBookingRepository, IUserRepository)
- Strongly-Typed IDs (BookingId, UserId, CampsiteId)

SECOND CIRCLE - Application Layer:
- ApiService (HTTP client wrapper)
- Kafka Producer/Consumer
- DTOs (BookingDto, UserDto, CampsiteDto)

THIRD CIRCLE - Infrastructure Layer:
- EF Core DbContext
- Repository Implementations
- Kafka Integration
- Security (InputValidator, JWT)
- MySQL Database

OUTERMOST CIRCLE - Presentation Layer:

## 5️⃣ **SCALE CUBE DIAGRAM**

**Tool:** Lucidchart (3D shapes)
**Formål:** Vise skalering strategi (X, Y, Z akse)

**Prompt:**
```
Create a 3D Scale Cube diagram showing three axes:

X-AXIS (Horizontal Duplication):
- Label: "Cloning / Load Balancing"
- Show: 3 identical API instances behind load balancer
- Example: "BookMyHome API x3 instances"
- Status: ✅ IMPLEMENTED

Y-AXIS (Functional Decomposition):
- Label: "Microservices / Service Split"
- Show: Split into separate services
  - Booking Service
  - User Service
  - Payment Service
  - Notification Service
- Status: ⚠️ PARTIAL (Modular Monolith)

Z-AXIS (Data Partitioning):
- Label: "Sharding / Data Split"
- Show: Database sharding by region
  - Shard 1: Nordjylland
  - Shard 2: Midtjylland
  - Shard 3: Syddanmark
- Status: ❌ NOT IMPLEMENTED

Use 3D cube with arrows along each axis. Label current implementation status.
```

---

## 6️⃣ **FURPS+ DIAGRAM**

**Tool:** Lucidchart (Mind map)
**Formål:** Vise kvalitetsattributter (non-functional requirements)

**Prompt:**
```
Create a FURPS+ quality attributes diagram as a mind map:

CENTER: BookMyHome System

FUNCTIONALITY:
- ✅ CRUD Operations (Bookings, Users, Campsites)
- ✅ Authentication (JWT)
- ✅ Authorization (Role-based)
- ✅ Search & Filter
- ✅ Payment Processing
- ✅ Email Notifications (Kafka)

USABILITY:
- ✅ Responsive UI (MudBlazor)
- ✅ Intuitive Navigation
- ✅ Clear Error Messages
- ✅ Swagger API Documentation
- ⚠️ Accessibility (basic)

RELIABILITY:
- ✅ Data Validation
- ✅ Error Handling
- ✅ Transaction Management (EF Core)
- ✅ Health Checks (YARP)
- ⚠️ Retry Logic (partial)

PERFORMANCE:
- ✅ Database Indexing
- ✅ Async/Await (non-blocking)
- ✅ Load Balancing (YARP)
- ⚠️ Caching (not implemented)
- ⚠️ CDN (not implemented)

SUPPORTABILITY:
- ✅ Logging (ILogger)
- ✅ Docker Deployment
- ✅ API Versioning Ready
- ✅ Comprehensive Documentation
- ✅ Unit Tests (372 tests)

PLUS (+):
- DESIGN: ✅ Clean Architecture, DDD, SOLID
- IMPLEMENTATION: ✅ .NET 9.0, C# 12
- INTERFACE: ✅ REST API (Level 2), Swagger
- PHYSICAL: ✅ Docker, MySQL, Kafka

Use color coding: Green (✅ Implemented), Yellow (⚠️ Partial), Red (❌ Not Implemented)
```

---

## 7️⃣ **DEPLOYMENT DIAGRAM**

**Tool:** Lucidchart UML
**Formål:** Vise fysisk deployment med Docker containers

**Prompt:**
```
Create a UML Deployment Diagram showing:

NODES:
- Developer Machine (Windows/Linux/Mac)
  - Contains: Visual Studio Code, Docker Desktop

- Docker Host
  - Contains: Docker Compose

- MySQL Container (mysql:8.0)
  - Port: 3306
  - Volume: mysql_data

- Kafka Container (confluentinc/cp-kafka:7.5.0)
  - Port: 9092

- Zookeeper Container (confluentinc/cp-zookeeper:7.5.0)
  - Port: 2181

- API Gateway Container (YARP)
  - Port: 8001

- BookMyHome API Container (.NET 9.0)
  - Port: 7001

- Nginx Container (nginx:alpine)
  - Port: 80, 443

NETWORK:
- bookmyhome-network (Bridge)

CONNECTIONS:
- Developer Machine → Docker Host (Docker CLI)
- API Gateway → BookMyHome API (HTTP/HTTPS)
- BookMyHome API → MySQL (TCP 3306)
- BookMyHome API → Kafka (TCP 9092)
- Kafka → Zookeeper (TCP 2181)
- Nginx → API Gateway (Reverse Proxy)

Use UML deployment diagram notation with 3D boxes for nodes.
```

---

## 8️⃣ **SEQUENCE DIAGRAM - CREATE BOOKING**

**Tool:** Lucidchart UML
**Formål:** Vise message flow for booking creation

**Prompt:**
```
Create a UML Sequence Diagram for "Create Booking" flow:

ACTORS/OBJECTS:
- User (actor)
- Browser (Blazor UI)
- API Gateway (YARP)
- BookingsController
- BookingRepository
- DbContext (EF Core)
- MySQL Database
- KafkaProducer
- Kafka Broker

SEQUENCE:
1. User → Browser: Fill booking form
2. Browser → API Gateway: POST /api/bookings (JWT token)
3. API Gateway → BookingsController: Forward request
4. BookingsController → BookingsController: Validate JWT
5. BookingsController → BookingsController: Validate input (XSS check)
6. BookingsController → Booking: Create() factory method
7. Booking → Booking: Validate business rules
8. BookingsController → BookingRepository: AddAsync(booking)
9. BookingRepository → DbContext: Add(booking)
10. BookingRepository → DbContext: SaveChangesAsync()
11. DbContext → MySQL: INSERT INTO Bookings
12. MySQL → DbContext: Success
13. BookingsController → Booking: GetDomainEvents()
14. BookingsController → KafkaProducer: PublishAsync(BookingCreatedEvent)
15. KafkaProducer → Kafka: Send event
16. BookingsController → API Gateway: 201 Created (BookingDto)
17. API Gateway → Browser: 201 Created
18. Browser → User: Show success message

Use standard UML sequence diagram notation with lifelines and activation boxes.
```

---

## 9️⃣ **STATE DIAGRAM - BOOKING STATUS**

**Tool:** Lucidchart UML
**Formål:** Vise booking lifecycle states

**Prompt:**
```
Create a UML State Diagram for Booking lifecycle:

STATES:
- [Initial State] (black dot)
- Pending (initial state after creation)
- Confirmed (payment received)
- Cancelled (user cancelled)
- Completed (check-out done)
- [Final State] (black dot with circle)

TRANSITIONS:
- [Initial] → Pending: Create booking
- Pending → Confirmed: Confirm() / payment received
- Pending → Cancelled: Cancel() / user cancels
- Confirmed → Cancelled: Cancel() / user cancels (with refund)
- Confirmed → Completed: Complete() / check-out date passed
- Cancelled → [Final]: Archive
- Completed → [Final]: Archive

GUARDS:
- Pending → Confirmed: [payment.Status == Paid]
- Confirmed → Cancelled: [cancellation.RefundAmount > 0]

ACTIONS:
- On Pending: Send confirmation email
- On Confirmed: Send booking voucher
- On Cancelled: Send cancellation email, process refund
- On Completed: Send review request

Use UML state diagram notation with rounded rectangles for states.
```

---

## 🔟 **COMPONENT DIAGRAM**

**Tool:** Lucidchart UML
**Formål:** Vise komponenter og deres interfaces

**Prompt:**
```
Create a UML Component Diagram showing:

COMPONENTS:

Presentation Layer:
- <<component>> Blazor UI
  - Provides: IUserInterface
  - Requires: IApiService

- <<component>> REST API Controllers
  - Provides: IRestApi
  - Requires: IRepository, IKafkaProducer

Application Layer:
- <<component>> ApiService
  - Provides: IApiService
  - Requires: IHttpClient

- <<component>> Kafka Producer
  - Provides: IKafkaProducer
  - Requires: IKafkaBroker

Domain Layer:
- <<component>> Domain Model
  - Provides: IEntity, IValueObject, IDomainEvent

- <<component>> Repository Interfaces
  - Provides: IRepository<T>

Infrastructure Layer:
- <<component>> EF Core Repositories
  - Provides: IRepository<T>
  - Requires: IDbContext

- <<component>> DbContext
  - Provides: IDbContext
  - Requires: IDatabase

- <<component>> MySQL Database
  - Provides: IDatabase

External:
- <<component>> Kafka Broker
  - Provides: IKafkaBroker

- <<component>> YARP Gateway
  - Provides: IApiGateway
  - Requires: IRestApi

Show interfaces as lollipops (provided) and sockets (required).
Use dependency arrows between components.
```

---

## 1️⃣1️⃣ **CLASS DIAGRAM - DOMAIN MODEL**

**Tool:** Lucidchart UML
**Formål:** Vise domain entities og value objects

**Prompt:**
```
Create a UML Class Diagram for the Domain Model:

ABSTRACT CLASSES:
- Entity<TId>
  - Properties: Id (TId), CreatedAt (DateTime)
  - Methods: Equals(), GetHashCode()

- ValueObject
  - Methods: Equals(), GetHashCode(), GetAtomicValues()

ENTITIES (inherit from Entity<TId>):
- Booking : Entity<BookingId>
  - Properties: GuestId, CampsiteId, AccommodationTypeId, CheckInDate, CheckOutDate, Status, TotalPrice, NumberOfAdults, NumberOfChildren
  - Methods: Create(), Confirm(), Cancel(), Complete(), AddDomainEvent()

- User : Entity<UserId>
  - Properties: Email, FirstName, LastName, Phone, Country, PasswordHash, IsActive
  - Methods: Create(), UpdateProfile(), ChangePassword()

- Campsite : Entity<CampsiteId>
  - Properties: Name, Region, Description, Latitude, Longitude, IsActive
  - Methods: Create(), AddAccommodationType(), AddAmenity()

VALUE OBJECTS (inherit from ValueObject):
- Email
  - Properties: Value (string)
  - Methods: Create(), Validate()

- Money
  - Properties: Amount (decimal), Currency (string)
  - Methods: Create(), Add(), Subtract()

- DateRange
  - Properties: StartDate, EndDate
  - Methods: Create(), Overlaps(), GetNumberOfNights()

- BookingStatus (enum)
  - Values: Pending, Confirmed, Cancelled, Completed

STRONGLY-TYPED IDs:
- BookingId : IEquatable<BookingId>
- UserId : IEquatable<UserId>
- CampsiteId : IEquatable<CampsiteId>

Show relationships:
- Booking has-a Email (composition)
- Booking has-a Money (composition)
- Booking has-a DateRange (composition)
- Booking has-a BookingStatus (composition)

Use UML notation with visibility (+, -, #), stereotypes (<<abstract>>, <<enum>>).
```

---

## 1️⃣2️⃣ **KAFKA EVENT FLOW DIAGRAM**

**Tool:** Lucidchart (Flowchart)
**Formål:** Vise event-driven architecture flow

**Prompt:**
```
Create a Kafka Event Flow Diagram:

PRODUCERS:
- BookingsController
  - Publishes: BookingCreatedEvent, BookingCancelledEvent

- UsersController
  - Publishes: UserCreatedEvent

- PaymentsController
  - Publishes: PaymentProcessedEvent

KAFKA BROKER:
- Topic: booking-events
- Topic: user-events
- Topic: payment-events

CONSUMERS:
- EmailConsumer (Background Service)
  - Subscribes to: booking-events, user-events, payment-events
  - Actions:
    - BookingCreatedEvent → Send confirmation email
    - BookingCancelledEvent → Send cancellation email
    - UserCreatedEvent → Send welcome email
    - PaymentProcessedEvent → Send receipt email

- SMSConsumer (Background Service)
  - Subscribes to: booking-events
  - Actions:
    - BookingCreatedEvent → Send SMS confirmation

- NotificationConsumer (Background Service)
  - Subscribes to: booking-events, user-events
  - Actions:
    - BookingCreatedEvent → Create in-app notification
    - UserCreatedEvent → Create welcome notification

FLOW:
1. API Controller → Kafka Producer → Kafka Broker (publish event)
2. Kafka Broker → Kafka Consumer → External Service (consume event)

Show message flow with arrows. Label topics and event types.
```

---

## 1️⃣3️⃣ **OWASP SECURITY DIAGRAM**

**Tool:** Lucidchart (Flowchart)
**Formål:** Vise sikkerhedsimplementering

**Prompt:**
```
Create an OWASP Security Implementation Diagram:

THREAT 1: SQL Injection (A03:2021)
- Attack Vector: Malicious SQL in user input
- Protection Layer 1: EF Core Parameterized Queries
- Protection Layer 2: Strongly-Typed Value Objects (Email, Money)
- Protection Layer 3: Input Validation
- Status: ✅ PROTECTED

THREAT 2: XSS (A07:2021)
- Attack Vector: Malicious JavaScript in user input
- Protection Layer 1: InputValidator.ValidateInput() (regex patterns)
- Protection Layer 2: Blazor Automatic HTML Encoding
- Protection Layer 3: Content Security Policy (CSP) headers
- Status: ✅ PROTECTED

THREAT 3: CSRF (A05:2021)
- Attack Vector: Forged requests from malicious site
- Protection Layer 1: Anti-Forgery Tokens
- Protection Layer 2: SameSite=Strict Cookies
- Protection Layer 3: HTTPS Enforcement
- Status: ✅ PROTECTED

THREAT 4: Broken Authentication (A07:2021)
- Attack Vector: Weak passwords, session hijacking
- Protection Layer 1: JWT Token Authentication
- Protection Layer 2: Password Hashing (BCrypt)
- Protection Layer 3: Token Expiration (24 hours)
- Status: ✅ PROTECTED

Show attack flow (red arrows) and protection layers (green shields).
Use traffic light colors: Green (Protected), Yellow (Partial), Red (Vulnerable).
```

---

## 📊 **DIAGRAM PRIORITERING TIL EKSAMEN**

### **MUST HAVE (Kritiske):**
1. ✅ **ERD** - Viser database design
2. ✅ **Clean Architecture** - Viser lagdelt arkitektur
3. ✅ **System Architecture** - Viser deployment
4. ✅ **Use Case** - Viser funktionalitet
5. ✅ **Sequence Diagram** - Viser flow

### **SHOULD HAVE (Vigtige):**
6. ✅ **Scale Cube** - Viser skalering strategi
7. ✅ **Component Diagram** - Viser komponenter
8. ✅ **Deployment Diagram** - Viser Docker setup
9. ✅ **State Diagram** - Viser booking lifecycle

### **NICE TO HAVE (Ekstra point):**
10. ✅ **FURPS+** - Viser kvalitetsattributter
11. ✅ **Class Diagram** - Viser domain model
12. ✅ **Kafka Event Flow** - Viser event-driven
13. ✅ **OWASP Security** - Viser sikkerhed

---

## 🎯 **TIPS TIL LUCIDCHART**

1. **Start med template:** Vælg "UML" eller "ERD" template
2. **Brug farver:** Grøn (implementeret), Gul (partial), Rød (ikke implementeret)
3. **Tilføj noter:** Forklar komplekse dele med text boxes
4. **Eksporter som PNG:** Høj opløsning (300 DPI) til rapport
5. **Gem som PDF:** Til digital aflevering

---

## 📝 **NÆSTE SKRIDT**

1. ✅ Opret Lucidchart account (gratis education license)
2. ✅ Generer alle 13 diagrammer
3. ✅ Eksporter som PNG/PDF
4. ✅ Indsæt i rapport med forklaringer
5. ✅ Øv præsentation af hvert diagram

**Estimeret tid:** 3-4 timer for alle diagrammer

---

**HELD OG LYKKE MED EKSAMEN!** 🚀

Show dependency arrows pointing INWARD (outer layers depend on inner layers, never the reverse).
Label: "Dependency Rule: Dependencies point inward"
```

---

## 4️⃣ **SYSTEM ARCHITECTURE DIAGRAM**

**Tool:** Lucidchart (Network diagram)
**Formål:** Vise deployment arkitektur med alle komponenter

**Prompt:**
```
Create a system architecture diagram showing:

CLIENT TIER:
- Web Browser (Blazor WebAssembly UI)
- Postman (API Testing)

API GATEWAY TIER:
- YARP API Gateway (Port 8001)
  - Load Balancing (RoundRobin)
  - Health Checks
  - Rate Limiting

APPLICATION TIER:
- BookMyHome API Instance 1 (Port 7001)
- BookMyHome API Instance 2 (Port 7002)
- BookMyHome API Instance 3 (Port 7003)
- Kafka Consumer (Background Service)

DATA TIER:
- MySQL Database (Port 3306)
- Kafka Broker (Port 9092)
- Zookeeper (Port 2181)

INFRASTRUCTURE:
- Docker Compose (orchestration)
- Nginx Reverse Proxy (Port 80/443)

Show connections:
- Browser → API Gateway (HTTPS)
- API Gateway → API Instances (Load Balanced)
- API Instances → MySQL (EF Core)
- API Instances → Kafka Producer
- Kafka Consumer → Kafka Broker
- Kafka Consumer → Email/SMS Services (external)

Use boxes for components, arrows for data flow, and group related components.
```

---


