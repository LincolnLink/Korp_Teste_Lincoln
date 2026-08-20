FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Korp.Faturamento.Api/Korp.Faturamento.Api.csproj", "Korp.Faturamento.Api/"]
COPY ["Korp.Faturamento.Application/Korp.Faturamento.Application.csproj", "Korp.Faturamento.Application/"]
COPY ["Korp.Faturamento.Domain/Korp.Faturamento.Domain.csproj", "Korp.Faturamento.Domain/"]
COPY ["Korp.Faturamento.Infrastructure/Korp.Faturamento.Infrastructure.csproj", "Korp.Faturamento.Infrastructure/"]

RUN dotnet restore "Korp.Faturamento.Api/Korp.Faturamento.Api.csproj"

COPY . .

WORKDIR "/src/Korp.Faturamento.Api"

RUN dotnet build "Korp.Faturamento.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Korp.Faturamento.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Korp.Faturamento.Api.dll"]