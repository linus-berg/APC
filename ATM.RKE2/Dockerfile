FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ATM.RKE2/ATM.RKE2.csproj", "ATM.RKE2/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "ATM.RKE2/ATM.RKE2.csproj"
COPY . .
WORKDIR "/src/ATM.RKE2"
RUN dotnet build "ATM.RKE2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ATM.RKE2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATM.RKE2.dll"]
