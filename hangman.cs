using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangmanMVC
{
    // ======================== MODEL ========================
    class HangmanModel
    {
        private List<(string Word, string Clue)> _words;
        private readonly Random _random = new Random();

        public string CurrentWord { get; private set; }
        public string CurrentClue { get; private set; }
        public HashSet<char> CorrectGuesses { get; private set; }
        public HashSet<char> WrongGuesses { get; private set; }
        public string Revealed => new string(CurrentWord.Select(c => CorrectGuesses.Contains(c) ? c : '_').ToArray());
        public int WrongCount => WrongGuesses.Count;

        public HangmanModel(string csvFilePath)
        {
            _words = LoadWordsFromCsv(csvFilePath);
        }

        private List<(string, string)> LoadWordsFromCsv(string path)
        {
            var list = new List<(string, string)>();
            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ CSV file not found at {path}");
                Console.ResetColor();
                Environment.Exit(1);
            }

            foreach (var line in File.ReadAllLines(path).Skip(1)) // skip header
            {
                var parts = line.Split(',');
                if (parts.Length >= 2)
                    list.Add((parts[0].Trim().ToLower(), parts[1].Trim()));
            }

            return list;
        }

        public void StartNewWord()
        {
            var item = _words[_random.Next(_words.Count)];
            CurrentWord = item.Word;
            CurrentClue = item.Clue;
            CorrectGuesses = new HashSet<char>();
            WrongGuesses = new HashSet<char>();
        }

        public bool ApplyGuess(char guess)
        {
            if (CurrentWord.Contains(guess))
            {
                CorrectGuesses.Add(guess);
                return true;
            }
            else
            {
                WrongGuesses.Add(guess);
                return false;
            }
        }

        public bool IsWon() => CurrentWord.All(c => CorrectGuesses.Contains(c));
        public bool IsLost() => WrongCount >= 3;
    }

    // ======================== VIEW ========================
    class ConsoleView
    {
        public void ShowWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=========================================");
            Console.WriteLine("       🌸 WELCOME TO HANGMAN 🌸         ");
            Console.WriteLine("=========================================\n");
            Console.ResetColor();
            Console.WriteLine("Rules:");
            Console.WriteLine("- Guess one letter at a time (a-z).");
            Console.WriteLine("- Only 3 wrong guesses allowed.");
            Console.WriteLine("- Press X to exit after each round.\n");
        }

        public void ShowClue(string clue)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n🔍 Clue: {clue.ToUpperInvariant()}");
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
                case 0:
                    Console.WriteLine(""); // nothing
                    break;
                case 1:
                    Console.WriteLine("\n  +---+\n      |\n      |\n      |\n     ===");
                    break;
                case 2:
                    Console.WriteLine("\n  +---+\n  O   |\n      |\n      |\n     ===");
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  +---+\n  O   |\n /|\\  |\n / \\  |\n     ===");
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
            Console.WriteLine("❌ Invalid input. Enter one letter (a-z).");
            Console.ResetColor();
        }

        public void ShowAlreadyGuessed(char letter)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ You already guessed '{letter}'. Try another.");
            Console.ResetColor();
        }

        public void ShowSuccessBanner(string word)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n YOU WON! ");
            Console.WriteLine($"The word was: {word.ToUpper()}\n");
            Console.WriteLine(@"      \O/        ");
            Console.WriteLine(@"       |         ");
            Console.WriteLine(@"      / \        ");
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

    // ======================== CONTROLLER ========================
    class GameController
    {
        private readonly HangmanModel _model;
        private readonly ConsoleView _view;

        public GameController(HangmanModel model, ConsoleView view)
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
                _view.ShowClue(_model.CurrentClue); // show clue for new word
                PlaySingleRound();

                _view.ShowRoundSeparator();
                _view.PromptNextAction();

                string next = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
                if (next == "x")
                    exit = true;
                else
                {
                    Console.Clear();
                    _view.ShowWelcome();
                }
            }

            Console.WriteLine("\nThanks for playing Hangman! Goodbye 👋");
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

                if (_model.CorrectGuesses.Contains(guess) || _model.WrongGuesses.Contains(guess))
                {
                    _view.ShowAlreadyGuessed(guess);
                    continue;
                }

                bool correct = _model.ApplyGuess(guess);
                if (!correct)
                {
                    _view.ShowGameState(_model.Revealed, _model.WrongGuesses, _model.WrongCount);
                }
            }

            if (_model.IsWon())
                _view.ShowSuccessBanner(_model.CurrentWord);
            else
                _view.ShowFailureBanner(_model.CurrentWord);
        }
    }

    // ======================== MAIN PROGRAM ========================
    class Program
    {
        static void Main()
        {
            string csvPath = "words.csv";
            var model = new HangmanModel(csvPath);
            var view = new ConsoleView();
            var controller = new GameController(model, view);
            controller.Run();
        }
    }
}
