CREATE MATERIALIZED VIEW region_value_stats AS
SELECT r.id,
       0                                                                                  AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
         INNER JOIN region r on d.region_id = r.id
GROUP BY r.id
UNION
SELECT r.id,
       ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
         INNER JOIN region r on d.region_id = r.id
GROUP BY r.id, ps.type;

CREATE UNIQUE INDEX pk_region_value_stats ON region_value_stats (id, type);

CREATE MATERIALIZED VIEW region_value_stats_by_year AS
SELECT r.id,
       0                                                                                  AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
         INNER JOIN region r on d.region_id = r.id
GROUP BY r.id, year
UNION
SELECT r.id,
       ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                                       AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)                          AS average,
       tdigest_percentile(CAST(ps.value AS decimal) / ps.building_surface_area, 100, 0.5) AS median,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10)            AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
         INNER JOIN region r on d.region_id = r.id
GROUP BY r.id, ps.type, year;

CREATE UNIQUE INDEX pk_region_value_stats_by_year ON region_value_stats_by_year (id, type, year);
