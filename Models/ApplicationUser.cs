using System;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Models;

public class ApplicationUser : IdentityUser
{
  public string? Name { get; set; }
}
