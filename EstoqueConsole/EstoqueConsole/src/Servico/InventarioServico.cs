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

        public Produto BuscarProdutoPorId(int id)
        {
            return _produtos.FirstOrDefault(p => p.Id == id);
        }

        public void CadastrarProduto(string nome, string categoria, int estoqueMinimo, int saldoInicial = 0)
        {
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

        public bool EditarProduto(int id, string novoNome, string novaCategoria, int novoEstoqueMinimo, int novoSaldo)
        {
            var produto = BuscarProdutoPorId(id);
            if (produto == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentException("O nome do produto é obrigatório.");
            }

            if (novoEstoqueMinimo < 0)
            {
                throw new ArgumentException("O estoque mínimo não pode ser negativo.");
            }

            if (novoSaldo < 0)
            {
                throw new ArgumentException("O saldo não pode ser negativo.");
            }

            produto.Nome = novoNome.Trim();
            produto.Categoria = novaCategoria.Trim();
            produto.EstoqueMinimo = novoEstoqueMinimo;
            produto.Saldo = novoSaldo;

            SalvarDados();
            return true;
        }

        public bool ExcluirProduto(int id)
        {
            var produto = BuscarProdutoPorId(id);
            if (produto == null)
            {
                return false;
            }

            _produtos.Remove(produto);
            SalvarDados();
            return true;
        }

        public bool EntradaEstoque(int id, int quantidade)
        {
            var produto = BuscarProdutoPorId(id);
            if (produto == null)
            {
                return false;
            }

            if (quantidade <= 0)
            {
                throw new ArgumentException("A quantidade de entrada deve ser maior que zero.");
            }

            produto.Saldo += quantidade;
            SalvarDados();
            return true;
        }

        public bool SaidaEstoque(int id, int quantidade)
        {
            var produto = BuscarProdutoPorId(id);
            if (produto == null)
            {
                return false;
            }

            if (quantidade <= 0)
            {
                throw new ArgumentException("A quantidade de saída deve ser maior que zero.");
            }

            if (produto.Saldo < quantidade)
            {
                throw new InvalidOperationException(
                    $"Saldo insuficiente! Saldo atual: {produto.Saldo}, tentou retirar: {quantidade}");
            }

            produto.Saldo -= quantidade;
            SalvarDados();
            return true;
        }

        public List<Produto> ListarProdutosAbaixoMinimo()
        {
            return _produtos.Where(p => p.Saldo < p.EstoqueMinimo).ToList();
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