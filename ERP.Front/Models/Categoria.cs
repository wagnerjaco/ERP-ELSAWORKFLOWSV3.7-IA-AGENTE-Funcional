namespace ERP.Front.Models;

public enum TipoCategoria
{
    PRODUTO,
    SERVICO,
    MATERIA_PRIMA
}

public class Categoria
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoCategoria Tipo { get; set; } = TipoCategoria.PRODUTO;
    public bool Ativa { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class CategoriaForm
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoCategoria Tipo { get; set; } = TipoCategoria.PRODUTO;
    public bool Ativa { get; set; } = true;
}