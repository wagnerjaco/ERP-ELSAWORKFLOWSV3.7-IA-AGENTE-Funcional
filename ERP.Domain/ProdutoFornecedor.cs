namespace ERP.Domain
{
    public class ProdutoFornecedor
    {
        public Guid ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public Guid FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;

        public string? CodigoFornecedor { get; set; }
        public int? PrazoEntrega { get; set; }
        public decimal? PrecoFornecedor { get; set; }
        public bool IsPadrao { get; set; }
    }
}