using ERP.Application;
using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.SetIsOriginAllowed(_ => true)
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));
builder.Services.AddScoped<ProdutosService>();
builder.Services.AddScoped<CategoriasService>();
builder.Services.AddScoped<MarcasService>();
builder.Services.AddScoped<FornecedoresService>();
builder.Services.AddScoped<PedidosComprasService>();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveSuperSecretaDoERPElsa2024PeloAmorDeDeus!@#$%";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ERPElsa";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ERPElsaAPI";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, PATCH, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With, Accept, Origin";
        await Task.CompletedTask;
        return;
    }
    await next();
});

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    if (!context.Usuarios.Any())
    {
        context.Usuarios.Add(new Usuario
        {
            Id = Guid.NewGuid(),
            UsuarioLogin = "admin",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Nome = "Administrador",
            Email = "admin@erpelsa.com.br",
            Permissao = Permissao.ADMIN,
            Ativo = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
        Console.WriteLine("Usuário admin criado: admin / password");
    }

    if (!context.Produtos.Any())
    {
        var cat1 = new Categoria { Id = Guid.NewGuid(), Nome = "Informática / Periféricos", Descricao = "Produtos de informática e periféricos", Tipo = TipoCategoria.PRODUTO, Ativa = true };
        var cat2 = new Categoria { Id = Guid.NewGuid(), Nome = "Material de Escritório", Descricao = "Material de escritório e papelaria", Tipo = TipoCategoria.PRODUTO, Ativa = true };
        var cat3 = new Categoria { Id = Guid.NewGuid(), Nome = "Ferramentas / Manuais", Descricao = "Ferramentas manuais e elétricas", Tipo = TipoCategoria.PRODUTO, Ativa = true };
        context.Categorias.AddRange(cat1, cat2, cat3);

        var mar1 = new Marca { Id = Guid.NewGuid(), Nome = "Dell", Descricao = "Dell Technologies", Ativa = true };
        var mar2 = new Marca { Id = Guid.NewGuid(), Nome = "Bosch", Descricao = "Bosch Professional", Ativa = true };
        var mar3 = new Marca { Id = Guid.NewGuid(), Nome = "Samsung", Descricao = "Samsung Electronics", Ativa = true };
        context.Marcas.AddRange(mar1, mar2, mar3);

        var forn1 = new Fornecedor { Id = Guid.NewGuid(), Nome = "Distribuidora ABC", NomeFantasia = "ABC Distribuições", CpfCnpj = "12.345.678/0001-90", TipoPessoa = TipoPessoa.JURIDICA, Email = "contato@abcdistribuidora.com.br", Telefone = "(11) 3456-7890", Cidade = "São Paulo", Uf = "SP", Situacao = SituacaoFornecedor.ATIVO };
        var forn2 = new Fornecedor { Id = Guid.NewGuid(), Nome = "XYZ Importação", NomeFantasia = "XYZ Imports", CpfCnpj = "98.765.432/0001-01", TipoPessoa = TipoPessoa.JURIDICA, Email = "vendas@xyzimport.com.br", Telefone = "(21) 98765-4321", Cidade = "Rio de Janeiro", Uf = "RJ", Situacao = SituacaoFornecedor.ATIVO };
        context.Fornecedores.AddRange(forn1, forn2);

        await context.SaveChangesAsync();

        var prod1 = new Produto { Id = Guid.NewGuid(), Sku = "MON-001", Ean = "7891234567890", Tipo = TipoProduto.MERCADORIA, UnidadeMedida = UnidadeMedida.UN, Descricao = "Monitor Dell 24 polegadas LED Full HD", DescricaoNFe = "Monitor LED 24 pol Full HD Dell", DescricaoComplementar = "Monitor de alta resolução para uso profissional", CategoriaId = cat1.Id, MarcaId = mar1.Id, Situacao = SituacaoProduto.ATIVO, CustoMedio = 450.00m, UltimaCompra = 480.00m, Markup = 25m, PrecoVenda = 562.50m, PrecoMinimo = 500.00m, Ncm = "85176200", CEST = "017100", Origem = 0, Cfop = "5102", CstIcms = "00", AliqIcms = 18m, CstPis = "01", AliqPis = 1.65m, AliqCofins = 7.6m, PesoLiquido = 3.5m, PesoBruto = 4.2m, Altura = 40m, Largura = 55m, Profundidade = 15m, EstoqueMinimo = 5m, EstoqueAtual = 15m, PontoReposicao = 10m, LeadTime = 7, ControlaLote = false, ControlaValidade = false, ControlaEstoque = true, Localizacao = "A-01-03-B" };
        var prod2 = new Produto { Id = Guid.NewGuid(), Sku = "FUR-002", Ean = "7899876543210", Tipo = TipoProduto.USO_CONSUMO, UnidadeMedida = UnidadeMedida.PC, Descricao = "Caneta esferográfica azul cx c/ 50", DescricaoNFe = "Caneta esferográfica azul cx c/ 50", CategoriaId = cat2.Id, Situacao = SituacaoProduto.ATIVO, CustoMedio = 25.00m, PrecoVenda = 45.00m, Ncm = "96083000", Origem = 0, CstIcms = "00", AliqIcms = 18m, ControlaEstoque = true, EstoqueMinimo = 10m, EstoqueAtual = 50m, Localizacao = "B-02-01-A" };
        var prod3 = new Produto { Id = Guid.NewGuid(), Sku = "FER-003", Ean = "7894561237890", Tipo = TipoProduto.MERCADORIA, UnidadeMedida = UnidadeMedida.PC, Descricao = "Furadeira Impacto Bosch 18V sem bateria", DescricaoNFe = "Furadeira impacto 18V s/ bateria Bosch", CategoriaId = cat3.Id, MarcaId = mar2.Id, Situacao = SituacaoProduto.ATIVO, CustoMedio = 280.00m, PrecoVenda = 450.00m, Ncm = "84672100", Origem = 0, CstIcms = "00", AliqIcms = 18m, PesoLiquido = 1.8m, PesoBruto = 2.0m, ControlaEstoque = true, EstoqueMinimo = 3m, EstoqueAtual = 2m, Localizacao = "C-03-02-A" };
        var prod4 = new Produto { Id = Guid.NewGuid(), Sku = "PAP-004", Ean = "7891239876540", Tipo = TipoProduto.MATERIA_PRIMA, UnidadeMedida = UnidadeMedida.KG, Descricao = "Papel A4 75g/m2 cx c/ 500 fls", DescricaoNFe = "Papel A4 75g cx c/ 500 fls", CategoriaId = cat2.Id, Situacao = SituacaoProduto.ATIVO, CustoMedio = 22.00m, PrecoVenda = 35.00m, Ncm = "48025500", Origem = 0, ControlaEstoque = true, EstoqueMinimo = 20m, EstoqueAtual = 25m, Localizacao = "D-01-01-A" };
        var prod5 = new Produto { Id = Guid.NewGuid(), Sku = "TEL-005", Ean = "7893216549870", Tipo = TipoProduto.MERCADORIA, UnidadeMedida = UnidadeMedida.UN, Descricao = "Teclado Dell USB ABNT2 Preto", DescricaoNFe = "Teclado USB ABNT2 Dell preto", CategoriaId = cat1.Id, MarcaId = mar1.Id, Situacao = SituacaoProduto.ATIVO, CustoMedio = 65.00m, PrecoVenda = 120.00m, Ncm = "84716052", Origem = 0, ControlaEstoque = true, EstoqueMinimo = 5m, EstoqueAtual = 10m, Localizacao = "A-01-04-B" };
        context.Produtos.AddRange(prod1, prod2, prod3, prod4, prod5);

        var pf1 = new ProdutoFornecedor { ProdutoId = prod1.Id, FornecedorId = forn1.Id, CodigoFornecedor = "MON-DELL-001", PrazoEntrega = 7, PrecoFornecedor = 450.00m, IsPadrao = true };
        var pf2 = new ProdutoFornecedor { ProdutoId = prod3.Id, FornecedorId = forn1.Id, CodigoFornecedor = "FER-BOSCH-003", PrazoEntrega = 10, PrecoFornecedor = 280.00m, IsPadrao = true };
        var pf3 = new ProdutoFornecedor { ProdutoId = prod4.Id, FornecedorId = forn2.Id, CodigoFornecedor = "PAP-A4-XYZ", PrazoEntrega = 5, PrecoFornecedor = 22.00m, IsPadrao = true };
        context.ProdutoFornecedores.AddRange(pf1, pf2, pf3);

        await context.SaveChangesAsync();
        Console.WriteLine("Seed executado: 5 produtos, 3 categorias, 3 marcas, 2 fornecedores");
    }
}

app.Run();