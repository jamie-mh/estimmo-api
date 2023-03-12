UPDATE street s
SET coordinates = (SELECT ST_Centroid(ST_Union(a.coordinates)) FROM address a WHERE a.street_id = s.id)
WHERE s.coordinates IS NULL
