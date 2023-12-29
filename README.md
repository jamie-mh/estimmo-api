![Estimmo API](./doc/logo.png)
<br/>

High-performance French real-estate estimation and history API built with ASP.NET and PostGIS. Using open data, Estimmo provides a visualisation of national property values and sale history.

[Website](https://estimmo.jmh.me)

[API Documentation](https://estimmo-api.jmh.me/swagger)

## Running

No data is provided and must be downloaded and imported.

### Requirements

- .NET 8
- PostgreSQL 14+ database with PostGIS and tdigest extensions
- cURL
- gzip
- Mapshaper

### Generating the database

Update the `appsettings.[env].json` files as required.

The init script will download the required data and process it.

```
./init.sh
```

## Data sources

- [France GeoJSON](https://github.com/gregoiredavid/france-geojson)
- [Etalab Géo API](https://geo.api.gouv.fr)
- [Etalab Cadastre](https://cadastre.data.gouv.fr/datasets/cadastre-etalab)
- [Base Adresse Nationale](https://adresse.data.gouv.fr)
- [Etalab DVF](https://www.data.gouv.fr/fr/datasets/demandes-de-valeurs-foncieres-geolocalisees/)
