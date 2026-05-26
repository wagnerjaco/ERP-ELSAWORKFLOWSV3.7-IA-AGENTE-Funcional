namespace ERP.Domain
{
    public enum TipoProduto
    {
        MERCADORIA,
        MATERIA_PRIMA,
        PRODUTO_ACABADO,
        EMBALAGEM,
        USO_CONSUMO
    }

    public enum UnidadeMedida
    {
        UN, KG, PC, CX, MT
    }

    public enum SituacaoProduto
    {
        RASCUNHO,
        AGUARDANDO_FISCAL,
        AGUARDANDO_PCP,
        PENDENTE_APROVACAO,
        ATIVO,
        INATIVO
    }

    public class Produto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Sku { get; set; }
        public string? Ean { get; set; }
        public TipoProduto Tipo { get; set; }
        public UnidadeMedida UnidadeMedida { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? DescricaoNFe { get; set; }
        public string? DescricaoComplementar { get; set; }

        public Guid? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public Guid? MarcaId { get; set; }
        public Marca? Marca { get; set; }

        public SituacaoProduto Situacao { get; set; } = SituacaoProduto.RASCUNHO;

        public decimal CustoMedio { get; set; }
        public decimal? UltimaCompra { get; set; }
        public decimal? Markup { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal? PrecoMinimo { get; set; }

        public string? Ncm { get; set; }
        public string? CEST { get; set; }
        public int Origem { get; set; }
        public string? Cfop { get; set; }
        public string? CstIcms { get; set; }
        public decimal? AliqIcms { get; set; }
        public decimal? MvaSt { get; set; }
        public string? CstPis { get; set; }
        public decimal? AliqPis { get; set; }
        public decimal? AliqCofins { get; set; }

        public decimal? PesoLiquido { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Largura { get; set; }
        public decimal? Profundidade { get; set; }

        public decimal? EstoqueMinimo { get; set; }
        public decimal? EstoqueAtual { get; set; }
        public decimal? PontoReposicao { get; set; }
        public int? LeadTime { get; set; }
        public bool ControlaLote { get; set; }
        public bool ControlaValidade { get; set; }
        public bool ControlaEstoque { get; set; } = true;
        public string? Localizacao { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProdutoFornecedor> ProdutoFornecedores { get; set; } = new List<ProdutoFornecedor>();
    }
}