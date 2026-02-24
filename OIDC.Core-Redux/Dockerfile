FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OIDC.Core-Redux/OIDC.Core-Redux.csproj", "OIDC.Core-Redux/"]
RUN dotnet restore "OIDC.Core-Redux/OIDC.Core-Redux.csproj"
COPY . .
WORKDIR "/src/OIDC.Core-Redux"
RUN dotnet build "OIDC.Core-Redux.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OIDC.Core-Redux.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OIDC.Core-Redux.dll"]
