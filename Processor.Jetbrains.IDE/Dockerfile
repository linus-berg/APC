FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Processor.Jetbrains.IDE/Processor.Jetbrains.IDE.csproj", "Processor.Jetbrains.IDE/"]
COPY ["Core.Kernel/Core.Kernel.csproj", "Core.Kernel/"]
RUN dotnet restore "Processor.Jetbrains.IDE/Processor.Jetbrains.IDE.csproj"
COPY . .
WORKDIR "/src/Processor.Jetbrains.IDE"
RUN dotnet build "Processor.Jetbrains.IDE.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Processor.Jetbrains.IDE.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processor.Jetbrains.IDE.dll"]
