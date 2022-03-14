CREATE MATERIALIZED VIEW place AS
SELECT type,
       id,
       name,
       short_name,
       UNACCENT(REPLACE(REGEXP_REPLACE(LOWER(name), '[(),]+', '', 'g'), '-', ' ')) AS search_name,
       post_code,
       parent_type,
       parent_id,
       geometry
FROM (
         SELECT 1    AS type,
                id,
                name,
                name AS short_name,
                NULL AS post_code,
                NULL AS parent_type,
                NULL AS parent_id,
                geometry
         FROM region
         UNION
         SELECT 2      AS type,
                d.id,
                d.name,
                d.name AS short_name,
                NULL   AS post_code,
                1      AS parent_type,
                r.id   AS parent_id,
                d.geometry
         FROM department d
                  INNER JOIN region r on d.region_id = r.id
         UNION
         SELECT 3                                      AS type,
                t.id,
                CONCAT(t.name, ' (', t.post_code, ')') AS name,
                t.name                                 AS short_name,
                t.post_code,
                2                                      AS parent_type,
                d.id                                   AS parent_id,
                t.geometry
         FROM town t
                  INNER JOIN department d on t.department_id = d.id
         UNION
         SELECT 4                                                                             AS type,
                a.id,
                CONCAT(a.number, a.suffix, ' ', s.name, ', ', t.name, ' (', a.post_code, ')') AS name,
                CONCAT(a.number, a.suffix, ' ', s.name)                                       AS short_name,
                a.post_code,
                3                                                                             AS parent_type,
                s.town_id                                                                     AS parent_id,
                a.coordinates                                                                 AS geometry
         FROM address a
                  INNER JOIN street s on a.street_id = s.id
                  INNER JOIN town t on s.town_id = t.id
     ) s;

CREATE UNIQUE INDEX pk_place ON place (type, id);
CREATE INDEX ix_place_type ON place (type);
CREATE INDEX ix_place_search_name ON place (search_name text_pattern_ops);
CREATE INDEX ix_place_geometry ON place USING GIST (geometry);
