FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["Processor.OperatorHub/Processor.OperatorHub.csproj", "Processor.OperatorHub/"]
RUN dotnet restore "Processor.OperatorHub/Processor.OperatorHub.csproj"
COPY . .
WORKDIR "/src/Processor.OperatorHub"
RUN dotnet build "Processor.OperatorHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Processor.OperatorHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processor.OperatorHub.dll"]
