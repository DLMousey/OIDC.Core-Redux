FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OIDC.Core-Minimal/OIDC.Core-Minimal.csproj", "OIDC.Core-Minimal/"]
RUN dotnet restore "OIDC.Core-Minimal/OIDC.Core-Minimal.csproj"
COPY . .
WORKDIR "/src/OIDC.Core-Minimal"
RUN dotnet build "OIDC.Core-Minimal.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OIDC.Core-Minimal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OIDC.Core-Minimal.dll"]
