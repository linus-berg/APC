FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APO.Ingestion/APO.Ingestion.csproj", "APO.Ingestion/"]
RUN dotnet restore "APO.Ingestion/APO.Ingestion.csproj"
COPY . .
WORKDIR "/src/APO.Ingestion"
RUN dotnet build "APO.Ingestion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APO.Ingestion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APO.Ingestion.dll"]
