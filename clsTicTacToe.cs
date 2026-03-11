using System;
using System.Collections.Generic;
using System.Text;

namespace Tictactoe
{
    /// <summary>
    /// This class handles all the game logic for Tic Tac Toe.
    /// It keeps track of the board, players, wins, and checks if someone won or if it's a tie.
    /// </summary>
    public class clsTicTacToe
    {
        /// <summary>
        /// This is the actual 3x3 board.
        /// Each spot stores "", "X", or "O".
        /// </summary>
        private string[,] saBoard;
        /// <summary>
        /// Counts how many times Player 1 (X) has won.
        /// </summary>
        private int iPlayer1Wins;
        /// <summary>
        /// Counts how many times Player 2 (O) has won.
        /// </summary>
        private int iPlayer2Wins;
        /// <summary>
        /// Counts how many ties have happened.
        /// </summary>
        private int iTies;
        /// <summary>
        /// Keeps track of which type of winning move happened. this helps us know what line won.
        /// </summary>
        private WinningMove eWinningMove;
        /// <summary>
        /// Stores the 3 winning cells so the UI can highlight them.
        /// </summary>
        private (int r, int c)[] aWinningCells;
        /// <summary>
        /// True when the round is finished.
        /// </summary>
        private bool bGameOver;
        /// <summary>
        /// Stores which player's turn it is ("X" or "O").
        /// </summary>
        private string sCurrentPlayer= "X";
        /// <summary>
        /// This enum just helps identify what kind of win happened.
        /// </summary>
        private enum WinningMove
        {
            Row0,
            Row1,
            Row2,
            Col0,
            Col1,
            Col2,
            DiagMain,
            DiagAnti
        }
        /// <summary>
        ///constructor sets up the board and prepares everything.
        /// </summary>
        public clsTicTacToe()
        {
            saBoard = new string[3, 3];
            aWinningCells = Array.Empty<(int r, int c)>();
            ResetRound();
        }

        /// <summary>
        /// property to get or set the board.
        /// it makes sure the board is always 3x3.
        /// </summary>
        public string[,] Board
        {
            get => saBoard;
            set
            {
                if (value == null || value.GetLength(0) != 3 || value.GetLength(1) != 3)
                    throw new ArgumentException("Board must be 3 by 3.");

                saBoard = value;
            }
        }
        /// <summary>
        /// Returns which player is currently playing.
        /// </summary>
        public string CurrentPlayer => sCurrentPlayer;
        /// <summary>
        /// Returns Player 1 win count.
        /// </summary>
        public int Player1Wins => iPlayer1Wins;
        /// <summary>
        /// Returns Player 2 win count.
        /// </summary>
        public int Player2Wins => iPlayer2Wins;
        /// <summary>
        /// Returns tie count.
        /// </summary>
        public int Ties => iTies;
        /// <summary>
        /// Lets UI know if the game is finished.
        /// </summary>
        public bool GameOver => bGameOver;
        /// <summary>
        /// Gives back the 3 winning cells so they can be highlighted.
        /// </summary>
        public (int r, int c)[] WinningCells => aWinningCells;

        /// <summary>
        /// This resets the board for a new round, it clears everything but keeps the win stats.
        /// </summary>
        public void ResetRound()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    saBoard[r, c] = ""; // clear each square
                }
            }
            sCurrentPlayer = "X";  // X always starts
            bGameOver = false;
            aWinningCells = Array.Empty<(int r, int c)>();
        }
        /// <summary>
        /// Tries to place the current player's mark in a specific spot.
        /// Returns false if the move is invalid.
        /// </summary>
        public bool TryMakeMove(int row, int col)
        {
            if (bGameOver) return false;
            if (row < 0 || row > 2 || col < 0 || col > 2) return false;
            if (!string.IsNullOrWhiteSpace(saBoard[row, col])) return false;
            saBoard[row, col] = sCurrentPlayer;
            return true;
        }
        /// <summary>
        /// Switches the turn to the other player.
        /// </summary>
        public void SwitchPlayer()
        {
            sCurrentPlayer = (sCurrentPlayer == "X") ? "O" : "X";
        }
        /// <summary>
        /// Checks if someone has won.
        /// It checks rows, columns, and diagonals.
        /// </summary>
        public bool IsWinningMove()
        {
            if (IsHorizontalWin()) return true;
            if (IsVerticalWin()) return true;
            if (IsDiagonalWin()) return true;
            return false;
        }
        /// <summary>
        /// Checks each row to see if all 3 spots match.
        /// </summary>
        private bool IsHorizontalWin()
        {
            for (int r = 0; r < 3; r++)
            {
                string a = saBoard[r, 0];
                if (string.IsNullOrWhiteSpace(a)) continue;

                if (saBoard[r, 1] == a && saBoard[r, 2] == a)
                {
                    aWinningCells = new[] { (r, 0), (r, 1), (r, 2) };
                    bGameOver = true;
                    AddWinToStats(a);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks each column for a win.
        /// </summary>
        private bool IsVerticalWin()
        {
            for (int c = 0; c < 3; c++)
            {
                string a = saBoard[0, c];
                if (string.IsNullOrWhiteSpace(a)) continue;

                if (saBoard[1, c] == a && saBoard[2, c] == a)
                {
                    aWinningCells = new[] { (0, c), (1, c), (2, c) };
                    bGameOver = true;
                    AddWinToStats(a);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks both diagonals for a win.
        /// </summary>
        private bool IsDiagonalWin()
        {
            string a = saBoard[0, 0];

            // Main diagonal
            if (!string.IsNullOrWhiteSpace(a) &&
                saBoard[1, 1] == a &&
                saBoard[2, 2] == a)
            {
                aWinningCells = new[] { (0, 0), (1, 1), (2, 2) };
                bGameOver = true;
                AddWinToStats(a);
                return true;
            }
            // Other diagonal
            string b = saBoard[0, 2];
            if (!string.IsNullOrWhiteSpace(b) &&
                saBoard[1, 1] == b &&
                saBoard[2, 0] == b)
            {
                aWinningCells = new[] { (0, 2), (1, 1), (2, 0) };
                bGameOver = true;
                AddWinToStats(b);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the board is full and nobody won.
        /// </summary>
        public bool IsTie()
        {
            if (bGameOver) return false;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (string.IsNullOrWhiteSpace(saBoard[r, c]))
                        return false;
                }
            }
            // If we got here, the board is full and no winner
            bGameOver = true;
            iTies++;
            aWinningCells = Array.Empty<(int r, int c)>();
            return true;
        }
        /// <summary>
        /// Updates win counter depending on who won.
        /// </summary>
        private void AddWinToStats(string mark)
        {
            if (mark == "X")
                iPlayer1Wins++;
            else
                iPlayer2Wins++;
        }
    }
}
