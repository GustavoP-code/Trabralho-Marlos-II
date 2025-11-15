using System;
using System.Collections.Generic;
using System.Linq;
using EstoqueConsole.Modelo;

namespace EstoqueConsole.Servico
{
    public class InventoryService
    {
        private List<Produto> _produtos;
        private List<Movimento> _movimentos;
        private int _proximoId;
        private int _proximoMovimentoId;
        private bool _dadosCarregados;

        public InventoryService()
        {
            _produtos = new List<Produto>();
            _movimentos = new List<Movimento>();
            _proximoId = 1;
            _proximoMovimentoId = 1;
            _dadosCarregados = false;
        }

        public void CarregarDados()
        {
            if (_dadosCarregados) return;

            var produtosSalvos = CsvArmazenamento.CarregarProdutos();
            var movimentosSalvos = CsvArmazenamento.CarregarMovimentos();

            _produtos.Clear();
            _movimentos.Clear();

            // Copia os dados para a memória
            _produtos.AddRange(produtosSalvos);
            _movimentos.AddRange(movimentosSalvos);

            _proximoId = _produtos.Count > 0 ? _produtos.Max(p => p.Id) + 1 : 1;
            _proximoMovimentoId = _movimentos.Count > 0 ? _movimentos.Max(m => m.Id) + 1 : 1;

            _dadosCarregados = true;
        }

        public void SalvarTudo()
        {
            // Salva apenas quando explicitamente chamado
            CsvArmazenamento.SalvarProdutos(_produtos);
            CsvArmazenamento.SalvarMovimentos(_movimentos);
            Console.WriteLine("✓ Todos os dados foram salvos nos arquivos CSV!");
        }

        public List<Produto> ListarProdutos()
        {
            CarregarDados(); // Garante que os dados estão carregados
            return _produtos;
        }

        public Produto BuscarProdutoPorId(int id)
        {
            CarregarDados();
            return _produtos.FirstOrDefault(p => p.Id == id);
        }

        public void CadastrarProduto(string nome, string categoria, int estoqueMinimo, int saldoInicial = 0)
        {
            CarregarDados();

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
        }

        public bool EditarProduto(int id, string novoNome, string novaCategoria, int novoEstoqueMinimo, int novoSaldo)
        {
            CarregarDados();

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

            return true;
        }

        public bool ExcluirProduto(int id)
        {
            CarregarDados();

            var produto = BuscarProdutoPorId(id);
            if (produto == null)
            {
                return false;
            }

            _produtos.Remove(produto);
            return true;
        }

        public bool EntradaEstoque(int id, int quantidade)
        {
            CarregarDados();

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

            var movimento = new Movimento
            {
                Id = _proximoMovimentoId++,
                ProdutoId = id,
                Tipo = produto.Categoria,
                Quantidade = quantidade,
                Data = DateTime.Now,
                Observacao = "ENTRADA"
            };
            _movimentos.Add(movimento);

            return true;
        }

        public bool SaidaEstoque(int id, int quantidade)
        {
            CarregarDados();

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

            var movimento = new Movimento
            {
                Id = _proximoMovimentoId++,
                ProdutoId = id,
                Tipo = produto.Categoria,
                Quantidade = quantidade,
                Data = DateTime.Now,
                Observacao = "SAIDA"
            };
            _movimentos.Add(movimento);

            return true;
        }

        public List<Produto> ListarProdutosAbaixoMinimo()
        {
            CarregarDados();
            return _produtos.Where(p => p.Saldo < p.EstoqueMinimo).ToList();
        }

        public void ListarProdutosConsole()
        {
            CarregarDados();

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

        public void ListarExtratoMovimentos()
        {
            CarregarDados();

            if (_movimentos.Count == 0)
            {
                Console.WriteLine("\nNenhum movimento registrado.");
                return;
            }

            Console.WriteLine("\n=== EXTRATO DE MOVIMENTOS POR PRODUTO ===");

            var movimentosPorProduto = _movimentos
                .GroupBy(m => m.ProdutoId)
                .OrderBy(g => g.Key);

            foreach (var grupo in movimentosPorProduto)
            {
                var produto = _produtos.FirstOrDefault(p => p.Id == grupo.Key);
                var nomeProduto = produto != null ? produto.Nome : $"ID:{grupo.Key}";

                Console.WriteLine($"\n--- Produto: {nomeProduto} (ID: {grupo.Key}) ---");
                Console.WriteLine("Data/Hora       | Categoria   | Quant | Movimento");
                Console.WriteLine(new string('-', 60));

                foreach (var movimento in grupo.OrderByDescending(m => m.Data))
                {
                    Console.WriteLine($"{movimento.Data:dd/MM/yyyy HH:mm} | {movimento.Tipo,-11} | {movimento.Quantidade,5} | {movimento.Observacao}");
                }

                var entradas = grupo.Count(m => m.Observacao == "ENTRADA");
                var saidas = grupo.Count(m => m.Observacao == "SAIDA");
                var totalEntrada = grupo.Where(m => m.Observacao == "ENTRADA").Sum(m => m.Quantidade);
                var totalSaida = grupo.Where(m => m.Observacao == "SAIDA").Sum(m => m.Quantidade);

                Console.WriteLine($"Resumo: +{totalEntrada} (↑{entradas}) / -{totalSaida} (↓{saidas})");
            }

            var totalEntradas = _movimentos.Count(m => m.Observacao == "ENTRADA");
            var totalSaidas = _movimentos.Count(m => m.Observacao == "SAIDA");
            var totalQuantidadeEntrada = _movimentos.Where(m => m.Observacao == "ENTRADA").Sum(m => m.Quantidade);
            var totalQuantidadeSaida = _movimentos.Where(m => m.Observacao == "SAIDA").Sum(m => m.Quantidade);

            Console.WriteLine($"\n📊 RESUMO GERAL:");
            Console.WriteLine($"Total de movimentos: {_movimentos.Count}");
            Console.WriteLine($"Entradas: {totalEntradas} movimento(s) - {totalQuantidadeEntrada} unidades");
            Console.WriteLine($"Saídas: {totalSaidas} movimento(s) - {totalQuantidadeSaida} unidades");
            Console.WriteLine($"Saldo líquido: {totalQuantidadeEntrada - totalQuantidadeSaida} unidades");

            Console.WriteLine($"\n⚠️  AVISO: Estes dados estão apenas em memória. Use a opção 9 para salvar no CSV.");
        }

        public bool TemAlteracoesNaoSalvas()
        {
            if (!_dadosCarregados) return false;

            var produtosSalvos = CsvArmazenamento.CarregarProdutos();
            var movimentosSalvos = CsvArmazenamento.CarregarMovimentos();

            // Verifica se há diferenças na contagem
            if (_produtos.Count != produtosSalvos.Count || _movimentos.Count != movimentosSalvos.Count)
                return true;

            // Verifica se há diferenças nos produtos
            foreach (var produto in _produtos)
            {
                var produtoSalvo = produtosSalvos.FirstOrDefault(p => p.Id == produto.Id);
                if (produtoSalvo == null) return true;
                if (produto.Nome != produtoSalvo.Nome || produto.Categoria != produtoSalvo.Categoria ||
                    produto.EstoqueMinimo != produtoSalvo.EstoqueMinimo || produto.Saldo != produtoSalvo.Saldo)
                    return true;
            }

            // Verifica se há diferenças nos movimentos
            foreach (var movimento in _movimentos)
            {
                var movimentoSalvo = movimentosSalvos.FirstOrDefault(m => m.Id == movimento.Id);
                if (movimentoSalvo == null) return true;
                if (movimento.ProdutoId != movimentoSalvo.ProdutoId || movimento.Tipo != movimentoSalvo.Tipo ||
                    movimento.Quantidade != movimentoSalvo.Quantidade || movimento.Observacao != movimentoSalvo.Observacao)
                    return true;
            }

            return false;
        }
    }
}