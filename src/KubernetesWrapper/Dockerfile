FROM mcr.microsoft.com/dotnet/sdk:7.0.400-alpine3.17@sha256:887841becc95c276f89ba3d6e73707fd39fe27fb2e6f41ae53de1ec424cd4cdd AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0.10-alpine3.17@sha256:c19b2fe254da9180bcf1c094f91e8955fce94b2e8b0c35a67ad042f860c93b89 AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "KubernetesWrapper.dll"]
