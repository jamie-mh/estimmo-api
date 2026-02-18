#!/usr/bin/env bash

set -e

REQUIREMENTS="dotnet curl gzip mapshaper"
DEPARTMENT_IDS="01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 21 22 23 24 25 26 27 28 29 2A 2B 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95"
DATA_DIR="/data"


function check_requirements {
    for req in $REQUIREMENTS; do
        if [[ ! -x "$(command -v "$req")" ]]; then
            echo "error: Cannot find $req on path"
            exit 1
        fi
    done
}

function download {
    if [[ ! -f "$DATA_DIR/regions.geojson" ]]; then
        echo "Downloading regions"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/regions.geojson" >"$DATA_DIR/regions.geojson"
    fi

    if [[ ! -f "$DATA_DIR/departments.geojson" ]]; then
        echo "Downloading departments"
        curl "https://raw.githubusercontent.com/gregoiredavid/france-geojson/5d34ee6d0140c29f785fdb047d9329f1aab58833/departements.geojson" >"$DATA_DIR/departments.geojson"
    fi

    mkdir -p $DATA_DIR/towns
    mkdir -p $DATA_DIR/sections
    mkdir -p $DATA_DIR/addresses
    mkdir -p $DATA_DIR/saidplaces

    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "$DATA_DIR/towns/$dep.geojson" ]]; then
            echo "Downloading towns for department $dep"

            # Download Paris from Etalab, get "arrondissements" as towns
            if [[ $dep = "75" ]]; then
                curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-communes.json.gz" >"$DATA_DIR/towns/$dep.geojson.gz"
                gzip -d "$DATA_DIR/towns/$dep.geojson.gz"
            else
                curl "https://geo.api.gouv.fr/departements/$dep/communes?geometry=contour&format=geojson&type=commune-actuelle" >"$DATA_DIR/towns/$dep.geojson"
            fi
        fi

        if [[ ! -f "$DATA_DIR/sections/$dep.geojson" ]]; then
            echo "Downloading sections for department $dep"
            curl "https://cadastre.data.gouv.fr/data/etalab-cadastre/latest/geojson/departements/$dep/cadastre-$dep-sections.json.gz" >"$DATA_DIR/sections/$dep.geojson.gz"
            gzip -d "$DATA_DIR/sections/$dep.geojson.gz"
        fi

        if [[ ! -f "$DATA_DIR/addresses/$dep.csv" ]]; then
            echo "Downloading addresses for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/adresses-$dep.csv.gz" >"$DATA_DIR/addresses/$dep.csv.gz"
            gzip -d "$DATA_DIR/addresses/$dep.csv.gz"
        fi

        if [[ ! -f "$DATA_DIR/saidplaces/$dep.csv" ]]; then
            echo "Downloading said places for department $dep"
            curl "https://adresse.data.gouv.fr/data/ban/adresses/latest/csv/lieux-dits-$dep-beta.csv.gz" >"$DATA_DIR/saidplaces/$dep.csv.gz"
            gzip -d "$DATA_DIR/saidplaces/$dep.csv.gz"
        fi
    done

    if [[ ! -f "$DATA_DIR/postcodes.csv" ]]; then
        echo "Downloading postcodes"
        curl "https://datanova.laposte.fr/data-fair/api/v1/datasets/laposte-hexasmal/data-files/019HexaSmal.csv" >"$DATA_DIR/postcodes.csv"
    fi

    mkdir -p "$DATA_DIR/sales"
    dvf_years=$(curl -vs https://files.data.gouv.fr/geo-dvf/latest/csv/ 2>&1 | sed -rn 's/^<a href="([0-9]{4}).*/\1/p')

    for year in $dvf_years; do
        if [[ -f "$DATA_DIR/sales/$year.csv" ]]; then
            continue
        fi

        echo "Downloading sales for year $year"

        if ! curl "https://files.data.gouv.fr/geo-dvf/latest/csv/$year/full.csv.gz" >"$DATA_DIR/sales/$year.csv.gz"; then
            echo "error: Failed to download sales for $year"
            continue
        fi

        gzip -d "$DATA_DIR/sales/$year.csv.gz"
    done
}

function prepare {
    if [[ ! -f "$DATA_DIR/regions-simplified.geojson" ]]; then
        mapshaper "$DATA_DIR/regions.geojson" -simplify 10% -o "$DATA_DIR/regions-simplified.geojson"
    fi

    if [[ ! -f "$DATA_DIR/departments-simplified.geojson" ]]; then
        mapshaper "$DATA_DIR/departments.geojson" -simplify 10% -o "$DATA_DIR/departments-simplified.geojson"
    fi
    
    mkdir -p "$DATA_DIR/towns/simplified"
    mkdir -p "$DATA_DIR/sections/simplified"
    
    for dep in $DEPARTMENT_IDS; do
        if [[ ! -f "$DATA_DIR/towns/simplified/$dep.geojson" ]]; then
            mapshaper "$DATA_DIR/towns/$dep.geojson" -simplify 10% -o "$DATA_DIR/towns/simplified/$dep.geojson"
        fi
        
        if [[ ! -f "$DATA_DIR/sections/simplified/$dep.geojson" ]]; then
            mapshaper "$DATA_DIR/sections/$dep.geojson" -simplify 10% -o "$DATA_DIR/sections/simplified/$dep.geojson"
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
    run ImportRegions file="$DATA_DIR/regions-simplified.geojson"

    echo "Importing departments"
    run ImportDepartments file="$DATA_DIR/departments-simplified.geojson"

    echo "Importing towns"
    run_parallel "ImportTowns" "$DATA_DIR/towns/simplified"/*.geojson

    echo "Importing sections"
    run_parallel "ImportSections" "$DATA_DIR/sections/simplified"/*.geojson

    echo "Importing addresses"
    run_parallel "ImportAddresses" "$DATA_DIR/addresses"/*.csv

    echo "Calculating street coordinates"
    run ExecuteSqlFile file=calculate_street_coordinates.sql

    echo "Importing said places"
    run_parallel "ImportSaidPlaces" "$DATA_DIR/saidplaces"/*.csv

    echo "Importing postcodes"
    run ImportPostCodes file="$DATA_DIR/postcodes.csv"

    echo "Importing property sales"
    run_parallel "ImportPropertySales" "$DATA_DIR/sales"/*.csv

    run RefreshMaterialisedViews type=stats

    echo "Removing outliers"
    run ExecuteSqlFile file=delete_outliers.sql

    echo "Calculating street coordinates"
    run ExecuteSqlFile file=calculate_street_coordinates.sql

    run RefreshMaterialisedViews
}

check_requirements

case $1 in
    "download")
        download
        ;;
    "prepare")
        prepare
        ;;
    "import")
        import
        ;;
    *)
        download
        prepare
        import
        ;;
esac
