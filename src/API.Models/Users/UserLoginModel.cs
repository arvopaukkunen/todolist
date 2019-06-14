﻿using System.ComponentModel.DataAnnotations;

namespace API.Models
{
  public class UserLoginModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [RegularExpression("[0-9]{6}")]
    public string TwoFactorToken { get; set; }
  }
}