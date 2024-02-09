FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Rancher/APM.Rancher.csproj", "APM.Rancher/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Github/APC.Github.csproj", "APC.Github/"]
RUN dotnet restore "APM.Rancher/APM.Rancher.csproj"
COPY . .
WORKDIR "/src/APM.Rancher"
RUN dotnet build "APM.Rancher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Rancher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Rancher.dll"]
