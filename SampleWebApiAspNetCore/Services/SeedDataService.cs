using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(AnimalDbContext animalContext)
        {

            animalContext.AnimalItems.Add(new AnimalEntity() { Calories = 1000, Type = "Mammal", Name = "Cat", Created = DateTime.Now });
            animalContext.AnimalItems.Add(new AnimalEntity() { Calories = 999, Type = "Fish", Name = "Clownfish", Created = DateTime.Now });
            animalContext.AnimalItems.Add(new AnimalEntity() { Calories = 998, Type = "Amphibian", Name = "Platypus", Created = DateTime.Now });
            animalContext.AnimalItems.Add(new AnimalEntity() { Calories = 997, Type = "Reptile", Name = "Chameleon", Created = DateTime.Now });

            animalContext.SaveChanges();


        }
    }
}
