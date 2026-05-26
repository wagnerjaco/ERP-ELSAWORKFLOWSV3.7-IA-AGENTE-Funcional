namespace ERP.Domain
{
    public enum Permissao
    {
        ADMIN,
        USUARIO,
        LEITURA
    }

    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UsuarioLogin { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public bool Ativo { get; set; } = true;
        public Permissao Permissao { get; set; } = Permissao.USUARIO;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}