CREATE MATERIALIZED VIEW france_value_stats AS
SELECT 0                                                                                  AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
UNION
SELECT ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
GROUP BY ps.type;

CREATE UNIQUE INDEX pk_france_value_stats ON france_value_stats (type);

CREATE MATERIALIZED VIEW france_value_stats_by_year AS
SELECT 0                                                                                  AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
GROUP BY year
UNION
SELECT ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
GROUP BY ps.type, year;

CREATE UNIQUE INDEX pk_france_value_stats_by_year ON france_value_stats_by_year (type, year);
