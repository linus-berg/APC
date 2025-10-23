FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["Core.Kernel/Core.Kernel.csproj", "Core.Kernel/"]
COPY ["Backpack.GitUnpack/Backpack.GitUnpack.csproj", "Backpack.GitUnpack/"]
RUN dotnet restore "Backpack.GitUnpack/Backpack.GitUnpack.csproj"
COPY . .
WORKDIR "/src/Backpack.GitUnpack"
RUN dotnet build "Backpack.GitUnpack.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Backpack.GitUnpack.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update && apt-get install -y git
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backpack.GitUnpack.dll"]
