﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APC.Tester/APC.Tester.csproj", "APC.Tester/"]
RUN dotnet restore "APC.Tester/APC.Tester.csproj"
COPY . .
WORKDIR "/src/APC.Tester"
RUN dotnet build "APC.Tester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APC.Tester.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APC.Tester.dll"]
