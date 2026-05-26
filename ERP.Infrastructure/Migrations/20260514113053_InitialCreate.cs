using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categorias')
                BEGIN
                    CREATE TABLE [Categorias] (
                        [Id] uniqueidentifier NOT NULL,
                        [Nome] nvarchar(100) NOT NULL,
                        [Descricao] nvarchar(250) NULL,
                        [Tipo] int NOT NULL,
                        [Ativa] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Marcas')
                BEGIN
                    CREATE TABLE [Marcas] (
                        [Id] uniqueidentifier NOT NULL,
                        [Nome] nvarchar(100) NOT NULL,
                        [Descricao] nvarchar(250) NULL,
                        [Ativa] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Marcas] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Fornecedores')
                BEGIN
                    CREATE TABLE [Fornecedores] (
                        [Id] uniqueidentifier NOT NULL,
                        [Nome] nvarchar(150) NOT NULL,
                        [NomeFantasia] nvarchar(150) NULL,
                        [CpfCnpj] nvarchar(18) NULL,
                        [RgIe] nvarchar(20) NULL,
                        [TipoPessoa] int NOT NULL,
                        [Email] nvarchar(100) NULL,
                        [Telefone] nvarchar(20) NULL,
                        [Celular] nvarchar(20) NULL,
                        [Cep] nvarchar(10) NULL,
                        [Endereco] nvarchar(200) NULL,
                        [Numero] nvarchar(20) NULL,
                        [Complemento] nvarchar(100) NULL,
                        [Bairro] nvarchar(100) NULL,
                        [Cidade] nvarchar(100) NULL,
                        [Uf] nvarchar(2) NULL,
                        [Observacoes] nvarchar(1000) NULL,
                        [Situacao] int NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Fornecedores] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuarios')
                BEGIN
                    CREATE TABLE [Usuarios] (
                        [Id] uniqueidentifier NOT NULL,
                        [UsuarioLogin] nvarchar(50) NOT NULL,
                        [SenhaHash] nvarchar(max) NOT NULL,
                        [Nome] nvarchar(150) NULL,
                        [Email] nvarchar(100) NULL,
                        [Ativo] bit NOT NULL,
                        [Permissao] int NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Produtos')
                BEGIN
                    CREATE TABLE [Produtos] (
                        [Id] uniqueidentifier NOT NULL,
                        [Sku] nvarchar(30) NULL,
                        [Ean] nvarchar(14) NULL,
                        [Tipo] int NOT NULL,
                        [UnidadeMedida] int NOT NULL,
                        [Descricao] nvarchar(120) NOT NULL,
                        [DescricaoNFe] nvarchar(120) NULL,
                        [DescricaoComplementar] nvarchar(500) NULL,
                        [CategoriaId] uniqueidentifier NULL,
                        [MarcaId] uniqueidentifier NULL,
                        [Situacao] int NOT NULL,
                        [CustoMedio] decimal(18,4) NOT NULL,
                        [UltimaCompra] decimal(18,4) NULL,
                        [Markup] decimal(18,4) NULL,
                        [PrecoVenda] decimal(18,4) NOT NULL,
                        [PrecoMinimo] decimal(18,4) NULL,
                        [Ncm] nvarchar(8) NULL,
                        [CEST] nvarchar(7) NULL,
                        [Origem] int NOT NULL,
                        [Cfop] nvarchar(4) NULL,
                        [CstIcms] nvarchar(2) NULL,
                        [AliqIcms] decimal(5,2) NULL,
                        [MvaSt] decimal(5,2) NULL,
                        [CstPis] nvarchar(2) NULL,
                        [AliqPis] decimal(5,4) NULL,
                        [AliqCofins] decimal(5,4) NULL,
                        [PesoLiquido] decimal(10,3) NULL,
                        [PesoBruto] decimal(10,3) NULL,
                        [Altura] decimal(10,2) NULL,
                        [Largura] decimal(10,2) NULL,
                        [Profundidade] decimal(10,2) NULL,
                        [EstoqueMinimo] decimal(18,4) NULL,
                        [EstoqueMaximo] decimal(18,4) NULL,
                        [PontoReposicao] decimal(18,4) NULL,
                        [LeadTime] int NULL,
                        [ControlaLote] bit NOT NULL,
                        [ControlaValidade] bit NOT NULL,
                        [ControlaEstoque] bit NOT NULL,
                        [Localizacao] nvarchar(50) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_Produtos] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Produtos_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_Produtos_Marcas_MarcaId] FOREIGN KEY ([MarcaId]) REFERENCES [Marcas] ([Id]) ON DELETE SET NULL
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PedidoCompras')
                BEGIN
                    CREATE TABLE [PedidoCompras] (
                        [Id] uniqueidentifier NOT NULL,
                        [NumeroPedido] nvarchar(20) NOT NULL,
                        [Descricao] nvarchar(200) NOT NULL,
                        [Tipo] int NOT NULL,
                        [Unidade] int NOT NULL,
                        [Quantidade] decimal(18,4) NOT NULL,
                        [CustoMedio] decimal(18,4) NOT NULL,
                        [ValorTotal] decimal(18,4) NOT NULL,
                        [Observacao] nvarchar(500) NULL,
                        [DataPedido] datetime2 NOT NULL,
                        [FornecedorId] uniqueidentifier NOT NULL,
                        [CategoriaId] uniqueidentifier NULL,
                        [MarcaId] uniqueidentifier NULL,
                        [ProdutoId] uniqueidentifier NULL,
                        [Aprovado] bit NOT NULL,
                        [EmailAprovacao] nvarchar(100) NULL,
                        [DataAprovacao] datetime2 NULL,
                        [Ativa] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_PedidoCompras] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_PedidoCompras_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_PedidoCompras_Fornecedores_FornecedorId] FOREIGN KEY ([FornecedorId]) REFERENCES [Fornecedores] ([Id]) ON DELETE RESTRICT,
                        CONSTRAINT [FK_PedidoCompras_Marcas_MarcaId] FOREIGN KEY ([MarcaId]) REFERENCES [Marcas] ([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_PedidoCompras_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE SET NULL
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProdutoFornecedores')
                BEGIN
                    CREATE TABLE [ProdutoFornecedores] (
                        [ProdutoId] uniqueidentifier NOT NULL,
                        [FornecedorId] uniqueidentifier NOT NULL,
                        [CodigoFornecedor] nvarchar(50) NULL,
                        [PrazoEntrega] int NULL,
                        [PrecoFornecedor] decimal(18,4) NULL,
                        [IsPadrao] bit NOT NULL,
                        CONSTRAINT [PK_ProdutoFornecedores] PRIMARY KEY ([ProdutoId], [FornecedorId]),
                        CONSTRAINT [FK_ProdutoFornecedores_Fornecedores_FornecedorId] FOREIGN KEY ([FornecedorId]) REFERENCES [Fornecedores] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_ProdutoFornecedores_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PedidoCompras')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PedidoCompras_CategoriaId' AND object_id = OBJECT_ID('PedidoCompras'))
                BEGIN
                    CREATE INDEX [IX_PedidoCompras_CategoriaId""] ON [PedidoCompras] ([CategoriaId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PedidoCompras')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PedidoCompras_FornecedorId' AND object_id = OBJECT_ID('PedidoCompras'))
                BEGIN
                    CREATE INDEX [IX_PedidoCompras_FornecedorId] ON [PedidoCompras] ([FornecedorId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PedidoCompras')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PedidoCompras_MarcaId' AND object_id = OBJECT_ID('PedidoCompras'))
                BEGIN
                    CREATE INDEX [IX_PedidoCompras_MarcaId] ON [PedidoCompras] ([MarcaId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PedidoCompras')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PedidoCompras_ProdutoId' AND object_id = OBJECT_ID('PedidoCompras'))
                BEGIN
                    CREATE INDEX [IX_PedidoCompras_ProdutoId] ON [PedidoCompras] ([ProdutoId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProdutoFornecedores')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProdutoFornecedores_FornecedorId' AND object_id = OBJECT_ID('ProdutoFornecedores'))
                BEGIN
                    CREATE INDEX [IX_ProdutoFornecedores_FornecedorId] ON [ProdutoFornecedores] ([FornecedorId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Produtos')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_CategoriaId' AND object_id = OBJECT_ID('Produtos'))
                BEGIN
                    CREATE INDEX [IX_Produtos_CategoriaId] ON [Produtos] ([CategoriaId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Produtos')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_MarcaId' AND object_id = OBJECT_ID('Produtos'))
                BEGIN
                    CREATE INDEX [IX_Produtos_MarcaId] ON [Produtos] ([MarcaId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuarios')
                   OR NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_UsuarioLogin' AND object_id = OBJECT_ID('Usuarios'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Usuarios_UsuarioLogin] ON [Usuarios] ([UsuarioLogin]);
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PedidoCompras");
            migrationBuilder.DropTable(name: "ProdutoFornecedores");
            migrationBuilder.DropTable(name: "Usuarios");
            migrationBuilder.DropTable(name: "Fornecedores");
            migrationBuilder.DropTable(name: "Produtos");
            migrationBuilder.DropTable(name: "Categorias");
            migrationBuilder.DropTable(name: "Marcas");
        }
    }
}