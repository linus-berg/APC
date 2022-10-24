FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACM.Container/ACM.Container.csproj", "ACM.Container/"]
RUN dotnet restore "ACM.Container/ACM.Container.csproj"
COPY . .
WORKDIR "/src/ACM.Container"
RUN dotnet build "ACM.Container.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACM.Container.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Container.dll"]
