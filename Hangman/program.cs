using System;
using System.IO;

namespace HangmanMVC
{
    class Program
    {
        static void Main()
        {
            string csvPath = "words.csv";
            bool exit = false;//program stop running=false(so the loop continues)

            while (!exit)
            {
                Console.WriteLine("\nAre you an Admin (A) or a User (U)? (X to exit)");
                string choice = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (choice == "a" || choice == "admin")
                {
                    bool adding = true;

                    // Ensure file exists and has header
                    if (!File.Exists(csvPath))
                    {
                        File.WriteAllText(csvPath, "Word,Clue\n");
                    }

                    while (adding)
                    {
                        Console.Write("Enter a new word: ");
                        string newWord = Console.ReadLine()?.Trim().ToLowerInvariant();

                        Console.Write("Enter a clue for this word: ");
                        string newClue = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrWhiteSpace(newWord) && !string.IsNullOrWhiteSpace(newClue))//Checks if both word and clue are not empty.
                        {
                            using (StreamWriter sw = File.AppendText(csvPath))//Opens the file in append mode
                            {
                                sw.WriteLine($"{newWord},{newClue}");
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($" Word '{newWord}' with clue '{newClue}' added.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Invalid input. Word and clue cannot be empty.");
                            Console.ResetColor();
                        }

                        Console.Write("Do you want to add another word? (Y/N): ");
                        string again = Console.ReadLine()?.Trim().ToLowerInvariant();
                        if (again != "y" && again != "yes")
                        {
                            adding = false;
                        }
                    }
                }
                else if (choice == "u" || choice == "user")
                {
                    // Run the game
                    var model = new HangmanModel(csvPath);
                    var view = new ConsoleView();
                    var controller = new GameController(model, view);
                    controller.Run();
                }
                else if (choice == "x")
                {
                    exit = true;
                    Console.WriteLine("Exiting Hangman... Goodbye!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Invalid choice. Please enter A, U, or X.");
                    Console.ResetColor();
                }
            }
        }
    }
}
