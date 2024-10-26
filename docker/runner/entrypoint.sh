#!/usr/bin/env bash

set -e

REQUIREMENTS="dotnet curl gzip mapshaper"
DEPARTMENT_IDS="01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 21 22 23 24 25 26 27 28 29 2A 2B 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95"


function check_requirements {
    for req in $REQUIREMENTS; do
        if [[ ! -x "$(command -v "$req")" ]]; then
            echo "error: Cannot find $req on path"
            exit 1
        fi
    done
}

function download {
    if [[ ! -f /data/regions.geojson ]]; then
        echo "Downloading regions"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/regions.geojson" >/data/regions.geojson
    fi

    if [[ ! -f /data/departments.geojson ]]; then
        echo "Downloading departments"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/departements.geojson" >/data/departments.geojson
    fi

    mkdir -p /data/towns
    mkdir -p /data/sections
    mkdir -p /data/addresses
    mkdir -p /data/saidplaces

    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "/data/towns/$dep.geojson" ]]; then
            echo "Downloading towns for department $dep"

            # Download Paris from Etalab, get "arrondissements" as towns
            if [[ $dep = "75" ]]; then
                curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-communes.json.gz" >"/data/towns/$dep.geojson.gz"
                gzip -d "/data/towns/$dep.geojson.gz"
            else
                curl "https://geo.api.gouv.fr/departements/$dep/communes?geometry=contour&format=geojson&type=commune-actuelle" >"/data/towns/$dep.geojson"
            fi
        fi

        if [[ ! -f "/data/sections/$dep.geojson" ]]; then
            echo "Downloading sections for department $dep"
            curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-sections.json.gz" >"/data/sections/$dep.geojson.gz"
            gzip -d "/data/sections/$dep.geojson.gz"
        fi

        if [[ ! -f "/data/addresses/$dep.csv" ]]; then
            echo "Downloading addresses for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/adresses-$dep.csv.gz" >"/data/addresses/$dep.csv.gz"
            gzip -d "/data/addresses/$dep.csv.gz"
        fi

        if [[ ! -f "/data/saidplaces/$dep.csv" ]]; then
            echo "Downloading said places for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/lieux-dits-$dep-beta.csv.gz" >"/data/saidplaces/$dep.csv.gz"
            gzip -d "/data/saidplaces/$dep.csv.gz"
        fi
    done

    if [[ ! -f /data/postcodes.csv ]]; then
        echo "Downloading postcodes"
        curl "https://datanova.laposte.fr/data-fair/api/v1/datasets/laposte-hexasmal/data-files/019HexaSmal.csv" >/data/postcodes.csv
    fi

    mkdir -p /data/sales
    dvf_years=$(curl -vs https://files.data.gouv.fr/geo-dvf/latest/csv/ 2>&1 | sed -rn 's/^<a href="([0-9]{4}).*/\1/p')

    for year in $dvf_years; do
        if [[ -f "/data/sales/$year.csv" ]]; then
            continue
        fi

        echo "Downloading sales for year $year"

        if ! curl "https://files.data.gouv.fr/geo-dvf/latest/csv/$year/full.csv.gz" >"/data/sales/$year.csv.gz"; then
            echo "error: Failed to download sales for $year"
            continue
        fi

        gzip -d "/data/sales/$year.csv.gz"
    done
}

function prepare {
    if [[ ! -f /data/regions-simplified.geojson ]]; then
        mapshaper /data/regions.geojson -simplify 10% -o /data/regions-simplified.geojson
    fi

    if [[ ! -f /data/departments-simplified.geojson ]]; then
        mapshaper /data/departments.geojson -simplify 10% -o /data/departments-simplified.geojson
    fi
    
    mkdir -p /data/towns/simplified
    mkdir -p /data/sections/simplified
    
    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "/data/towns/simplified/$dep.geojson" ]]; then
            mapshaper "/data/towns/$dep.geojson" -simplify 10% -o "/data/towns/simplified/$dep.geojson"
        fi
        
        if [[ ! -f "/data/sections/simplified/$dep.geojson" ]]; then
            mapshaper "/data/sections/$dep.geojson" -simplify 10% -o "/data/sections/simplified/$dep.geojson"
        fi
    done
}

function run {
    dotnet /app/Estimmo.Runner.dll "$@"
}

function run_parallel {
    args=""
    module=$1
    shift
    
    for file in "$@"; do
        args+="$module file=$file ; "
    done
   
    run $args
}

function import {
    run MigrateDatabase

    echo "Importing regions"
    run ImportRegions file="/data/regions-simplified.geojson"

    echo "Importing departments"
    run ImportDepartments file="/data/departments-simplified.geojson"

    echo "Importing towns"
    run_parallel "ImportTowns" /data/towns/simplified/*.geojson

    echo "Importing sections"
    run_parallel "ImportSections" /data/sections/simplified/*.geojson

    echo "Importing addresses"
    run_parallel "ImportAddresses" /data/addresses/*.csv

    echo "Calculating street coordinates"
    run ExecuteSqlFile file=calculate_street_coordinates.sql

    echo "Importing said places"
    run_parallel "ImportSaidPlaces" /data/saidplaces/*.csv

    echo "Importing postcodes"
    run ImportPostCodes file="/data/postcodes.csv"

    echo "Importing property sales"
    run_parallel "ImportPropertySales" /data/sales/*.csv

    run RefreshMaterialisedViews type=stats

    echo "Removing outliers"
    run ExecuteSqlFile file=delete_outliers.sql

    echo "Calculating street coordinates"
    run ExecuteSqlFile file=calculate_street_coordinates.sql

    run RefreshMaterialisedViews
}

check_requirements
download
prepare
import
