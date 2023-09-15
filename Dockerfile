FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 6499
EXPOSE 6999
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:6499;http://+:6999;http://+:443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["ComplexAssetAdministrationShellScenario/ComplexAssetAdministrationShellScenario.csproj", "ComplexAssetAdministrationShellScenario/"]
RUN dotnet restore "ComplexAssetAdministrationShellScenario/ComplexAssetAdministrationShellScenario.csproj"
COPY . .
WORKDIR "/src/ComplexAssetAdministrationShellScenario"
RUN dotnet build "ComplexAssetAdministrationShellScenario.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComplexAssetAdministrationShellScenario.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "ComplexAssetAdministrationShellScenario.dll"]
