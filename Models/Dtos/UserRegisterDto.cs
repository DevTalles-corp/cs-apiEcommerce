using System;

namespace ApiEcommerce.Models.Dtos;

public class UserRegisterDto
{
  public string? ID { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
  public string? Name { get; set; }
  public string? Role { get; set; }
}
