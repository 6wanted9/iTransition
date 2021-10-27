using System;
using System.Security.Cryptography;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            string strGElements;
            string[] gameElements;
            //Entering game elements:
            while (true)
            {
                Console.Write("Enter game elements: ");
                strGElements = Console.ReadLine();
                gameElements = strGElements.Split(" ");
                bool duplicate = false;
                for (int i = 0; i < gameElements.Length; i++)
                    for (int j = 0; j < gameElements.Length; j++)
                        if (gameElements[i].ToLower() == gameElements[j].ToLower() && i != j)
                        {
                            duplicate = true;
                            break;
                        }
                if (!duplicate && gameElements.Length >= 3 && (gameElements.Length % 2) == 1)
                    break;
                else
                {
                    if (duplicate)
                    {
                        Console.WriteLine("Don't use duplicate game elements.");
                    }
                    if (gameElements.Length < 3)
                    {
                        Console.WriteLine("Use at least 3 game elemets.");
                    }
                    if ((gameElements.Length % 2) != 1)
                    {
                        Console.WriteLine("Use an odd number of game elemets.");
                    }
                }
            }
            //Actions after succesfull entering of elements:
            //Setting rules:
            var rules = new Rules(gameElements, "PLAYER", "PC");
            //
            string currentPCTurn = RandomPCTurn(gameElements);
            string currentPCKey = KeyAndHMACGenerator.KeyGenerator();
            string currentHMAC = KeyAndHMACGenerator.HMACGenerator(currentPCKey, currentPCTurn);
            Console.WriteLine("============================");
            Console.WriteLine($"Generated HMAC: {currentHMAC}");
            string menuOption = Menu(gameElements);
            if (menuOption == "?")
            {

            }
            else if (menuOption == "0")
            {
                Console.WriteLine("\t\t\tZ");
                Console.WriteLine("\t\tZ");
                Console.WriteLine("\tz");
                Console.WriteLine("(*.*)");
                Console.WriteLine("Application terminated.");
                return;
            }
            else
            {
                string currentPlayerTurn = PlayerTurn(gameElements, menuOption);
                Console.WriteLine($"Your move: {currentPlayerTurn}");
                Console.WriteLine($"PC move: {currentPCTurn}");
                Console.WriteLine("----------------------------");
                string gameResult = rules.WinnerChoose(currentPlayerTurn, currentPCTurn);
                Console.Write(gameResult);
                if (gameResult != "DRAW")
                {
                    Console.Write(" WON");
                }
                Console.WriteLine();
                Console.WriteLine("============================");
                Console.WriteLine($"Original KEY: {currentPCKey}");
                Console.WriteLine("============================");
            }
        }

        static string RandomPCTurn(string[] arrTurns)
        {
            var result = RandomNumberGenerator.GetInt32(arrTurns.Length);
            return arrTurns[result];
        }
        static string PlayerTurn(string[] arrTurns, string option)
        {
            return arrTurns[Convert.ToInt32(option) - 1];
        }
        static string Menu(string[] gameEls)
        {
            while(true)
            {
                Console.WriteLine("============================");
                for (int i = 0; i < gameEls.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {gameEls[i]}");
                }
                Console.WriteLine("0 - Exit.");
                Console.WriteLine("? - Help.");
                Console.Write("Please, choose option: ");
                string option = Console.ReadLine();
                Console.WriteLine("============================");
                if (Convert.ToInt32(option) >= 0 && Convert.ToInt32(option) <= gameEls.Length || option == "?")
                {
                    return option;
                }
                else
                {
                    continue;
                }
            }
            
        }
    }
}
