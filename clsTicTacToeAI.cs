using System;
using System.Collections.Generic;
using System.Text;

namespace Tictactoe
{
    /// <summary>
    /// This class controls the computer player i.e. player 2
    /// It tries to win first, if it can't then it blocks the player and if nothing urgent is happening it just picks a smart open spot.
    /// </summary>
    public class clsTicTacToeAI
    {
        /// <summary>
        /// Figures out the best move for the computer.
        /// Returns the row and column it wants to play and if no moves are left, it returns (-1, -1).
        /// </summary>
        public (int row, int col) ChooseMove(string[,] board, string aiMark, string humanMark)
        {
            // first try to win if possible
            var win = FindImmediateMove(board, aiMark);
            if (win.row != -1) return win;

            // if we can't win, try blocking the human
            var block = FindImmediateMove(board, humanMark);
            if (block.row != -1) return block;

            // try taking the center (usually best move)
            if (IsEmpty(board, 1, 1)) return (1, 1);
            // try taking one of the corners
            (int r, int c)[] corners = { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var p in corners)
                if (IsEmpty(board, p.r, p.c)) return p;

            // if nothing else, just take any open space
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (IsEmpty(board, r, c)) return (r, c);
            // no moves available
            return (-1, -1);
        }
        /// <summary>
        /// Looks for a move that would complete a row, column or diagonal with two matching marks and one empty spot.
        /// </summary>
        private (int row, int col) FindImmediateMove(string[,] board, string mark)
        {
            // Check all rows
            for (int r = 0; r < 3; r++)
            {
                var move = FindLineMove(board, mark, (r, 0), (r, 1), (r, 2));
                if (move.row != -1) return move;
            }
            // Check all columns
            for (int c = 0; c < 3; c++)
            {
                var move = FindLineMove(board, mark, (0, c), (1, c), (2, c));
                if (move.row != -1) return move;
            }
            // Check diagonals
            var d1 = FindLineMove(board, mark, (0, 0), (1, 1), (2, 2));
            if (d1.row != -1) return d1;

            var d2 = FindLineMove(board, mark, (0, 2), (1, 1), (2, 0));
            if (d2.row != -1) return d2;

            return (-1, -1);
        }

        /// <summary>
        /// Checks a specific line of 3 cells.
        /// If two are the same mark and one is empty, it returns the empty position so the AI can use it.
        /// </summary>
        private (int row, int col) FindLineMove(string[,] board, string mark, (int r, int c) a, (int r, int c) b, (int r, int c) c)
        {
            int markCount = 0;
            (int r, int c) empty = (-1, -1);

            foreach (var p in new[] { a, b, c })
            {
                string val = board[p.r, p.c];

                if (val == mark)
                    markCount++;
                else if (string.IsNullOrWhiteSpace(val))
                    empty = p;
            }

            // If we found exactly 2 of the same mark and 1 empty spot,that means this is a winning or blocking move
            if (markCount == 2 && empty.r != -1)
                return empty;

            return (-1, -1);
        }
        /// <summary>
        /// Just checks if a specific board position is empty.
        /// </summary>
        private bool IsEmpty(string[,] board, int r, int c)
        {
            return string.IsNullOrWhiteSpace(board[r, c]);
        }
    }
}