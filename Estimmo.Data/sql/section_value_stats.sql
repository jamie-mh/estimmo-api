CREATE MATERIALIZED VIEW section_value_stats AS
SELECT s.id,
       s.town_id,
       0                                                                                  AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id
UNION
SELECT s.id,
       s.town_id,
       ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, ps.type;

CREATE UNIQUE INDEX pk_section_value_stats ON section_value_stats (id, type);
CREATE INDEX ix_section_value_stats_town_id ON section_value_stats (town_id);

CREATE MATERIALIZED VIEW section_value_stats_by_year AS
SELECT s.id,
       s.town_id,
       0                                                                                  AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, year
UNION
SELECT s.id,
       s.town_id,
       ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, ps.type, year;

CREATE UNIQUE INDEX pk_section_value_stats_by_year ON section_value_stats_by_year (id, type, year);
CREATE INDEX ix_section_value_stats_by_year_town_id ON section_value_stats_by_year (town_id);
