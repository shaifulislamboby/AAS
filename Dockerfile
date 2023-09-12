#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80


FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["HelloAssetAdministrationShell/HelloAssetAdministrationShell.csproj", "HelloAssetAdministrationShell/"]
RUN dotnet restore "HelloAssetAdministrationShell/HelloAssetAdministrationShell.csproj"
COPY . .
WORKDIR "/src/HelloAssetAdministrationShell"
RUN dotnet build "HelloAssetAdministrationShell.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelloAssetAdministrationShell.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "HelloAssetAdministrationShell.dll"]