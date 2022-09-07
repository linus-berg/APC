FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APM.Npm/APM.Npm.csproj", "APM.Npm/"]
RUN dotnet restore "APM.Npm/APM.Npm.csproj"
COPY . .
WORKDIR "/src/APM.Npm"
RUN dotnet build "APM.Npm.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Npm.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Npm.dll"]
