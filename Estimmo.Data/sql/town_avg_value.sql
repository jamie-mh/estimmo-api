CREATE MATERIALIZED VIEW town_avg_value AS
SELECT t.id, t.department_id, 0 AS type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
GROUP BY t.id
UNION
SELECT t.id, t.department_id, ps.type, AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
GROUP BY t.id, ps.type;

CREATE UNIQUE INDEX pk_town_avg_value ON town_avg_value (id, type);
CREATE INDEX ix_town_avg_value ON town_avg_value (department_id);

CREATE MATERIALIZED VIEW town_avg_value_by_year AS
SELECT t.id,
       t.department_id,
       0                                                         AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)              AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
GROUP BY t.id, year
UNION
SELECT t.id,
       t.department_id,
       ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)              AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area) AS value
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
         INNER JOIN town t on s.town_id = t.id
GROUP BY t.id, ps.type, year;

CREATE UNIQUE INDEX pk_town_avg_value_by_year ON town_avg_value_by_year (id, type, year);
CREATE INDEX ix_town_avg_value_by_year ON town_avg_value_by_year (department_id);
