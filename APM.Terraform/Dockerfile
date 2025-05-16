FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["APM.Terraform/APM.Terraform.csproj", "APM.Terraform/"]
COPY ["APC.Kernel/APC.Kernel.csproj", "APC.Kernel/"]
RUN dotnet restore "APM.Terraform/APM.Terraform.csproj"
COPY . .
WORKDIR "/src/APM.Terraform"
RUN dotnet build "APM.Terraform.csproj" -a $TARGETARCH -c Release -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "APM.Terraform.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APM.Terraform.dll"]
