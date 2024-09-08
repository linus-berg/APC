FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["ACM.Container/ACM.Container.csproj", "ACM.Container/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["ACM.Kernel/ACM.Kernel.csproj", "ACM.Kernel/"]
COPY ["APC.Skopeo/APC.Skopeo.csproj", "APC.Skopeo/"]
RUN dotnet restore "ACM.Container/ACM.Container.csproj"
COPY . .
WORKDIR "/src/ACM.Container"
RUN dotnet build "ACM.Container.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "ACM.Container.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
RUN apt-get -y update && apt-get install -y skopeo
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Container.dll"]
