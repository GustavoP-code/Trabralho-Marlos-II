using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EstoqueConsole.Modelo;

namespace EstoqueConsole.Servico
{
    public static class CsvArmazenamento
    {
        private static readonly string CaminhoProdutos = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "data", "produtos.csv"
        );

        private static readonly string CaminhoMovimentos = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "data", "movimentos.csv"
        );

        // Métodos para Produtos (já existentes)
        public static List<Produto> CarregarProdutos()
        {
            var produtos = new List<Produto>();

            try
            {
                string caminhoAbsoluto = Path.GetFullPath(CaminhoProdutos);

                if (!File.Exists(caminhoAbsoluto))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(caminhoAbsoluto));
                    File.WriteAllText(caminhoAbsoluto, "id;nome;categoria;estoqueMinimo;saldo\n");
                    return produtos;
                }

                var linhas = File.ReadAllLines(caminhoAbsoluto, System.Text.Encoding.UTF8);

                for (int i = 1; i < linhas.Length; i++)
                {
                    var linha = linhas[i].Trim();

                    if (string.IsNullOrWhiteSpace(linha) || linha.StartsWith("id;"))
                        continue;

                    var campos = linha.Split(';');

                    for (int j = 0; j < campos.Length; j++)
                    {
                        campos[j] = campos[j].Trim();
                    }

                    if (campos.Length >= 5 &&
                        !string.IsNullOrEmpty(campos[0]) &&
                        int.TryParse(campos[0], out _))
                    {
                        try
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
                        catch (FormatException)
                        {
                            // Ignora linhas com formato inválido
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar produtos: {ex.Message}");
            }

            return produtos;
        }

        public static void SalvarProdutos(List<Produto> produtos)
        {
            try
            {
                string caminhoAbsoluto = Path.GetFullPath(CaminhoProdutos);

                Directory.CreateDirectory(Path.GetDirectoryName(caminhoAbsoluto));

                var linhas = new List<string>
                {
                    "id;nome;categoria;estoqueMinimo;saldo"
                };

                foreach (var produto in produtos.OrderBy(p => p.Id))
                {
                    var linha = $"{produto.Id};{produto.Nome};{produto.Categoria};{produto.EstoqueMinimo};{produto.Saldo}";
                    linhas.Add(linha);
                }

                var tempFile = Path.Combine(Path.GetDirectoryName(caminhoAbsoluto), "produtos_temp.csv");
                File.WriteAllLines(tempFile, linhas, System.Text.Encoding.UTF8);

                File.Delete(caminhoAbsoluto);
                File.Move(tempFile, caminhoAbsoluto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar produtos: {ex.Message}");
            }
        }

        // Métodos para Movimentos
        public static List<Movimento> CarregarMovimentos()
        {
            var movimentos = new List<Movimento>();

            try
            {
                string caminhoAbsoluto = Path.GetFullPath(CaminhoMovimentos);

                if (!File.Exists(caminhoAbsoluto))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(caminhoAbsoluto));
                    File.WriteAllText(caminhoAbsoluto, "id;produtoId;tipo;quantidade;data;observacao\n");
                    return movimentos;
                }

                var linhas = File.ReadAllLines(caminhoAbsoluto, System.Text.Encoding.UTF8);

                for (int i = 1; i < linhas.Length; i++)
                {
                    var linha = linhas[i].Trim();

                    if (string.IsNullOrWhiteSpace(linha) || linha.StartsWith("id;"))
                        continue;

                    var campos = linha.Split(';');

                    for (int j = 0; j < campos.Length; j++)
                    {
                        campos[j] = campos[j].Trim();
                    }

                    if (campos.Length >= 6 &&
                        !string.IsNullOrEmpty(campos[0]) &&
                        int.TryParse(campos[0], out _))
                    {
                        try
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
                        catch (FormatException)
                        {
                            // Ignora linhas com formato inválido
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar movimentos: {ex.Message}");
            }

            return movimentos;
        }

        public static void SalvarMovimentos(List<Movimento> movimentos)
        {
            try
            {
                string caminhoAbsoluto = Path.GetFullPath(CaminhoMovimentos);

                Directory.CreateDirectory(Path.GetDirectoryName(caminhoAbsoluto));

                var linhas = new List<string>
                {
                    "id;produtoId;tipo;quantidade;data;observacao"
                };

                foreach (var movimento in movimentos.OrderBy(m => m.Id))
                {
                    var linha = $"{movimento.Id};{movimento.ProdutoId};{movimento.Tipo};{movimento.Quantidade};{movimento.Data:yyyy-MM-dd HH:mm:ss};{movimento.Observacao}";
                    linhas.Add(linha);
                }

                var tempFile = Path.Combine(Path.GetDirectoryName(caminhoAbsoluto), "movimentos_temp.csv");
                File.WriteAllLines(tempFile, linhas, System.Text.Encoding.UTF8);

                File.Delete(caminhoAbsoluto);
                File.Move(tempFile, caminhoAbsoluto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar movimentos: {ex.Message}");
            }
        }
    }
}