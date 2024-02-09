FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ACM.Git/ACM.Git.csproj", "ACM.Git/"]
COPY ["ACM.Kernel/ACM.Kernel.csproj", "ACM.Kernel/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "ACM.Git/ACM.Git.csproj"
COPY . .
WORKDIR "/src/ACM.Git"
RUN dotnet build "ACM.Git.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACM.Git.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get -y update && apt-get install -y git
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACM.Git.dll"]
