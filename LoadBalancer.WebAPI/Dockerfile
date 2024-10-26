FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LoadBalancer.WebAPI/LoadBalancer.WebAPI.csproj", "LoadBalancer.WebAPI/"]
RUN dotnet restore "./LoadBalancer.WebAPI/LoadBalancer.WebAPI.csproj"
COPY . .
WORKDIR "/src/LoadBalancer.WebAPI"
RUN dotnet build "./LoadBalancer.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./LoadBalancer.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoadBalancer.WebAPI.dll"]