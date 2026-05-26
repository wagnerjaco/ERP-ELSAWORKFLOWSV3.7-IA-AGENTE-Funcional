using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ERP.Application
{
    public class PedidosComprasService
    {
        private readonly AppDbContext context;

        public PedidosComprasService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<PedidoCompra>> GetAll()
        {
            return await context.PedidoCompras
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Produto)
                .Where(p => p.Ativa)
                .ToListAsync();
        }

        public async Task<PedidoCompra?> GetById(Guid id)
        {
            return await context.PedidoCompras
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Produto)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<PedidoCompra> Create(PedidoCompra pedido)
        {
            if (string.IsNullOrWhiteSpace(pedido.NumeroPedido))
                throw new ArgumentException("Número do pedido é obrigatório");
            if (string.IsNullOrWhiteSpace(pedido.Descricao))
                throw new ArgumentException("Descrição é obrigatória");
            if (pedido.FornecedorId == Guid.Empty)
                throw new ArgumentException("Fornecedor é obrigatório");
            if (string.IsNullOrWhiteSpace(pedido.EmailAprovacao))
                throw new ArgumentException("Email para aprovação é obrigatório");

            var existeNumero = await context.PedidoCompras.AnyAsync(p => p.NumeroPedido == pedido.NumeroPedido && p.Ativa);
            if (existeNumero)
                throw new ArgumentException("Número do pedido já existe");

            if (pedido.Id == Guid.Empty)
                pedido.Id = Guid.NewGuid();

            pedido.Aprovado = false;
            pedido.ValorTotal = pedido.Quantidade * pedido.CustoMedio;
            pedido.CreatedAt = DateTime.UtcNow;
            pedido.Ativa = true;

            context.PedidoCompras.Add(pedido);
            try
            {
                await context.SaveChangesAsync();
                using var httpClient = new HttpClient();
                var body = new {
                    email = pedido.EmailAprovacao,
                    id = pedido.NumeroPedido,
                    descricao = pedido.Descricao,
                    quantidade = pedido.Quantidade,
                    valorTotal = pedido.ValorTotal,
                    custoMedio = pedido.CustoMedio,
                    dataPedido = pedido.DataPedido
                };
                var json = JsonSerializer.Serialize(body);
                var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );
                var response = await httpClient.PostAsync(
                        "http://192.168.5.57:5011/workflows/aprovacao",
                        content
                    );

                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {

                throw;
            }
            return pedido;
        }

        public async Task<PedidoCompra?> Update(Guid id, PedidoCompra input)
        {
            var pedido = await context.PedidoCompras.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (pedido == null)
                return null;

            if (pedido.Aprovado)
                return pedido;
            else
            {

                pedido.NumeroPedido = input.NumeroPedido;
                pedido.Descricao = input.Descricao;
                pedido.Tipo = input.Tipo;
                pedido.Unidade = input.Unidade;
                pedido.Quantidade = input.Quantidade;
                pedido.CustoMedio = input.CustoMedio;
                pedido.ValorTotal = input.Quantidade * input.CustoMedio;
                pedido.Observacao = input.Observacao;
                pedido.DataPedido = input.DataPedido;
                pedido.FornecedorId = input.FornecedorId;
                pedido.CategoriaId = input.CategoriaId;
                pedido.MarcaId = input.MarcaId;
                pedido.ProdutoId = input.ProdutoId;
                pedido.EmailAprovacao = input.EmailAprovacao;

                try
                {
                    await context.SaveChangesAsync();
                    using var httpClient = new HttpClient();
                    var body = new {
                        email = pedido.EmailAprovacao,
                        id = pedido.NumeroPedido,
                        descricao = pedido.Descricao,
                        quantidade = pedido.Quantidade,
                        valorTotal = pedido.ValorTotal,
                        custoMedio = pedido.CustoMedio,
                        dataPedido = pedido.DataPedido
                    };
                    var json = JsonSerializer.Serialize(body);
                    var content =
                        new StringContent(
                            json,
                            Encoding.UTF8,
                            "application/json"
                        );
                    var response = await httpClient.PostAsync(
                            "http://192.168.5.57:5011/workflows/aprovacao",
                            content
                        );

                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {

                    throw;
                }
                return pedido; 
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            var pedido = await context.PedidoCompras.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (pedido == null) return false;

            if (pedido.Aprovado)
                throw new ArgumentException("Pedido aprovado não pode ser excluído");

            pedido.Ativa = false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Aprovar(string id, string email)
        {
            var pedido = await context.PedidoCompras.Where(p => p.NumeroPedido == id).SingleOrDefaultAsync();
            if (pedido == null)
                throw new ArgumentException("Pedido não encontrado");

            if (pedido.Aprovado)
                throw new ArgumentException("Pedido já está aprovado");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório");

            if (email != pedido.EmailAprovacao)
                return false;

            pedido.Aprovado = true;
            pedido.DataAprovacao = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PedidoCompra>> GetPendentes()
        {
            return await context.PedidoCompras
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Produto)
                .Where(p => p.Ativa && !p.Aprovado)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}