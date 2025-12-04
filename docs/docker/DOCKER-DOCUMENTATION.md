# BookMyHome - Docker & Docker Compose Dokumentation

## 📋 **OVERSIGT**

Dette dokument beskriver Docker setup for **BookMyHome** campsite booking system.

---

## 🐳 **DOCKER COMPOSE ARKITEKTUR**

### **Services:**
1. **MySQL Database** - Persistent data storage
2. **Zookeeper** - Kafka coordination service
3. **Kafka** - Event-driven messaging
4. **Nginx** - Reverse proxy (optional)

---

## 📄 **DOCKER-COMPOSE.YML**

```yaml
version: '3.8'

services:
  # MySQL Database
  mysql:
    image: mysql:8.0
    container_name: bookmyhome-mysql
    environment:
      MYSQL_ROOT_PASSWORD: rootpassword
      MYSQL_DATABASE: CampsiteBookingDb
      MYSQL_USER: bookmyhome
      MYSQL_PASSWORD: bookmyhome123
    ports:
      - "3306:3306"  # Host:Container
    volumes:
      - mysql_data:/var/lib/mysql
    networks:
      - bookmyhome-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Zookeeper (Kafka dependency)
  zookeeper:
    image: confluentinc/cp-zookeeper:7.5.0
    container_name: bookmyhome-zookeeper
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    ports:
      - "2181:2181"
    networks:
      - bookmyhome-network
    restart: unless-stopped

  # Kafka Message Broker
  kafka:
    image: confluentinc/cp-kafka:7.5.0
    container_name: bookmyhome-kafka
    depends_on:
      - zookeeper
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"
    ports:
      - "9092:9092"
    networks:
      - bookmyhome-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "kafka-topics", "--bootstrap-server", "localhost:9092", "--list"]
      interval: 30s
      timeout: 10s
      retries: 5

  # Nginx Reverse Proxy (Optional - for production)
  nginx:
    image: nginx:alpine
    container_name: bookmyhome-nginx
    ports:
      - "80:80"    # HTTP
      - "443:443"  # HTTPS
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./ssl:/etc/nginx/ssl:ro
    networks:
      - bookmyhome-network
    restart: unless-stopped
    depends_on:
      - mysql
      - kafka

volumes:
  mysql_data:
    driver: local

networks:
  bookmyhome-network:
    driver: bridge
```

---

## 🔌 **PORT MAPPING**

| Service | Container Port | Host Port | Protocol | Purpose |
|---------|---------------|-----------|----------|---------|
| MySQL | 3306 | 3306 | TCP | Database connections |
| Zookeeper | 2181 | 2181 | TCP | Kafka coordination |
| Kafka | 9092 | 9092 | TCP | Message broker |
| Nginx | 80 | 80 | HTTP | Reverse proxy |
| Nginx | 443 | 443 | HTTPS | Secure reverse proxy |
| .NET App | 5000 | 5000 | HTTP | Application (dev) |
| .NET App | 7001 | 7001 | HTTPS | Application (dev) |

### **Port Mapping Forklaring:**
- **3306:3306** → MySQL database tilgængelig på `localhost:3306`
- **9092:9092** → Kafka broker tilgængelig på `localhost:9092`
- **2181:2181** → Zookeeper tilgængelig på `localhost:2181`

---

## 🚀 **DEPLOYMENT KOMMANDOER**

### **Start alle services:**
```bash
docker-compose up -d
```
- `-d` = detached mode (kører i baggrunden)

### **Stop alle services:**
```bash
docker-compose down
```

### **Stop og slet volumes (ADVARSEL: sletter data!):**
```bash
docker-compose down -v
```

### **Se logs:**
```bash
# Alle services
docker-compose logs -f

# Specifik service
docker-compose logs -f mysql
docker-compose logs -f kafka
```

### **Genstart service:**
```bash
docker-compose restart mysql
docker-compose restart kafka
```

### **Se kørende containers:**
```bash
docker-compose ps
```

### **Rebuild images:**
```bash
docker-compose up -d --build
```

---

## 🔍 **HEALTH CHECKS**

### **MySQL Health Check:**
```bash
docker exec bookmyhome-mysql mysqladmin ping -h localhost -u root -prootpassword
```

**Expected output:** `mysqld is alive`

### **Kafka Health Check:**
```bash
docker exec bookmyhome-kafka kafka-topics --bootstrap-server localhost:9092 --list
```

**Expected output:** Liste af topics (booking-events, user-events, etc.)

### **Check container status:**
```bash
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

---

## 📊 **NETWORKS**

### **bookmyhome-network (Bridge Network):**
- Type: Bridge
- Driver: bridge
- Isolerer containers fra host network
- Tillader inter-container communication

**Fordele:**
- ✅ Service discovery via container names (mysql, kafka, zookeeper)
- ✅ Network isolation
- ✅ Port mapping til host

**Eksempel:**
```csharp
// .NET kan connecte til MySQL via container name
"Server=mysql;Port=3306;Database=CampsiteBookingDb;..."

// Eller via localhost (port mapping)
"Server=localhost;Port=3306;Database=CampsiteBookingDb;..."
```

---

## 💾 **VOLUMES**

### **mysql_data (Named Volume):**
- Type: Named volume
- Driver: local
- Path: `/var/lib/mysql` (inside container)
- Persistent storage for MySQL data

**Fordele:**
- ✅ Data overlever container restart
- ✅ Data overlever `docker-compose down`
- ✅ Managed by Docker

**Slet volume (ADVARSEL: sletter alle data!):**
```bash
docker volume rm bookmyhome_mysql_data
```

**Inspect volume:**
```bash
docker volume inspect bookmyhome_mysql_data
```

---

## 🔐 **ENVIRONMENT VARIABLES**

### **MySQL:**
- `MYSQL_ROOT_PASSWORD` - Root password (rootpassword)
- `MYSQL_DATABASE` - Initial database (CampsiteBookingDb)
- `MYSQL_USER` - Application user (bookmyhome)
- `MYSQL_PASSWORD` - Application password (bookmyhome123)

### **Kafka:**
- `KAFKA_BROKER_ID` - Unique broker ID (1)
- `KAFKA_ZOOKEEPER_CONNECT` - Zookeeper connection (zookeeper:2181)
- `KAFKA_ADVERTISED_LISTENERS` - External listener (localhost:9092)
- `KAFKA_AUTO_CREATE_TOPICS_ENABLE` - Auto-create topics (true)

### **Zookeeper:**
- `ZOOKEEPER_CLIENT_PORT` - Client port (2181)
- `ZOOKEEPER_TICK_TIME` - Heartbeat interval (2000ms)

---

## 🛡️ **SECURITY CONSIDERATIONS**

### **Production Recommendations:**

1. **Environment Variables:**
```bash
# Use .env file (NOT committed to Git)
MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD}
MYSQL_PASSWORD=${MYSQL_PASSWORD}
```

2. **Network Isolation:**
```yaml
# Don't expose ports to 0.0.0.0 in production
ports:
  - "127.0.0.1:3306:3306"  # Only localhost
```

3. **SSL/TLS:**
```yaml
# Enable MySQL SSL
environment:
  MYSQL_SSL_MODE: REQUIRED
```

4. **Resource Limits:**
```yaml
deploy:
  resources:
    limits:
      cpus: '2'
      memory: 2G
```

---

## 📈 **MONITORING**

### **Container Stats:**
```bash
docker stats
```

### **Disk Usage:**
```bash
docker system df
```

### **Cleanup:**
```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove everything unused
docker system prune -a --volumes
```

---

**Dato:** 2025-11-13  
**Projekt:** BookMyHome - 3. Semester Eksamensprojekt

