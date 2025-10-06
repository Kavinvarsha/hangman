// Program.cs
// Hangman Console Game - MVC + OOP
// Compatible with .NET Core / .NET 5+

using System;
using System.Collections.Generic;
using System.Linq;

namespace HangmanMVC
{
    // -----------------------
    // MODEL
    // -----------------------
    public class GameModel
    {
        private readonly List<string> _wordBank = new List<string>
        {
            "computer", "hangman", "architecture", "programming", "variable",
            "function", "inheritance", "polymorphism", "encapsulation", "abstraction",
            "console", "application", "randomize", "controller", "view",
            "model", "exception", "threading", "dictionary", "interface",
            "delegate", "namespace", "algorithm", "iteration", "recursion"
        };

        private readonly Random _rnd = new Random();

        public string CurrentWord { get; private set; } = string.Empty;
        public char[] Revealed { get; private set; } = new char[0];
        public HashSet<char> CorrectGuesses { get; } = new HashSet<char>();
        public HashSet<char> WrongGuesses { get; } = new HashSet<char>();
        public int MaxWrongAllowed { get; } = 3;
        public int WrongCount => WrongGuesses.Count;

        public void StartNewWord()
        {
            CurrentWord = _wordBank[_rnd.Next(_wordBank.Count)].ToLowerInvariant();
            Revealed = new string('_', CurrentWord.Length).ToCharArray();
            CorrectGuesses.Clear();
            WrongGuesses.Clear();
        }

        public bool IsLetterInWord(char letter)
        {
            return CurrentWord.Contains(letter);
        }

        public bool ApplyGuess(char letter)
        {
            letter = char.ToLowerInvariant(letter);

            if (CorrectGuesses.Contains(letter) || WrongGuesses.Contains(letter))
                return true; // Already guessed - treat as no state change, controller will notify view.

            if (IsLetterInWord(letter))
            {
                CorrectGuesses.Add(letter);
                for (int i = 0; i < CurrentWord.Length; i++)
                {
                    if (CurrentWord[i] == letter) Revealed[i] = letter;
                }
                return true;
            }
            else
            {
                WrongGuesses.Add(letter);
                return false;
            }
        }

        public bool IsWon()
        {
            return !Revealed.Contains('_');
        }

        public bool IsLost()
        {
            return WrongGuesses.Count >= MaxWrongAllowed;
        }
    }

    // -----------------------
    // VIEW
    // -----------------------
    public class ConsoleView
    {
        public void ShowWelcome()
        {
            Console.Clear();
            
            Console.WriteLine("       WELCOME TO HANGMAN (MVC OOP)     ");
           
            Console.WriteLine("Rules:");
            Console.WriteLine("- You get up to 3 wrong letter guesses per word.");
            Console.WriteLine("- Guess one letter at a time (a-z).");
            Console.WriteLine("- After each round you may play again or exit.\n");
        }

        public void ShowGameState(char[] revealed, IEnumerable<char> wrongGuesses, int wrongCount)
        {
            Console.WriteLine("\nCurrent word:");
            Console.WriteLine(" " + string.Join(" ", revealed));
            Console.WriteLine($"\nWrong guesses [{wrongCount}/3]: {string.Join(", ", wrongGuesses)}\n");
            ShowHangmanArt(wrongCount);
        }

        public void ShowHangmanArt(int wrongCount)
        {
            // 0: nothing, 1: hanger, 2: man under hanger, 3: hanging man (full)
            switch (wrongCount)
            {
                case 0:
                    Console.WriteLine("   ");
                    break;
                case 1:
                    Console.WriteLine("  _______");
                    Console.WriteLine("  |     |");
                    Console.WriteLine("  |");
                    Console.WriteLine("  |");
                    Console.WriteLine("  |");
                    Console.WriteLine(" _|_\n");
                    break;
                case 2:
                    Console.WriteLine("  _______");
                    Console.WriteLine("  |     |");
                    Console.WriteLine("  |     O");
                    Console.WriteLine("  |    /");
                    Console.WriteLine("  |");
                    Console.WriteLine(" _|_\n");
                    break;
                case 3:
                    // final hanging man picture used also in failure banner, but we display here too
                    Console.WriteLine("  _______");
                    Console.WriteLine("  |     |");
                    Console.WriteLine("  |     O");
                    Console.WriteLine("  |    /|\\");
                    Console.WriteLine("  |    / \\");
                    Console.WriteLine(" _|_\n");
                    break;
            }
        }

        public void ShowAlreadyGuessed(char letter)
        {
            Console.WriteLine($"You already guessed '{letter}'. Try another letter.");
        }

        public void ShowInvalidInput()
        {
            Console.WriteLine("Invalid input. Please enter a single alphabetic letter (a-z).");
        }

        public void ShowSuccessBanner(string word)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            
            Console.WriteLine("             CONGRATULATIONS!            ");
            Console.WriteLine("             YOU SAVED THE MAN           ");
          
            Console.WriteLine(GetHappyManArt());
            Console.WriteLine($"\nThe word was: {word.ToUpperInvariant()}");
            Console.ResetColor();
        }

        public void ShowFailureBanner(string word)
        {
            Console.ForegroundColor = ConsoleColor.Red;
         
            Console.WriteLine("                GAME OVER                ");
            Console.WriteLine("             THE MAN HAS HANGED          ");
          
            Console.WriteLine(GetHangingManArt());
            Console.WriteLine($"\nThe word was: {word.ToUpperInvariant()}");
            Console.ResetColor();
        }

        public void ShowRoundSeparator()
        {
            Console.WriteLine("\n-----------------------------------------\n");
        }

        public void PromptNextAction()
        {
            Console.WriteLine("\nPress Enter to play next word, or type X then Enter to exit.");
        }

        public string AskForLetter()
        {
            Console.Write("Guess a letter: ");
            return Console.ReadLine() ?? string.Empty;
        }

        private string GetHappyManArt()
        {
            // A simple ASCII happy man picture
            return @"
      \o/      <-- happy!
       |
      / \
     ( ^_^ )
";
        }

        private string GetHangingManArt()
        {
            // A simple ASCII hanging man picture
            return @"
   _______
   |     |
   |     O
   |    /|\
   |    / \
  _|_
 (R.I.P.)
";
        }
    }

    // -----------------------
    // CONTROLLER
    // -----------------------
    public class GameController
    {
        private readonly GameModel _model;
        private readonly ConsoleView _view;

        public GameController(GameModel model, ConsoleView view)
        {
            _model = model;
            _view = view;
        }

        public void Run()
        {
            _view.ShowWelcome();

            bool exit = false;
            while (!exit)
            {
                _model.StartNewWord();
                PlaySingleRound();

                _view.ShowRoundSeparator();
                _view.PromptNextAction();
                string next = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
                if (next == "x") exit = true;
                else
                {
                    Console.Clear();
                    _view.ShowWelcome();
                }
            }

            Console.WriteLine("\nThanks for playing Hangman! Goodbye.");
        }

        private void PlaySingleRound()
        {
            while (!_model.IsWon() && !_model.IsLost())
            {
                _view.ShowGameState(_model.Revealed, _model.WrongGuesses, _model.WrongCount);

                string input = _view.AskForLetter();
                if (string.IsNullOrWhiteSpace(input))
                {
                    _view.ShowInvalidInput();
                    continue;
                }

                input = input.Trim().ToLowerInvariant();

                if (input.Length != 1 || !char.IsLetter(input[0]))
                {
                    _view.ShowInvalidInput();
                    continue;
                }

                char guess = input[0];

                // Already guessed checks
                if (_model.CorrectGuesses.Contains(guess) || _model.WrongGuesses.Contains(guess))
                {
                    _view.ShowAlreadyGuessed(guess);
                    continue;
                }

                bool wasCorrect = _model.ApplyGuess(guess);

                if (!wasCorrect)
                {
                    // wrong guess applied
                    _view.ShowGameState(_model.Revealed, _model.WrongGuesses, _model.WrongCount);
                }
            }

            // Round ended
            if (_model.IsWon())
            {
                _view.ShowSuccessBanner(_model.CurrentWord);
            }
            else if (_model.IsLost())
            {
                _view.ShowFailureBanner(_model.CurrentWord);
            }
        }
    }

    // -----------------------
    // PROGRAM ENTRY
    // -----------------------
    class Program
    {
        static void Main(string[] args)
        {
            // Ensure console supports colors (most terminals do)
            try
            {
                var model = new GameModel();
                var view = new ConsoleView();
                var controller = new GameController(model, view);
                controller.Run();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
