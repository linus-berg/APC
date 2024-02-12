FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Helm/APM.Helm.csproj", "APM.Helm/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Infrastructure/APC.Infrastructure.csproj", "APC.Infrastructure/"]
RUN dotnet restore "APM.Helm/APM.Helm.csproj"
COPY . .
WORKDIR "/src/APM.Helm"
RUN dotnet build "APM.Helm.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Helm.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Helm.dll"]
