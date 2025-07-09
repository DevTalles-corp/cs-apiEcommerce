using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public static class MapsterConfig
{
  public static void RegisterMappings()
  {
    // User <-> UserDto
    TypeAdapterConfig<User, UserDto>.NewConfig().TwoWays();
    TypeAdapterConfig<User, CreateUserDto>.NewConfig().TwoWays();
    TypeAdapterConfig<User, UserLoginDto>.NewConfig().TwoWays();
    TypeAdapterConfig<User, UserLoginResponseDto>.NewConfig().TwoWays();
    TypeAdapterConfig<ApplicationUser, UserDataDto>.NewConfig().TwoWays();
    TypeAdapterConfig<ApplicationUser, UserDto>.NewConfig().TwoWays();

    // Product <-> ProductDto
    TypeAdapterConfig<Product, ProductDto>.NewConfig()
        .Map(dest => dest.CategoryName, src => src.Category.Name)
        .TwoWays();
    TypeAdapterConfig<Product, CreateProductDto>.NewConfig().TwoWays();
    TypeAdapterConfig<Product, UpdateProductDto>.NewConfig().TwoWays();

    // Category <-> CategoryDto
    TypeAdapterConfig<Category, CategoryDto>.NewConfig().TwoWays();
    TypeAdapterConfig<Category, CreateCategoryDto>.NewConfig().TwoWays();
  }
}
