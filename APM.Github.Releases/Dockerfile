FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Github.Releases/APM.Github.Releases.csproj", "APM.Github.Releases/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Github/APC.Github.csproj", "APC.Github/"]
RUN dotnet restore "APM.Github.Releases/APM.Github.Releases.csproj"
COPY . .
WORKDIR "/src/APM.Github.Releases"
RUN dotnet build "APM.Github.Releases.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Github.Releases.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Github.Releases.dll"]
