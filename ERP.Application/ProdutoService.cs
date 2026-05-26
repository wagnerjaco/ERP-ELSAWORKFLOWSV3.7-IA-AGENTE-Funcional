using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application
{
    public class ProdutosService
    {
        private readonly AppDbContext context;

        public ProdutosService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Produto>> GetAll()
        {
            return await context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .ToListAsync();
        }

        public async Task<Produto?> GetById(Guid id)
        {
            return await context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.ProdutoFornecedores)
                    .ThenInclude(pf => pf.Fornecedor)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Produto> Create(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Descricao))
                throw new ArgumentException("Descrição é obrigatória");
            if (produto.PrecoVenda < 0)
                throw new ArgumentException("Preço de venda não pode ser negativo");
            if (produto.CustoMedio < 0)
                throw new ArgumentException("Custo médio não pode ser negativo");
            if (produto.Id == Guid.Empty)
                produto.Id = Guid.NewGuid();

            produto.CreatedAt = DateTime.UtcNow;
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto?> Update(Guid id, Produto input)
        {
            var produto = await context.Produtos.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (produto == null) 
                return null;

            produto.Sku = input.Sku;
            produto.Ean = input.Ean;
            produto.Tipo = input.Tipo;
            produto.UnidadeMedida = input.UnidadeMedida;
            produto.Descricao = input.Descricao;
            produto.DescricaoNFe = input.DescricaoNFe;
            produto.DescricaoComplementar = input.DescricaoComplementar;
            produto.CategoriaId = input.CategoriaId;
            produto.MarcaId = input.MarcaId;
            produto.Situacao = input.Situacao;
            produto.CustoMedio = input.CustoMedio;
            produto.UltimaCompra = input.UltimaCompra;
            produto.Markup = input.Markup;
            produto.PrecoVenda = input.PrecoVenda;
            produto.PrecoMinimo = input.PrecoMinimo;
            produto.Ncm = input.Ncm;
            produto.CEST = input.CEST;
            produto.Origem = input.Origem;
            produto.Cfop = input.Cfop;
            produto.CstIcms = input.CstIcms;
            produto.AliqIcms = input.AliqIcms;
            produto.MvaSt = input.MvaSt;
            produto.CstPis = input.CstPis;
            produto.AliqPis = input.AliqPis;
            produto.AliqCofins = input.AliqCofins;
            produto.PesoLiquido = input.PesoLiquido;
            produto.PesoBruto = input.PesoBruto;
            produto.Altura = input.Altura;
            produto.Largura = input.Largura;
            produto.Profundidade = input.Profundidade;
            produto.EstoqueMinimo = input.EstoqueMinimo;
            produto.EstoqueAtual = input.EstoqueAtual;
            produto.PontoReposicao = input.PontoReposicao;
            produto.LeadTime = input.LeadTime;
            produto.ControlaLote = input.ControlaLote;
            produto.ControlaValidade = input.ControlaValidade;
            produto.ControlaEstoque = input.ControlaEstoque;
            produto.Localizacao = input.Localizacao;
            produto.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> Delete(Guid id)
        {
            var produto = await context.Produtos.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (produto == null) return false;
            context.Produtos.Remove(produto);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Produto> Encaminhar(Guid id, SituacaoProduto novaSituacao)
        {
            var produto = await context.Produtos.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");
            
            produto.Situacao = novaSituacao;
            produto.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return produto;
        }

        public async Task<List<Produto>> GetEstoqueBaixo()
        {
            return await context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.ControlaEstoque 
                    && p.EstoqueMinimo.HasValue 
                    && p.EstoqueAtual.HasValue 
                    && p.EstoqueAtual < p.EstoqueMinimo)
                .OrderBy(p => p.EstoqueAtual)
                .ToListAsync();
        }

        public async Task<ProdutoParaPedidoDto?> GetParaPedido(Guid id)
        {
            var produto = await context.Produtos
                .Where(p => p.Id == id)
                .Select(p => new ProdutoParaPedidoDto
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    CategoriaId = p.CategoriaId,
                    MarcaId = p.MarcaId,
                    Tipo = p.Tipo,
                    UnidadeMedida = p.UnidadeMedida,
                    CustoMedio = p.CustoMedio
                })
                .FirstOrDefaultAsync();

            return produto;
        }
    }

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
}