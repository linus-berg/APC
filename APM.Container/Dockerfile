FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APM.Container/APM.Container.csproj", "APM.Container/"]
RUN dotnet restore "APM.Container/APM.Container.csproj"
COPY . .
WORKDIR "/src/APM.Container"
RUN dotnet build "APM.Container.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Container.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Container.dll"]
