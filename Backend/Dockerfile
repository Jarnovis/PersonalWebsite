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
RUN ls -R /app/publish

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

# Install Selenium WebDriver if needed
# (This step may vary depending on how your app interacts with Selenium)
RUN curl -sS https://chromedriver.storage.googleapis.com/114.0.5735.90/chromedriver_linux64.zip -o chromedriver.zip \
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
    
#Debug
RUN which chromium

# Copy the entire publish directory
COPY --from=build /app/publish/selenium-manager/linux/selenium-manager /app/selenium-manager

# Verify the selenium-manager files are copied correctly
RUN ls -R /app/publish


# Copy the published .NET app into the container
COPY --from=build /app/publish .

# Set up the application user and group (optional but recommended for security)
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the ports your application will run on
EXPOSE 5072

# Command to run the application
CMD ["dotnet", "Backend.dll"]

