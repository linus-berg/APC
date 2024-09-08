FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APM.Container/APM.Container.csproj", "APM.Container/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Infrastructure/APC.Infrastructure.csproj", "APC.Infrastructure/"]
COPY ["APC.Skopeo/APC.Skopeo.csproj", "APC.Skopeo/"]
RUN dotnet restore "APM.Container/APM.Container.csproj"
COPY . .
WORKDIR "/src/APM.Container"
RUN dotnet build "APM.Container.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APM.Container.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
RUN apt-get -y update && apt-get install -y skopeo
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Container.dll"]
