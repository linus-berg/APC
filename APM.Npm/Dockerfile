FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Npm/APM.Npm.csproj", "APM.Npm/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Infrastructure/APC.Infrastructure.csproj", "APC.Infrastructure/"]
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
