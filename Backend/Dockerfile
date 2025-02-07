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
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Set a non-root user for security
RUN addgroup --system appgroup && adduser --system --group appuser
USER appuser

# Expose the port your app runs on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Backend.dll"]
