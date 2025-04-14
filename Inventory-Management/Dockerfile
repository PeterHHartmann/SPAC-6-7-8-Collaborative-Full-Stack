FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runner
WORKDIR /App
COPY --from=builder /App/out .
COPY --from=assets .env ../
ENTRYPOINT ["dotnet", "Inventory-Management.dll"]

