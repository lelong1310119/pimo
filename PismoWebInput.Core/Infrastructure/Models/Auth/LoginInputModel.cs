using System.ComponentModel.DataAnnotations;

namespace PismoWebInput.Core.Infrastructure.Models.Auth;

public class LoginInputModel
{
    [Required]
    public string UserName { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}