FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APM.Jetbrains.IDE/APM.Jetbrains.IDE.csproj", "APM.Jetbrains.IDE/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Jetbrains.IDE/APM.Jetbrains.IDE.csproj"
COPY . .
WORKDIR "/src/APM.Jetbrains.IDE"
RUN dotnet build "APM.Jetbrains.IDE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Jetbrains.IDE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Jetbrains.IDE.dll"]
