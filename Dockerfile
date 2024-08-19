# Base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY RelationshipAnalysis/RelationshipAnalysis.csproj ./RelationshipAnalysis/
RUN dotnet restore ./RelationshipAnalysis/RelationshipAnalysis.csproj
COPY . .
RUN dotnet test && \
    dotnet clean && \
    dotnet build  ./RelationshipAnalysis/RelationshipAnalysis.csproj -c Release -o build

# Publish
FROM build AS publish
RUN dotnet publish ./RelationshipAnalysis/RelationshipAnalysis.csproj -c Release -o publish

# Migration
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migration
WORKDIR /app
COPY RelationAnalysis.Migrations/RelationAnalysis.Migrations.csproj ./RelationAnalysis.Migrations/
RUN dotnet restore ./RelationAnalysis.Migrations/RelationAnalysis.Migrations.csproj
COPY . .
RUN dotnet build ./RelationAnalysis.Migrations/RelationAnalysis.Migrations.csproj -c Release -o migration

# Migrate
FROM base AS migrate
COPY --from=migration /app/migration .
CMD ["dotnet", "RelationAnalysis.Migrations.dll"]

# Run
FROM base AS final
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:80
CMD ["dotnet", "RelationshipAnalysis.dll"]
