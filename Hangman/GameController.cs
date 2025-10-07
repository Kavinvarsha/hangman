//runs the game loop..manages the flow of game

using System;

namespace HangmanMVC
{
    class GameController
    {
        private readonly HangmanModel _model;//stores the game data (current word, guesses, rules)
        private readonly ConsoleView _view;//handles display and input for the player

        public GameController(HangmanModel model, ConsoleView view)//constructor for gamecontroller.links the controller to the data(model) and UI(view).
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
                _view.ShowClue(_model.CurrentClue);
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
            Console.WriteLine("\nThanks for playing Hangman! Goodbye ??");
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
}
