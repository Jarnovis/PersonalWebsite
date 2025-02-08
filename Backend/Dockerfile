# Use the official .NET 9 SDK image for building the application
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#WORKDIR /app

# Copy the solution file and backend project files from the context
#COPY ./Backend/*.csproj ./Backend/
#COPY *.sln ./

# Restore dependencies for the Backend project
#RUN dotnet restore ./Backend/Backend.csproj

# Copy the rest of the app files and build the project
#COPY ./Backend/. ./Backend/

# Run dotnet restore explicitly again before publish (to make sure packages are resolved)
#RUN dotnet restore ./Backend/Backend.csproj

# Publish the application
#RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for running the application
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
#WORKDIR /app

# Install Chromium, Chromedriver, and required dependencies for Selenium
#RUN apt-get update \
#    && apt-get install -y chromium-browser chromium-chromedriver \
#    && apt-get install -y libgconf-2-4 libnss3 libatk-bridge2.0-0 libx11-xcb1 libxcomposite1 libxrandr2 libgbm1 \
#    && rm -rf /var/lib/apt/lists/*

# Set environment variables for Selenium to use headless Chromium
#ENV DISPLAY=:99
#ENV CHROME_BIN=/usr/bin/chromium-browser
#ENV CHROMEDRIVER_BIN=/usr/lib/chromium-browser/chromedriver

# Copy the published files from the build stage
#COPY --from=build /app/publish .

# Set a non-root user for security
#RUN addgroup --system appgroup && adduser --system --group appuser
#USER appuser

# Expose the port your app runs on
#EXPOSE 8080

# Set the entry point for the container
#ENTRYPOINT ["dotnet", "Backend.dll"]

# Stage 1: Build stage for .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Clean the NuGet cache to avoid potential corrupted caches
RUN rm -rf /root/.nuget/packages/* && dotnet nuget locals all --clear
RUN dotnet nuget list source | grep -q 'nuget.org' || dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
# Copy the .NET solution and restore dependencies
COPY *.sln ./
COPY Backend/*.csproj ./Backend/
# Ensure restore happens before publish
RUN dotnet restore ./Backend/Backend.csproj --no-cache

# Copy the remaining files and build the application
COPY ./Backend/. ./Backend/
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish --no-restore

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

# Copy the published .NET app into the container
COPY --from=build /app/publish .

# Set up the application user and group (optional but recommended for security)
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the ports your application will run on
EXPOSE 5072

# Command to run the application
CMD ["dotnet", "Backend.dll"]

