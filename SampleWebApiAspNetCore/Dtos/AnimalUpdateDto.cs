
namespace SampleWebApiAspNetCore.Dtos
{
    public class AnimalUpdateDto
    {
        public string? Name { get; set; }
        public int Calories { get; set; }
        public string? Type { get; set; }
        public DateTime Created { get; set; }
    }
}
