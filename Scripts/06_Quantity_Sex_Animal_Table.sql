
-- Create a script to list the quantity of animals by sex.

SELECT S.Sex, COUNT(*) AS Total
FROM Animal A
INNER JOIN Sex S ON S.SexId = A.Sex
GROUP BY S.Sex;