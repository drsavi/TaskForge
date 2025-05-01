# TaskForge

> **Portfolio-grade .NET Web API** • *Under Construction*

---

## 📖 Project Overview  
TaskForge is a modular, clean-architecture Web API for managing projects. It demonstrates professional .NET development practices—exposing CRUD endpoints for “Project” entities, wired up with authentication, validation, resilience and observability.

---

## 🎯 Objectives  
- Provide a clean, scalable back-end for project management  
- Showcase Domain-Driven Design, Clean Architecture & CQRS  
- Integrate robust authentication (ASP NET Core Identity + JWT)  
- Implement end-to-end validation, logging, health checks & retry policies  
- Serve as a launchpad for a full-stack sample (client app, CI/CD, Docker)

---

## 🚀 Future Roadmap  
- **Full CQRS Read/Write Separation**: introduce a dedicated ReadDbContext or read replica  
- **Event-Driven Projections**: publish domain events & build materialized views  
- **2FA & Role-Based Access**: extend Identity for multi-factor & permissions  
- **Container Orchestration**: Docker Compose & Kubernetes manifests  
- **CI/CD Pipelines**: GitHub Actions or Azure DevOps for build, test, deploy  
- **Comprehensive Testing**: expand unit & integration tests (xUnit, Moq, FluentAssertions)  
- **Sample Front-End**: Angular or Vue.js SPA consuming the API

---

## 🛠 Technologies  
- **Framework**: .NET 8, C#, ASP.NET Core Web API  
- **Persistence**: Entity Framework Core, PostgreSQL  
- **Authentication**: ASP.NET Core Identity, JWT Bearer  
- **Mediation & CQRS**: MediatR  
- **Validation**: FluentValidation + MediatR Pipeline Behavior  
- **Resilience**: Polly (retry policies), HealthChecks  
- **Observability**: Serilog, Swagger/OpenAPI  
- **DevOps**: Docker, Docker Compose (planned: Kubernetes, GitHub Actions)  
- **Testing**: xUnit, Moq, FluentAssertions

---

## 🏛 Architecture & Patterns  
- **Domain-Driven Design (DDD)**: Entities, Value Objects & Domain Events  
- **Clean/Onion Architecture**: Layered solution (Domain → Application → Infrastructure → API)  
- **CQRS**: Separate Command (writes) and Query (reads) models via MediatR  
- **Repository Pattern**: `IProjectRepository` + EF Core implementation  
- **Dependency Injection**: Built-in ASP.NET Core container  
- **SOLID Principles**: Single Responsibility, Open-Closed, etc.

---

## 🚀 Getting Started

1. **Clone the repository**  
   ```git clone https://github.com/your-username/taskforge.git
   cd taskforge

2. **Configure appsettings.json**

    ``` {
      {  
         "ConnectionStrings": {
            "Default": "Host=localhost;Database=taskforge;Username=postgres;Password=postgres"
          },
          "Jwt": {
            "Key": "LOCAL_DEV_SECRET_KEY_CHANGE_ME",
            "Issuer": "TaskForgeApi",
            "Audience": "TaskForgeClient",
            "ExpireMinutes": 60
          }
    }

3. **Run migrations**

     ```dotnet ef database update \
      --project TaskForge.Infrastructure \
      --startup-project TaskForge.Api
    ```

4. **Launch the API**

     ```
    cd TaskForge.Api
    dotnet run
    ```

5. **Explore Swagger UI**

Open your browser at https://localhost:{port}/swagger

## 🤝 Contributing

Contributions, feedback and PRs are welcome! Feel free to open issues or submit pull requests as the project evolves.

⚠️ Note: This is an evolving portfolio project. Features, patterns and infrastructure will continue to be refined.
Your insights and suggestions are highly appreciated!