using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models;

public class User
{
  [Key]
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public string? Role { get; set; }
}
