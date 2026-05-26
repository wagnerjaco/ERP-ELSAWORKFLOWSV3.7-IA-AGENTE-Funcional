using System.ComponentModel.DataAnnotations;

namespace ERP.Front.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Usuario é Obrigatório")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é Obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
