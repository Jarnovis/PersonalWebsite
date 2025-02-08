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

# Install necessary dependencies, including Chromium and Chromedriver for Selenium
RUN apt-get update \
    && apt-get install -y \
    chromium \
    chromium-driver \
    libnss3 \
    libgdk-pixbuf2.0-0 \
    unzip \
    curl \
    && apt-get clean

# Download and install the correct version of Chromedriver
RUN LATEST_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE) \
    && curl -sS https://chromedriver.storage.googleapis.com/$LATEST_VERSION/chromedriver_linux64.zip -o chromedriver.zip \
    && unzip chromedriver.zip \
    && mv chromedriver /usr/local/bin/ \
    && chmod +x /usr/local/bin/chromedriver \
    && rm chromedriver.zip

# Set environment variables for Selenium to use headless Chromium
ENV DISPLAY=:99
ENV CHROME_BIN=/usr/bin/chromium
ENV CHROMEDRIVER_BIN=/usr/local/bin/chromedriver

# Make sure chromedriver and chromium have the correct permissions
RUN chmod +x /usr/local/bin/chromedriver
RUN chmod +x /usr/bin/chromium

# Debug: Verify chromium installation
RUN which chromium

# Copy the published app from the build stage
COPY --from=build /app/publish/ /app/

# Debug: Verify if the files were copied correctly
RUN ls -R /app

# Set up the application user and group (optional but recommended for security)
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the ports your application will run on
EXPOSE 5072

# Ensure the selenium-manager binary has execute permissions
RUN ls -R /app/selenium-manager

# Verify the path to chromium
RUN which chromium

# Switch to root user to change permissions
USER root

# Ensure selenium-manager has execute permissions
RUN chmod +x /app/selenium-manager/linux/selenium-manager
RUN ls -l /app/selenium-manager/linux/selenium-manager

# Switch back to appuser or your regular non-root user
USER appuser

# Run the selenium-manager command
RUN /app/selenium-manager/linux/selenium-manager --browser "chrome" --browser-path "/usr/bin/chromium" --language-binding csharp --output json





# Command to run the application
CMD ["dotnet", "Backend.dll"]
