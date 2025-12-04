# BookMyHome API - Postman Test Collection

## 📋 **OVERSIGT**

Dette dokument beskriver hvordan man tester **BookMyHome REST API** med Postman.

---

## 🚀 **SETUP**

### **1. Start systemet:**
```bash
# Start Docker services (MySQL + Kafka)
docker-compose up -d

# Start .NET application
dotnet run
```

### **2. Åbn Swagger UI:**
- URL: https://localhost:7001/swagger
- Her kan du se alle API endpoints og teste dem direkte

### **3. Base URL:**
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`

---

## 🔐 **AUTHENTICATION FLOW**

### **Step 1: Register User**
**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+45 12345678",
  "country": "Denmark"
}
```

**Expected Response (201 Created):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "email": "test@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Copy the `token` value for next requests!**

---

### **Step 2: Login User**
**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "test@example.com",
  "password": "Test123!"
}
```

**Expected Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "email": "test@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

---

## 📝 **BOOKINGS API (Requires Authentication)**

### **Setup Authorization Header:**
For all requests below, add this header:
```
Authorization: Bearer <your-jwt-token>
```

In Postman:
1. Go to **Authorization** tab
2. Select **Type: Bearer Token**
3. Paste your JWT token

---

### **1. Create Booking**
**Endpoint:** `POST /api/bookings`

**Request Body:**
```json
{
  "guestId": 1,
  "campsiteId": 1,
  "accommodationTypeId": 1,
  "checkInDate": "2025-07-01",
  "checkOutDate": "2025-07-08",
  "numberOfAdults": 2,
  "numberOfChildren": 1,
  "basePriceAmount": 1500.00,
  "currency": "DKK",
  "specialRequests": "Late check-in please"
}
```

**Expected Response (201 Created):**
```json
{
  "bookingId": 1,
  "guestId": 1,
  "campsiteId": 1,
  "accommodationTypeId": 1,
  "checkInDate": "2025-07-01T00:00:00",
  "checkOutDate": "2025-07-08T00:00:00",
  "status": "Pending",
  "basePriceAmount": 1500.00,
  "totalPriceAmount": 1500.00,
  "numberOfAdults": 2,
  "numberOfChildren": 1,
  "specialRequests": "Late check-in please"
}
```

---

### **2. Get All Bookings**
**Endpoint:** `GET /api/bookings`

**Expected Response (200 OK):**
```json
[
  {
    "bookingId": 1,
    "guestId": 1,
    "checkInDate": "2025-07-01T00:00:00",
    "checkOutDate": "2025-07-08T00:00:00",
    "status": "Pending",
    "totalPriceAmount": 1500.00
  }
]
```

---

### **3. Get Booking by ID**
**Endpoint:** `GET /api/bookings/1`

**Expected Response (200 OK):**
```json
{
  "bookingId": 1,
  "guestId": 1,
  "campsiteId": 1,
  "accommodationTypeId": 1,
  "checkInDate": "2025-07-01T00:00:00",
  "checkOutDate": "2025-07-08T00:00:00",
  "status": "Pending",
  "basePriceAmount": 1500.00,
  "totalPriceAmount": 1500.00,
  "specialRequests": "Late check-in please"
}
```

**Expected Response (404 Not Found) if ID doesn't exist:**
```json
{
  "message": "Booking with ID 999 not found"
}
```

---

### **4. Update Booking**
**Endpoint:** `PUT /api/bookings/1`

**Request Body:**
```json
{
  "specialRequests": "Updated: Early check-in requested"
}
```

**Expected Response (200 OK):**
```json
{
  "bookingId": 1,
  "specialRequests": "Updated: Early check-in requested"
}
```

---

### **5. Delete Booking (Cancel)**
**Endpoint:** `DELETE /api/bookings/1`

**Expected Response (204 No Content)**
- No response body
- Status code: 204

---

## 👥 **USERS API (Requires Authentication)**

### **1. Get User by ID**
**Endpoint:** `GET /api/users/1`

**Expected Response (200 OK):**
```json
{
  "userId": 1,
  "email": "test@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+45 12345678",
  "country": "Denmark",
  "joinedDate": "2025-11-13T00:00:00",
  "isActive": true
}
```

---

## 🏕️ **CAMPSITES API (Requires Authentication)**

### **1. Get All Campsites**
**Endpoint:** `GET /api/campsites`

**Expected Response (200 OK):**
```json
[
  {
    "campsiteId": 1,
    "name": "Skagen Camping",
    "region": "Nordjylland",
    "description": "Beautiful campsite near the beach",
    "latitude": 57.7209,
    "longitude": 10.5831,
    "isActive": true
  }
]
```

---

### **2. Get Campsite by ID**
**Endpoint:** `GET /api/campsites/1`

**Expected Response (200 OK):**
```json
{
  "campsiteId": 1,
  "name": "Skagen Camping",
  "region": "Nordjylland",
  "description": "Beautiful campsite near the beach",
  "latitude": 57.7209,
  "longitude": 10.5831,
  "isActive": true
}
```

---

## ✅ **TEST SCENARIOS**

### **Scenario 1: Happy Path - Complete Booking Flow**
1. ✅ Register user → Get JWT token
2. ✅ Create booking → Get booking ID
3. ✅ Get booking by ID → Verify data
4. ✅ Update booking → Verify changes
5. ✅ Delete booking → Verify 204 response

### **Scenario 2: Authentication Tests**
1. ✅ Call `/api/bookings` without token → Expect 401 Unauthorized
2. ✅ Call `/api/bookings` with invalid token → Expect 401 Unauthorized
3. ✅ Call `/api/bookings` with valid token → Expect 200 OK

### **Scenario 3: Validation Tests**
1. ✅ Register with invalid email → Expect 400 Bad Request
2. ✅ Create booking with invalid dates → Expect 400 Bad Request
3. ✅ Create booking with XSS payload → Expect 400 Bad Request (InputValidator)

### **Scenario 4: Error Handling**
1. ✅ Get non-existent booking → Expect 404 Not Found
2. ✅ Get non-existent user → Expect 404 Not Found
3. ✅ Register with existing email → Expect 400 Bad Request

---

## 📊 **EXPECTED STATUS CODES**

| Status Code | Meaning | When |
|------------|---------|------|
| 200 OK | Success | GET, PUT requests |
| 201 Created | Resource created | POST requests (Register, Create Booking) |
| 204 No Content | Success, no body | DELETE requests |
| 400 Bad Request | Validation error | Invalid input data |
| 401 Unauthorized | Missing/invalid token | No JWT or expired token |
| 404 Not Found | Resource not found | Invalid ID |
| 500 Internal Server Error | Server error | Unexpected errors |

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt

