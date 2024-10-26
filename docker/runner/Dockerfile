FROM mcr.microsoft.com/dotnet/runtime:8.0-noble
LABEL org.opencontainers.image.source=https://github.com/jamie-mh/estimmo-api

RUN apt-get update && apt-get install -y curl nodejs npm
RUN npm install -g mapshaper

WORKDIR /app
COPY Estimmo.Runner/bin/Release/net8.0/linux-x64/publish /app
COPY docker/runner/entrypoint.sh /app

RUN mkdir /data
VOLUME /data

ENTRYPOINT ["/app/entrypoint.sh"]
