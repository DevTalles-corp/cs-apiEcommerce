using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
  [Authorize(Roles = "Admin")]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiController]
  [ApiVersionNeutral]
  public class ProductsController : ControllerBase
  {
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
      _productRepository = productRepository;
      _categoryRepository = categoryRepository;
      _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
      var products = _productRepository.GetProducts();
      var productsDto = _mapper.Map<List<ProductDto>>(products);
      return Ok(productsDto);
    }

    [AllowAnonymous]
    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct(int productId)
    {
      var product = _productRepository.GetProduct(productId);
      if (product == null)
      {
        return NotFound($"El producto con el id {productId} no existe");
      }
      var productDto = _mapper.Map<ProductDto>(product);
      return Ok(productDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
    {
      if (createProductDto == null)
      {
        return BadRequest(ModelState);
      }
      if (_productRepository.ProductExists(createProductDto.Name))
      {
        ModelState.AddModelError("CustomError", "El producto ya existe");
        return BadRequest(ModelState);
      }
      if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
      {
        ModelState.AddModelError("CustomError", $"La categoría con el {createProductDto.CategoryId} no existe");
        return BadRequest(ModelState);
      }
      var product = _mapper.Map<Product>(createProductDto);
      // Agregando imagen
      if (createProductDto.Image != null)
      {
        string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(createProductDto.Image.FileName);
        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
        if (!Directory.Exists(imagesFolder))
        {
          Directory.CreateDirectory(imagesFolder);
        }
        var filePath = Path.Combine(imagesFolder, fileName);
        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
          file.Delete();
        }
        using var fileStream = new FileStream(filePath, FileMode.Create);
        createProductDto.Image.CopyTo(fileStream);
        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
        product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
        product.ImgUrlLocal = filePath;
      }
      else
      {
        product.ImgUrl = "https://placehold.co/300x300";
      }
      if (!_productRepository.CreateProduct(product))
      {
        ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {product.Name}");
        return StatusCode(500, ModelState);
      }
      var createdProduct = _productRepository.GetProduct(product.ProductId);
      var productoDto = _mapper.Map<ProductDto>(createdProduct);
      return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productoDto);
    }

    [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsForCategory(int categoryId)
    {
      var products = _productRepository.GetProductsForCategory(categoryId);
      if (products.Count == 0)
      {
        return NotFound($"Los productos con la categoría {categoryId} no existen");
      }
      var productsDto = _mapper.Map<List<ProductDto>>(products);
      return Ok(productsDto);
    }

    [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SearchProducts(string searchTerm)
    {
      var products = _productRepository.SearchProducts(searchTerm);
      if (products.Count == 0)
      {
        return NotFound($"Los productos con el nombre o descripción '{searchTerm}' no existen");
      }
      var productsDto = _mapper.Map<List<ProductDto>>(products);
      return Ok(productsDto);
    }

    [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult BuyProduct(string name, int quantity)
    {
      if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
      {
        return BadRequest("El nombre del producto o la cantidad no son válidos");
      }
      var foundProduct = _productRepository.ProductExists(name);
      if (!foundProduct)
      {
        return NotFound($"El producto con el nombre {name} no existe");
      }
      if (!_productRepository.BuyProduct(name, quantity))
      {
        ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {name} o la cantidad solicitada es mayor al stock disponible");
        return BadRequest(ModelState);
      }
      var units = quantity == 1 ? "unidad" : "unidades";
      return Ok($"Se compro {quantity} {units} del producto '{name}'");
    }

    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
    {
      if (updateProductDto == null)
      {
        return BadRequest(ModelState);
      }
      if (!_productRepository.ProductExists(productId))
      {
        ModelState.AddModelError("CustomError", "El producto no existe");
        return BadRequest(ModelState);
      }
      if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
      {
        ModelState.AddModelError("CustomError", $"La categoría con el {updateProductDto.CategoryId} no existe");
        return BadRequest(ModelState);
      }
      var product = _mapper.Map<Product>(updateProductDto);
      product.ProductId = productId;
      // Agregando imagen
      if (updateProductDto.Image != null)
      {
        string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(updateProductDto.Image.FileName);
        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
        if (!Directory.Exists(imagesFolder))
        {
          Directory.CreateDirectory(imagesFolder);
        }
        var filePath = Path.Combine(imagesFolder, fileName);
        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
          file.Delete();
        }
        using var fileStream = new FileStream(filePath, FileMode.Create);
        updateProductDto.Image.CopyTo(fileStream);
        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
        product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
        product.ImgUrlLocal = filePath;
      }
      else
      {
        product.ImgUrl = "https://placehold.co/300x300";
      }
      if (!_productRepository.UpdateProduct(product))
      {
        ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {product.Name}");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }

    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProduct(int productId)
    {
      if (productId == 0)
      {
        return BadRequest(ModelState);
      }

      var product = _productRepository.GetProduct(productId);
      if (product == null)
      {
        return NotFound($"El producto con el id {productId} no existe");
      }
      if (!_productRepository.DeleteProduct(product))
      {
        ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {product.Name}");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }
  }
}
