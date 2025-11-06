using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EstoqueConsole.Modelo;

namespace EstoqueConsole.Servico
{
    public class CsvArmazenamento
    {
        private string pastaData = "data";
        private string arquivoProdutos = "produtos.csv";
        private string arquivoMovimentos = "movimentos.csv";

        public (List<Produto> produtos, List<Movimento> movimentos) CarregarTodos()
        {
            var produtos = CarregarProdutos();
            var movimentos = CarregarMovimentos();

            return (produtos, movimentos);
        }

        public void SalvarTodos(List<Produto> produtos, List<Movimento> movimentos)
        {
            SalvarProdutos(produtos);
            SalvarMovimentos(movimentos);
        }

        private List<Produto> CarregarProdutos()
        {
            var caminho = Path.Combine(pastaData, arquivoProdutos);
            if (!File.Exists(caminho)) return new List<Produto>();

            var linhas = File.ReadAllLines(caminho).Skip(1); // Pular cabeçalho
            var produtos = new List<Produto>();

            foreach (var linha in linhas)
            {
                var campos = linha.Split(';');
                if (campos.Length >= 5)
                {
                    var produto = new Produto
                    {
                        Id = int.Parse(campos[0]),
                        Nome = campos[1],
                        Categoria = campos[2],
                        EstoqueMinimo = int.Parse(campos[3]),
                        Saldo = int.Parse(campos[4])
                    };
                    produtos.Add(produto);
                }
            }

            return produtos;
        }

        private List<Movimento> CarregarMovimentos()
        {
            var caminho = Path.Combine(pastaData, arquivoMovimentos);
            if (!File.Exists(caminho)) return new List<Movimento>();

            var linhas = File.ReadAllLines(caminho).Skip(1); // Pular cabeçalho
            var movimentos = new List<Movimento>();

            foreach (var linha in linhas)
            {
                var campos = linha.Split(';');
                if (campos.Length >= 6)
                {
                    var movimento = new Movimento
                    {
                        Id = int.Parse(campos[0]),
                        ProdutoId = int.Parse(campos[1]),
                        Tipo = campos[2],
                        Quantidade = int.Parse(campos[3]),
                        Data = DateTime.Parse(campos[4]),
                        Observacao = campos[5]
                    };
                    movimentos.Add(movimento);
                }
            }

            return movimentos;
        }

        private void SalvarProdutos(List<Produto> produtos)
        {
            var caminho = Path.Combine(pastaData, arquivoProdutos);
            var caminhoTemp = caminho + ".tmp";

            // Garantir que a pasta existe
            Directory.CreateDirectory(pastaData);

            using (var writer = new StreamWriter(caminhoTemp))
            {
                writer.WriteLine("id;nome;categoria;estoqueMinimo;saldo");

                foreach (var produto in produtos.OrderBy(p => p.Id))
                {
                    writer.WriteLine($"{produto.Id};{produto.Nome};{produto.Categoria};{produto.EstoqueMinimo};{produto.Saldo}");
                }
            }

            // Substituir arquivo original (escrita atômica)
            File.Delete(caminho);
            File.Move(caminhoTemp, caminho);
        }

        private void SalvarMovimentos(List<Movimento> movimentos)
        {
            var caminho = Path.Combine(pastaData, arquivoMovimentos);
            var caminhoTemp = caminho + ".tmp";

            // Garantir que a pasta existe
            Directory.CreateDirectory(pastaData);

            using (var writer = new StreamWriter(caminhoTemp))
            {
                writer.WriteLine("id;produtoId;tipo;quantidade;data;observacao");

                foreach (var movimento in movimentos.OrderBy(m => m.Id))
                {
                    writer.WriteLine($"{movimento.Id};{movimento.ProdutoId};{movimento.Tipo};{movimento.Quantidade};{movimento.Data:yyyy-MM-dd HH:mm:ss};{movimento.Observacao}");
                }
            }

            // Substituir arquivo original (escrita atômica)
            File.Delete(caminho);
            File.Move(caminhoTemp, caminho);
        }
    }
}