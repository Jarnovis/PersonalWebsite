# Stage 1: Build stage for .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Clean the NuGet cache to avoid potential corrupted caches
RUN rm -rf /root/.nuget/packages/* && dotnet nuget locals all --clear
RUN dotnet nuget list source | grep -q 'nuget.org' || dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org

# Copy the solution and project files
COPY *.sln ./
COPY Backend/*.csproj ./Backend/

# Restore dependencies
RUN dotnet restore ./Backend/Backend.csproj --no-cache

# Copy the rest of the project files
COPY ./Backend/. ./Backend/

# Publish the application
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish

# Stage 2: Runtime stage for the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install necessary dependencies
RUN apt-get update \
    && apt-get install -y \
    libnss3 \
    libgdk-pixbuf2.0-0 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libcups2 \
    libxss1 \
    libappindicator3-1 \
    fonts-liberation \
    libu2f-udev \
    lsb-release \
    wget \
    curl \
    unzip \
    libgconf-2-4 \
    && apt-get clean

# Copy the selenium-manager binary from the build stage
COPY --from=build /app/publish/selenium-manager/linux/selenium-manager /app/selenium-manager/linux/selenium-manager

# Ensure the selenium-manager binary has execute permissions
RUN chmod +x /app/selenium-manager/linux/selenium-manager

# Copy the published app from the build stage
COPY --from=build /app/publish/ /app/

# Set up the application user and group (optional but recommended for security)
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the ports your application will run on
EXPOSE 5072

# Command to run the application
CMD ["dotnet", "Backend.dll"]
