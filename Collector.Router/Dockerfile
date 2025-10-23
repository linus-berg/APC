FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Collector.Router/Collector.Router.csproj", "Collector.Router/"]
COPY ["Core.Kernel/Core.Kernel.csproj", "Core.Kernel/"]
RUN dotnet restore "Collector.Router/Collector.Router.csproj"
COPY . .
WORKDIR "/src/Collector.Router"
RUN dotnet build "Collector.Router.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Collector.Router.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.Router.dll"]
