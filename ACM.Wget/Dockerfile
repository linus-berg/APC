﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ACM.Wget/ACM.Wget.csproj", "ACM.Wget/"]
RUN dotnet restore "ACM.Wget/ACM.Wget.csproj"
COPY . .
WORKDIR "/src/ACM.Wget"
RUN dotnet build "ACM.Wget.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACM.Wget.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update && apt-get install -y wget
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Wget.dll"]
