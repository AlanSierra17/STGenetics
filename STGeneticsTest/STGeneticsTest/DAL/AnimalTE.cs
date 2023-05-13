using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STGeneticsTest.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace STGeneticsTest.DAL
{
    public class AnimalTE
    {
        private readonly IConfiguration _config;

        public AnimalTE(IConfiguration config)
        {
            _config = config;
        }
        public async Task<IEnumerable<AnimalDTO>> GetAllAnimals()
        {

            DefaultResponse<IEnumerable<AnimalDTO>> response = new DefaultResponse<IEnumerable<AnimalDTO>>();

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                IEnumerable<AnimalDTO> animals = await SelectAllAnimals(connection);

            return animals;

        }

        public async Task<AnimalDTO> GetAnimal(int animalId)
        {

                using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var data = await connection.QueryFirstOrDefaultAsync<AnimalDTO>("SELECT AnimalId, [Name], B.Breed, BirthDate, S.Sex, Price, St.[Status] FROM Animal A " +
                                                                     "LEFT JOIN Breed B ON B.BreedId = A.Breed " +
                                                                     "LEFT JOIN Sex S ON S.SexId = A.Sex " +
                                                                     "LEFT JOIN [Status] St ON St.StatusId = A.[Status] " +
                                                                     "WHERE A.AnimalId = @Id",
                                                                      new { Id = animalId });
                if (data == null)
                    return new AnimalDTO();
                else
                    return data;
        }

        public async Task<string> CreateAnimal(Animal animal)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            int affectedrRows = await connection.ExecuteAsync("INSERT INTO Animal " +
                                            "([Name], Breed, BirthDate, Sex, Price, [Status]) " +
                                            "VALUES (@Name, @Breed, @BirthDate, @Sex, @Price, @Status)", animal);

            if (affectedrRows > 0)
            {
                return "Animal successfully created.";
            }
            else
            {
                throw new Exception("It was not possible to create the animal");
            }
        }


        public async Task<AnimalDTO> UpdateAnimal(Animal animal)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            AnimalDTO animalDTO = await GetAnimal(animal.AnimalId);
            if (animalDTO == null)
            {
                return new AnimalDTO();
            }
            else
            {
                await connection.ExecuteAsync("UPDATE Animal " +
                          "SET [Name] = @Name, Breed = @Breed, BirthDate = @BirthDate, Sex = @Sex, Price = @Price, [Status] = @Status " +
                          "WHERE AnimalId = @AnimalId", animal);
                return await GetAnimal(animal.AnimalId);
            }
        }

        public async Task<string> DeleteAnimal(int animalId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            AnimalDTO animalDTO = await GetAnimal(animalId);

            if (animalDTO.AnimalId == 0)
            {
                return string.Empty;
            }
            else
            {
                await connection.ExecuteAsync("DELETE FROM Animal " +
                                              "WHERE AnimalId = @AnimalId", new { AnimalId = animalId });

                return $"Animal with ID {animalId} deleted.";
            }


        }

        public async Task<IEnumerable<AnimalDTO>> GetAnimalsFiltered(AnimalFilteredRequest request)
        {
            string nameTrimmed = request.Name.Trim();

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<AnimalDTO> animals = await connection.QueryAsync<AnimalDTO>("SELECT AnimalId, [Name], B.Breed, BirthDate, S.Sex, Price, St.[Status] " +
                "FROM Animal A LEFT JOIN Breed B ON B.BreedId = A.Breed " +
                "LEFT JOIN Sex S ON S.SexId = A.Sex " +
                "LEFT JOIN[Status] St ON St.StatusId = A.[Status] " +
                "WHERE (@Sex = 0 OR A.Sex = @Sex) " +
                "AND (@Status = 0 OR A.[Status] = @Status) " +
                "AND (@Name = '' OR [Name] LIKE '%'+@Name+'%' )",
                new
                {
                    Sex = request.Sex,
                    Status = request.Status,
                    Name = nameTrimmed,
                });

            return animals;
        }


        public async Task<Purchase> Purchase(PurchaseRequest request)
        {
            List<int> AnimalIdList = new List<int>();
            decimal totalAmount = 0;
            decimal AmountWithDiscount = 0;
            int totalAnimalsCount = 0;
            decimal discount = 0;
            string strDiscount = "0%";
            decimal Freight = (decimal)1000.00;

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            //Select animals by range

            IEnumerable<Animal> animals = await connection.QueryAsync<Animal>("SELECT * FROM Animal " +
                "WHERE AnimalId BETWEEN @startAnimalId AND @endAnimalId " +
                "AND [Status] = 1",
                new
                {
                    startAnimalId = request.startAnimalId,
                    endAnimalId = request.endAnimalId
                });


            foreach (Animal animal in animals)
            {
                totalAmount += animal.Price;
                AnimalIdList.Add(animal.AnimalId);
            }

            totalAnimalsCount = animals.Count();

            if(totalAnimalsCount == 0)
            {
                //No animals available to purchase
                return new Purchase();
            }
            else
            {
                //Business Rules
                if (totalAnimalsCount > 0 && totalAnimalsCount <= 200)
                {
                    discount = (decimal)0.05;
                }
                else if (totalAnimalsCount > 200)
                {
                    discount = (decimal)0.08;

                    if(totalAnimalsCount> 300)
                        Freight = 0;
                }

                if (discount != 0)
                {
                    decimal discountProcessed = totalAmount * discount;

                    AmountWithDiscount = (totalAmount - discountProcessed);

                    strDiscount = (discount * 100).ToString() + "%";
                }


                //Create purchase
                await connection.ExecuteAsync("INSERT INTO Purchase (CreatedDate, PurchaseAmount, AmountWithDiscount, Freight, Discount) " +
                                              "VALUES (@CreatedDate, @PurchaseAmount, @AmountWithDiscount, @Freight, @Discount)",
                                              new
                                              {
                                                  CreatedDate = DateTime.Now,
                                                  PurchaseAmount = totalAmount,
                                                  AmountWithDiscount = AmountWithDiscount,
                                                  Freight = Freight,
                                                  Discount = strDiscount
                                              });
                //Select Purchase created
                var PurchaseList = await connection.QueryAsync<Purchase>("SELECT TOP 1 * FROM Purchase ORDER BY PurchaseId DESC");
                var lastPurchase = PurchaseList.FirstOrDefault();


                foreach (int animalId in AnimalIdList)
                {
                    await connection.ExecuteAsync("INSERT INTO  AnimalPurchase (AnimalId, PurchaseId) " +
                    "VALUES(@AnimalId, @PurchaseId)",
                    new
                    {
                        AnimalId = animalId,
                        PurchaseId = lastPurchase?.PurchaseId
                    });

                    await connection.ExecuteAsync("UPDATE Animal " +
                              "SET [Status] = 2 " +
                              "WHERE AnimalId = @AnimalId", new { AnimalId = animalId });
                }

                return lastPurchase;
            } 
        }

        private static async Task<IEnumerable<AnimalDTO>> SelectAllAnimals(SqlConnection connection)
        {
            return await connection.QueryAsync<AnimalDTO>("SELECT AnimalId, [Name], B.Breed, BirthDate, S.Sex, Price, St.[Status] FROM Animal A " +
                                                          "LEFT JOIN Breed B ON B.BreedId = A.Breed " +
                                                          "LEFT JOIN Sex S ON S.SexId = A.Sex " +
                                                          "LEFT JOIN [Status] St ON St.StatusId = A.[Status]");
        }
    }
}
