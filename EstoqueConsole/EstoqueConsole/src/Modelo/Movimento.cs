using System;

namespace EstoqueConsole.Modelo
{
    public record struct Movimento
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Tipo { get; set; } // "ENTRADA" ou "SAIDA"
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
    }
}