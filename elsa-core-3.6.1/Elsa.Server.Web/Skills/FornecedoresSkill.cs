using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;

namespace Elsa.Server.Web.Skills;

[Description("Consulta fornecedores e retorna informações detalhadas.")]
public class FornecedoresSkill(IConfiguration configuration)
{
    [KernelFunction("ConsultarFornecedores")]
    [Description("Consulta fornecedores. Filtros: 'ativos', 'inativos', 'bloqueados', 'todos', ou nome/CNPJ do fornecedor. Retorna lista textual de fornecedores.")]
    public async Task<string> ConsultarFornecedoresAsync([Description("Filtro: 'ativos', 'inativos', 'bloqueados', 'todos', ou nome/CNPJ")] string filtro = "ativos")
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var filtroLower = filtro.ToLowerInvariant();

        var whereClause = filtroLower switch
        {
            "ativos" => "WHERE f.Situacao = 0",
            "inativos" => "WHERE f.Situacao = 1",
            "bloqueados" => "WHERE f.Situacao = 2",
            "todos" => "",
            _ when !string.IsNullOrEmpty(filtroLower) => "WHERE f.Nome LIKE @filtro OR f.CpfCnpj LIKE @filtro",
            _ => ""
        };

        var sql = $@"
            SELECT f.Id, f.Nome, f.NomeFantasia, f.CpfCnpj, f.Email, f.Telefone,
                   f.Situacao, f.Cidade, f.Uf
            FROM Fornecedores f
            {whereClause}
            ORDER BY f.Nome";

        var fornecedores = new List<string>();

        await using var conn = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, conn);
        if (!string.IsNullOrEmpty(filtroLower) && filtroLower is not ("ativos" or "inativos" or "bloqueados"))
            cmd.Parameters.AddWithValue("@filtro", $"%{filtroLower}%");

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var nome = reader.GetString(1);
            var fantasia = reader.IsDBNull(2) ? "" : reader.GetString(2);
            var cnpj = reader.IsDBNull(3) ? "N/D" : reader.GetString(3);
            var email = reader.IsDBNull(4) ? "N/D" : reader.GetString(4);
            var telefone = reader.IsDBNull(5) ? "N/D" : reader.GetString(5);
            var situacao = reader.GetInt32(6) switch { 0 => "Ativo", 1 => "Inativo", 2 => "Bloqueado", _ => "Desconhecido" };
            var cidade = reader.IsDBNull(7) ? "" : reader.GetString(7);
            var estado = reader.IsDBNull(8) ? "" : reader.GetString(8);

            fornecedores.Add($"Fornecedor: {nome} | Fantasia: {fantasia} | CNPJ/CPF: {cnpj} | Situacao: {situacao} | Cidade: {cidade}/{estado} | Contato: {telefone} / {email}");
        }

        if (fornecedores.Count == 0)
            return $"Nenhum fornecedor encontrado com o filtro '{filtro}'.";

        var resumo = $"Total de fornecedores encontrados: {fornecedores.Count}\n";
        resumo += string.Join("\n", fornecedores);
        return resumo;
    }
}
