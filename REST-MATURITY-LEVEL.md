# BookMyHome - REST Maturity Level Analyse

## 📋 **OVERSIGT**

Dette dokument analyserer **BookMyHome REST API** i forhold til **Richardson Maturity Model** for REST APIs.

---

## 🎯 **RICHARDSON MATURITY MODEL**

Richardson Maturity Model definerer 4 niveauer af REST API modenhed:

```
Level 3: Hypermedia Controls (HATEOAS)
    ↑
Level 2: HTTP Verbs
    ↑
Level 1: Resources
    ↑
Level 0: The Swamp of POX (Plain Old XML)
```

---

## 📊 **BOOKMYHOME API - LEVEL 2**

**BookMyHome API implementerer Level 2: HTTP Verbs**

---

## ✅ **LEVEL 0: THE SWAMP OF POX**

**Definition:** Alle requests bruger samme endpoint og HTTP method (typisk POST).

**Eksempel (IKKE implementeret):**
```http
POST /api/service
{
  "action": "getBooking",
  "bookingId": 1
}

POST /api/service
{
  "action": "createBooking",
  "data": { ... }
}
```

**Status:** ❌ IKKE BRUGT - Vi bruger ikke POX/RPC-style API

---

## ✅ **LEVEL 1: RESOURCES**

**Definition:** Forskellige endpoints for forskellige ressourcer.

**BookMyHome Implementation:**
```http
GET /api/bookings
GET /api/users
GET /api/campsites
GET /api/auth
```

**Status:** ✅ IMPLEMENTERET

**Fordele:**
- ✅ Klare ressource-baserede endpoints
- ✅ Logisk opdeling af funktionalitet
- ✅ Nemmere at forstå API struktur

---

## ✅ **LEVEL 2: HTTP VERBS**

**Definition:** Bruger HTTP verbs (GET, POST, PUT, DELETE) korrekt + HTTP status codes.

### **BookMyHome Implementation:**

#### **1. Bookings API:**
```http
# Create booking
POST /api/bookings
→ 201 Created (success)
→ 400 Bad Request (validation error)

# Get all bookings
GET /api/bookings
→ 200 OK

# Get booking by ID
GET /api/bookings/1
→ 200 OK (found)
→ 404 Not Found (not found)

# Update booking
PUT /api/bookings/1
→ 200 OK (success)
→ 404 Not Found (not found)

# Delete booking
DELETE /api/bookings/1
→ 204 No Content (success)
→ 404 Not Found (not found)
```

#### **2. Authentication API:**
```http
# Register user
POST /api/auth/register
→ 201 Created (success)
→ 400 Bad Request (validation error)

# Login user
POST /api/auth/login
→ 200 OK (success)
→ 401 Unauthorized (invalid credentials)
```

#### **3. Users API:**
```http
# Get user by ID
GET /api/users/1
→ 200 OK (found)
→ 404 Not Found (not found)
→ 401 Unauthorized (no JWT token)
```

#### **4. Campsites API:**
```http
# Get all campsites
GET /api/campsites
→ 200 OK
→ 401 Unauthorized (no JWT token)

# Get campsite by ID
GET /api/campsites/1
→ 200 OK (found)
→ 404 Not Found (not found)
```

**Status:** ✅ IMPLEMENTERET

**Fordele:**
- ✅ Korrekt brug af HTTP verbs (GET, POST, PUT, DELETE)
- ✅ Korrekt brug af HTTP status codes (200, 201, 204, 400, 401, 404)
- ✅ Idempotent operations (GET, PUT, DELETE)
- ✅ Safe operations (GET)
- ✅ RESTful naming conventions

---

## ❌ **LEVEL 3: HYPERMEDIA CONTROLS (HATEOAS)**

**Definition:** API responses indeholder links til relaterede ressourcer (Hypermedia As The Engine Of Application State).

**Eksempel (IKKE implementeret):**
```json
{
  "bookingId": 1,
  "guestId": 1,
  "status": "Pending",
  "_links": {
    "self": { "href": "/api/bookings/1" },
    "guest": { "href": "/api/users/1" },
    "campsite": { "href": "/api/campsites/1" },
    "cancel": { "href": "/api/bookings/1", "method": "DELETE" },
    "confirm": { "href": "/api/bookings/1/confirm", "method": "POST" }
  }
}
```

**Status:** ❌ IKKE IMPLEMENTERET

**Hvorfor ikke?**
- ⚠️ HATEOAS er komplekst at implementere korrekt
- ⚠️ Ikke et krav for 3. semester eksamen
- ⚠️ Level 2 er tilstrækkeligt for de fleste REST APIs
- ⚠️ Swagger/OpenAPI dokumentation giver samme fordele

**Fremtidig forbedring:**
- Kunne implementeres med HAL (Hypertext Application Language)
- Eller JSON:API specification

---

## 📊 **HTTP STATUS CODES - KOMPLET LISTE**

### **Success Codes (2xx):**
| Code | Meaning | BookMyHome Usage |
|------|---------|------------------|
| 200 OK | Success | GET, PUT requests |
| 201 Created | Resource created | POST /api/bookings, POST /api/auth/register |
| 204 No Content | Success, no body | DELETE /api/bookings/{id} |

### **Client Error Codes (4xx):**
| Code | Meaning | BookMyHome Usage |
|------|---------|------------------|
| 400 Bad Request | Validation error | Invalid input data, XSS detected |
| 401 Unauthorized | Missing/invalid auth | No JWT token or expired token |
| 404 Not Found | Resource not found | Invalid booking/user/campsite ID |

### **Server Error Codes (5xx):**
| Code | Meaning | BookMyHome Usage |
|------|---------|------------------|
| 500 Internal Server Error | Unexpected error | Unhandled exceptions |

---

## 🎯 **KONKLUSION**

### **BookMyHome API = Level 2 (HTTP Verbs)**

**Implementeret:**
- ✅ Level 1: Resources (separate endpoints)
- ✅ Level 2: HTTP Verbs + Status Codes

**Ikke implementeret:**
- ❌ Level 3: HATEOAS (Hypermedia Controls)

**Begrundelse:**
- Level 2 er **industry standard** for de fleste REST APIs
- Level 3 (HATEOAS) er sjældent implementeret i praksis
- Swagger/OpenAPI dokumentation kompenserer for manglende HATEOAS
- Tilstrækkeligt for 3. semester eksamensprojekt

**Fremtidige forbedringer:**
1. Implementer HATEOAS med HAL eller JSON:API
2. Tilføj pagination headers (Link, X-Total-Count)
3. Tilføj versioning (Accept: application/vnd.bookmyhome.v1+json)
4. Tilføj ETag headers for caching

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt  
**REST Maturity Level:** Level 2 (HTTP Verbs)

