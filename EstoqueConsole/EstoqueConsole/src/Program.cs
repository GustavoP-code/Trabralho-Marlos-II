using System;
using EstoqueConsole.Servico;

namespace EstoqueConsole
{
    class Program
    {
        static void Main()
        {

            Console.WriteLine("=== SISTEMA DE CONTROLE DE ESTOQUE ===");

            while (true)
            {
                ExibirMenu();
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        ListarProdutos();
                        break;
                    case "2":
                        Console.WriteLine("Cadastrar produto - Em desenvolvimento...");
                        break;
                    case "3":
                        Console.WriteLine("Editar produto - Em desenvolvimento...");
                        break;
                    case "4":
                        Console.WriteLine("Excluir produto - Em desenvolvimento...");
                        break;
                    case "5":
                        Console.WriteLine("Entrada em estoque - Em desenvolvimento...");
                        break;
                    case "6":
                        Console.WriteLine("Saída de estoque - Em desenvolvimento...");
                        break;
                    case "7":
                        Console.WriteLine("Relatório estoque mínimo - Em desenvolvimento...");
                        break;
                    case "8":
                        Console.WriteLine("Extrato por produto - Em desenvolvimento...");
                        break;
                    case "9":
                        Console.WriteLine("Salvar - Em desenvolvimento...");
                        break;
                    case "0":
                        Console.WriteLine("Saindo do sistema...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida! Tente novamente.");
                        break;
                }

                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        static void ExibirMenu()
        {
            Console.WriteLine("\n=== MENU PRINCIPAL ===");
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
            Console.Write("Escolha uma opção: ");
        }

        static void ListarProdutos()
        {
            CsvArmazenamento.ListarProdutos();
        }
    }
}