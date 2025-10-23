FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Processor.Github.Releases/Processor.Github.Releases.csproj", "Processor.Github.Releases/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
COPY ["APC.Github/APC.Github.csproj", "APC.Github/"]
RUN dotnet restore "Processor.Github.Releases/Processor.Github.Releases.csproj"
COPY . .
WORKDIR "/src/Processor.Github.Releases"
RUN dotnet build "Processor.Github.Releases.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Processor.Github.Releases.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processor.Github.Releases.dll"]
