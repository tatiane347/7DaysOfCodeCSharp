// main.cs

// 1. Diretivas 'using' necessárias para este arquivo
using System;
using System.Threading.Tasks; // Essencial para usar async/await no Main

// 2. Classe Principal do Programa
class Program
{
    // O método Main agora é assíncrono para poder chamar métodos await da ConsoleHandler
    public static async Task Main(string[] args)
    {
        // Crie uma instância da sua nova classe ConsoleHandler
        ConsoleHandler handler = new ConsoleHandler();

        // Chame os métodos de boas-vindas e o menu principal
        handler.ExibirBoasVindas();
        await handler.ExibirMenuPrincipal(); // O "await" aqui é crucial para esperar o menu terminar
    }
}