using System.ComponentModel;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;

namespace Elsa.Server.Web.Skills;

[Description("Consulta estoque de produtos e retorna informações detalhadas.")]
public class EstoqueSkill(IConfiguration configuration)
{
    [KernelFunction("ConsultarEstoque")]
    [Description("Consulta o estoque de produtos. Filtros: 'baixo', 'zerado', 'disponivel', 'todos', ou nome do produto. Retorna texto com lista de produtos e status de estoque.")]
    public async Task<string> ConsultarEstoqueAsync([Description("Filtro: 'baixo', 'zerado', 'disponivel', 'todos', ou nome do produto")] string filtro = "todos")
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var filtroLower = filtro.ToLowerInvariant();

        var whereClause = filtroLower switch
        {
            "baixo" => "WHERE p.EstoqueAtual <= p.EstoqueMinimo",
            "zerado" => "WHERE p.EstoqueAtual = 0",
            "disponivel" => "WHERE p.EstoqueAtual > 0",
            "todos" => "",
            _ when !string.IsNullOrEmpty(filtroLower) => "WHERE p.Descricao LIKE @filtro",
            _ => ""
        };

        var sql = $@"
            SELECT p.Id, p.Sku, p.Descricao, c.Nome as Categoria, m.Nome as Marca,
                   p.EstoqueAtual, p.EstoqueMinimo, p.PontoReposicao,
                   p.PrecoVenda, p.CustoMedio
            FROM Produtos p
            LEFT JOIN Categorias c ON p.CategoriaId = c.Id
            LEFT JOIN Marcas m ON p.MarcaId = m.Id
            {whereClause}";

        var produtos = new List<string>();

        await using var conn = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrEmpty(filtroLower) && filtroLower is not ("baixo" or "zerado" or "disponivel"))
            cmd.Parameters.AddWithValue("@filtro", $"%{filtroLower}%");

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var estoqueAtual = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
            var estoqueMinimo = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6);
            var pontoReposicao = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7);

            var status = estoqueAtual <= 0 ? "ZERADO" :
                         estoqueAtual <= estoqueMinimo ? "BAIXO" :
                         estoqueAtual > 0 ? "DISPONIVEL" : 
            estoqueAtual <= pontoReposicao ? "ATENCAO" : "NORMAL";

            var descricao = reader.GetString(2);
            var sku = reader.IsDBNull(1) ? "N/D" : reader.GetString(1);
            var categoria = reader.IsDBNull(3) ? "N/D" : reader.GetString(3);
            var marca = reader.IsDBNull(4) ? "N/D" : reader.GetString(4);

            var estoqueStr = estoqueAtual.HasValue ? estoqueAtual.Value.ToString("0.##", CultureInfo.InvariantCulture) : "N/A";
            var minimoStr = estoqueMinimo.HasValue ? estoqueMinimo.Value.ToString("0.##", CultureInfo.InvariantCulture) : "N/A";
            produtos.Add($"▸ {descricao} (SKU: {sku})\n  Categoria: {categoria} | Marca: {marca}\n  Estoque: {estoqueStr} unidades | Mínimo: {minimoStr} | Status: {status}");
        }

        if (produtos.Count == 0)
            return $"Nenhum produto encontrado com o filtro '{filtro}'.";

        return string.Join("\n\n", produtos);
    }
}
