# BookMyHome - API Gateway (YARP) Dokumentation

## 📋 **OVERSIGT**

Dette dokument beskriver **YARP (Yet Another Reverse Proxy)** API Gateway implementering i BookMyHome projektet.

---

## 🎯 **HVAD ER YARP?**

**YARP** er Microsoft's officielle reverse proxy toolkit til .NET:
- Open source (https://github.com/microsoft/reverse-proxy)
- High performance (bygget på Kestrel)
- Highly customizable
- Production-ready (bruges af Microsoft Teams, Bing, etc.)

---

## 🏗️ **ARKITEKTUR**

### **Før YARP (Direct API Calls):**
```
Client (Browser/Postman)
    ↓
https://localhost:7001/api/bookings  (Direct call til BookMyHome API)
https://localhost:7001/api/users
https://localhost:7001/api/auth
```

### **Efter YARP (API Gateway Pattern):**
```
Client (Browser/Postman)
    ↓
https://localhost:8001/api/bookings  (API Gateway)
    ↓
API Gateway (YARP)
    ↓
https://localhost:7001/api/bookings  (BookMyHome API)
```

**Fordele:**
- ✅ **Single Entry Point** - Alle requests går gennem gateway
- ✅ **Load Balancing** - Fordel requests mellem multiple instances
- ✅ **Health Checks** - Automatisk failover hvis en instance fejler
- ✅ **Rate Limiting** - Beskyt API mod DDoS attacks
- ✅ **Centralized Authentication** - JWT validation i gateway
- ✅ **Request/Response Transformation** - Modificer headers, body, etc.
- ✅ **Monitoring** - Centralized logging og metrics

---

## 📊 **YARP KONFIGURATION**

### **appsettings.json:**
```json
{
  "ReverseProxy": {
    "Routes": {
      "bookings-route": {
        "ClusterId": "bookmyhome-api",
        "Match": {
          "Path": "/api/bookings/{**catch-all}"
        }
      },
      "users-route": {
        "ClusterId": "bookmyhome-api",
        "Match": {
          "Path": "/api/users/{**catch-all}"
        }
      },
      "auth-route": {
        "ClusterId": "bookmyhome-api",
        "Match": {
          "Path": "/api/auth/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "bookmyhome-api": {
        "Destinations": {
          "destination1": {
            "Address": "https://localhost:7001"
          }
        },
        "LoadBalancingPolicy": "RoundRobin",
        "HealthCheck": {
          "Active": {
            "Enabled": true,
            "Interval": "00:00:10",
            "Timeout": "00:00:05",
            "Policy": "ConsecutiveFailures",
            "Path": "/health"
          }
        }
      }
    }
  }
}
```

### **Forklaring:**

#### **Routes:**
- **bookings-route** - Matcher `/api/bookings/*` og router til `bookmyhome-api` cluster
- **users-route** - Matcher `/api/users/*` og router til `bookmyhome-api` cluster
- **auth-route** - Matcher `/api/auth/*` og router til `bookmyhome-api` cluster
- **{**catch-all}** - Matcher alle sub-paths (f.eks. `/api/bookings/1`, `/api/bookings/1/cancel`)

#### **Clusters:**
- **bookmyhome-api** - Gruppe af destinations (backend servers)
- **Destinations** - Liste af backend servers (kan have flere for load balancing)
- **LoadBalancingPolicy** - RoundRobin (fordel requests ligeligt mellem destinations)
- **HealthCheck** - Tjek `/health` endpoint hvert 10. sekund

---

## 🚀 **DEPLOYMENT**

### **Start BookMyHome API:**
```bash
cd /home/pc/Documents/CampsiteBooking
dotnet run
```
- Kører på: https://localhost:7001

### **Start API Gateway:**
```bash
cd /home/pc/Documents/CampsiteBooking/ApiGateway
dotnet run
```
- Kører på: https://localhost:8001

### **Test via Gateway:**
```bash
# Register user via gateway
curl -X POST https://localhost:8001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "John",
    "lastName": "Doe"
  }'

# Get bookings via gateway (requires JWT token)
curl -X GET https://localhost:8001/api/bookings \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## 📊 **LOAD BALANCING**

### **Multiple Destinations:**

Hvis du vil køre 3 instances af BookMyHome API:

```json
{
  "Clusters": {
    "bookmyhome-api": {
      "Destinations": {
        "destination1": {
          "Address": "https://localhost:7001"
        },
        "destination2": {
          "Address": "https://localhost:7002"
        },
        "destination3": {
          "Address": "https://localhost:7003"
        }
      },
      "LoadBalancingPolicy": "RoundRobin"
    }
  }
}
```

**Load Balancing Policies:**
- **RoundRobin** - Fordel requests ligeligt (1 → 2 → 3 → 1 → 2 → 3)
- **LeastRequests** - Send til destination med færrest aktive requests
- **Random** - Tilfældig destination
- **PowerOfTwoChoices** - Vælg 2 tilfældige, send til den med færrest requests

---

## 🔍 **HEALTH CHECKS**

### **BookMyHome API Health Endpoint:**

```csharp
// Program.cs
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow 
}));
```

### **YARP Health Check Configuration:**

```json
{
  "HealthCheck": {
    "Active": {
      "Enabled": true,
      "Interval": "00:00:10",      // Check every 10 seconds
      "Timeout": "00:00:05",       // Timeout after 5 seconds
      "Policy": "ConsecutiveFailures",
      "Path": "/health"
    }
  }
}
```

**Hvad sker der hvis en destination fejler?**
1. YARP kalder `/health` endpoint hvert 10. sekund
2. Hvis timeout (5 sek) eller fejl → marker destination som unhealthy
3. YARP stopper med at sende requests til unhealthy destination
4. Når destination bliver healthy igen → YARP starter med at sende requests igen

---

## 🔐 **AUTHENTICATION I GATEWAY**

### **Centralized JWT Validation:**

Du kan flytte JWT validation til gateway i stedet for i hver API:

```csharp
// ApiGateway/Program.cs
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "BookMyHome",
            ValidAudience = "BookMyHomeUsers",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("BookMyHome-SuperSecret-Key-For-3Semester-Exam-Project-2025"))
        };
    });

builder.Services.AddAuthorization();

// Add authentication to YARP routes
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            // Forward JWT token to backend
            var token = transformContext.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token))
            {
                transformContext.ProxyRequest.Headers.Add("Authorization", token.ToString());
            }
        });
    });

app.UseAuthentication();
app.UseAuthorization();
```

**Fordele:**
- ✅ Centralized authentication logic
- ✅ Backend APIs kan være stateless (ingen JWT validation)
- ✅ Nemmere at ændre authentication strategi

---

## 📊 **RATE LIMITING**

### **Beskyt API mod DDoS:**

```csharp
// ApiGateway/Program.cs
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,           // Max 100 requests
                Window = TimeSpan.FromMinutes(1),  // Per minute
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));
});

app.UseRateLimiter();
```

**Resultat:**
- Max 100 requests per minut per IP address
- Hvis overskredet → HTTP 429 Too Many Requests

---

## 🎯 **HVORFOR YARP I BOOKMYHOME?**

### **Eksamenskrav (Modul 7 - Teknologi):**
- ✅ **API Gateway** er del af pensum
- ✅ Demonstrerer forståelse af microservices patterns
- ✅ Viser skalering strategi (load balancing)

### **Real-World Benefits:**
1. **Single Entry Point** - Clients behøver kun kende én URL
2. **Load Balancing** - Kan køre multiple API instances
3. **Health Checks** - Automatisk failover
4. **Centralized Security** - Authentication, rate limiting, CORS
5. **Monitoring** - Centralized logging
6. **Versioning** - Kan route til forskellige API versioner

---

## 📊 **YARP vs. ANDRE GATEWAYS**

| Feature | YARP | Ocelot | Kong | Nginx |
|---------|------|--------|------|-------|
| .NET Native | ✅ | ✅ | ❌ | ❌ |
| Open Source | ✅ | ✅ | ✅ (Community) | ✅ |
| Performance | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Configuration | Code + JSON | JSON | YAML/DB | Config files |
| Load Balancing | ✅ | ✅ | ✅ | ✅ |
| Health Checks | ✅ | ✅ | ✅ | ✅ |
| Rate Limiting | ✅ (via middleware) | ✅ | ✅ | ✅ |
| Microsoft Support | ✅ | ❌ | ❌ | ❌ |

**Hvorfor YARP?**
- Microsoft's officielle løsning
- Perfekt til .NET projekter
- High performance (Kestrel)
- Production-ready

---

## 🚀 **FREMTIDIGE FORBEDRINGER**

1. **Request Transformation:**
```csharp
.AddTransforms(builderContext =>
{
    builderContext.AddRequestHeader("X-Forwarded-For", context => 
        context.HttpContext.Connection.RemoteIpAddress?.ToString());
});
```

2. **Response Caching:**
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

3. **Distributed Tracing:**
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder.AddHttpClientInstrumentation();
        tracerProviderBuilder.AddAspNetCoreInstrumentation();
    });
```

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt  
**API Gateway:** YARP (Yet Another Reverse Proxy) 2.1.0

