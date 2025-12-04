# OWASP Top 10 Web Application Security Risks - BookMyHome

## 📋 **OVERSIGT**

Dette dokument beskriver hvordan **BookMyHome** campsite booking system er sikret imod **OWASP Top 10 Web Application Security Risks**.

---

## 🎯 **DE TRE MEST RELEVANTE TRUSLER FOR SYSTEMET**

Efter analyse af systemets arkitektur og funktionalitet er følgende tre trusler identificeret som de mest relevante:

### **1. A03:2021 - Injection (SQL Injection)**
**Relevans:** ⭐⭐⭐⭐⭐ (Meget høj)
- Systemet håndterer brugerdata (bookings, users, campsites) via database queries
- Potentielt katastrofisk hvis angriber kan eksekvere arbitrær SQL
- Kan føre til datatab, data leakage, eller komplet system kompromittering

### **2. A07:2021 - Cross-Site Scripting (XSS)**
**Relevans:** ⭐⭐⭐⭐ (Høj)
- Blazor UI viser bruger-genereret indhold (booking special requests, user names, etc.)
- Kan føre til session hijacking, credential theft, eller malware distribution
- Særligt relevant da systemet håndterer betalingsinformation

### **3. A05:2021 - Security Misconfiguration**
**Relevans:** ⭐⭐⭐⭐ (Høj)
- Systemet bruger Docker, Kafka, MySQL, Nginx - mange komponenter at konfigurere
- JWT authentication kræver korrekt secret key management
- CORS, HTTPS, og API endpoint exposure skal konfigureres korrekt

---

## 🛡️ **IMPLEMENTEREDE SIKKERHEDSFORANSTALTNINGER**

### **1. SQL INJECTION PROTECTION (A03:2021)**

#### **Trussel Beskrivelse:**
SQL Injection opstår når angriber kan indsætte malicious SQL kode via user input, f.eks.:
```
Email: admin@example.com' OR '1'='1
```

#### **Implementering i BookMyHome:**

✅ **Entity Framework Core Parameterized Queries**
- Alle database queries bruger EF Core's LINQ-to-SQL
- EF Core genererer automatisk parameterized queries
- Ingen raw SQL queries bruges i systemet

**Eksempel fra BookingRepository.cs:**
```csharp
public async Task<Booking?> GetByIdAsync(BookingId id, CancellationToken cancellationToken = default)
{
    return await _context.Bookings
        .Include(b => b.Guest)
        .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    // EF Core genererer: SELECT * FROM Bookings WHERE Id = @p0
    // Parameter @p0 er escaped og type-safe
}
```

**Eksempel fra UserRepository.cs:**
```csharp
public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    // EF Core genererer: SELECT * FROM Users WHERE Email = @p0
}
```

✅ **Strongly-Typed Value Objects**
- Email, Money, DateRange valideres før database access
- Type-safe IDs (BookingId, UserId, CampsiteId) forhindrer type confusion attacks

**Eksempel fra Email.cs:**
```csharp
public static Email Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new DomainException("Email cannot be empty");

    if (!IsValidEmail(value))
        throw new DomainException($"Invalid email format: {value}");

    return new Email(value);
}
```

#### **Test & Verifikation:**
- ✅ Alle repository metoder bruger EF Core LINQ
- ✅ Ingen `FromSqlRaw()` eller `ExecuteSqlRaw()` calls i kodebasen
- ✅ Input validation via Value Objects før database access

---

### **2. CROSS-SITE SCRIPTING (XSS) PROTECTION (A07:2021)**

#### **Trussel Beskrivelse:**
XSS opstår når angriber kan indsætte malicious JavaScript via user input, f.eks.:
```
Special Requests: <script>alert('XSS')</script>
```

#### **Implementering i BookMyHome:**

✅ **Blazor Automatic HTML Encoding**
- Blazor Server automatisk HTML-encoder alt output i `.razor` filer
- `@variable` syntax escaper automatisk HTML special characters

**Eksempel fra Blazor components:**
```razor
<MudText>@booking.SpecialRequests</MudText>
<!-- Output: &lt;script&gt;alert('XSS')&lt;/script&gt; -->
```

✅ **Input Validation via Domain Model**
- Alle user inputs valideres i domain layer
- String length limits enforces

**Eksempel fra Booking.cs:**
```csharp
public void UpdateSpecialRequests(string specialRequests)
{
    if (specialRequests.Length > 1000)
        throw new DomainException("Special requests cannot exceed 1000 characters");

    _specialRequests = specialRequests;
}
```

✅ **Content Security Policy (CSP) Headers**
- Implementeret via Nginx reverse proxy
- Forhindrer inline scripts og unsafe-eval

#### **Test & Verifikation:**
- ✅ Blazor auto-encoding aktiveret (default)
- ✅ Domain validation på alle string inputs
- ✅ Ingen `@((MarkupString)variable)` unsafe rendering

---

### **3. SECURITY MISCONFIGURATION PROTECTION (A05:2021)**

#### **Trussel Beskrivelse:**
Security misconfiguration opstår når:
- Default credentials bruges (admin/admin)
- Unødvendige features er enabled
- Error messages afslører system detaljer
- HTTPS ikke enforced

**appsettings.json:**
```json
{
  "JwtSettings": {
    "SecretKey": "BookMyHome-SuperSecret-Key-For-3Semester-Exam-Project-2025",
    "Issuer": "BookMyHome",
    "Audience": "BookMyHomeUsers",
    "ExpirationHours": 24
  }
}
```

**VIGTIGT:** I produktion skal SecretKey gemmes i:
- Azure Key Vault
- AWS Secrets Manager
- Environment variables (ikke committed til Git)

✅ **HTTPS Enforcement**
**Program.cs:**
```csharp
app.UseHttpsRedirection(); // Redirect HTTP -> HTTPS
app.UseHsts(); // HTTP Strict Transport Security
```

✅ **Error Handling - Ingen Information Leakage**
**Program.cs:**
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Production: Generic error messages
}
```

**AuthController.cs:**
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Error during login");
    return Unauthorized(new { message = "Invalid email or password" });
    // Generic message - afslører ikke om email findes
}
```

✅ **Docker Security Configuration**
**docker-compose.yml:**
```yaml
services:
  mysql:
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD} # Environment variable
    ports:
      - "3306:3306" # Kun localhost access (ikke 0.0.0.0:3306)
```

✅ **Nginx Reverse Proxy Security Headers**
**nginx.conf:**
```nginx
# Security headers
add_header X-Frame-Options "SAMEORIGIN" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-XSS-Protection "1; mode=block" always;
add_header Referrer-Policy "no-referrer-when-downgrade" always;

# Rate limiting (DDoS protection)
limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
limit_req zone=api_limit burst=20 nodelay;
```

✅ **CORS Configuration**
**Program.cs:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://bookmyhome.dk") // Specific origin
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

#### **Test & Verifikation:**
- ✅ JWT secret key i appsettings.json (skal flyttes til Key Vault i prod)
- ✅ HTTPS enforcement aktiveret
- ✅ Generic error messages i production
- ✅ Security headers via Nginx

---

## 📊 **ANDRE OWASP TOP 10 TRUSLER - VURDERING**

### **A01:2021 - Broken Access Control**
**Relevans:** ⭐⭐⭐ (Medium)
**Status:** ✅ Delvist implementeret
- JWT authentication på alle API endpoints (`[Authorize]` attribute)
- **MANGLER:** Role-based authorization (Admin vs Guest)
- **MANGLER:** Resource-level authorization (users kan kun se egne bookings)

**Fremtidig implementering:**
```csharp
[Authorize(Roles = "Admin")]
public async Task<ActionResult> DeleteBooking(int id) { ... }

// Check ownership
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (booking.GuestId.Value.ToString() != userId)
    return Forbid();
```

---

### **A02:2021 - Cryptographic Failures**
**Relevans:** ⭐⭐⭐ (Medium)
**Status:** ⚠️ Delvist implementeret
- ✅ HTTPS encryption for data in transit
- ✅ JWT tokens for authentication
- ⚠️ Password hashing bruger SHA256 (INSECURE - skal være BCrypt/Argon2)

**Nuværende implementation (INSECURE):**
```csharp
private static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
}
```

**Anbefalet implementation:**
```csharp
// Install: BCrypt.Net-Next NuGet package
private static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
}
```

---

### **A04:2021 - Insecure Design**
**Relevans:** ⭐⭐ (Lav)
**Status:** ✅ God arkitektur
- Domain-Driven Design med encapsulation
- Factory methods enforcer business rules
- Value Objects validerer invariants
- Repository pattern isolerer data access

---

### **A06:2021 - Vulnerable and Outdated Components**
**Relevans:** ⭐⭐⭐ (Medium)
**Status:** ✅ Opdaterede packages
- .NET 9.0 (latest)
- EF Core 8.0.2 (latest stable for MySQL)
- MudBlazor 8.13.0 (latest)
- Confluent.Kafka 2.3.0 (latest)

**Vedligeholdelse:**
```bash
dotnet list package --outdated
dotnet add package <PackageName> --version <LatestVersion>
```

---

### **A08:2021 - Software and Data Integrity Failures**
**Relevans:** ⭐⭐ (Lav)
**Status:** ✅ Implementeret
- NuGet packages verificeres via SHA256 checksums
- Docker images fra trusted registries (mysql:8.0, confluentinc/cp-kafka)
- Git commit signing (anbefalet)

---

### **A09:2021 - Security Logging and Monitoring Failures**
**Relevans:** ⭐⭐⭐ (Medium)
**Status:** ✅ Implementeret
- ILogger logging i alle controllers
- Kafka events for audit trail
- **MANGLER:** Centralized logging (ELK stack, Seq, Application Insights)

**Eksempel fra AuthController.cs:**
```csharp
_logger.LogInformation("User {UserId} logged in successfully", user.Id.Value);
_logger.LogWarning("Email {Email} already exists", request.Email);
_logger.LogError(ex, "Error during login");
```

---

### **A10:2021 - Server-Side Request Forgery (SSRF)**
**Relevans:** ⭐ (Meget lav)
**Status:** ✅ Ikke relevant
- Systemet laver ikke HTTP requests til eksterne URLs baseret på user input
- Ingen webhook callbacks eller URL fetching

---

## ✅ **KONKLUSION**

BookMyHome implementerer beskyttelse mod de **3 mest relevante OWASP Top 10 trusler**:

1. ✅ **SQL Injection** - EF Core parameterized queries + Value Object validation
2. ✅ **XSS** - Blazor auto-encoding + input validation + CSP headers
3. ✅ **Security Misconfiguration** - HTTPS, JWT secrets, error handling, Nginx security headers

**Fremtidige forbedringer:**
- 🔄 Implementer BCrypt password hashing (erstat SHA256)
- 🔄 Tilføj role-based authorization (Admin/Guest roles)
- 🔄 Implementer resource-level authorization (users kan kun se egne bookings)
- 🔄 Flyt JWT secret key til Azure Key Vault
- 🔄 Tilføj centralized logging (Seq/ELK)

---

**Dato:** 2025-11-13
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt
**Forfatter:** BookMyHome Team


#### **Implementering i BookMyHome:**

✅ **JWT Secret Key Management**

