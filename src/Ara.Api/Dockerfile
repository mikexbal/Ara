# Build context is the repository root (see docker-compose.yml), because the
# Api project references sibling projects under src/.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/TravelPolynesia.Api/TravelPolynesia.Api.csproj src/TravelPolynesia.Api/
COPY src/TravelPolynesia.Application/TravelPolynesia.Application.csproj src/TravelPolynesia.Application/
COPY src/TravelPolynesia.Domain/TravelPolynesia.Domain.csproj src/TravelPolynesia.Domain/
COPY src/TravelPolynesia.Infrastructure/TravelPolynesia.Infrastructure.csproj src/TravelPolynesia.Infrastructure/
RUN dotnet restore src/TravelPolynesia.Api/TravelPolynesia.Api.csproj

COPY src/ src/
RUN dotnet publish src/TravelPolynesia.Api/TravelPolynesia.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TravelPolynesia.Api.dll"]
