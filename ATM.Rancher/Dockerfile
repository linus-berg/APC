FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ATM.Rancher/ATM.Rancher.csproj", "ATM.Rancher/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "ATM.Rancher/ATM.Rancher.csproj"
COPY . .
WORKDIR "/src/ATM.Rancher"
RUN dotnet build "ATM.Rancher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ATM.Rancher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATM.Rancher.dll"]
