FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Korp.Estoque.Api/Korp.Estoque.Api.csproj", "Korp.Estoque.Api/"]
COPY ["Korp.Estoque.Application/Korp.Estoque.Application.csproj", "Korp.Estoque.Application/"]
COPY ["Korp.Estoque.Domain/Korp.Estoque.Domain.csproj", "Korp.Estoque.Domain/"]
COPY ["Korp.Estoque.Infrastructure/Korp.Estoque.Infrastructure.csproj", "Korp.Estoque.Infrastructure/"]

RUN dotnet restore "Korp.Estoque.Api/Korp.Estoque.Api.csproj"

COPY . .

WORKDIR "/src/Korp.Estoque.Api"

RUN dotnet build "Korp.Estoque.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Korp.Estoque.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Korp.Estoque.Api.dll"]