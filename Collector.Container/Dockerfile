FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Collector.Container/Collector.Container.csproj", "Collector.Container/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["Collector.Kernel/Collector.Kernel.csproj", "Collector.Kernel/"]
COPY ["APC.Skopeo/APC.Skopeo.csproj", "APC.Skopeo/"]
RUN dotnet restore "Collector.Container/Collector.Container.csproj"
COPY . .
WORKDIR "/src/Collector.Container"
RUN dotnet build "Collector.Container.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Collector.Container.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
RUN apt-get -y update && apt-get install -y skopeo
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.Container.dll"]
