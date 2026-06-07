# Build context: solution root (TaskForge/)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TaskForge.Domain/TaskForge.Domain.csproj", "TaskForge.Domain/"]
COPY ["TaskForge.Application/TaskForge.Application.csproj", "TaskForge.Application/"]
COPY ["TaskForge.Infrastructure/TaskForge.Infrastructure.csproj", "TaskForge.Infrastructure/"]
COPY ["TaskForge.Api/TaskForge.Api.csproj", "TaskForge.Api/"]

RUN dotnet restore "TaskForge.Api/TaskForge.Api.csproj"

COPY . .

WORKDIR /src/TaskForge.Api
RUN dotnet publish "TaskForge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskForge.Api.dll"]
