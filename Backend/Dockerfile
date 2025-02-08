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

# Install dependencies including Chromium, ChromiumDriver, Xvfb, and other libraries
RUN apt-get update \
    && apt-get install -y \
       chromium \
       chromium-driver \
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
       libgl1-mesa-glx \
       libegl1-mesa \
       libgbm1 \
       libxshmfence1 \
       xvfb \
    && apt-get clean

# Create appuser and appgroup before using chown
RUN addgroup --system appgroup && adduser --system --group appuser

# Fix permissions for cache folder
RUN mkdir -p /app/.cache/selenium && chown -R appuser:appgroup /app/.cache

# Fix permissions for Xvfb
RUN mkdir -p /tmp/.X11-unix && chmod 1777 /tmp/.X11-unix

# Set environment variables for Selenium to use headless Chromium
ENV DISPLAY=:99
ENV CHROME_BIN=/usr/bin/chromium
ENV CHROMEDRIVER_BIN=/usr/local/bin/chromedriver

# Set the correct Selenium cache path
ENV SELENIUM_CACHE=/app/.cache/selenium

# Ensure cache directory exists and has correct permissions
RUN mkdir -p /app/.cache/selenium && chown -R appuser:appgroup /app/.cache

# Ensure compatibility of Chromedriver with Chromium version
RUN LATEST_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE_133) \
    && curl -sS https://chromedriver.storage.googleapis.com/$LATEST_VERSION/chromedriver_linux64.zip -o chromedriver.zip \
    && unzip chromedriver.zip \
    && mv chromedriver /usr/local/bin/ \
    && chmod +x /usr/local/bin/chromedriver \
    && rm chromedriver.zip


# Copy the published app from the build stage
COPY --from=build /app/publish/ /app/

# Expose the port your application will run on
EXPOSE 5072

# Ensure the selenium-manager binary has execute permissions
RUN chmod +x /app/selenium-manager/linux/selenium-manager
RUN chmod +x /usr/local/bin/chromedriver

# Start Xvfb in the background, then run the selenium-manager command
RUN echo "#!/bin/bash \n\
    Xvfb :99 -ac & \n\
    export DISPLAY=:99 \n\
    /app/selenium-manager/linux/selenium-manager --browser \"chrome\" --browser-path \"/usr/bin/chromium\" --language-binding csharp --output json" > /app/start-selenium.sh

RUN chmod +x /app/start-selenium.sh

# Set the user to run the application as a non-root user (appuser)
USER appuser

# Command to run the application
CMD ["/app/start-selenium.sh"]
