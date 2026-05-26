using System.ComponentModel;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;

namespace Elsa.Server.Web.Skills;

[Description("Consulta pedidos de compra e retorna informações detalhadas.")]
public class PedidosSkill(IConfiguration configuration)
{
    [KernelFunction("ConsultarPedidos")]
    [Description("Consulta pedidos de compra. Filtros: 'pendentes', 'aprovados', 'todos', ou número do pedido (ex: PC-001). Retorna lista textual de pedidos com status.")]
    public async Task<string> ConsultarPedidosAsync([Description("Filtro: 'pendentes', 'aprovados', 'todos', ou número do pedido")] string filtro = "todos")
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var filtroLower = filtro.ToLowerInvariant();

        var whereClause = filtroLower switch
        {
            "pendentes" => "WHERE p.Ativa = 1 AND p.Aprovado = 0",
            "aprovados" => "WHERE p.Ativa = 1 AND p.Aprovado = 1",
            _ when !string.IsNullOrEmpty(filtroLower) && filtroLower.StartsWith("pc-") => "WHERE p.Ativa = 1 AND UPPER(p.NumeroPedido) = @filtro",
            _ => "WHERE p.Ativa = 1"
        };

        var sql = $@"
            SELECT p.Id, p.NumeroPedido, p.Descricao, p.Quantidade, p.ValorTotal,
                   f.Nome as Fornecedor, pr.Descricao as Produto,
                   p.Aprovado, p.DataPedido
            FROM PedidoCompras p
            LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
            LEFT JOIN Produtos pr ON p.ProdutoId = pr.Id
            {whereClause}
            ORDER BY p.CreatedAt DESC";

        var pedidos = new List<string>();

        await using var conn = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrEmpty(filtroLower) && filtroLower.StartsWith("pc-"))
            cmd.Parameters.AddWithValue("@filtro", filtroLower.ToUpper());

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var numero = reader.GetString(1);
            var descricao = reader.GetString(2);
            var qtd = reader.GetDecimal(3);
            var valor = reader.GetDecimal(4);
            var fornecedor = reader.IsDBNull(5) ? "N/D" : reader.GetString(5);
            var produto = reader.IsDBNull(6) ? "N/D" : reader.GetString(6);
            var status = reader.GetBoolean(7) ? "APROVADO" : "PENDENTE";
            var data = reader.GetDateTime(8).ToString("dd/MM/yyyy");

            pedidos.Add($"Pedido: {numero} | Produto: {produto} | Qtd: {qtd.ToString("0.##", CultureInfo.InvariantCulture)} | Valor: R$ {valor.ToString("0.00", CultureInfo.InvariantCulture)} | Fornecedor: {fornecedor} | Status: {status} | Data: {data}");
        }

        if (pedidos.Count == 0)
            return $"Nenhum pedido encontrado com o filtro '{filtro}'.";

        var resumo = $"Total de pedidos encontrados: {pedidos.Count}\n";
        resumo += string.Join("\n", pedidos);
        return resumo;
    }
}
