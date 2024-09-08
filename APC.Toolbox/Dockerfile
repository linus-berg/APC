FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["APC.Toolbox/APC.Toolbox.csproj", "APC.Toolbox/"]
RUN dotnet restore "APC.Toolbox/APC.Toolbox.csproj"
COPY . .
WORKDIR "/src/APC.Toolbox"
RUN dotnet build "APC.Toolbox.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APC.Toolbox.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APC.Toolbox.dll"]
