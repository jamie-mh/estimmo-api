#!/usr/bin/env bash

set -e

REQUIREMENTS="dotnet curl gzip mapshaper"
DEPARTMENT_IDS="01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 21 22 23 24 25 26 27 28 29 2A 2B 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95"
DOTNET_ARGS="--configuration Release --project Estimmo.Runner/Estimmo.Runner.csproj"


function check_requirements {
    for req in $REQUIREMENTS; do
        if [[ ! -x "$(command -v "$req")" ]]; then
            echo "error: Cannot find $req on path"
            exit 1
        fi
    done
}

function download {
    mkdir -p download

    if [[ ! -f download/regions.geojson ]]; then
        echo "Downloading regions"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/regions.geojson" >download/regions.geojson
    fi

    if [[ ! -f download/departments.geojson ]]; then
        echo "Downloading departments"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/departements.geojson" >download/departments.geojson
    fi

    mkdir -p download/towns
    mkdir -p download/sections
    mkdir -p download/addresses
    mkdir -p download/saidplaces

    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "download/towns/$dep.geojson" ]]; then
            echo "Downloading towns for department $dep"

            # Download Paris from Etalab, get "arrondissements" as towns
            if [[ $dep = "75" ]]; then
                curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-communes.json.gz" >"download/towns/$dep.geojson.gz"
                gzip -d "download/towns/$dep.geojson.gz"
            else
                curl "https://geo.api.gouv.fr/departements/$dep/communes?geometry=contour&format=geojson&type=commune-actuelle" >"download/towns/$dep.geojson"
            fi
        fi

        if [[ ! -f "download/sections/$dep.geojson" ]]; then
            echo "Downloading sections for department $dep"
            curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-sections.json.gz" >"download/sections/$dep.geojson.gz"
            gzip -d "download/sections/$dep.geojson.gz"
        fi

        if [[ ! -f "download/addresses/$dep.csv" ]]; then
            echo "Downloading addresses for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/adresses-$dep.csv.gz" >"download/addresses/$dep.csv.gz"
            gzip -d "download/addresses/$dep.csv.gz"
        fi

        if [[ ! -f "download/saidplaces/$dep.csv" ]]; then
            echo "Downloading said places for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/lieux-dits-$dep-beta.csv.gz" >"download/saidplaces/$dep.csv.gz"
            gzip -d "download/saidplaces/$dep.csv.gz"
        fi
    done

    if [[ ! -f download/postcodes.csv ]]; then
        echo "Downloading postcodes"
        curl "https://datanova.laposte.fr/data-fair/api/v1/datasets/laposte-hexasmal/data-files/019HexaSmal.csv" >download/postcodes.csv
    fi

    mkdir -p download/sales
    dvf_years=$(curl -vs https://files.data.gouv.fr/geo-dvf/latest/csv/ 2>&1 | sed -rn 's/^<a href="([0-9]{4}).*/\1/p')

    for year in $dvf_years; do
        if [[ -f "download/sales/$year.csv" ]]; then
            continue
        fi

        echo "Downloading sales for year $year"

        if ! curl "https://files.data.gouv.fr/geo-dvf/latest/csv/$year/full.csv.gz" >"download/sales/$year.csv.gz"; then
            echo "error: Failed to download sales for $year"
            continue
        fi

        gzip -d "download/sales/$year.csv.gz"
    done
}

function prepare {
    if [[ ! -f download/regions-simplified.geojson ]]; then
        mapshaper download/regions.geojson -simplify 10% -o download/regions-simplified.geojson
    fi

    if [[ ! -f download/departments-simplified.geojson ]]; then
        mapshaper download/departments.geojson -simplify 10% -o download/departments-simplified.geojson
    fi
    
    mkdir -p download/towns/simplified
    mkdir -p download/sections/simplified
    
    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "download/towns/simplified/$dep.geojson" ]]; then
            mapshaper "download/towns/$dep.geojson" -simplify 10% -o "download/towns/simplified/$dep.geojson"
        fi
        
        if [[ ! -f "download/sections/simplified/$dep.geojson" ]]; then
            mapshaper "download/sections/$dep.geojson" -simplify 10% -o "download/sections/simplified/$dep.geojson"
        fi
    done
}

function run_parallel_import {
    args=""
    module=$1
    shift
    
    for file in "$@"; do
        args+="$module file=$file ; "
    done
   
    dotnet run $DOTNET_ARGS $args
}

function import {
    dotnet run $DOTNET_ARGS MigrateDatabase

    echo "Importing regions"
    dotnet run $DOTNET_ARGS ImportRegions file="download/regions-simplified.geojson"

    echo "Importing departments"
    dotnet run $DOTNET_ARGS ImportDepartments file="download/departments-simplified.geojson"

    echo "Importing towns"
    run_parallel_import "ImportTowns" download/towns/simplified/*.geojson

    echo "Importing sections"
    run_parallel_import "ImportSections" download/sections/simplified/*.geojson

    echo "Importing addresses"
    run_parallel_import "ImportAddresses" download/addresses/*.csv

    echo "Calculating street coordinates"
    dotnet run $DOTNET_ARGS ExecuteSqlFile file=calculate_street_coordinates.sql

    echo "Importing said places"
    run_parallel_import "ImportSaidPlaces" download/saidplaces/*.csv

    echo "Importing postcodes"
    dotnet run $DOTNET_ARGS ImportPostCodes file="download/postcodes.csv"

    echo "Importing property sales"
    run_parallel_import "ImportPropertySales" download/sales/*.csv

    dotnet run $DOTNET_ARGS RefreshMaterialisedViews type=stats

    echo "Removing outliers"
    dotnet run $DOTNET_ARGS ExecuteSqlFile file=delete_outliers.sql

    echo "Calculating street coordinates"
    dotnet run $DOTNET_ARGS ExecuteSqlFile file=calculate_street_coordinates.sql

    dotnet run $DOTNET_ARGS RefreshMaterialisedViews
}

check_requirements
download
prepare
import
