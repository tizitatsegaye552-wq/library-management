FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Environment variables for Fly.io / Docker
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LibraryManagement.csproj", "./"]
RUN dotnet restore "LibraryManagement.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LibraryManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibraryManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a directory for the database to allow volume mounting
RUN mkdir -p /data
# Change the connection string at runtime using an environment variable
ENV ConnectionStrings__DefaultConnection="Data Source=/data/Library.db"

ENTRYPOINT ["dotnet", "LibraryManagement.dll"]
