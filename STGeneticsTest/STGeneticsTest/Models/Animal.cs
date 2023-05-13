using System.Reflection.Metadata.Ecma335;

namespace STGeneticsTest.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        public string Name { get; set; }
        public int Breed { get; set; }
        public DateTime BirthDate { get; set; }
        public int Sex { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
    }
}
