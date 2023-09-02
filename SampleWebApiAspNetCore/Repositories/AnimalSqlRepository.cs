using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using System.Linq.Dynamic.Core;

namespace SampleWebApiAspNetCore.Repositories
{
    public class AnimalSqlRepository : IAnimalRepository
    {
        private readonly AnimalDbContext _animalDbContext;

        public AnimalSqlRepository(AnimalDbContext AnimalDbContext)
        {
            _animalDbContext = AnimalDbContext;
        }

        public AnimalEntity GetSingle(int id)
        {
            return _animalDbContext.AnimalItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(AnimalEntity item)
        {
            _animalDbContext.AnimalItems.Add(item);
        }

        public void Delete(int id)
        {
            AnimalEntity foodItem = GetSingle(id);
            _animalDbContext.AnimalItems.Remove(foodItem);
        }

        public AnimalEntity Update(int id, AnimalEntity item)
        {
            _animalDbContext.AnimalItems.Update(item);
            return item;
        }

        public IQueryable<AnimalEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<AnimalEntity> _allItems = _animalDbContext.AnimalItems.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _animalDbContext.AnimalItems.Count();
        }

        public bool Save()
        {
            return (_animalDbContext.SaveChanges() >= 0);
        }

        public ICollection<AnimalEntity> GetRandomMeal()
        {
            List<AnimalEntity> toReturn = new List<AnimalEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }

        private AnimalEntity GetRandomItem(string type)
        {
            return _animalDbContext.AnimalItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
