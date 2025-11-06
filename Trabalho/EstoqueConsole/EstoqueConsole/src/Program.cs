using System;
using EstoqueConsole.Servico;
using EstoqueConsole.Modelo;

namespace EstoqueConsole
{
    class Program
    {
        private static InventarioServico inventario = new InventarioServico();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Carregar dados dos arquivos CSV ao iniciar
            try
            {
                inventario.CarregarDados();
                Console.WriteLine("Dados carregados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aviso: Não foi possível carregar os dados: {ex.Message}");
                Console.WriteLine("Sistema iniciado com dados vazios.");
            }

            MostrarMenu();
        }

        static void MostrarMenu()
        {
            bool sair = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("    SISTEMA DE CONTROLE DE ESTOQUE");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Listar produtos");
                Console.WriteLine("2. Cadastrar produto");
                Console.WriteLine("3. Editar produto");
                Console.WriteLine("4. Excluir produto");
                Console.WriteLine("5. Dar ENTRADA em estoque");
                Console.WriteLine("6. Dar SAÍDA de estoque");
                Console.WriteLine("7. Relatório: Estoque abaixo do mínimo");
                Console.WriteLine("8. Relatório: Extrato de movimentos por produto");
                Console.WriteLine("9. Salvar (CSV)");
                Console.WriteLine("0. Sair");
                Console.WriteLine("====================================");
                Console.Write("Digite uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        ListarProdutos();
                        break;
                    case "2":
                        CadastrarProduto();
                        break;
                    case "3":
                        EditarProduto();
                        break;
                    case "4":
                        ExcluirProduto();
                        break;
                    case "5":
                        DarEntradaEstoque();
                        break;
                    case "6":
                        DarSaidaEstoque();
                        break;
                    case "7":
                        RelatorioEstoqueAbaixoMinimo();
                        break;
                    case "8":
                        RelatorioExtratoPorProduto();
                        break;
                    case "9":
                        SalvarDados();
                        break;
                    case "0":
                        sair = true;
                        Console.WriteLine("Saindo do sistema...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para continuar.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ... (todos os métodos restantes do código anterior)
        // ListarProdutos(), CadastrarProduto(), EditarProduto(), etc...
    }
}