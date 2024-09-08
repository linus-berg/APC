FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Maven/APM.Maven.csproj", "APM.Maven/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Infrastructure/APC.Infrastructure.csproj", "APC.Infrastructure/"]
RUN dotnet restore "APM.Maven/APM.Maven.csproj"
COPY . .
WORKDIR "/src/APM.Maven"
RUN dotnet build "APM.Maven.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Maven.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Maven.dll"]
