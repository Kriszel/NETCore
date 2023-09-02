using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Services;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Repositories;
using System.Text.Json;

namespace SampleWebApiAspNetCore.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<AnimalController> _linkService;

        public AnimalController(
            IAnimalRepository AnimalRepository,
            IMapper mapper,
            ILinkService<AnimalController> linkService)
        {
            _animalRepository = AnimalRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllAnimals))]
        public ActionResult GetAllAnimals(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<AnimalEntity> AnimalItems = _animalRepository.GetAll(queryParameters).ToList();

            var allItemCount = _animalRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = AnimalItems.Select(x => _linkService.ExpandSingleItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleAnimal))]
        public ActionResult GetSingleAnimal(ApiVersion version, int id)
        {
            AnimalEntity AnimalItem = _animalRepository.GetSingle(id);

            if (AnimalItem == null)
            {
                return NotFound();
            }

            AnimalDto item = _mapper.Map<AnimalDto>(AnimalItem);

            return Ok(_linkService.ExpandSingleItem(item, item.Id, version));
        }

        [HttpPost(Name = nameof(AddAnimal))]
        public ActionResult<AnimalDto> AddAnimal(ApiVersion version, [FromBody] AnimalCreateDto animalCreateDto)
        {
            if (animalCreateDto == null)
            {
                return BadRequest();
            }

            AnimalEntity toAdd = _mapper.Map<AnimalEntity>(animalCreateDto);

            _animalRepository.Add(toAdd);

            if (!_animalRepository.Save())
            {
                throw new Exception("Creating a Animalitem failed on save.");
            }

            AnimalEntity newAnimalItem = _animalRepository.GetSingle(toAdd.Id);
            AnimalDto AnimalDto = _mapper.Map<AnimalDto>(newAnimalItem);

            return CreatedAtRoute(nameof(GetSingleAnimal),
                new { version = version.ToString(), id = newAnimalItem.Id },
                _linkService.ExpandSingleItem(AnimalDto, AnimalDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateAnimal))]
        public ActionResult<AnimalDto> PartiallyUpdateAnimal(ApiVersion version, int id, [FromBody] JsonPatchDocument<AnimalUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            AnimalEntity existingEntity = _animalRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            AnimalUpdateDto animalUpdateDto = _mapper.Map<AnimalUpdateDto>(existingEntity);
            patchDoc.ApplyTo(animalUpdateDto);

            TryValidateModel(animalUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(animalUpdateDto, existingEntity);
            AnimalEntity updated = _animalRepository.Update(id, existingEntity);

            if (!_animalRepository.Save())
            {
                throw new Exception("Updating a Animalitem failed on save.");
            }

            AnimalDto animalDto = _mapper.Map<AnimalDto>(updated);

            return Ok(_linkService.ExpandSingleItem(animalDto, animalDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveAnimal))]
        public ActionResult RemoveAnimal(int id)
        {
            AnimalEntity animalItem = _animalRepository.GetSingle(id);

            if (animalItem == null)
            {
                return NotFound();
            }

            _animalRepository.Delete(id);

            if (!_animalRepository.Save())
            {
                throw new Exception("Deleting a Animalitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateAnimal))]
        public ActionResult<AnimalDto> UpdateAnimal(ApiVersion version, int id, [FromBody] AnimalUpdateDto AnimalUpdateDto)
        {
            if (AnimalUpdateDto == null)
            {
                return BadRequest();
            }

            var existingAnimalItem = _animalRepository.GetSingle(id);

            if (existingAnimalItem == null)
            {
                return NotFound();
            }

            _mapper.Map(AnimalUpdateDto, existingAnimalItem);

            _animalRepository.Update(id, existingAnimalItem);

            if (!_animalRepository.Save())
            {
                throw new Exception("Updating a Animalitem failed on save.");
            }

            AnimalDto AnimalDto = _mapper.Map<AnimalDto>(existingAnimalItem);

            return Ok(_linkService.ExpandSingleItem(AnimalDto, AnimalDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            ICollection<AnimalEntity> AnimalItems = _animalRepository.GetRandomMeal();

            IEnumerable<AnimalDto> dtos = AnimalItems.Select(x => _mapper.Map<AnimalDto>(x));

            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
