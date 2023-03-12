CREATE MATERIALIZED VIEW department_avg_value AS
SELECT d.id,
       d.region_id,
       0                                                                       AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
GROUP BY d.id
UNION
SELECT d.id,
       d.region_id,
       ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
GROUP BY d.id, ps.type;

CREATE UNIQUE INDEX pk_department_avg_value ON department_avg_value (id, type);
CREATE INDEX ix_department_avg_value_region_id ON department_avg_value (region_id);

CREATE MATERIALIZED VIEW department_avg_value_by_year AS
SELECT d.id,
       d.region_id,
       0                                                                       AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
GROUP BY d.id, year
UNION
SELECT d.id,
       d.region_id,
       ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
         INNER JOIN department d on t.department_id = d.id
GROUP BY d.id, ps.type, year;

CREATE UNIQUE INDEX pk_department_avg_value_by_year ON department_avg_value_by_year (id, type, year);
CREATE INDEX ix_department_avg_value_by_year_region_id ON department_avg_value_by_year (region_id);
