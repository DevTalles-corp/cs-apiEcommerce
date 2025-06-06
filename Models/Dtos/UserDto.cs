using System;

namespace ApiEcommerce.Models.Dtos;

public class UserDto
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public string? Role { get; set; }
}
