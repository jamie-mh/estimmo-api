![Estimmo API](./doc/logo.png)
<br/>

High-performance French real-estate estimation and history API built with ASP.NET and PostGIS. Using open data, Estimmo provides a visualisation of national property values and sale history.

[Website](https://estimmo.jmh.me)

[API Documentation](https://estimmo-api.jmh.me/swagger)

## Data sources

- [France GeoJSON](https://github.com/gregoiredavid/france-geojson)
- [Etalab Géo API](https://geo.api.gouv.fr)
- [Etalab Cadastre](https://cadastre.data.gouv.fr/datasets/cadastre-etalab)
- [Base Adresse Nationale](https://adresse.data.gouv.fr)
- [Etalab DVF](https://www.data.gouv.fr/fr/datasets/demandes-de-valeurs-foncieres-geolocalisees/)

## Running the API

Download the Docker Compose file (`compose.yaml`) and run it to start the service.

```
docker compose up -d
```

The API should now be accessible at `http://localhost:8080`.

### Populating the database

The runner image downloads all the necessary data and imports it into the database.

It can be run using the following command:

```
docker run -v ./data:/data --net estimmo_estimmo --user "$(id -u):$(id -g)" ghcr.io/jamie-mh/estimmo-runner:latest
```

This will download the data into the `data` directory relative to where the command is run.

By default, this will run all the steps. Alternatively, an argument can be passed to the container to select the desired step (`download`, `prepare`, `import`).
