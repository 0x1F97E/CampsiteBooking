# 📊 BookMyHome - Diagram Prompts til Lucidchart

Denne mappe indeholder 13 detaljerede prompts til at generere alle nødvendige diagrammer til eksamensprojektet.

## 🎯 Sådan bruger du disse prompts

1. Åbn Lucidchart (https://www.lucidchart.com)
2. Klik på "Create New" → "Blank Document"
3. Kopier indholdet fra en af prompt-filerne nedenfor
4. Brug Lucidchart AI eller opbyg diagrammet manuelt baseret på prompten
5. Eksporter som PNG/PDF til rapporten

---

## 📋 Oversigt over alle 13 diagrammer

### 1️⃣ ERD (Entity Relationship Diagram)
**Fil:** `01-ERD-prompt.txt`  
**Formål:** Vise database struktur med alle 14 entities og relationer  
**Notation:** Crow's foot notation  
**Indhold:** Booking, Guest, User, Campsite, AccommodationType, Payment, Review, Amenity, Availability, Pricing, Newsletter, Notification + junction tables  
**Eksamenskrav:** ✅ Database design (Modul 4)

---

### 2️⃣ Use Case Diagram
**Fil:** `02-UseCase-prompt.txt`  
**Formål:** Vise aktører og use cases  
**Notation:** UML Use Case Diagram  
**Indhold:** 4 aktører (Guest, User, Admin, System), 30+ use cases, include/extend relationer  
**Eksamenskrav:** ✅ Kravspecifikation (Modul 1)

---

### 3️⃣ Clean Architecture Diagram
**Fil:** `03-CleanArchitecture-prompt.txt`  
**Formål:** Vise lagdelt arkitektur (Onion Architecture)  
**Notation:** Concentric circles  
**Indhold:** 4 lag (Domain, Application, Infrastructure, Presentation), dependency rule  
**Eksamenskrav:** ✅ Software arkitektur (Modul 2)

---

### 4️⃣ System Architecture Diagram
**Fil:** `04-SystemArchitecture-prompt.txt`  
**Formål:** Vise deployment arkitektur med Docker  
**Notation:** Layered architecture  
**Indhold:** 4 tiers (Client, Gateway, Application, Data), Docker containers, networks, volumes  
**Eksamenskrav:** ✅ Deployment (Modul 7)

---

### 5️⃣ Scale Cube Diagram
**Fil:** `05-ScaleCube-prompt.txt`  
**Formål:** Vise skaleringsstrategier  
**Notation:** 3D cube med X/Y/Z akser  
**Indhold:** X-axis (load balancing ✅), Y-axis (microservices ⚠️), Z-axis (sharding ❌)  
**Eksamenskrav:** ✅ Skalering (Modul 7)

---

### 6️⃣ FURPS+ Diagram
**Fil:** `06-FURPS-prompt.txt`  
**Formål:** Vise kvalitetsattributter  
**Notation:** Mind map  
**Indhold:** Functionality, Usability, Reliability, Performance, Supportability + constraints  
**Eksamenskrav:** ✅ Kvalitetskrav (Modul 1)

---

### 7️⃣ Deployment Diagram
**Fil:** `07-Deployment-prompt.txt`  
**Formål:** Vise fysisk deployment med UML notation  
**Notation:** UML Deployment Diagram (3D boxes)  
**Indhold:** Docker containers, networks, volumes, ports, artifacts  
**Eksamenskrav:** ✅ Deployment (Modul 7)

---

### 8️⃣ Sequence Diagram - Create Booking
**Fil:** `08-Sequence-CreateBooking-prompt.txt`  
**Formål:** Vise message flow for booking creation  
**Notation:** UML Sequence Diagram  
**Indhold:** User → Browser → Gateway → API → DB → Kafka (29 steps)  
**Eksamenskrav:** ✅ Interaktionsdesign (Modul 3)

---

### 9️⃣ State Diagram - Booking Lifecycle
**Fil:** `09-State-BookingLifecycle-prompt.txt`  
**Formål:** Vise booking states og transitions  
**Notation:** UML State Diagram  
**Indhold:** 5 states (Pending, Confirmed, Cancelled, Completed), guards, actions  
**Eksamenskrav:** ✅ Tilstandsdiagram (Modul 3)

---

### 🔟 Component Diagram
**Fil:** `10-Component-prompt.txt`  
**Formål:** Vise komponenter og interfaces  
**Notation:** UML Component Diagram (lollipops/sockets)  
**Indhold:** 9 komponenter, provided/required interfaces, dependencies  
**Eksamenskrav:** ✅ Komponentarkitektur (Modul 2)

---

### 1️⃣1️⃣ Class Diagram - Domain Model
**Fil:** `11-ClassDiagram-prompt.txt`  
**Formål:** Vise domain entities og value objects  
**Notation:** UML Class Diagram  
**Indhold:** 6 entities, 5 value objects, 2 enums, relationships  
**Eksamenskrav:** ✅ Domain-Driven Design (Modul 2)

---

### 1️⃣2️⃣ Kafka Event Flow Diagram
**Fil:** `12-KafkaEventFlow-prompt.txt`  
**Formål:** Vise event-driven arkitektur  
**Notation:** Flow diagram  
**Indhold:** Producers, 3 topics, consumers, event structure (JSON)  
**Eksamenskrav:** ✅ Event-driven (Modul 6)

---

### 1️⃣3️⃣ OWASP Security Diagram
**Fil:** `13-OWASP-Security-prompt.txt`  
**Formål:** Vise sikkerhedstrusler og beskyttelse  
**Notation:** Layered defense diagram  
**Indhold:** 4 trusler (SQL Injection, XSS, CSRF, Auth), 4 protection layers  
**Eksamenskrav:** ✅ OWASP Top 10 (Modul 8)

---

## ✅ Status

Alle 13 diagrammer er klar til at blive genereret i Lucidchart!

**Næste skridt:**
1. Åbn Lucidchart
2. Generer hvert diagram ved at bruge de tilsvarende prompts
3. Eksporter som PNG (300 DPI) eller PDF
4. Indsæt i eksamensprojekt rapporten

---

## 📌 Tips til Lucidchart

- Brug **UML shape libraries** til UML diagrammer
- Brug **Entity Relationship** library til ERD
- Brug **AWS/Cloud** library til deployment diagrammer
- Brug **Flowchart** library til event flow
- Eksporter i høj opløsning (300 DPI minimum)
- Gem alle diagrammer i samme Lucidchart workspace for konsistens

---

**Oprettet:** 2024-01-15  
**Projekt:** BookMyHome Campsite Booking System  
**Eksamen:** 3. Semester Datamatiker

