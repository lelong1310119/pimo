using System.ComponentModel.DataAnnotations;

namespace PismoWebInput.Core.Infrastructure.Models.Auth;

public class TokenModel
{
    [Required] public string AccessToken { get; set; }

    [Required] public string RefreshToken { get; set; }
}