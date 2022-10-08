FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APC.API/APC.API.csproj", "APC.API/"]
RUN dotnet restore "APC.API/APC.API.csproj"
COPY . .
WORKDIR "/src/APC.API"
RUN dotnet build "APC.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APC.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APC.API.dll"]
