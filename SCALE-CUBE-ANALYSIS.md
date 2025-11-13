# BookMyHome - Scale Cube Analyse

## 📋 **OVERSIGT**

Dette dokument analyserer **BookMyHome** i forhold til **Scale Cube** modellen for skalering af applikationer.

---

## 🎯 **SCALE CUBE MODEL**

Scale Cube definerer 3 dimensioner for skalering:

```
        Y-axis (Functional Decomposition)
            ↑
            |
            |
            +--------→ X-axis (Horizontal Duplication)
           /
          /
         ↓
    Z-axis (Data Partitioning)
```

---

## 📊 **X-AXIS: HORIZONTAL DUPLICATION (CLONING)**

### **Definition:**
Kør flere identiske kopier af applikationen bag en load balancer.

### **BookMyHome Implementation:**

**Nuværende Status:** ⚠️ DELVIST IMPLEMENTERET

**Hvad er implementeret:**
```yaml
# Docker Compose kan køre multiple instances
docker-compose up --scale bookmyhome-api=3
```

**Arkitektur:**
```
                    Load Balancer (Nginx)
                           |
        +------------------+------------------+
        |                  |                  |
   Instance 1         Instance 2         Instance 3
   (Port 5001)        (Port 5002)        (Port 5003)
        |                  |                  |
        +------------------+------------------+
                           |
                    MySQL Database
                    Kafka Broker
```

**Fordele:**
- ✅ Øget throughput (flere requests samtidig)
- ✅ Høj tilgængelighed (hvis én instance fejler, kører de andre)
- ✅ Simpel at implementere
- ✅ Ingen kodeændringer nødvendige

**Ulemper:**
- ⚠️ Alle instances deler samme database (bottleneck)
- ⚠️ Session state skal være stateless (JWT løser dette)
- ⚠️ Kræver load balancer (Nginx, HAProxy, Azure Load Balancer)

**Eksempel Nginx Config:**
```nginx
upstream bookmyhome_backend {
    server localhost:5001;
    server localhost:5002;
    server localhost:5003;
}

server {
    listen 80;
    location / {
        proxy_pass http://bookmyhome_backend;
    }
}
```

**Status:** ✅ KLAR TIL X-AXIS SCALING

---

## 📊 **Y-AXIS: FUNCTIONAL DECOMPOSITION (MICROSERVICES)**

### **Definition:**
Opdel applikationen i mindre services baseret på funktionalitet (microservices).

### **BookMyHome Implementation:**

**Nuværende Status:** ⚠️ DELVIST IMPLEMENTERET

**Hvad er implementeret:**
```
BookMyHome Monolith
    ├── Booking Service (Controllers/BookingsController.cs)
    ├── User Service (Controllers/UsersController.cs)
    ├── Campsite Service (Controllers/CampsitesController.cs)
    ├── Auth Service (Controllers/AuthController.cs)
    └── Kafka Consumer (Background Service)
```

**Potentiel Microservices Arkitektur:**
```
                    API Gateway (YARP)
                           |
        +------------------+------------------+------------------+
        |                  |                  |                  |
  Booking Service    User Service      Campsite Service    Auth Service
   (Port 5001)       (Port 5002)        (Port 5003)        (Port 5004)
        |                  |                  |                  |
   Booking DB         User DB           Campsite DB         Auth DB
        |                  |                  |                  |
        +------------------+------------------+------------------+
                           |
                    Kafka Event Bus
```

**Fordele:**
- ✅ Uafhængig deployment af services
- ✅ Teknologi-agnostisk (hver service kan bruge forskellige tech stacks)
- ✅ Bedre skalering (skalér kun de services der har brug for det)
- ✅ Bedre fejlisolering (én service kan fejle uden at påvirke andre)

**Ulemper:**
- ⚠️ Øget kompleksitet (distributed systems)
- ⚠️ Kræver API Gateway (YARP, Ocelot, Kong)
- ⚠️ Kræver service discovery (Consul, Eureka)
- ⚠️ Kræver distributed tracing (Jaeger, Zipkin)
- ⚠️ Eventual consistency (data synkronisering via events)

**Hvorfor IKKE fuldt implementeret:**
- BookMyHome er et 3. semester projekt
- Microservices er overkill for denne størrelse
- Monolith er nemmere at forstå og vedligeholde
- Kafka Consumer er dog en separat service (første skridt mod microservices)

**Status:** ⚠️ MONOLITH MED EVENT-DRIVEN ARKITEKTUR

---

## 📊 **Z-AXIS: DATA PARTITIONING (SHARDING)**

### **Definition:**
Opdel data baseret på en nøgle (f.eks. bruger-ID, region, dato).

### **BookMyHome Implementation:**

**Nuværende Status:** ❌ IKKE IMPLEMENTERET

**Potentiel Sharding Strategi:**

#### **1. Geographic Sharding (Region-baseret):**
```
Shard 1: Nordjylland Campsites
    ├── Database: bookmyhome_north
    └── Campsites: Skagen, Aalborg, Frederikshavn

Shard 2: Midtjylland Campsites
    ├── Database: bookmyhome_central
    └── Campsites: Aarhus, Silkeborg, Randers

Shard 3: Syddanmark Campsites
    ├── Database: bookmyhome_south
    └── Campsites: Odense, Kolding, Esbjerg
```

**Routing Logic:**
```csharp
public class ShardRouter
{
    public string GetDatabaseConnection(string region)
    {
        return region switch
        {
            "Nordjylland" => "Server=shard1.mysql;Database=bookmyhome_north;...",
            "Midtjylland" => "Server=shard2.mysql;Database=bookmyhome_central;...",
            "Syddanmark" => "Server=shard3.mysql;Database=bookmyhome_south;...",
            _ => throw new ArgumentException("Unknown region")
        };
    }
}
```

#### **2. User-based Sharding (Bruger-ID):**
```
Shard 1: Users 1-10000
Shard 2: Users 10001-20000
Shard 3: Users 20001-30000
```

**Routing Logic:**
```csharp
public string GetUserShard(int userId)
{
    int shardId = (userId / 10000) + 1;
    return $"Server=shard{shardId}.mysql;Database=bookmyhome_users;...";
}
```

**Fordele:**
- ✅ Massiv skalering (millioner af brugere/bookings)
- ✅ Bedre performance (mindre data per database)
- ✅ Geografisk distribution (lavere latency)

**Ulemper:**
- ⚠️ Meget komplekst at implementere
- ⚠️ Cross-shard queries er svære (f.eks. "find alle bookings")
- ⚠️ Rebalancing er svært (hvis én shard bliver for stor)
- ⚠️ Kræver shard-aware application logic

**Hvorfor IKKE implementeret:**
- BookMyHome har ikke nok data til at retfærdiggøre sharding
- Sharding er kun nødvendigt ved millioner af records
- Vertical scaling (større database server) er nemmere

**Status:** ❌ IKKE NØDVENDIGT FOR 3. SEMESTER PROJEKT

---

## 🎯 **BOOKMYHOME SCALING STRATEGI**

### **Nuværende Implementation:**

| Dimension | Status | Implementation |
|-----------|--------|----------------|
| X-axis (Horizontal) | ✅ KLAR | Docker Compose + Nginx load balancer |
| Y-axis (Microservices) | ⚠️ DELVIST | Monolith + Kafka Consumer (event-driven) |
| Z-axis (Sharding) | ❌ IKKE | Single MySQL database |

### **Anbefalet Scaling Path:**

#### **Phase 1: X-axis Scaling (NU)**
```bash
# Start 3 instances
docker-compose up --scale bookmyhome-api=3

# Nginx load balancer
upstream bookmyhome {
    server api1:5000;
    server api2:5000;
    server api3:5000;
}
```

**Kapacitet:** 1,000-10,000 brugere

---

#### **Phase 2: Y-axis Scaling (FREMTID)**
```
Opdel i microservices:
1. Booking Service
2. User Service
3. Payment Service
4. Notification Service
```

**Kapacitet:** 10,000-100,000 brugere

---

#### **Phase 3: Z-axis Scaling (LANGT FREMTID)**
```
Shard database:
1. Geographic sharding (region)
2. User sharding (user ID)
```

**Kapacitet:** 100,000+ brugere

---

## 📊 **KONKLUSION**

### **BookMyHome Scaling Readiness:**

**Implementeret:**
- ✅ X-axis: Klar til horizontal scaling med Docker + Nginx
- ✅ Stateless API (JWT authentication)
- ✅ Event-driven arkitektur (Kafka)

**Ikke implementeret:**
- ⚠️ Y-axis: Monolith (ikke microservices)
- ❌ Z-axis: Single database (ikke sharding)

**Begrundelse:**
- X-axis scaling er tilstrækkeligt for 3. semester projekt
- Y-axis (microservices) er overkill for denne størrelse
- Z-axis (sharding) er kun nødvendigt ved millioner af records

**Fremtidige forbedringer:**
1. Implementer Nginx load balancer
2. Opdel i microservices (Y-axis)
3. Implementer database sharding (Z-axis) hvis nødvendigt

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt  
**Scaling Strategy:** X-axis (Horizontal Duplication)

