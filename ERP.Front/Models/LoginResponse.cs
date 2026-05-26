namespace ERP.Front.Models
{
    public enum Permissao
    {
        USUARIO = 0,
        ADMIN = 1,
        GERENTE = 2,
        FUNCIONARIO = 3
    }

    public class loginResult
    {
        public string Token { get; set; } = string.Empty;
        public string Refreshtoken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public UserData User { get; set; } = new();
    }

    public class UserData
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioLogin { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Permissao Permissao { get; set; }
    }
}
