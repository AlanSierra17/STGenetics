--Insert

INSERT INTO Animal ([Name], Breed, BirthDate, Sex, Price, [Status])
VALUES ('Bella', 1, '2020-03-02', '2', 1750.99, '1');

--Read

SELECT AnimalId, [Name], B.Breed, BirthDate, S.Sex, Price, St.[Status] FROM Animal A
LEFT JOIN Breed B ON B.BreedId = A.Breed
LEFT JOIN Sex S ON S.SexId = A.Sex
LEFT JOIN [Status] St ON St.StatusId = A.[Status]

-- Update

UPDATE Animal
SET Breed = 2, Price = 1500.45
WHERE AnimalId = 2;

-- Delete

DELETE FROM Animal
WHERE AnimalId = 6;

