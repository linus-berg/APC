FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Pypi/APM.Pypi.csproj", "APM.Pypi/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Pypi/APM.Pypi.csproj"
COPY . .
WORKDIR "/src/APM.Pypi"
RUN dotnet build "APM.Pypi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Pypi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Pypi.dll"]
