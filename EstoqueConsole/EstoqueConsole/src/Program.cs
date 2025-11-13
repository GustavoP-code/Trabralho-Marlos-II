using System;
using EstoqueConsole.Servico;

namespace EstoqueConsole
{
    class Program
    {
        private static InventoryService _inventoryService = new InventoryService();

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
                        CadastrarProduto();
                        break;
                    case "3":
                        EditarProduto();
                        break;
                    case "4":
                        ExcluirProduto();
                        break;
                    case "5":
                        EntradaEstoque();
                        break;
                    case "6":
                        SaidaEstoque();
                        break;
                    case "7":
                        RelatorioEstoqueAbaixoMinimo();
                        break;
                    case "8":
                        Console.WriteLine("Extrato por produto - Em desenvolvimento...");
                        break;
                    case "9":
                        _inventoryService.SalvarDados();
                        Console.WriteLine("Dados salvos com sucesso!");
                        break;
                    case "0":
                        Console.WriteLine("Saindo do sistema...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida! Trente novamente.");
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
            _inventoryService.ListarProdutosConsole();
        }

        static void CadastrarProduto()
        {
            Console.WriteLine("\n=== CADASTRAR NOVO PRODUTO ===");

            try
            {
                Console.Write("Nome do produto: ");
                string nome = Console.ReadLine();

                Console.Write("Categoria: ");
                string categoria = Console.ReadLine();

                Console.Write("Estoque mínimo: ");
                if (!int.TryParse(Console.ReadLine(), out int estoqueMinimo) || estoqueMinimo < 0)
                {
                    Console.WriteLine("Erro: Estoque mínimo deve ser um número não negativo.");
                    return;
                }

                Console.Write("Saldo inicial (Enter para 0): ");
                string saldoInput = Console.ReadLine();
                int saldoInicial = 0;

                if (!string.IsNullOrWhiteSpace(saldoInput))
                {
                    if (!int.TryParse(saldoInput, out saldoInicial) || saldoInicial < 0)
                    {
                        Console.WriteLine("Erro: Saldo inicial deve ser um número não negativo.");
                        return;
                    }
                }

                Console.WriteLine($"\nConfirme os dados:");
                Console.WriteLine($"Nome: {nome}");
                Console.WriteLine($"Categoria: {categoria}");
                Console.WriteLine($"Estoque Mínimo: {estoqueMinimo}");
                Console.WriteLine($"Saldo Inicial: {saldoInicial}");
                Console.Write("\nConfirmar cadastro? (S/N): ");

                string confirmacao = Console.ReadLine();
                if (confirmacao?.ToUpper() == "S")
                {
                    _inventoryService.CadastrarProduto(nome, categoria, estoqueMinimo, saldoInicial);
                    Console.WriteLine("\n✓ Produto cadastrado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nCadastro cancelado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao cadastrar produto: {ex.Message}");
            }
        }

        static void EditarProduto()
        {
            Console.WriteLine("\n=== EDITAR PRODUTO ===");

            try
            {
                _inventoryService.ListarProdutosConsole();

                Console.Write("\nDigite o ID do produto que deseja editar: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID inválido!");
                    return;
                }

                var produto = _inventoryService.BuscarProdutoPorId(id);
                if (produto == null)
                {
                    Console.WriteLine($"Produto com ID {id} não encontrado!");
                    return;
                }

                Console.WriteLine($"\nEditando produto: {produto.Nome}");
                Console.WriteLine("Deixe em branco para manter o valor atual.");

                Console.Write($"Nome atual [{produto.Nome}]: ");
                string novoNome = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(novoNome))
                {
                    novoNome = produto.Nome;
                }

                Console.Write($"Categoria atual [{produto.Categoria}]: ");
                string novaCategoria = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(novaCategoria))
                {
                    novaCategoria = produto.Categoria;
                }

                Console.Write($"Estoque mínimo atual [{produto.EstoqueMinimo}]: ");
                string estoqueMinimoInput = Console.ReadLine();
                int novoEstoqueMinimo;
                if (string.IsNullOrWhiteSpace(estoqueMinimoInput))
                {
                    novoEstoqueMinimo = produto.EstoqueMinimo;
                }
                else
                {
                    if (!int.TryParse(estoqueMinimoInput, out novoEstoqueMinimo) || novoEstoqueMinimo < 0)
                    {
                        Console.WriteLine("Erro: Estoque mínimo deve ser um número não negativo.");
                        return;
                    }
                }

                Console.Write($"Saldo atual [{produto.Saldo}]: ");
                string saldoInput = Console.ReadLine();
                int novoSaldo;
                if (string.IsNullOrWhiteSpace(saldoInput))
                {
                    novoSaldo = produto.Saldo;
                }
                else
                {
                    if (!int.TryParse(saldoInput, out novoSaldo) || novoSaldo < 0)
                    {
                        Console.WriteLine("Erro: Saldo deve ser um número não negativo.");
                        return;
                    }
                }

                Console.WriteLine($"\nAlterações:");
                Console.WriteLine($"Nome: {produto.Nome} → {novoNome}");
                Console.WriteLine($"Categoria: {produto.Categoria} → {novaCategoria}");
                Console.WriteLine($"Estoque Mínimo: {produto.EstoqueMinimo} → {novoEstoqueMinimo}");
                Console.WriteLine($"Saldo: {produto.Saldo} → {novoSaldo}");
                Console.Write("\nConfirmar alterações? (S/N): ");

                string confirmacao = Console.ReadLine();
                if (confirmacao?.ToUpper() == "S")
                {
                    _inventoryService.EditarProduto(id, novoNome, novaCategoria, novoEstoqueMinimo, novoSaldo);
                    Console.WriteLine("\n✓ Produto editado com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nEdição cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao editar produto: {ex.Message}");
            }
        }

        static void ExcluirProduto()
        {
            Console.WriteLine("\n=== EXCLUIR PRODUTO ===");

            try
            {
                _inventoryService.ListarProdutosConsole();

                Console.Write("\nDigite o ID do produto que deseja excluir: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID inválido!");
                    return;
                }

                var produto = _inventoryService.BuscarProdutoPorId(id);
                if (produto == null)
                {
                    Console.WriteLine($"Produto com ID {id} não encontrado!");
                    return;
                }

                Console.WriteLine($"\nProduto selecionado:");
                Console.WriteLine($"ID: {produto.Id}");
                Console.WriteLine($"Nome: {produto.Nome}");
                Console.WriteLine($"Categoria: {produto.Categoria}");
                Console.WriteLine($"Estoque Mínimo: {produto.EstoqueMinimo}");
                Console.WriteLine($"Saldo: {produto.Saldo}");

                if (produto.Saldo > 0)
                {
                    Console.WriteLine($"\n⚠️  ATENÇÃO: Este produto possui {produto.Saldo} unidades em estoque!");
                }

                Console.Write("\n⚠️  TEM CERTEZA que deseja excluir este produto? Esta ação não pode ser desfeita! (S/N): ");

                string confirmacao = Console.ReadLine();
                if (confirmacao?.ToUpper() == "S")
                {
                    _inventoryService.ExcluirProduto(id);
                    Console.WriteLine("\n✓ Produto excluído com sucesso!");
                }
                else
                {
                    Console.WriteLine("\nExclusão cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao excluir produto: {ex.Message}");
            }
        }

        static void EntradaEstoque()
        {
            Console.WriteLine("\n=== ENTRADA EM ESTOQUE ===");

            try
            {
                _inventoryService.ListarProdutosConsole();

                Console.Write("\nDigite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID inválido!");
                    return;
                }

                var produto = _inventoryService.BuscarProdutoPorId(id);
                if (produto == null)
                {
                    Console.WriteLine($"Produto com ID {id} não encontrado!");
                    return;
                }

                Console.WriteLine($"\nProduto selecionado: {produto.Nome}");
                Console.WriteLine($"Saldo atual: {produto.Saldo} unidades");

                Console.Write("Digite a quantidade para entrada: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
                {
                    Console.WriteLine("Quantidade inválida! Deve ser um número maior que zero.");
                    return;
                }

                int novoSaldo = produto.Saldo + quantidade;
                Console.WriteLine($"\nCálculo: {produto.Saldo} + {quantidade} = {novoSaldo}");

                Console.Write("Confirmar entrada em estoque? (S/N): ");

                string confirmacao = Console.ReadLine();
                if (confirmacao?.ToUpper() == "S")
                {
                    _inventoryService.EntradaEstoque(id, quantidade);
                    Console.WriteLine($"\n✓ Entrada de {quantidade} unidades realizada com sucesso!");
                    Console.WriteLine($"Novo saldo: {novoSaldo} unidades");
                }
                else
                {
                    Console.WriteLine("\nEntrada cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao realizar entrada em estoque: {ex.Message}");
            }
        }

        static void SaidaEstoque()
        {
            Console.WriteLine("\n=== SAÍDA DE ESTOQUE ===");

            try
            {
                _inventoryService.ListarProdutosConsole();

                Console.Write("\nDigite o ID do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID inválido!");
                    return;
                }

                var produto = _inventoryService.BuscarProdutoPorId(id);
                if (produto == null)
                {
                    Console.WriteLine($"Produto com ID {id} não encontrado!");
                    return;
                }

                Console.WriteLine($"\nProduto selecionado: {produto.Nome}");
                Console.WriteLine($"Saldo atual: {produto.Saldo} unidades");

                Console.Write("Digite a quantidade para saída: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
                {
                    Console.WriteLine("Quantidade inválida! Deve ser um número maior que zero.");
                    return;
                }

                if (produto.Saldo < quantidade)
                {
                    Console.WriteLine($"\n❌ ERRO: Saldo insuficiente!");
                    Console.WriteLine($"Saldo atual: {produto.Saldo} unidades");
                    Console.WriteLine($"Tentativa de retirada: {quantidade} unidades");
                    Console.WriteLine($"Faltam: {quantidade - produto.Saldo} unidades");
                    return;
                }

                int novoSaldo = produto.Saldo - quantidade;
                Console.WriteLine($"\nCálculo: {produto.Saldo} - {quantidade} = {novoSaldo}");

                Console.Write("Confirmar saída de estoque? (S/N): ");

                string confirmacao = Console.ReadLine();
                if (confirmacao?.ToUpper() == "S")
                {
                    _inventoryService.SaidaEstoque(id, quantidade);
                    Console.WriteLine($"\n✓ Saída de {quantidade} unidades realizada com sucesso!");
                    Console.WriteLine($"Novo saldo: {novoSaldo} unidades");
                }
                else
                {
                    Console.WriteLine("\nSaída cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao realizar saída de estoque: {ex.Message}");
            }
        }

        static void RelatorioEstoqueAbaixoMinimo()
        {
            Console.WriteLine("\n=== RELATÓRIO: ESTOQUE ABAIXO DO MÍNIMO ===");

            try
            {
                var produtosAbaixoMinimo = _inventoryService.ListarProdutosAbaixoMinimo();

                if (produtosAbaixoMinimo.Count == 0)
                {
                    Console.WriteLine("\n✅ Nenhum produto está abaixo do estoque mínimo.");
                    return;
                }

                Console.WriteLine($"\n⚠️  {produtosAbaixoMinimo.Count} produto(s) abaixo do estoque mínimo:");
                Console.WriteLine("ID | Nome | Categoria | Estoque Mínimo | Saldo Atual | Déficit");
                Console.WriteLine(new string('-', 80));

                foreach (var produto in produtosAbaixoMinimo)
                {
                    int deficit = produto.EstoqueMinimo - produto.Saldo;
                    Console.WriteLine($"{produto.Id,-3} | {produto.Nome,-15} | {produto.Categoria,-12} | {produto.EstoqueMinimo,-14} | {produto.Saldo,-11} | {deficit,-7}");
                }

                Console.WriteLine($"\n📊 Resumo:");
                Console.WriteLine($"Total de produtos críticos: {produtosAbaixoMinimo.Count}");

                var maiorDeficit = produtosAbaixoMinimo.OrderByDescending(p => p.EstoqueMinimo - p.Saldo).First();
                Console.WriteLine($"Produto com maior déficit: {maiorDeficit.Nome} (faltam {maiorDeficit.EstoqueMinimo - maiorDeficit.Saldo} unidades)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao gerar relatório: {ex.Message}");
            }
        }
    }
}