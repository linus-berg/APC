FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Processor.Rancher/Processor.Rancher.csproj", "Processor.Rancher/"]
COPY ["Core.Kernel/Core.Kernel.csproj", "Core.Kernel/"]
COPY ["Library.Github/Library.Github.csproj", "Library.Github/"]
RUN dotnet restore "Processor.Rancher/Processor.Rancher.csproj"
COPY . .
WORKDIR "/src/Processor.Rancher"
RUN dotnet build "Processor.Rancher.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Processor.Rancher.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processor.Rancher.dll"]
