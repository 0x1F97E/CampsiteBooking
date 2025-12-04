# BookMyHome - Microservices Arkitektur Analyse

## 📋 **OVERSIGT**

Dette dokument analyserer **BookMyHome** i forhold til microservices arkitektur principper.

---

## 🎯 **ER BOOKMYHOME EN MICROSERVICES ARKITEKTUR?**

**Svar:** ⚠️ **HYBRID - Modular Monolith med Event-Driven Elementer**

BookMyHome er primært en **monolith** med **event-driven** kommunikation via Kafka.

---

## 📊 **MICROSERVICES KARAKTERISTIKA**

### **1. Independently Deployable Services**

**Definition:** Hver service kan deployes uafhængigt uden at påvirke andre services.

**BookMyHome Status:** ❌ IKKE OPFYLDT

**Nuværende Arkitektur:**
```
BookMyHome.dll (Single Deployment Unit)
    ├── Controllers/BookingsController.cs
    ├── Controllers/UsersController.cs
    ├── Controllers/CampsitesController.cs
    ├── Controllers/AuthController.cs
    └── Infrastructure/Kafka/KafkaConsumer.cs
```

**Alle controllers er i samme deployment unit** → Kan ikke deployes separat.

---

### **2. Decentralized Data Management**

**Definition:** Hver service har sin egen database (Database per Service pattern).

**BookMyHome Status:** ❌ IKKE OPFYLDT

**Nuværende Arkitektur:**
```
Single MySQL Database (CampsiteBookingDb)
    ├── Bookings table
    ├── Users table
    ├── Campsites table
    ├── AccommodationTypes table
    └── ... (22 tables total)
```

**Alle services deler samme database** → Tight coupling.

---

### **3. Asynchronous Communication**

**Definition:** Services kommunikerer via events/messages (ikke direkte API calls).

**BookMyHome Status:** ✅ DELVIST OPFYLDT

**Implementeret:**
```csharp
// BookingsController publishes events to Kafka
var events = booking.GetDomainEvents();
foreach (var domainEvent in events)
{
    await _kafkaProducer.PublishAsync(domainEvent, cancellationToken);
}

// KafkaConsumer listens to events
public class KafkaConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _consumer.ConsumeAsync(stoppingToken))
        {
            // Handle BookingCreatedEvent
            // Handle UserCreatedEvent
            // Send emails, SMS, notifications
        }
    }
}
```

**Kafka Topics:**
- `booking-events` → BookingCreatedEvent, BookingCancelledEvent
- `user-events` → UserCreatedEvent
- `payment-events` → PaymentProcessedEvent (future)

**Status:** ✅ EVENT-DRIVEN COMMUNICATION IMPLEMENTERET

---

### **4. Service Autonomy**

**Definition:** Hver service kan udvikles, testes og deployes af separate teams.

**BookMyHome Status:** ❌ IKKE OPFYLDT

**Nuværende:**
- Single codebase
- Single team (dig)
- Single deployment pipeline

---

### **5. API Gateway**

**Definition:** Single entry point for alle client requests (routing, authentication, rate limiting).

**BookMyHome Status:** ❌ IKKE IMPLEMENTERET

**Nuværende:**
```
Client → https://localhost:7001/api/bookings (Direct API call)
Client → https://localhost:7001/api/users (Direct API call)
```

**Potentiel YARP Gateway:**
```
Client → API Gateway (YARP) → Booking Service
                            → User Service
                            → Auth Service
```

**Status:** ⚠️ PLANLAGT MEN IKKE IMPLEMENTERET (FASE 9 CANCELLED)

---

## 📊 **BOOKMYHOME ARKITEKTUR ANALYSE**

### **Nuværende Arkitektur: Modular Monolith**

```
┌─────────────────────────────────────────────────────────┐
│           BookMyHome Monolith (.NET 9.0)                │
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │   Booking    │  │     User     │  │   Campsite   │ │
│  │  Controller  │  │  Controller  │  │  Controller  │ │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘ │
│         │                  │                  │         │
│  ┌──────▼──────────────────▼──────────────────▼──────┐ │
│  │         Shared Database (MySQL)                   │ │
│  │  - Bookings, Users, Campsites, etc.              │ │
│  └───────────────────────────────────────────────────┘ │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │         Kafka Producer (Manual Publish)          │  │
│  └──────────────────┬───────────────────────────────┘  │
└─────────────────────┼───────────────────────────────────┘
                      │
                      ▼
              ┌───────────────┐
              │  Kafka Broker │
              └───────┬───────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│         Kafka Consumer (Background Service)             │
│  - Listens to booking-events, user-events              │
│  - Sends emails, SMS, notifications                    │
└─────────────────────────────────────────────────────────┘
```

**Karakteristika:**
- ✅ Single deployment unit
- ✅ Shared database
- ✅ Event-driven communication (Kafka)
- ✅ Modular controllers (logical separation)
- ❌ NOT true microservices

---

### **Potentiel Microservices Arkitektur (Fremtid):**

```
                    ┌─────────────────┐
                    │   API Gateway   │
                    │     (YARP)      │
                    └────────┬────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
        ▼                    ▼                    ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│    Booking    │    │     User      │    │   Campsite    │
│    Service    │    │    Service    │    │    Service    │
│  (Port 5001)  │    │  (Port 5002)  │    │  (Port 5003)  │
└───────┬───────┘    └───────┬───────┘    └───────┬───────┘
        │                    │                    │
        ▼                    ▼                    ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│  Booking DB   │    │   User DB     │    │  Campsite DB  │
│    (MySQL)    │    │   (MySQL)     │    │   (MySQL)     │
└───────────────┘    └───────────────┘    └───────────────┘
        │                    │                    │
        └────────────────────┼────────────────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  Kafka Event Bus│
                    │  (Async Comms)  │
                    └─────────────────┘
```

**Fordele:**
- ✅ Independent deployment
- ✅ Technology diversity (hver service kan bruge forskellige tech stacks)
- ✅ Better scalability (skalér kun de services der har brug for det)
- ✅ Fault isolation (én service kan fejle uden at påvirke andre)

**Ulemper:**
- ⚠️ Increased complexity (distributed systems)
- ⚠️ Network latency (inter-service communication)
- ⚠️ Data consistency challenges (eventual consistency)
- ⚠️ Distributed transactions (Saga pattern)

---

## 🎯 **HVORFOR IKKE MICROSERVICES?**

### **Begrundelse for Modular Monolith:**

1. **Projekt Størrelse:**
   - BookMyHome er et 3. semester eksamensprojekt
   - ~5,000 lines of code
   - Microservices er overkill for denne størrelse

2. **Team Størrelse:**
   - Single developer (dig)
   - Microservices er designet til multiple teams

3. **Kompleksitet:**
   - Microservices kræver:
     - Service discovery (Consul, Eureka)
     - API Gateway (YARP, Ocelot)
     - Distributed tracing (Jaeger, Zipkin)
     - Circuit breakers (Polly)
     - Saga pattern for distributed transactions
   - Dette er for komplekst for 3. semester

4. **Deployment:**
   - Monolith er nemmere at deploye (single Docker container)
   - Microservices kræver Kubernetes/Docker Swarm

5. **Performance:**
   - Monolith har lavere latency (in-process calls)
   - Microservices har network overhead

---

## ✅ **HVAD ER IMPLEMENTERET?**

### **Event-Driven Architecture (Kafka):**

**BookMyHome bruger Kafka til asynchronous communication:**

```csharp
// Domain Events
public record BookingCreatedEvent(
    int BookingId,
    int GuestId,
    int CampsiteId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    decimal TotalPrice
) : IDomainEvent;

// Publish event
await _kafkaProducer.PublishAsync(new BookingCreatedEvent(...));

// Consume event
public class KafkaConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _consumer.ConsumeAsync(stoppingToken))
        {
            var eventType = message.Headers.GetString("event-type");
            
            if (eventType == "BookingCreatedEvent")
            {
                // Send confirmation email
                // Send SMS notification
                // Update analytics
            }
        }
    }
}
```

**Fordele:**
- ✅ Decoupling (controllers don't need to know about email/SMS logic)
- ✅ Scalability (Kafka Consumer kan køre på separate server)
- ✅ Reliability (events er persistent i Kafka)
- ✅ Første skridt mod microservices

---

## 📊 **KONKLUSION**

### **BookMyHome Arkitektur:**

| Karakteristika | Status | Kommentar |
|----------------|--------|-----------|
| Independent Deployment | ❌ | Monolith (single deployment unit) |
| Database per Service | ❌ | Shared MySQL database |
| Asynchronous Communication | ✅ | Kafka event-driven |
| Service Autonomy | ❌ | Single codebase |
| API Gateway | ❌ | Direct API calls |

**Samlet vurdering:** ⚠️ **MODULAR MONOLITH MED EVENT-DRIVEN ELEMENTER**

**Ikke en microservices arkitektur, men:**
- ✅ Godt fundament for fremtidig migration til microservices
- ✅ Event-driven communication er implementeret (Kafka)
- ✅ Modular controllers (logisk separation)
- ✅ Domain-centric architecture (Clean Architecture)

**Fremtidige forbedringer:**
1. Opdel i separate services (Booking Service, User Service, etc.)
2. Implementer API Gateway (YARP)
3. Database per Service pattern
4. Service discovery (Consul)
5. Distributed tracing (Jaeger)

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt  
**Arkitektur:** Modular Monolith med Event-Driven Communication

