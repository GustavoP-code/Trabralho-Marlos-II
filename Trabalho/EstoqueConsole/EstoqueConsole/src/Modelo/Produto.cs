namespace EstoqueConsole.Modelo  // ← Namespace correto
{
    public record struct Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int EstoqueMinimo { get; set; }
        public int Saldo { get; set; }
    }
}