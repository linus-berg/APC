﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["APC.Scheduler/APC.Scheduler.csproj", "APC.Scheduler/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Infrastructure/APC.Infrastructure.csproj", "APC.Infrastructure/"]
RUN dotnet restore "APC.Scheduler/APC.Scheduler.csproj"
COPY . .
WORKDIR "/src/APC.Scheduler"
RUN dotnet build "APC.Scheduler.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "APC.Scheduler.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APC.Scheduler.dll"]
