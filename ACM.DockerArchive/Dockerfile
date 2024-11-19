FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["ACM.DockerArchive/ACM.DockerArchive.csproj", "ACM.DockerArchive/"]
COPY ["ACM.Kernel/ACM.Kernel.csproj", "ACM.Kernel/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Skopeo/APC.Skopeo.csproj", "APC.Skopeo/"]
RUN dotnet restore "ACM.DockerArchive/ACM.DockerArchive.csproj"
COPY . .
WORKDIR "/src/ACM.DockerArchive"
RUN dotnet build "ACM.DockerArchive.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ACM.DockerArchive.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update && apt-get install -y skopeo
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.DockerArchive.dll"]
