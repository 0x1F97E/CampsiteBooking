# BookMyHome - 3. Semester Eksamensprojekt

## 📋 **PROJEKT OVERSIGT**

**Projekt:** BookMyHome - Campsite Booking System  
**Studerende:** [Dit navn]  
**Semester:** 3. semester  
**Dato:** November 2025  
**Teknologi:** .NET 9.0, Blazor Server, MySQL, Kafka, Docker

---

## ✅ **EKSAMENSKRAV - OPFYLDT**

### **Programmering (Modul 8):**
- ✅ **Concurrency handling** - EF Core transactions med IsolationLevel.Serializable
- ✅ **Domain-centric architecture** - Clean Architecture med DDD principper
- ✅ **Browser-based frontend** - Blazor Server med MudBlazor
- ✅ **REST API med CRUD** - BookingsController, UsersController, CampsitesController
- ✅ **SQL database** - MySQL 8.0 med EF Core
- ✅ **SOLID principper** - Dependency Inversion, Single Responsibility, etc.
- ✅ **Unit tests** - 372 xUnit tests (domain layer)
- ✅ **Swagger/Postman dokumentation** - OpenAPI spec + Postman guide
- ✅ **Login & Authentication** - JWT tokens med Bearer authentication
- ✅ **OWASP Top 10 sikkerhed** - SQL Injection, XSS, CSRF protection

### **Teknologi (Modul 7):**
- ✅ **Docker Desktop** - Docker Compose med MySQL, Kafka, Zookeeper
- ✅ **Dockerfile + Docker Compose** - Multi-container setup
- ✅ **Port mapping dokumentation** - DOCKER-DOCUMENTATION.md
- ✅ **REST maturity level** - Level 2 (HTTP verbs + status codes)
- ✅ **Microservices analyse** - Modular Monolith med event-driven arkitektur
- ✅ **Kafka event-driven** - Asynchronous communication via Kafka
- ✅ **API Gateway** - YARP (Yet Another Reverse Proxy) implementeret

---

## 🏗️ **ARKITEKTUR**

### **Clean Architecture (Domain-Centric):**

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│  - Blazor Components (Pages/, Components/)              │
│  - REST API Controllers (Controllers/)                  │
│  - DTOs (Controllers/DTOs/)                             │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                   Application Layer                     │
│  - ApiService (Services/ApiService.cs)                  │
│  - Kafka Producer/Consumer (Infrastructure/Kafka/)      │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                     Domain Layer                        │
│  - Entities (Models/)                                   │
│  - Value Objects (Email, Money, DateRange)              │
│  - Repository Interfaces (Models/Repositories/)         │
│  - Domain Events (Events/)                              │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                 Infrastructure Layer                    │
│  - EF Core DbContext (Data/CampsiteBookingDbContext.cs) │
│  - Repository Implementations (Data/Repositories/)      │
│  - Kafka Integration (Infrastructure/Kafka/)            │
│  - Security (Infrastructure/Security/)                  │
└─────────────────────────────────────────────────────────┘
```

---

## 📊 **DOMAIN MODEL**

### **Aggregates (22 Entities):**
- **Booking** - Booking, Guest, Payment, Review, Cancellation
- **User** - User, NewsletterSubscription
- **Campsite** - Campsite, AccommodationType, Amenity, CampsiteAmenity, AccommodationTypeAmenity, Availability, Pricing, SeasonalPricing
- **Maintenance** - MaintenanceSchedule, MaintenanceLog
- **Promotion** - Promotion, PromotionUsage
- **Notification** - Notification
- **ActivityLog** - ActivityLog

### **Value Objects:**
- Email, Money, DateRange, BookingStatus, PaymentStatus, ReviewRating, etc.

### **Strongly-Typed IDs:**
- BookingId, GuestId, UserId, CampsiteId, AccommodationTypeId, etc.

---

## 🔌 **REST API ENDPOINTS**

### **Authentication:**
```
POST   /api/auth/register  - Register new user
POST   /api/auth/login     - Login user (returns JWT token)
```

### **Bookings (Requires JWT):**
```
GET    /api/bookings       - Get all bookings
GET    /api/bookings/{id}  - Get booking by ID
POST   /api/bookings       - Create new booking
PUT    /api/bookings/{id}  - Update booking
DELETE /api/bookings/{id}  - Cancel booking
```

### **Users (Requires JWT):**
```
GET    /api/users/{id}     - Get user by ID
```

### **Campsites (Requires JWT):**
```
GET    /api/campsites      - Get all campsites
GET    /api/campsites/{id} - Get campsite by ID
```

**Swagger UI:** https://localhost:7001/swagger

---

## 🐳 **DOCKER SETUP**

### **Services:**
- **MySQL** - Database (Port 3306)
- **Kafka** - Message broker (Port 9092)
- **Zookeeper** - Kafka coordination (Port 2181)
- **Nginx** - Reverse proxy (Port 80/443) - Optional

### **Start systemet:**
```bash
# Start Docker services
docker-compose up -d

# Run migrations
dotnet ef database update

# Start .NET application
dotnet run
```

### **Stop systemet:**
```bash
docker-compose down
```

---

## 🔐 **SIKKERHED (OWASP TOP 10)**

### **Implementeret:**

1. **SQL Injection Protection (A03:2021)**
   - EF Core parameterized queries
   - Strongly-typed Value Objects
   - No raw SQL queries

2. **XSS Protection (A07:2021)**
   - InputValidator helper class
   - Regex pattern detection
   - Blazor automatic HTML encoding

3. **CSRF Protection (A05:2021)**
   - Anti-forgery tokens
   - HttpOnly + Secure + SameSite cookies
   - HTTPS enforcement

**Dokumentation:** Se `OWASP-SECURITY.md`

---

## 📚 **DOKUMENTATION**

| Dokument | Beskrivelse |
|----------|-------------|
| `POSTMAN-API-TESTS.md` | Postman test collection med alle API endpoints |
| `DOCKER-DOCUMENTATION.md` | Docker Compose setup, port mapping, deployment |
| `REST-MATURITY-LEVEL.md` | Richardson Maturity Model analyse (Level 2) |
| `SCALE-CUBE-ANALYSIS.md` | Scale Cube analyse (X-axis, Y-axis, Z-axis) |
| `MICROSERVICES-ARCHITECTURE.md` | Microservices analyse (Modular Monolith) |
| `OWASP-SECURITY.md` | OWASP Top 10 sikkerhed implementering |
| `API-GATEWAY-YARP.md` | YARP API Gateway implementering og konfiguration |
| `DIAGRAM-PROMPTS.md` | Lucidchart prompts til 13 UML/ERD diagrammer |

---

## 🧪 **TESTS**

### **Unit Tests:**
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

**Test Coverage:**
- 372 unit tests (domain layer)
- 100% coverage af domain entities
- 100% coverage af value objects

---

## 🚀 **DEPLOYMENT**

### **Development:**
```bash
dotnet run
```
- HTTP: http://localhost:5000
- HTTPS: https://localhost:7001

### **Production (Docker):**
```bash
docker-compose up -d
```

---

## 📊 **TEKNISK STACK**

| Kategori | Teknologi | Version |
|----------|-----------|---------|
| Framework | .NET | 9.0 |
| UI | Blazor Server | 9.0 |
| UI Library | MudBlazor | 8.13.0 |
| Database | MySQL | 8.0 |
| ORM | Entity Framework Core | 8.0.2 |
| Message Broker | Kafka | 7.5.0 |
| Authentication | JWT Bearer | 8.0.0 |
| API Docs | Swagger/OpenAPI | 6.5.0 |
| API Gateway | YARP | 2.1.0 |
| Testing | xUnit | 2.9.3 |
| Containerization | Docker | Latest |

---

## 🎯 **EKSAMENS FOKUSPUNKTER**

### **1. Domain-Centric Architecture:**
- Clean Architecture med DDD principper
- Aggregate Roots, Entities, Value Objects
- Domain Events
- Repository Pattern
- SOLID principper

### **2. REST API:**
- Richardson Maturity Level 2
- HTTP verbs (GET, POST, PUT, DELETE)
- HTTP status codes (200, 201, 204, 400, 401, 404)
- JWT authentication
- Swagger dokumentation

### **3. Event-Driven Architecture:**
- Kafka Producer/Consumer
- Domain Events
- Asynchronous communication
- Eventual consistency

### **4. Sikkerhed:**
- OWASP Top 10 (SQL Injection, XSS, CSRF)
- JWT authentication
- Input validation
- HTTPS enforcement

### **5. Skalering:**
- Scale Cube analyse
- X-axis scaling (horizontal duplication)
- Docker Compose multi-instance setup

### **6. API Gateway:**
- YARP (Yet Another Reverse Proxy)
- Load balancing (RoundRobin)
- Health checks
- Single entry point pattern

---

**Projekt Status:** ✅ COMPLETE  
**Alle eksamenskrav opfyldt:** ✅ JA  
**Klar til eksamen:** ✅ JA

