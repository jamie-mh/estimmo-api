CREATE MATERIALIZED VIEW france_avg_value AS
SELECT 0                                                                       AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
UNION
SELECT ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
GROUP BY ps.type;

CREATE UNIQUE INDEX pk_france_avg_value ON france_avg_value (type);

CREATE MATERIALIZED VIEW france_avg_value_by_year AS
SELECT 0                                                                       AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
GROUP BY year
UNION
SELECT ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
GROUP BY ps.type, year;

CREATE UNIQUE INDEX pk_france_avg_value_by_year ON france_avg_value_by_year (type, year);
