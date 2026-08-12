using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class LoginModelDTO
{
    [Required(ErrorMessage = "Nome de usuario é obrigatório")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    public string? Password { get; set; }
}
