// ConsoleHandler.cs

// --- Usings necessários ---
using System;
using System.Net.Http; // Para o HttpClient
using System.Threading.Tasks; // Para async/await
using System.Collections.Generic; // Para List<T>
using System.Text.Json; // Para JsonSerializer
using System.Linq; // Para o método .Any() usado nas habilidades

// --- Classes para mapeamento da API (Pokémon List Response) ---
// Elas devem ficar fora da classe ConsoleHandler, mas no mesmo arquivo ou em um Models.cs
public class PokemonListItem
{
    public string name { get; set; }
    public string url { get; set; }
}

public class PokemonListResponse
{
    public int count { get; set; }
    public string next { get; set; }
    public object previous { get; set; } // Pode ser string ou null
    public List<PokemonListItem> results { get; set; }
}

// --- Classes para mapeamento da API (Pokémon Detail Response - Mascote) ---
// Certifique-se que estas classes também estão definidas no seu projeto
// Se elas já estão em main.cs ou Models.cs, não precisa duplicar aqui.
// Se não estiverem, cole-as aqui fora da ConsoleHandler.
public class Mascote
{
    public List<AbilitiesClass> abilities { get; set; }
    public int base_experience { get; set; }
    public int height { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int order { get; set; }
    public int weight { get; set; }
}

public class AbilitiesClass
{
    public AbilityDetail ability { get; set; }
    public bool is_hidden { get; set; }
    public int slot { get; set; }
}

public class AbilityDetail
{
    public string name { get; set; }
    public string url { get; set; }
}


// --- Início da classe ConsoleHandler ---
public class ConsoleHandler
{
    // Crie uma instância de HttpClient estática e reutilizável para fazer as requisições HTTP
    private static readonly HttpClient httpClient = new HttpClient();

    public void ExibirBoasVindas()
    {
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("    Bem-vindo ao Jogo de Mascotes Virtuais!");
        Console.WriteLine("------------------------------------------");
        Console.Write("Qual é o seu nome, treinador(a)? ");
        string nome = Console.ReadLine();
        Console.WriteLine($"Olá, {nome}! Prepare-se para uma aventura!");
        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
        Console.Clear(); // Limpa a tela para o menu principal
    }

    public async Task ExibirMenuPrincipal()
    {
        int opcao = 0;
        while (opcao != 3) // Loop para manter o menu rodando até o usuário escolher Sair
        {
            Console.WriteLine("\n--- MENU PRINCIPAL ---");
            Console.WriteLine("1. Adoção de Mascotes");
            Console.WriteLine("2. Ver Mascotes Adotados (em breve!)");
            Console.WriteLine("3. Sair do Jogo");
            Console.Write("Escolha uma opção: ");

            string entrada = Console.ReadLine();
            if (int.TryParse(entrada, out opcao))
            {
                switch (opcao)
                {
                    case 1:
                        await MenuAdocao(); // Chama o menu de adoção
                        break;
                    case 2:
                        Console.WriteLine("Esta opção estará disponível em breve! Por favor, aguarde.");
                        break;
                    case 3:
                        Console.WriteLine("Obrigado por jogar! Até a próxima!");
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Por favor, escolha 1, 2 ou 3.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Entrada inválida. Por favor, digite um número.");
            }
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu principal...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private async Task MenuAdocao()
    {
        Console.Clear(); // Limpa a tela antes de mostrar o menu de adoção
        Console.WriteLine("\n--- MENU DE ADOÇÃO DE MASCOTES ---");

        PokemonListResponse pokemonList = null;
        try
        {
            // --- DIA 1: Obter a lista de Pokémons ---
            Console.WriteLine("Buscando a lista inicial de Pokémons...");
            var responseLista = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon/?limit=20"); // Limite para os primeiros 20
            responseLista.EnsureSuccessStatusCode(); // Lança exceção se a resposta não for sucesso
            string jsonListaPokemons = await responseLista.Content.ReadAsStringAsync();

            pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(jsonListaPokemons);

            Console.WriteLine("\nEspécies Disponíveis para Adoção (primeiros 20):");
            if (pokemonList != null && pokemonList.results != null)
            {
                foreach (var pokemon in pokemonList.results)
                {
                    Console.WriteLine($"- {pokemon.name}");
                }
            }
            else
            {
                Console.WriteLine("Não foi possível carregar a lista de Pokémons.");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Erro ao buscar a lista de Pokémons: {e.Message}");
            return; // Sai do método se não conseguir a lista
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Erro ao processar a lista de Pokémons (JSON): {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado ao carregar a lista: {ex.Message}");
            return;
        }

        // --- Parte de interação para escolher e ver detalhes (Dia 2) ---
        string escolhaPokemon = "";
        while (true)
        {
            Console.WriteLine("\n-------------------------------------------------");
            Console.Write("Digite o NOME do Pokémon que deseja ver detalhes (ou 'voltar' para o Menu Principal): ");
            escolhaPokemon = Console.ReadLine().ToLower().Trim(); // Converte para minúsculas e remove espaços

            if (escolhaPokemon == "voltar")
            {
                Console.WriteLine("Voltando ao Menu Principal...");
                return; // Sai do MenuAdocao
            }

            Mascote mascoteEscolhido = null;
            try
            {
                // --- DIA 2: Obter detalhes de um Pokémon específico ---
                Console.WriteLine($"Buscando detalhes de {escolhaPokemon}...");
                var responseEspecifico = await httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{escolhaPokemon}/");
                responseEspecifico.EnsureSuccessStatusCode(); // Lança exceção se 404 (não encontrado)
                string jsonPokemonEspecifico = await responseEspecifico.Content.ReadAsStringAsync();

                mascoteEscolhido = JsonSerializer.Deserialize<Mascote>(jsonPokemonEspecifico);

                if (mascoteEscolhido != null)
                {
                    Console.WriteLine("\n--- DETALHES DO MASCOTE ---");
                    Console.WriteLine($"Nome: {mascoteEscolhido.name}");
                    Console.WriteLine($"Altura: {mascoteEscolhido.height} decimetros");
                    Console.WriteLine($"Peso: {mascoteEscolhido.weight} hectogramas");
                    Console.WriteLine($"Experiência Base: {mascoteEscolhido.base_experience}");

                    Console.WriteLine("\n--- Habilidades ---");
                    if (mascoteEscolhido.abilities != null && mascoteEscolhido.abilities.Any())
                    {
                        foreach (var habilidade in mascoteEscolhido.abilities)
                        {
                            Console.WriteLine($"- {habilidade.ability.name} (Escondida: {habilidade.is_hidden})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nenhuma habilidade encontrada para este mascote.");
                    }

                    // --- Perguntar sobre adoção ---
                    Console.WriteLine("\n-------------------------------------------------");
                    Console.Write($"Você gostaria de adotar {mascoteEscolhido.name}? (sim/nao): ");
                    string respostaAdocao = Console.ReadLine().ToLower().Trim();

                    if (respostaAdocao == "sim")
                    {
                        Console.WriteLine($"🎉 Parabéns! Você adotou o {mascoteEscolhido.name}!");
                        // Futuramente, aqui você pode adicionar o mascote a uma lista de mascotes adotados
                        return; // Sai do MenuAdocao após a adoção
                    }
                    else
                    {
                        Console.WriteLine($"Ok, {mascoteEscolhido.name} não foi adotado. Você pode escolher outro.");
                    }
                }
                else
                {
                    Console.WriteLine($"Erro: Não foi possível obter detalhes de {escolhaPokemon}.");
                }
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Pokémon '{escolhaPokemon}' não encontrado. Verifique a grafia.");
                }
                else
                {
                    Console.WriteLine($"Erro ao buscar detalhes de {escolhaPokemon} (Requisição HTTP): {e.Message}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao processar detalhes de {escolhaPokemon} (JSON): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro inesperado ao buscar detalhes de {escolhaPokemon}: {ex.Message}");
            }
        }
    }
}