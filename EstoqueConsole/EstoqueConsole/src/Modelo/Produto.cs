namespace EstoqueConsole.Modelo
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int EstoqueMinimo { get; set; }
        public int Saldo { get; set; }

        public Produto()
        {
            Nome = string.Empty;
            Categoria = string.Empty;
        }

        public override string ToString()
        {
            return $"ID: {Id} | Nome: {Nome} | Categoria: {Categoria} | Estoque Mínimo: {EstoqueMinimo} | Saldo: {Saldo}";
        }
    }
}