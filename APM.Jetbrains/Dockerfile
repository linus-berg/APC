FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Jetbrains/APM.Jetbrains.csproj", "APM.Jetbrains/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Jetbrains/APM.Jetbrains.csproj"
COPY . .
WORKDIR "/src/APM.Jetbrains"
RUN dotnet build "APM.Jetbrains.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Jetbrains.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Jetbrains.dll"]
