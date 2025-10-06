using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangmanMVC
{
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
                Console.WriteLine($"? CSV file not found at {path}");
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
}
