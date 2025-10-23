FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Collector.Wget/Collector.Wget.csproj", "Collector.Wget/"]
RUN dotnet restore "Collector.Wget/Collector.Wget.csproj"
COPY . .
WORKDIR "/src/Collector.Wget"
RUN dotnet build "Collector.Wget.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Collector.Wget.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update && apt-get install -y wget
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.Wget.dll"]
