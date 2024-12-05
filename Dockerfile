FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY . .
RUN dotnet restore Apps.csproj
RUN dotnet publish Apps.csproj -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
LABEL service="servicestack-apps"
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Apps.dll"]
