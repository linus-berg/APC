FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APC.Ingestion/APC.Ingestion.csproj", "APC.Ingestion/"]
RUN dotnet restore "APC.Ingestion/APC.Ingestion.csproj"
COPY . .
WORKDIR "/src/APC.Ingestion"
RUN dotnet build "APC.Ingestion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APC.Ingestion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APC.Ingestion.dll"]
