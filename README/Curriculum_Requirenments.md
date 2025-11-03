## Curriculum requirements

Purpose: short, testable expectations for the semester project. Keep scope focused on a working booking demo, solid code, and basic deployment infrastructure.

### Required deliverables

- Working booking application (frontend + backend) with core flows: search, book, cancel, and view bookings.
- REST API with CRUD endpoints, minimal documentation (OpenAPI/Swagger or Postman collection).
- Source code in a GitHub repo and a short PDF report describing architecture and decisions.
- Dockerized services and a `docker-compose.yml` to run the system locally.
- Simple integration of an asynchronous messaging pattern using Kafka (or a Kafka-compatible emulator) to demonstrate event-driven notifications or booking events.

### Minimum technical expectations

- Architecture: domain-centric layering (e.g., Clean/Hexagonal) and separation between domain logic and infrastructure.
- Tests: unit tests for core domain logic and at least one integration test for the API.
- Authentication: basic token-based auth (JWT or similar) for protected endpoints.
- Deployment: Dockerfiles for services and a `docker-compose.yml` that brings up the app and Kafka for local testing.
- Messaging (Kafka): demonstrate at least one event flow (e.g., booking.created -> notification published/consumed). It's acceptable to run Kafka in Docker Compose for local evaluation.

### Grading / evaluation criteria

- Functionality: core booking flows work end-to-end.
- Code quality: readable, modular, follows SOLID where practical, and has tests.
- Infrastructure: reproducible local run using Docker Compose; services build into images.
- Messaging: clear use of Kafka (or documented emulator) showing at least one producer and one consumer.
- Documentation: compact README showing how to build, run, and exercise the system; link to API docs and any scripts to create topics or seed data.

### Quick notes for students

- Use Docker Compose to provide a consistent local environment (app + Kafka + Zookeeper or a single-node Kafka image).
- Keep event contracts simple (JSON) and document the topic names and payload shapes in the repo.
- Focus on a small, polished feature set rather than many half-baked features.
