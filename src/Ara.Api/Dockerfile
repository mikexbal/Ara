# Build context is the repository root (see docker-compose.yml), because the
# Api project references sibling projects under src/.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Ara.Api/Ara.Api.csproj src/Ara.Api/
COPY src/Ara.Application/Ara.Application.csproj src/Ara.Application/
COPY src/Ara.Domain/Ara.Domain.csproj src/Ara.Domain/
COPY src/Ara.Infrastructure/Ara.Infrastructure.csproj src/Ara.Infrastructure/
RUN dotnet restore src/Ara.Api/Ara.Api.csproj

COPY src/ src/
RUN dotnet publish src/Ara.Api/Ara.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Npgsql probes for GSSAPI/Kerberos support at startup; the slim base image
# doesn't ship it, which otherwise fails every DB connection attempt.
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ara.Api.dll"]
