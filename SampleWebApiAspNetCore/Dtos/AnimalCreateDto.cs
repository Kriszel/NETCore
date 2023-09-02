using System.ComponentModel.DataAnnotations;

namespace SampleWebApiAspNetCore.Dtos
{
    public class AnimalCreateDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
    }
}
