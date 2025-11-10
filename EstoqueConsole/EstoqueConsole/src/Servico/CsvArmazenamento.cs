using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EstoqueConsole.Modelo;

namespace EstoqueConsole.Servico
{
    public static class CsvArmazenamento
    {
        private static readonly string CaminhoProdutos = Path.Combine("data", "produtos.csv");

        public static List<Produto> CarregarProdutos()
        {
            var produtos = new List<Produto>();

            try
            {
                if (!File.Exists(CaminhoProdutos))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(CaminhoProdutos));
                    File.WriteAllText(CaminhoProdutos, "id;nome;categoria;estoqueMinimo;saldo\n");
                    return produtos;
                }

                var linhas = File.ReadAllLines(CaminhoProdutos, System.Text.Encoding.UTF8);

                // Pula o cabeçalho (primeira linha)
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

        public static void ListarProdutos()
        {
            var produtos = CarregarProdutos();

            if (produtos.Count == 0)
            {
                Console.WriteLine("\nNenhum produto cadastrado.");
                return;
            }

            Console.WriteLine("\n=== LISTA DE PRODUTOS ===");
            Console.WriteLine("ID | Nome | Categoria | Estoque Mínimo | Saldo");
            Console.WriteLine(new string('-', 60));

            foreach (var produto in produtos)
            {
                Console.WriteLine($"{produto.Id,-3} | {produto.Nome,-15} | {produto.Categoria,-12} | {produto.EstoqueMinimo,-14} | {produto.Saldo,-5}");
            }
        }

        public static void SalvarProdutos(List<Produto> produtos)
        {
            try
            {
                // Garante que o diretório existe
                Directory.CreateDirectory(Path.GetDirectoryName(CaminhoProdutos));

                var linhas = new List<string>
        {
            "id;nome;categoria;estoqueMinimo;saldo" // Cabeçalho
        };

                foreach (var produto in produtos.OrderBy(p => p.Id))
                {
                    var linha = $"{produto.Id};{produto.Nome};{produto.Categoria};{produto.EstoqueMinimo};{produto.Saldo}";
                    linhas.Add(linha);
                }

                // Escrita atômica (cria arquivo temporário e depois move)
                var tempFile = Path.Combine(Path.GetDirectoryName(CaminhoProdutos), "produtos_temp.csv");
                File.WriteAllLines(tempFile, linhas, System.Text.Encoding.UTF8);

                // Substitui o arquivo original
                File.Delete(CaminhoProdutos);
                File.Move(tempFile, CaminhoProdutos);

                Console.WriteLine($"\n✓ Produtos salvos com sucesso! Total: {produtos.Count} produtos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Erro ao salvar produtos: {ex.Message}");
            }
        }
    }
}