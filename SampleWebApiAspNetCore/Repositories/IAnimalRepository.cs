using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;

namespace SampleWebApiAspNetCore.Repositories
{
    public interface IAnimalRepository
    {
        AnimalEntity GetSingle(int id);
        void Add(AnimalEntity item);
        void Delete(int id);
        AnimalEntity Update(int id, AnimalEntity item);
        IQueryable<AnimalEntity> GetAll(QueryParameters queryParameters);
        ICollection<AnimalEntity> GetRandomMeal();
        int Count();
        bool Save();
    }
}
