FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /
COPY . .

RUN dotnet test && \
    dotnet publish ./src/GameOfLife/GameOfLife.csproj -c Release -o ./../_publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

WORKDIR /app
COPY --from=build ./../_publish .

EXPOSE 8080

ENTRYPOINT ["./GameOfLife"]