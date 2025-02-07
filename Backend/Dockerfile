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

# Use the official .NET 9 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution file and backend project files from the context
COPY ./Backend/*.csproj ./Backend/
COPY *.sln ./

# Restore dependencies for the Backend project
RUN dotnet restore ./Backend/Backend.csproj

# Copy the rest of the app files and build the project
COPY ./Backend/. ./Backend/

# Run dotnet restore explicitly again before publish (to make sure packages are resolved)
RUN dotnet restore ./Backend/Backend.csproj

# Publish the application
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install Chromium, Chromedriver, and required dependencies for Selenium
RUN apt-get update \
    && apt-get install -y chromium-browser chromium-chromedriver \
    && apt-get install -y libgconf-2-4 libnss3 libatk-bridge2.0-0 libx11-xcb1 libxcomposite1 libxrandr2 libgbm1 \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

# Install Selenium Manager globally to manage drivers
RUN curl -sS https://selenium-release.storage.googleapis.com/selenium-manager-linux-x64.tar.gz -o selenium-manager.tar.gz \
    && tar -xzf selenium-manager.tar.gz \
    && mv selenium-manager-linux-x64 /usr/local/bin/selenium-manager \
    && chmod +x /usr/local/bin/selenium-manager \
    && rm selenium-manager.tar.gz

# Set environment variables for Selenium to use headless Chromium
ENV DISPLAY=:99
ENV CHROME_BIN=/usr/bin/chromium-browser
ENV CHROMEDRIVER_BIN=/usr/lib/chromium-browser/chromedriver

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Set a non-root user for security
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the port your app runs on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Backend.dll"]
