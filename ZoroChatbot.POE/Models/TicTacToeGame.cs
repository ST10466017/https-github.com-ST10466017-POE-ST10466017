using System;

namespace ZoroChatbot.POE.Models
{
    public class TicTacToeGame
    {
        private char[,] board;
        private char currentPlayer;
        private bool gameOver;
        private int movesCount;

        public TicTacToeGame()
        {
            board = new char[3, 3];
            currentPlayer = 'X';
            gameOver = false;
            movesCount = 0;
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    board[row, col] = ' ';
                }
            }
        }

        public string GetBoardDisplay()
        {
            string display = "\n";
            display += "    1   2   3\n";
            display += "  ┌───┬───┬───┐\n";

            for (int row = 0; row < 3; row++)
            {
                display += $"{row + 1} │";
                for (int col = 0; col < 3; col++)
                {
                    display += $" {board[row, col]} ";
                    if (col < 2) display += "│";
                }
                display += "│\n";
                if (row < 2) display += "  ├───┼───┼───┤\n";
            }
            display += "  └───┴───┴───┘\n";

            return display;
        }

        public string MakeMove(int row, int col)
        {
            if (gameOver)
            {
                return "The game is already over! Start a new game to play again.";
            }

            if (row < 0 || row > 2 || col < 0 || col > 2)
            {
                return "Invalid position! Please enter row and column numbers between 1 and 3.";
            }

            if (board[row, col] != ' ')
            {
                return $"Position ({row + 1}, {col + 1}) is already taken! Choose an empty spot.";
            }

            // Make the move
            board[row, col] = currentPlayer;
            movesCount++;

            // Check for win or draw
            if (CheckWin())
            {
                gameOver = true;
                string winner = currentPlayer == 'X' ? "You" : "Computer";
                return $"\n🎉 {winner} wins! Congratulations!\n\n{GetBoardDisplay()}";
            }
            else if (movesCount == 9)
            {
                gameOver = true;
                return $"\n🤝 It's a draw! Well played!\n\n{GetBoardDisplay()}";
            }

            // Switch player
            currentPlayer = currentPlayer == 'X' ? 'O' : 'X';

            string moveResult = $"Move placed at ({row + 1}, {col + 1})\n\n{GetBoardDisplay()}";

            // If it's computer's turn, make computer move
            if (!gameOver && currentPlayer == 'O')
            {
                moveResult += "\n" + MakeComputerMove();
            }

            return moveResult;
        }

        public string MakeComputerMove()
        {
            if (gameOver)
                return "The game is already over!";

            // Simple AI: Try to win, block player, or take center/corners
            var (row, col) = GetBestMove();

            if (row == -1 || col == -1)
                return "No valid moves available.";

            board[row, col] = 'O';
            movesCount++;

            if (CheckWin())
            {
                gameOver = true;
                return $"\n🤖 Computer wins! Better luck next time!\n\n{GetBoardDisplay()}";
            }
            else if (movesCount == 9)
            {
                gameOver = true;
                return $"\n🤝 It's a draw! Well played!\n\n{GetBoardDisplay()}";
            }

            currentPlayer = 'X';
            return $"Computer placed at ({row + 1}, {col + 1})\n\n{GetBoardDisplay()}\nYour turn (X):";
        }

        private (int row, int col) GetBestMove()
        {
            // Check if computer can win
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = 'O';
                        if (CheckWin())
                        {
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        board[i, j] = ' ';
                    }
                }
            }

            // Check if player can win (block)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = 'X';
                        if (CheckWin())
                        {
                            board[i, j] = ' ';
                            return (i, j);
                        }
                        board[i, j] = ' ';
                    }
                }
            }

            // Take center if available
            if (board[1, 1] == ' ')
                return (1, 1);

            // Take corners if available
            int[,] corners = { { 0, 0 }, { 0, 2 }, { 2, 0 }, { 2, 2 } };
            foreach (var corner in corners)
            {
                if (board[corner[0], corner[1]] == ' ')
                    return (corner[0], corner[1]);
            }

            // Take any remaining spot
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                        return (i, j);
                }
            }

            return (-1, -1);
        }

        private bool CheckWin()
        {
            // Check rows
            for (int row = 0; row < 3; row++)
            {
                if (board[row, 0] != ' ' && board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2])
                    return true;
            }

            // Check columns
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col] != ' ' && board[0, col] == board[1, col] && board[1, col] == board[2, col])
                    return true;
            }

            // Check diagonals
            if (board[0, 0] != ' ' && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return true;

            if (board[0, 2] != ' ' && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return true;

            return false;
        }

        public bool IsGameOver()
        {
            return gameOver;
        }

        public char GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public void ResetGame()
        {
            InitializeBoard();
            currentPlayer = 'X';
            gameOver = false;
            movesCount = 0;
        }

        public string GetGameStatus()
        {
            if (gameOver)
                return "Game over! Start a new game to play again.";

            string player = currentPlayer == 'X' ? "You" : "Computer";
            return $"It's {player}'s turn ({currentPlayer})";
        }
    }
}