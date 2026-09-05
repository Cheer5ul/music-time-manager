FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["music-time-manager.API/music-time-manager.API.csproj", "music-time-manager.API/"]
COPY ["music-time-manager.Persistence/music-time-manager.Persistence.csproj", "music-time-manager.Persistence/"]
COPY ["music-time-manager.Core/music-time-manager.Core.csproj", "music-time-manager.Core/"]
COPY ["music-time-manager.Application/music-time-manager.Application.csproj", "music-time-manager.Application/"]
COPY ["music-time-manager.Infrastructure/music-time-manager.Infrastructure.csproj", "music-time-manager.Infrastructure/"]
RUN dotnet restore "music-time-manager.API/music-time-manager.API.csproj"
COPY . .
WORKDIR "/src/music-time-manager.API"
RUN dotnet build "./music-time-manager.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./music-time-manager.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "music-time-manager.API.dll"]
