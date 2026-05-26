namespace Elsa.Server.Web.DB.Class
{
    public class Produto
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal PontoReposicao { get; set; }
    }
}
