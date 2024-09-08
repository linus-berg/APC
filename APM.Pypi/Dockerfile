FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["APM.Pypi/APM.Pypi.csproj", "APM.Pypi/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Pypi/APM.Pypi.csproj"
COPY . .
WORKDIR "/src/APM.Pypi"
RUN dotnet build "APM.Pypi.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "APM.Pypi.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Pypi.dll"]
