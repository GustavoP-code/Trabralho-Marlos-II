using System;
using System.Collections.Generic;
using System.Linq;
using EstoqueConsole.Modelo;  // ← Usando o namespace correto

namespace EstoqueConsole.Servico  // ← Namespace correto
{
    public class InventarioServico
    {
        private List<Produto> produtos = new List<Produto>();
        private List<Movimento> movimentos = new List<Movimento>();
        private CsvArmazenamento csvArmazenamento = new CsvArmazenamento();
        private int proximoIdProduto = 1;
        private int proximoIdMovimento = 1;

        // Métodos para Produtos
        public List<Produto> ListarProdutos() => produtos;

        public Produto? ObterProdutoPorId(int id) => produtos.FirstOrDefault(p => p.Id == id);

        public void CriarProduto(Produto produto)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produto.EstoqueMinimo < 0)
                throw new ArgumentException("Estoque mínimo não pode ser negativo");

            produto.Id = proximoIdProduto++;
            produtos.Add(produto);
        }

        public void AtualizarProduto(Produto produtoAtualizado)
        {
            var produtoExistente = ObterProdutoPorId(produtoAtualizado.Id);
            if (produtoExistente == null)
                throw new ArgumentException("Produto não encontrado");

            // Validações
            if (string.IsNullOrWhiteSpace(produtoAtualizado.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produtoAtualizado.EstoqueMinimo < 0)
                throw new ArgumentException("Estoque mínimo não pode ser negativo");

            var index = produtos.FindIndex(p => p.Id == produtoAtualizado.Id);
            produtos[index] = produtoAtualizado;
        }

        public void RemoverProduto(int id)
        {
            var produto = ObterProdutoPorId(id);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");

            if (produto.Value.Saldo != 0)
                throw new InvalidOperationException("Não é possível excluir produto com saldo diferente de zero");

            produtos.RemoveAll(p => p.Id == id);
        }

        // Métodos para Movimentos
        public void EntradaEstoque(int produtoId, int quantidade, string observacao)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            var produto = ObterProdutoPorId(produtoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");

            // Atualizar saldo do produto
            var produtoAtualizado = produto.Value;
            produtoAtualizado.Saldo += quantidade;
            AtualizarProduto(produtoAtualizado);

            // Registrar movimento
            var movimento = new Movimento
            {
                Id = proximoIdMovimento++,
                ProdutoId = produtoId,
                Tipo = "ENTRADA",
                Quantidade = quantidade,
                Data = DateTime.Now,
                Observacao = observacao
            };
            movimentos.Add(movimento);
        }

        public void SaidaEstoque(int produtoId, int quantidade, string observacao)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            var produto = ObterProdutoPorId(produtoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");

            if (produto.Value.Saldo < quantidade)
                throw new InvalidOperationException("Saldo insuficiente para realizar a saída");

            // Atualizar saldo do produto
            var produtoAtualizado = produto.Value;
            produtoAtualizado.Saldo -= quantidade;
            AtualizarProduto(produtoAtualizado);

            // Registrar movimento
            var movimento = new Movimento
            {
                Id = proximoIdMovimento++,
                ProdutoId = produtoId,
                Tipo = "SAIDA",
                Quantidade = quantidade,
                Data = DateTime.Now,
                Observacao = observacao
            };
            movimentos.Add(movimento);
        }

        // Relatórios
        public List<Produto> ListarProdutosAbaixoMinimo() =>
            produtos.Where(p => p.Saldo < p.EstoqueMinimo).ToList();

        public List<Movimento> ExtratoPorProduto(int produtoId) =>
            movimentos.Where(m => m.ProdutoId == produtoId)
                     .OrderBy(m => m.Data)
                     .ToList();

        // Persistência
        public void CarregarDados()
        {
            var dados = csvArmazenamento.CarregarTodos();
            produtos = dados.produtos;
            movimentos = dados.movimentos;

            // Atualizar próximos IDs
            proximoIdProduto = produtos.Count > 0 ? produtos.Max(p => p.Id) + 1 : 1;
            proximoIdMovimento = movimentos.Count > 0 ? movimentos.Max(m => m.Id) + 1 : 1;
        }

        public void SalvarDados()
        {
            csvArmazenamento.SalvarTodos(produtos, movimentos);
        }
    }
}