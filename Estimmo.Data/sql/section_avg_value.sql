CREATE MATERIALIZED VIEW section_avg_value AS
SELECT s.id,
       s.town_id,
       0                                                                       AS type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id
UNION
SELECT s.id,
       s.town_id,
       ps.type,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, ps.type;

CREATE UNIQUE INDEX pk_section_avg_value ON section_avg_value (id, type);
CREATE INDEX ix_section_avg_value_town_id ON section_avg_value (town_id);

CREATE MATERIALIZED VIEW section_avg_value_by_year AS
SELECT s.id,
       s.town_id,
       0                                                                       AS type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, year
UNION
SELECT s.id,
       s.town_id,
       ps.type,
       CAST(EXTRACT(YEAR FROM ps.date) AS smallint)                            AS year,
       AVG(CAST(ps.value AS decimal) / ps.building_surface_area)               AS value,
       ROUND(STDDEV(CAST(ps.value AS decimal) / ps.building_surface_area), 10) AS standard_deviation
FROM property_sale ps
         INNER JOIN section s on ps.section_id = s.id
GROUP BY s.id, ps.type, year;

CREATE UNIQUE INDEX pk_section_avg_value_by_year ON section_avg_value_by_year (id, type, year);
CREATE INDEX ix_section_avg_value_by_year_town_id ON section_avg_value_by_year (town_id);
