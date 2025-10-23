FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Collector.Git/Collector.Git.csproj", "Collector.Git/"]
COPY ["Collector.Kernel/Collector.Kernel.csproj", "Collector.Kernel/"]
COPY ["Core.Kernel/Core.Kernel.csproj", "Core.Kernel/"]
RUN dotnet restore "Collector.Git/Collector.Git.csproj"
COPY . .
WORKDIR "/src/Collector.Git"
RUN dotnet build "Collector.Git.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Collector.Git.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
RUN apt-get -y update && apt-get install -y git
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.Git.dll"]
