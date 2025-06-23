// main.cs - Código Completo para mostrar Dia 1 e Dia 2

// 1. Diretivas 'using':
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

// 2. Classes para Mapear o JSON da PokeAPI (necessárias para o Dia 2)

public class AbilityDetail
{
    public string name { get; set; }
    public string url { get; set; }
}

public class AbilitiesClass
{
    public AbilityDetail ability { get; set; }
    public bool is_hidden { get; set; }
    public int slot { get; set; }
}

public class Mascote
{
    public List<AbilitiesClass> abilities { get; set; }
    public int base_experience { get; set; }
    public int height { get; set; }
    public string name { get; set; }
    public int order { get; set; }
    public int weight { get; set; }
}

// Opcional: Classe para mapear a resposta da lista do Dia 1, se você quiser desserializar a lista também
// public class PokemonListItem
// {
//     public string name { get; set; }
//     public string url { get; set; }
// }

// public class PokemonListResponse
// {
//     public int count { get; set; }
//     public string next { get; set; }
//     public object previous { get; set; } // Pode ser string ou null
//     public List<PokemonListItem> results { get; set; }
// }


// 3. Classe Principal do Programa
class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Iniciando demonstração: Desafio Dia 1 e Dia 2!");

        var httpClient = new HttpClient();
        string jsonListaPokemons = "";
        string jsonPokemonEspecifico = "";
        
        // --- DESAFIO DIA 1: Obter e Imprimir a Lista Geral de Pokémons ---
        Console.WriteLine("\n--- EXECUTANDO DESAFIO DIA 1: LISTA GERAL DE POKÉMONS ---");
        try
        {
            var responseLista = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon/");
            responseLista.EnsureSuccessStatusCode();
            jsonListaPokemons = await responseLista.Content.ReadAsStringAsync();

            Console.WriteLine("\nJSON Bruto Recebido (Lista de Pokémons):");
            Console.WriteLine(jsonListaPokemons);
            Console.WriteLine("\nDesafio Dia 1 Concluído!");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Erro no Dia 1 (Requisição HTTP): {e.Message}");
        }

        // --- DESAFIO DIA 2: Obter e Imprimir Detalhes de um Pokémon Específico ---
        Console.WriteLine("\n\n--- EXECUTANDO DESAFIO DIA 2: DETALHES DE UM POKÉMON ESPECÍFICO ---");
        try
        {
            // Use a URL de um Pokémon específico aqui (ex: Pikachu)
            var responseEspecifico = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon/pikachu/");
            responseEspecifico.EnsureSuccessStatusCode();
            jsonPokemonEspecifico = await responseEspecifico.Content.ReadAsStringAsync();

            Console.WriteLine("\nJSON Bruto Recebido (Pokémon Específico):");
            // Se você quiser, pode comentar a linha abaixo para não poluir tanto o console
            // Console.WriteLine(jsonPokemonEspecifico);

            Mascote meuMascote = JsonSerializer.Deserialize<Mascote>(jsonPokemonEspecifico);

            if (meuMascote != null)
            {
                Console.WriteLine("\n--- Detalhes do Mascote Adotado ---");
                Console.WriteLine($"Nome: {meuMascote.name}");
                Console.WriteLine($"Altura: {meuMascote.height} decimetros");
                Console.WriteLine($"Peso: {meuMascote.weight} hectogramas");
                Console.WriteLine($"Experiência Base: {meuMascote.base_experience}");

                Console.WriteLine("\n--- Habilidades ---");
                if (meuMascote.abilities != null && meuMascote.abilities.Count > 0)
                {
                    foreach (var habilidade in meuMascote.abilities)
                    {
                        Console.WriteLine($"- {habilidade.ability.name} (Escondida: {habilidade.is_hidden})");
                    }
                }
                else
                {
                    Console.WriteLine("Nenhuma habilidade encontrada para este mascote.");
                }
            }
            else
            {
                Console.WriteLine("Erro ao desserializar o JSON do Pokémon específico.");
            }
            Console.WriteLine("\nDesafio Dia 2 Concluído!");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Erro no Dia 2 (Requisição HTTP): {e.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Erro no Dia 2 (Desserialização JSON): {ex.Message}");
            Console.WriteLine("Verifique se as classes de mapeamento estão corretas para o JSON do Pokémon específico.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado no Dia 2: {ex.Message}");
        }

        Console.WriteLine("\nDemonstração de Desafios Dia 1 e Dia 2 Concluída com Sucesso!");
    }
}
