using System;
using System.Collections.Generic;

namespace HangmanMVC
{
    class ConsoleView
    {
        public void ShowWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" WELCOME TO HANGMAN ");
            Console.ResetColor();
            Console.WriteLine("Rules:");
            Console.WriteLine("- Guess one letter at a time (a-z).");
            Console.WriteLine("- Only 3 wrong guesses allowed.");
            Console.WriteLine("- Press X to exit after each round.\n");
        }

        public void ShowClue(string clue)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n Clue: {clue.ToUpperInvariant()}");
            Console.ResetColor();
        }

        public void ShowGameState(string revealed, HashSet<char> wrongGuesses, int wrongCount)
        {
            Console.WriteLine($"\nWord: {string.Join(" ", revealed.ToUpper().ToCharArray())}");
            Console.WriteLine($"Wrong guesses ({wrongCount}/3): {string.Join(", ", wrongGuesses)}");
            ShowHangman(wrongCount);
        }

        private void ShowHangman(int stage)
        {
            switch (stage)
            {
                case 0: Console.WriteLine(""); break;
                case 1: Console.WriteLine("\n +---+\n     |\n     |\n     |\n    ==="); break;
                case 2: Console.WriteLine("\n +---+\n O   |\n     |\n     |\n    ==="); break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n +---+\n O   |\n/|\\  |\n/ \\  |\n    ===");
                    Console.ResetColor();
                    break;
            }
        }

        public string AskForLetter()
        {
            Console.Write("\nEnter a letter: ");
            return Console.ReadLine();
        }

        public void ShowInvalidInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Invalid input. Enter one letter (a-z).");
            Console.ResetColor();
        }

        public void ShowAlreadyGuessed(char letter)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" You already guessed '{letter}'. Try another.");
            Console.ResetColor();
        }

        public void ShowSuccessBanner(string word)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n YOU WON! ");
            Console.WriteLine($"The word was: {word.ToUpper()}\n");
            Console.WriteLine(@" \O/ ");
            Console.WriteLine(@"  |  ");
            Console.WriteLine(@" / \ ");
            Console.ResetColor();
        }

        public void ShowFailureBanner(string word)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nGAME OVER ");
            Console.WriteLine($"The word was: {word.ToUpper()}");
            Console.ResetColor();
        }

        public void ShowRoundSeparator()
        {
            Console.WriteLine("\n-----------------------------------------");
        }

        public void PromptNextAction()
        {
            Console.WriteLine("\nPress ENTER to play again, or X to exit:");
        }
    }
}
