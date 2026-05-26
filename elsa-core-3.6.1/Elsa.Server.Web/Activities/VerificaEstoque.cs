using Elsa.Extensions;
using Elsa.Server.Web.DB.Class;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Microsoft.EntityFrameworkCore;
using Elsa.Workflows.Models;
using System.Text.Json;
namespace Elsa.Server.Web.Activities;
[Activity("Elsa", "VerificaEstoque", "verifica o estoque lista os produtos que estão no minimo ou abaixo.")]
public class VerificaEstoqueFlow : CodeActivity
{
    //[Output(
    //    Description = "Lista de produtos abaixo do estoque mínimo")]
    //public Output<List<Produto>> ProdutosBaixoEstoque { get; set; } = default!;

    [Output( Description = "Lista de produtos abaixo do estoque mínimo")]
    public Output<string> Listaprodutos { get; set; } = default!;

    [Output(Description = "Quantidade encontrada")]
    public Output<int> Quantidade { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var db = context.GetRequiredService<ErpDbContext>();

        var produtos = await db.Produtos
            .Where(x => x.EstoqueAtual <= x.EstoqueMinimo)
            .ToListAsync();

        var produtosJson = JsonSerializer.Serialize(produtos);
        Listaprodutos.Set(context, produtosJson);

        Quantidade.Set(context, produtos.Count);

    }
}
