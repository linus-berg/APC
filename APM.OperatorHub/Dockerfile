FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["APM.OperatorHub/APM.OperatorHub.csproj", "APM.OperatorHub/"]
RUN dotnet restore "APM.OperatorHub/APM.OperatorHub.csproj"
COPY . .
WORKDIR "/src/APM.OperatorHub"
RUN dotnet build "APM.OperatorHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APM.OperatorHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.OperatorHub.dll"]
