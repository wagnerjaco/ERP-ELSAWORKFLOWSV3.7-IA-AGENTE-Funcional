namespace ERP.Front.Models;

public class ProdutoParaPedidoDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid? CategoriaId { get; set; }
    public Guid? MarcaId { get; set; }
    public TipoProduto Tipo { get; set; }
    public UnidadeMedida UnidadeMedida { get; set; }
    public decimal CustoMedio { get; set; }
}