FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Collector.Rsync/Collector.Rsync.csproj", "Collector.Rsync/"]
RUN dotnet restore "Collector.Rsync/Collector.Rsync.csproj"
COPY . .
WORKDIR "/src/Collector.Rsync"
RUN dotnet build "Collector.Rsync.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Collector.Rsync.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.Rsync.dll"]
