-- Specify the path and filename of the data file to be imported
DECLARE @FilePath NVARCHAR(1000) = 'C:\Data\animals.csv'

-- Create a temporary table to hold the imported data
CREATE TABLE #TempAnimal (
    AnimalId INT,
    Name VARCHAR(50),
    Breed VARCHAR(50),
    BirthDate DATE,
    Sex CHAR(1),
    Price MONEY,
    Status VARCHAR(50)
)

-- Bulk insert the data from the file into the temporary table
BULK INSERT #TempAnimals
FROM @FilePath
WITH (
    FIRSTROW = 2, -- Skip the header row if it exists
    FIELDTERMINATOR = ',', -- Specify the field delimiter
    ROWTERMINATOR = '\n' -- Specify the row delimiter
)

-- Insert the data from the temporary table into the main table
INSERT INTO Animal (
    Name,
    Breed,
    BirthDate,
    Sex,
    Price,
    Status
)
SELECT
    Name,
    Breed,
    BirthDate,
    Sex,
    Price,
    Status
FROM #TempAnimals

-- Drop the temporary table
DROP TABLE #TempAnimals