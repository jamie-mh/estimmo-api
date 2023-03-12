CREATE MATERIALIZED VIEW place AS
SELECT type,
       id,
       name,
       short_name,
       UNACCENT(REPLACE(REGEXP_REPLACE(LOWER(name), '[(),]+', '', 'g'), '-', ' ')) AS search_name,
       post_code,
       parent_type,
       parent_id,
       is_searchable,
       geometry
FROM (SELECT 1    AS type,
             id,
             name,
             name AS short_name,
             NULL AS post_code,
             NULL AS parent_type,
             NULL AS parent_id,
             true AS is_searchable,
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
             true   AS is_searchable,
             d.geometry
      FROM department d
               INNER JOIN region r ON d.region_id = r.id
      UNION
      SELECT 3                                      AS type,
             t.id,
             CONCAT(t.name, ' (', t.post_code, ')') AS name,
             t.name                                 AS short_name,
             t.post_code,
             2                                      AS parent_type,
             d.id                                   AS parent_id,
             true                                   AS is_searchable,
             t.geometry
      FROM town t
               INNER JOIN department d ON t.department_id = d.id
      UNION
      SELECT 4         AS type,
             s.id,
             s.id      AS name,
             s.id      AS short_name,
             t.post_code,
             3         AS parent_type,
             s.town_id AS parent_id,
             false     AS is_searchable,
             s.geometry
      FROM section s
               INNER JOIN town t ON s.town_id = t.id
      UNION
      SELECT 5                                                     AS type,
             st.id,
             CONCAT(st.name, ', ', t.name, ' (', t.post_code, ')') AS name,
             st.name                                               AS short_name,
             t.post_code,
             3                                                     AS parent_type,
             t.id                                                  AS parent_id,
             true                                                  AS is_searchable,
             st.coordinates                                        AS geometry
      FROM street st
               INNER JOIN town t ON st.town_id = t.id
      UNION
      SELECT 6                                                                                      AS type,
             sp.id,
             CONCAT(sp.name, ', ', t.name, ' (', sp.post_code, ')')                                 AS name,
             sp.name                                                                                AS short_name,
             sp.post_code,
             4                                                                                      AS parent_type,
             (SELECT sc.id FROM section sc WHERE ST_COVEREDBY(sp.coordinates, sc.geometry) LIMIT 1) AS parent_id,
             true                                                                                   AS is_searchable,
             sp.coordinates                                                                         AS geometry
      FROM said_place sp
               INNER JOIN town t ON sp.town_id = t.id
      UNION
      SELECT 7                                                                                     AS type,
             a.id,
             CONCAT(a.number, a.suffix, ' ', s.name, ', ', t.name, ' (', a.post_code, ')')         AS name,
             CONCAT(a.number, a.suffix, ' ', s.name)                                               AS short_name,
             a.post_code,
             4                                                                                     AS parent_type,
             (SELECT sc.id FROM section sc WHERE ST_COVEREDBY(a.coordinates, sc.geometry) LIMIT 1) AS parent_id,
             true                                                                                  AS is_searchable,
             a.coordinates                                                                         AS geometry
      FROM address a
               INNER JOIN street s ON a.street_id = s.id
               INNER JOIN town t ON s.town_id = t.id) s;

CREATE UNIQUE INDEX pk_place ON place (type, id);
CREATE INDEX ix_place_type ON place (type);
CREATE INDEX ix_place_search_name ON place (search_name text_pattern_ops) WHERE is_searchable;
CREATE INDEX ix_place_post_code ON place (post_code) WHERE type = 3;
CREATE INDEX ix_place_is_searchable ON place (is_searchable);
CREATE INDEX ix_place_geometry ON place USING GIST (geometry);
