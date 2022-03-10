#!/usr/bin/env bash

set -e

function download {
    mkdir -p download

    echo "Downloading regions"
    curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/regions.geojson" > download/regions.geojson

    echo "Downloading departments"
    curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/departements.geojson" > download/departments.geojson

    echo "Downloading towns and sections"
    mkdir -p download/towns
    mkdir -p download/sections

    DEPARTMENT_IDS="01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 21 22 23 24 25 26 27 28 29 2A 2B 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95"

    for dep in $DEPARTMENT_IDS ; do
        echo "Downloading towns for department $dep"

        # Download Paris from Etalab, get "arrondissements" as towns
        if [[ $dep = "75" ]] ; then
            curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-communes.json.gz" > "download/towns/$dep.geojson.gz"
            gzip -d "download/towns/$dep.geojson.gz"
        else
            curl "https://geo.api.gouv.fr/departements/$dep/communes?geometry=contour&format=geojson&type=commune-actuelle" > "download/towns/$dep.geojson"
        fi

        echo "Downloading sections for department $dep"
        curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-sections.json.gz" > "download/sections/$dep.geojson.gz"
        gzip -d "download/sections/$dep.geojson.gz"
    done

    echo "Downloading postcodes"
    curl "https://datanova.legroupe.laposte.fr/explore/dataset/laposte_hexasmal/download/?format=csv&timezone=Europe/Berlin&use_labels_for_header=true" > download/postcodes.csv

    echo "Downloading property sales"
    mkdir -p download/sales

    DVF_YEARS="2016 2017 2018 2019 2020 2021"

    for year in $DVF_YEARS ; do
        echo "Downloading sales for year $year"
        curl "https://files.data.gouv.fr/geo-dvf/latest/csv/$year/full.csv.gz" > "download/sales/$year.csv.gz"
        gzip -d "download/sales/$year.csv.gz"
    done
}

function import {
    DOTNET_ARGS="--configuration Release --project Estimmo.Runner/Estimmo.Runner.csproj"
    dotnet run $DOTNET_ARGS MigrateDatabase

    echo "Importing regions"
    dotnet run $DOTNET_ARGS ImportRegions download/regions.geojson

    echo "Importing departments"
    dotnet run $DOTNET_ARGS ImportDepartments download/departments.geojson

    echo "Importing towns"
    for file in download/towns/*.geojson ; do
        dotnet run $DOTNET_ARGS ImportTowns $file
    done

    echo "Importing sections"
    for file in download/sections/*.geojson ; do
        dotnet run $DOTNET_ARGS ImportSections $file
    done

    echo "Importing postcodes"
    dotnet run $DOTNET_ARGS ImportPostCodes download/postcodes.csv

    echo "Importing property sales"
    for file in download/sales/*.csv ; do
        dotnet run $DOTNET_ARGS ImportPropertySales $file
    done

    dotnet run $DOTNET_ARGS RefreshMaterialisedViews
    dotnet run $DOTNET_ARGS AddUser estimmo "3sP9S^3gzg5SYyvQ"
}

if [[ ! -d "download" ]] ; then
    download
fi

import
