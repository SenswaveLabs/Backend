FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

COPY . .
WORKDIR "/src/Presentation/Senswave.Presentation.Seed"

RUN dotnet restore "./Senswave.Presentation.Seed.csproj"
RUN dotnet build "./Senswave.Presentation.Seed.csproj" -c $BUILD_CONFIGURATION
RUN dotnet publish "./Senswave.Presentation.Seed.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y curl

WORKDIR /app

EXPOSE 8080
EXPOSE 8081
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Senswave.Presentation.Seed.dll"]