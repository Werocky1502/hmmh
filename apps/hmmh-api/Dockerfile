# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["apps/hmmh-api/Hmmh.Api.csproj", "apps/hmmh-api/"]
RUN dotnet restore "apps/hmmh-api/Hmmh.Api.csproj"

COPY . .
WORKDIR "/src/apps/hmmh-api"
RUN dotnet publish "Hmmh.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
COPY --from=build /app/publish .
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Hmmh.Api.dll"]
