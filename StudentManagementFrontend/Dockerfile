# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["StudentManagementFrontend.csproj", "./"]
RUN dotnet restore "StudentManagementFrontend.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "StudentManagementFrontend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StudentManagementFrontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install nginx for serving static files
RUN apt-get update && apt-get install -y nginx && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/sites-available/default

# Expose port
EXPOSE 5000

# Create startup script
RUN echo '#!/bin/bash\n\
service nginx start\n\
dotnet StudentManagementFrontend.dll' > /app/start.sh && chmod +x /app/start.sh

CMD ["/app/start.sh"]