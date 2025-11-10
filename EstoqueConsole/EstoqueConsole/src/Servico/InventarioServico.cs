using System;
using System.Collections.Generic;
using System.Linq;
using EstoqueConsole.Modelo;

namespace EstoqueConsole.Servico
{
    public class InventoryService
    {
        private List<Produto> _produtos;
        private int _proximoId;

        public InventoryService()
        {
            _produtos = new List<Produto>();
            CarregarDados();
        }

        private void CarregarDados()
        {
            _produtos = CsvArmazenamento.CarregarProdutos();
            _proximoId = _produtos.Count > 0 ? _produtos.Max(p => p.Id) + 1 : 1;
        }

        public void SalvarDados()
        {
            CsvArmazenamento.SalvarProdutos(_produtos);
        }

        public List<Produto> ListarProdutos()
        {
            return _produtos;
        }

        public void CadastrarProduto(string nome, string categoria, int estoqueMinimo, int saldoInicial = 0)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("O nome do produto é obrigatório.");
            }

            if (estoqueMinimo < 0)
            {
                throw new ArgumentException("O estoque mínimo não pode ser negativo.");
            }

            if (saldoInicial < 0)
            {
                throw new ArgumentException("O saldo inicial não pode ser negativo.");
            }

            var novoProduto = new Produto
            {
                Id = _proximoId++,
                Nome = nome.Trim(),
                Categoria = categoria.Trim(),
                EstoqueMinimo = estoqueMinimo,
                Saldo = saldoInicial
            };

            _produtos.Add(novoProduto);
            SalvarDados();
        }

        public void ListarProdutosConsole()
        {
            if (_produtos.Count == 0)
            {
                Console.WriteLine("\nNenhum produto cadastrado.");
                return;
            }

            Console.WriteLine("\n=== LISTA DE PRODUTOS ===");
            Console.WriteLine("ID | Nome | Categoria | Estoque Mínimo | Saldo");
            Console.WriteLine(new string('-', 60));

            foreach (var produto in _produtos)
            {
                Console.WriteLine($"{produto.Id,-3} | {produto.Nome,-15} | {produto.Categoria,-12} | {produto.EstoqueMinimo,-14} | {produto.Saldo,-5}");
            }
        }
    }
}