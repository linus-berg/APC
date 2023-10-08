FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACM.Rsync/ACM.Rsync.csproj", "ACM.Rsync/"]
RUN dotnet restore "ACM.Rsync/ACM.Rsync.csproj"
COPY . .
WORKDIR "/src/ACM.Rsync"
RUN dotnet build "ACM.Rsync.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACM.Rsync.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Rsync.dll"]
