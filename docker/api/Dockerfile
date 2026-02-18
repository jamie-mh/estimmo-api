FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
LABEL org.opencontainers.image.source=https://github.com/jamie-mh/estimmo-api

RUN apk --no-cache add curl

WORKDIR /app
COPY Estimmo.Api/bin/Release/net10.0/linux-x64/publish /app

ENV PORT 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "/app/Estimmo.Api.dll"]
