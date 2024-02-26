# Use a base image that contains tools for generating certificates (e.g., openssl)
FROM alpine AS certificate_generator

RUN apk --no-cache add openssl

RUN mkdir -p /app

# Run commands to generate the self-signed certificate
RUN openssl req -x509 -newkey rsa:4096 -keyout /app/cert.key -out /app/cert.crt -days 365 -nodes -subj "/CN=myapp.com" -passout pass:yourpassphrase

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app


# Copy the generated certificates from the certificate_generator stage
COPY --from=certificate_generator /app/cert.crt /app/cert.crt
COPY --from=certificate_generator /app/cert.key /app/cert.key

# Set up HTTPS configuration to use the copied certificate
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=yourpassphrase
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/app/cert.key

#Set MES_AAS_ENV Variables
ENV BROKER_ADDRESS="test.mosquitto.org"
ENV BROKER_PORT=1883
ENV SUBSCRIPTION_TOPIC="BasyxMesAASOrderHandling"
ENV PUBLICATION_TOPIC="aas-notification"
ENV PUBLICATION_TOPIC_USE_CASE2="Basys/aas-notification/usecase2"
ENV MES_ENDPOINT="https://b184-141-44-206-87.ngrok-free.app/api/MesOrder/"
ENV MES_ENDPOINT_USE_CASE2="http://localhost:7486/api/Usecase2endpoint",

ENV MES_AAS_ENDPOINT="http://host.docker.internal:5111"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

EXPOSE 6499
EXPOSE 6999
EXPOSE 5499
EXPOSE 5999
EXPOSE 5411
EXPOSE 443
EXPOSE 5222
EXPOSE 5422
EXPOSE 5111
EXPOSE 8001

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
