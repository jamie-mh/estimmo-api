DELETE
FROM property_sale
WHERE hash IN (SELECT ps.hash
               FROM property_sale ps
                        INNER JOIN section s on ps.section_id = s.id
                        INNER JOIN town_value_stats tms ON s.town_id = tms.id AND tms.type = ps.type
               WHERE CAST(ps.value AS decimal) / ps.building_surface_area > tms.average + 4 * tms.standard_deviation
                  OR CAST(ps.value AS decimal) / ps.building_surface_area > 25000);