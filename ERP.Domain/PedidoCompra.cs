namespace ERP.Domain
{
    public class PedidoCompra
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NumeroPedido { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public TipoProduto Tipo { get; set; }
        public UnidadeMedida Unidade { get; set; }
        public decimal Quantidade { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataPedido { get; set; }

        public Guid FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public Guid? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public Guid? MarcaId { get; set; }
        public Marca? Marca { get; set; }

        public Guid? ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        public bool Aprovado { get; set; } = false;
        public string? EmailAprovacao { get; set; }
        public DateTime? DataAprovacao { get; set; }

        public bool Ativa { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}