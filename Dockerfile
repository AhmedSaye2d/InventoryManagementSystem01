# ──────────────────────────────────────────────────────────────
# Multi-stage Dockerfile for Inventory Management System
# Stage 1: Restore & Build
# Stage 2: Publish
# Stage 3: Runtime (minimal image)
# ──────────────────────────────────────────────────────────────

# ══════════════════════════════════════════════════════════════
# Stage 1 — Restore dependencies (cached layer)
# ══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore

WORKDIR /src

# Copy only project files first for better Docker layer caching
COPY ["Inventory Management System.slnx", "./"]
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Host/Host.csproj", "Host/"]

RUN dotnet restore "Inventory Management System.slnx"

# ══════════════════════════════════════════════════════════════
# Stage 2 — Build & Publish
# ══════════════════════════════════════════════════════════════
FROM restore AS build

ARG BUILD_CONFIGURATION=Release

# Copy all source code
COPY . .

# Build the solution
RUN dotnet build "Inventory Management System.slnx" \
    -c ${BUILD_CONFIGURATION} \
    --no-restore

# Publish the API project
RUN dotnet publish "API/API.csproj" \
    -c ${BUILD_CONFIGURATION} \
    --no-build \
    -o /app/publish \
    /p:UseAppHost=false

# ══════════════════════════════════════════════════════════════
# Stage 3 — Runtime (minimal image)
# ══════════════════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Security: run as non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup -s /bin/false appuser

WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Switch to non-root user
USER appuser

ENTRYPOINT ["dotnet", "API.dll"]
