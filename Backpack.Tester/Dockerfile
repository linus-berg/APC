FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Backpack.Tester/Backpack.Tester.csproj", "Backpack.Tester/"]
RUN dotnet restore "Backpack.Tester/Backpack.Tester.csproj"
COPY . .
WORKDIR "/src/Backpack.Tester"
RUN dotnet build "Backpack.Tester.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Backpack.Tester.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backpack.Tester.dll"]
