using System.ComponentModel.DataAnnotations;

namespace ERP.Front.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;

        public Permissao Permissao { get; set; } = Permissao.USUARIO;
    }
}