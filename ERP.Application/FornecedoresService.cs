using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ERP.Application
{
    public class FornecedoresService
    {
        private readonly AppDbContext context;

        public FornecedoresService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Fornecedor>> GetAll()
        {
            return await context.Fornecedores.Where(f => f.Situacao == SituacaoFornecedor.ATIVO).ToListAsync();
        }

        public async Task<Fornecedor?> GetById(Guid id)
        {
            return await context.Fornecedores.Where(f => f.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Fornecedor> Create(Fornecedor fornecedor)
        {
            if (string.IsNullOrWhiteSpace(fornecedor.Nome))
                throw new ArgumentException("Nome é obrigatório");
            if (fornecedor.Id == Guid.Empty)
                fornecedor.Id = Guid.NewGuid();

            fornecedor.CreatedAt = DateTime.UtcNow;
            context.Fornecedores.Add(fornecedor);
            try
            {

                await context.SaveChangesAsync();
                using var httpClient = new HttpClient();
                var body = new  {email = fornecedor.Email };
                var json = JsonSerializer.Serialize(body);
                var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );
                var response = await httpClient.PostAsync(
                        "http://192.168.5.57:5011/workflows/fornecedor",
                        content
                    );

                response.EnsureSuccessStatusCode();
                return fornecedor;
            }
            catch (Exception)
            {
                throw new ArgumentException("Cadastro não realizado");

            }
            
        }

        public async Task<Fornecedor?> Update(Guid id, Fornecedor input)
        {
            var fornecedor = await context.Fornecedores.Where(f => f.Id == id).SingleOrDefaultAsync();
            if (fornecedor == null)
                return null;

            fornecedor.Nome = input.Nome;
            fornecedor.NomeFantasia = input.NomeFantasia;
            fornecedor.CpfCnpj = input.CpfCnpj;
            fornecedor.RgIe = input.RgIe;
            fornecedor.TipoPessoa = input.TipoPessoa;
            fornecedor.Email = input.Email;
            fornecedor.Telefone = input.Telefone;
            fornecedor.Celular = input.Celular;
            fornecedor.Cep = input.Cep;
            fornecedor.Endereco = input.Endereco;
            fornecedor.Numero = input.Numero;
            fornecedor.Complemento = input.Complemento;
            fornecedor.Bairro = input.Bairro;
            fornecedor.Cidade = input.Cidade;
            fornecedor.Uf = input.Uf;
            fornecedor.Observacoes = input.Observacoes;
            fornecedor.Situacao = input.Situacao;

            await context.SaveChangesAsync();
            return fornecedor;
        }

        public async Task<bool> Delete(Guid id)
        {
            var fornecedor = await context.Fornecedores.Where(f => f.Id == id).SingleOrDefaultAsync();
            if (fornecedor == null) return false;
            fornecedor.Situacao = SituacaoFornecedor.INATIVO;
            await context.SaveChangesAsync();
            return true;
        }
    }
}