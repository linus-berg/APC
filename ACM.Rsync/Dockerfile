FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["ACM.Rsync/ACM.Rsync.csproj", "ACM.Rsync/"]
RUN dotnet restore "ACM.Rsync/ACM.Rsync.csproj"
COPY . .
WORKDIR "/src/ACM.Rsync"
RUN dotnet build "ACM.Rsync.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "ACM.Rsync.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Rsync.dll"]
