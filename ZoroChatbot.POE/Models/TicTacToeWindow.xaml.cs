using System;
using System.Windows;
using System.Windows.Controls;
using ZoroChatbot.POE.Models;

namespace ZoroChatbot.POE
{
    public partial class TicTacToeWindow : Window
    {
        private TicTacToeGame game;
        private Button[,] buttons;
        private ActivityLog activityLog;

        public TicTacToeWindow(ActivityLog log = null)
        {
            InitializeComponent();
            activityLog = log;
            game = new TicTacToeGame();

            // Initialize button array
            buttons = new Button[3, 3]
            {
                { Btn00, Btn01, Btn02 },
                { Btn10, Btn11, Btn12 },
                { Btn20, Btn21, Btn22 }
            };

            UpdateBoard();
            UpdateStatus();

            if (activityLog != null)
            {
                activityLog.AddActivity("Tic-Tac-Toe game started");
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (game.IsGameOver())
            {
                MessageBox.Show("The game is already over! Click 'New Game' to play again.",
                               "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (game.GetCurrentPlayer() != 'X')
            {
                MessageBox.Show("Wait for the computer to make its move!",
                               "Computer's Turn", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            string[] tagParts = clickedButton.Tag.ToString().Split(',');
            int row = int.Parse(tagParts[0]);
            int col = int.Parse(tagParts[1]);

            string result = game.MakeMove(row, col);
            UpdateBoard();
            UpdateStatus();

            // Log the move
            if (activityLog != null)
            {
                activityLog.AddActivity($"Tic-Tac-Toe: Player moved at ({row + 1}, {col + 1})");
            }

            // Check if game is over
            if (game.IsGameOver())
            {
                if (result.Contains("wins"))
                {
                    MessageBox.Show("🎉 You won! Great job!",
                                   "Victory!", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (activityLog != null)
                    {
                        activityLog.AddActivity("Tic-Tac-Toe: Player won");
                    }
                }
                else if (result.Contains("draw"))
                {
                    MessageBox.Show("🤝 It's a draw! Well played!",
                                   "Draw", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (activityLog != null)
                    {
                        activityLog.AddActivity("Tic-Tac-Toe: Draw");
                    }
                }
                else if (result.Contains("Computer wins"))
                {
                    MessageBox.Show("🤖 Computer wins! Better luck next time!",
                                   "Computer Wins", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (activityLog != null)
                    {
                        activityLog.AddActivity("Tic-Tac-Toe: Computer won");
                    }
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            game.ResetGame();
            UpdateBoard();
            UpdateStatus();

            if (activityLog != null)
            {
                activityLog.AddActivity("Tic-Tac-Toe: New game started");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    char value = game.GetBoardValue(row, col);
                    buttons[row, col].Content = value == ' ' ? "" : value.ToString();

                    // Color X and O differently
                    if (value == 'X')
                    {
                        buttons[row, col].Foreground = System.Windows.Media.Brushes.LightGreen;
                    }
                    else if (value == 'O')
                    {
                        buttons[row, col].Foreground = System.Windows.Media.Brushes.Orange;
                    }
                    else
                    {
                        buttons[row, col].Foreground = System.Windows.Media.Brushes.#00D2FF;
                    }
                }
            }
        }

        private void UpdateStatus()
        {
            if (game.IsGameOver())
            {
                string result = game.GetGameResult();
                if (result.Contains("wins"))
                {
                    StatusText.Text = result;
                    StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
                }
                else if (result.Contains("draw"))
                {
                    StatusText.Text = "🤝 It's a draw!";
                    StatusText.Foreground = System.Windows.Media.Brushes.Yellow;
                }
                else
                {
                    StatusText.Text = "Game Over! Click 'New Game' to play again.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Orange;
                }
            }
            else
            {
                string player = game.GetCurrentPlayer() == 'X' ? "Your" : "Computer's";
                StatusText.Text = $"{player} turn ({game.GetCurrentPlayer()})";
                StatusText.Foreground = System.Windows.Media.Brushes.#00D2FF;
            }
        }
    }

    // Extension method to access board value
    public static class TicTacToeExtensions
    {
        public static char GetBoardValue(this TicTacToeGame game, int row, int col)
        {
            var field = game.GetType().GetField("board",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var board = field.GetValue(game) as char[,];
                if (board != null)
                {
                    return board[row, col];
                }
            }
            return ' ';
        }

        public static string GetGameResult(this TicTacToeGame game)
        {
            if (!game.IsGameOver())
                return "Game in progress";

            // Check if it's a draw
            if (game.GetType().GetField("movesCount",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance) is var movesField)
            {
                int moves = (int)movesField.GetValue(game);
                if (moves == 9)
                    return "draw";
            }

            // Check who won
            char[,] board = null;
            var boardField = game.GetType().GetField("board",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (boardField != null)
            {
                board = boardField.GetValue(game) as char[,];
            }

            if (board != null)
            {
                // Check rows
                for (int row = 0; row < 3; row++)
                {
                    if (board[row, 0] != ' ' && board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2])
                        return board[row, 0] == 'X' ? "You win!" : "Computer wins!";
                }

                // Check columns
                for (int col = 0; col < 3; col++)
                {
                    if (board[0, col] != ' ' && board[0, col] == board[1, col] && board[1, col] == board[2, col])
                        return board[0, col] == 'X' ? "You win!" : "Computer wins!";
                }

                // Check diagonals
                if (board[0, 0] != ' ' && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                    return board[0, 0] == 'X' ? "You win!" : "Computer wins!";

                if (board[0, 2] != ' ' && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                    return board[0, 2] == 'X' ? "You win!" : "Computer wins!";
            }

            return "Game over";
        }
    }
}