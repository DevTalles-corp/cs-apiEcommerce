using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CategoriesController : ControllerBase
  {
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
    {
      _categoryRepository = categoryRepository;
      _mapper = mapper;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
      var categories = _categoryRepository.GetCategories();
      var categoriesDto = new List<CategoryDto>();
      foreach (var category in categories)
      {
        categoriesDto.Add(_mapper.Map<CategoryDto>(category));
      }
      return Ok(categoriesDto);
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategory(int id)
    {
      var category = _categoryRepository.GetCategory(id);
      if (category == null)
      {
        return NotFound($"La categoría con el id {id} no existe");
      }
      var categoryDto = _mapper.Map<CategoryDto>(category);
      return Ok(categoryDto);
    }
  }
}
