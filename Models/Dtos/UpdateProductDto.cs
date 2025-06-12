namespace ApiEcommerce.Models.Dtos;

public class UpdateProductDto
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public string ImgUrl { get; set; } = string.Empty;
  public string SKU { get; set; } = string.Empty;
  public int Stock { get; set; }
  public DateTime? UpdateDate { get; set; } = null;
  public int CategoryId { get; set; }
}
