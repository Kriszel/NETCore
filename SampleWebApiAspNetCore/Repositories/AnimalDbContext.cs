using Microsoft.EntityFrameworkCore;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.Repositories
{
    public class AnimalDbContext : DbContext
    {
        public AnimalDbContext(DbContextOptions<AnimalDbContext> options)
            : base(options)
        {
        }

        public DbSet<AnimalEntity> AnimalItems { get; set; } = null!;
    }
}
