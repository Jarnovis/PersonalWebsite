# Stage 1: Build stage for .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Clean the NuGet cache
RUN rm -rf /root/.nuget/packages/* && dotnet nuget locals all --clear
RUN dotnet nuget list source | grep -q 'nuget.org' || dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org

# Copy solution and project files
COPY *.sln ./
COPY Backend/*.csproj ./Backend/

# Restore dependencies
RUN dotnet restore ./Backend/Backend.csproj --no-cache

# Copy the rest of the project files
COPY ./Backend/. ./Backend/

# Publish the application
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish

# Debugging: Check if the Backend.dll exists
RUN ls -R /app/publish
RUN ls -R /app/publish/Backend  # Check if Backend.dll is in this directory

# Stage 2: Runtime stage for the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install dependencies
RUN apt-get update \
    && apt-get install -y \
    chromium \
    chromium-driver \
    libnss3 \
    libgdk-pixbuf2.0-0 \
    unzip \
    curl \
    && apt-get clean

# Install Selenium WebDriver
RUN curl -sS https://chromedriver.storage.googleapis.com/114.0.5735.90/chr  -o chromedriver.zip \
    && unzip chromedriver.zip \
    && mv chromedriver /usr/local/bin/ \
    && chmod +x /usr/local/bin/chromedriver \
    && rm chromedriver.zip

# Set up environment variables for Selenium
ENV DISPLAY=:99
ENV CHROME_BIN=/usr/bin/chromium
ENV CHROMEDRIVER_BIN=/usr/local/bin/chromedriver

# Check if Chromium is available
RUN which chromium

# Copy the entire publish directory to the runtime container
COPY --from=build /app/publish /app/publish

# Check if the Backend.dll file is copied correctly
RUN ls -R /app/publish  # Check the runtime image's /app/publish directory

# Expose ports
EXPOSE 5072

# Set up user for security
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Command to run the application
CMD ["dotnet", "/app/publish/Backend.dll"]
