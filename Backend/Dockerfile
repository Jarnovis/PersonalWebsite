# Stage 1: Build stage for .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Clean NuGet cache and ensure sources
RUN rm -rf /root/.nuget/packages/* && \
    dotnet nuget locals all --clear && \
    (dotnet nuget list source | grep -q 'nuget.org' || dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org)

# Copy the solution and project files
COPY *.sln ./
COPY Backend/*.csproj ./Backend/

# Restore dependencies (using --disable-parallel to avoid restore conflicts)
RUN dotnet restore ./Backend/Backend.csproj --no-cache --disable-parallel

# Copy the rest of the project files
COPY ./Backend/. ./Backend/

# Publish the application
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish

# Stage 2: Runtime stage for the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Install dependencies in parallel where possible
RUN apt-get update -y && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    unzip curl gnupg2 xvfb libxi6 libgconf-2-4 libxkbcommon-x11-0 libxshmfence1 \
    fonts-liberation libasound2 libatk-bridge2.0-0 libatk1.0-0 libatspi2.0-0 libcairo2 \
    libcups2 libdbus-1-3 libdrm2 libgbm1 libglib2.0-0 libgtk-3-0 libnspr4 libnss3 \
    libpango-1.0-0 libu2f-udev libvulkan1 libx11-6 libxcb1 libxcomposite1 \
    libxdamage1 libxext6 libxfixes3 libxkbcommon0 libxrandr2 xdg-utils wget \
    libcanberra-gtk* libgl1-mesa-dri libgl1-mesa-glx libpulse0 libv4l-0 fonts-symbola \
    && rm -rf /var/lib/apt/lists/*

# Install Chrome and ChromeDriver in parallel
RUN (curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add -) & \
    echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
    apt-get update && apt-get install -y --no-install-recommends google-chrome-stable && \
    wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
    dpkg -i google-chrome-stable_current_amd64.deb || apt-get -fy install && \
    rm -rf /var/lib/apt/lists/*

# Ensure Selenium Manager is executable
RUN chmod +x /app/selenium-manager/linux/selenium-manager

# Set the entry point to run the .NET application
ENTRYPOINT [ "dotnet", "Backend.dll" ]
