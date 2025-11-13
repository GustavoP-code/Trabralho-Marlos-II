using System;

namespace EstoqueConsole.Modelo
{
    public class Movimento
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Tipo { get; set; } // "ENTRADA" ou "SAIDA"
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }

        public Movimento()
        {
            Tipo = string.Empty;
            Observacao = string.Empty;
            Data = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Data:dd/MM/yyyy HH:mm} | {Tipo,-7} | {Quantidade,3} unidades | {Observacao}";
        }
    }
}