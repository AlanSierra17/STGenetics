
--Create a script to list animals older than 2 years and female, sorted by name.

SELECT AnimalId, [Name], B.Breed, BirthDate, S.Sex, Price, St.[Status], DATEDIFF(year, BirthDate, GETDATE()) as Age FROM Animal A
left join Breed B on B.BreedId = A.Breed
left join Sex S on S.SexId = A.Sex
left join [Status] St on St.StatusId = A.[Status]
WHERE A.Sex = 2 and DATEDIFF(year, BirthDate, GETDATE()) > 2
ORDER BY [name]

