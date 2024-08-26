# Base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY RelationshipAnalysis/*.csproj ./RelationshipAnalysis/
COPY RelationAnalysis.Migrations/*.csproj ./RelationAnalysis.Migrations/
COPY RelationshipAnalysis.Test/*.csproj ./RelationshipAnalysis.Test/
COPY RelationshipAnalysis.Integration.Test/*.csproj ./RelationshipAnalysis.Integration.Test/

RUN dotnet restore RelationshipAnalysis.sln

COPY . ./

RUN dotnet test && \
    dotnet clean && \
    dotnet build  RelationshipAnalysis.sln -c Release -o build

RUN dotnet publish RelationshipAnalysis.sln -c Release -o publish

FROM base AS final

COPY --from=build /app/publish .

CMD ["dotnet", "RelationshipAnalysis.dll"]
