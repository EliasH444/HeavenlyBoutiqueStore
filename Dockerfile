# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first (better layer caching)
COPY ["learning/learning.csproj", "learning/"]
RUN dotnet restore "learning/learning.csproj"

# Copy everything else
COPY . .

# Set working directory to the project folder
WORKDIR /src/learning

# Publish the application
RUN dotnet publish "learning.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Production environment settings
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 80

# Copy published output from build stage
COPY --from=build /app/publish .

# Run the app
ENTRYPOINT ["dotnet", "learning.dll"]
