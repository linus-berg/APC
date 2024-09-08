FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["APM.Jetbrains.IDE/APM.Jetbrains.IDE.csproj", "APM.Jetbrains.IDE/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Jetbrains.IDE/APM.Jetbrains.IDE.csproj"
COPY . .
WORKDIR "/src/APM.Jetbrains.IDE"
RUN dotnet build "APM.Jetbrains.IDE.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "APM.Jetbrains.IDE.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Jetbrains.IDE.dll"]
