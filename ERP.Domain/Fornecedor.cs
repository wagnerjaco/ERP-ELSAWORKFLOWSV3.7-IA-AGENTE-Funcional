namespace ERP.Domain
{
    public enum TipoPessoa
    {
        FISICA,
        JURIDICA
    }

    public enum SituacaoFornecedor
    {
        ATIVO,
        INATIVO,
        BLOQUEADO
    }

    public class Fornecedor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string? CpfCnpj { get; set; }
        public string? RgIe { get; set; }
        public TipoPessoa TipoPessoa { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Cep { get; set; }
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Observacoes { get; set; }
        public SituacaoFornecedor Situacao { get; set; } = SituacaoFornecedor.ATIVO;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}