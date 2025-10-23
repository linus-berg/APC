FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["Backpack.Toolbox/Backpack.Toolbox.csproj", "Backpack.Toolbox/"]
RUN dotnet restore "Backpack.Toolbox/Backpack.Toolbox.csproj"
COPY . .
WORKDIR "/src/Backpack.Toolbox"
RUN dotnet build "Backpack.Toolbox.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Backpack.Toolbox.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backpack.Toolbox.dll"]
