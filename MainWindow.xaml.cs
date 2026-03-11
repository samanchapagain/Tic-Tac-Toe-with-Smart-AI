using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tictactoe
{
    /// <summary>
    /// This is the UI part of the game.
    /// It handles button clicks and updates the screen but all actual game rules are handled in the other classes.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance of the game logic class. This controls the board and checks wins/ties.
        /// </summary>
        private clsTicTacToe TicTacToe;

        /// <summary>
        /// Instance of the AI class this makes the computer move when playing vs AI.
        /// </summary>
        private clsTicTacToeAI TicTacToeAI;

        /// <summary>
        /// Keeps track if a round is currently active.
        /// Prevents clicking squares before pressing Start.
        /// </summary>
        private bool bHasGameStarted;
        
        /// <summary>
        /// Dictionary that connects board positions (row,col) to their matching label on the screen.
        /// </summary>
        private Dictionary<(int r, int c), Label> dLabels;

        /// <summary>
        /// Constructor runs when the window opens. Sets everything up and prepares the board.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create game logic and AI objects
            TicTacToe = new clsTicTacToe();
            TicTacToeAI = new clsTicTacToeAI();

            // Map board positions to UI labels
            dLabels = new Dictionary<(int r, int c), Label>
            {
                {(0,0), lbl00}, {(0,1), lbl01}, {(0,2), lbl02},
                {(1,0), lbl10}, {(1,1), lbl11}, {(1,2), lbl12},
                {(2,0), lbl20}, {(2,1), lbl21}, {(2,2), lbl22},
            };

            bHasGameStarted = false;

            ResetBoardUI();
            RefreshStats();
            SetStatus("Click Start Game to begin.");
        }

        /// <summary>
        /// This runs when the Start Game button is clicked.
        /// It clears the board and starts a new round.
        /// </summary>
        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            TicTacToe.ResetRound();
            bHasGameStarted = true;

            ResetBoardUI();
            RefreshStats();
            SetStatus("Player 1's turn (X).");
        }

        /// <summary>
        /// This single method handles all square clicks.
        /// It places moves, checks for wins or ties,
        /// and switches turns.
        /// </summary>
        private void PlayerMoveClick(object sender, MouseButtonEventArgs e)
        {
            // don't allow clicking if the game hasn't started
            if (!bHasGameStarted) return;

            // stop if the round is already over
            if (TicTacToe.GameOver) return;

            if (sender is not Label clickedLabel) return;

            // get row and column from label name (example: lbl12 -> row 1 col 2)
            string name = clickedLabel.Name;
            int row = int.Parse(name[3].ToString());
            int col = int.Parse(name[4].ToString());

            // try placing the move (if spot already filled, nothing happens)
            if (!TicTacToe.TryMakeMove(row, col)) return;

            // update the screen with X or O
            clickedLabel.Content = TicTacToe.CurrentPlayer;

            // check if someone won
            if (TicTacToe.IsWinningMove())
            {
                HighlightWinningMove();
                RefreshStats();

                if (TicTacToe.CurrentPlayer == "X")
                    SetStatus("Player 1 Wins!");
                else
                    SetStatus(chkVsComputer.IsChecked == true ? "Computer Wins!" : "Player 2 Wins!");

                bHasGameStarted = false;
                return;
            }

            // check if it's a tie
            if (TicTacToe.IsTie())
            {
                RefreshStats();
                SetStatus("It's a tie!");
                bHasGameStarted = false;
                return;
            }

            // switch to next player
            TicTacToe.SwitchPlayer();

            // if playing vs computer and it's O's turn, let the AI move
            if (chkVsComputer.IsChecked == true && TicTacToe.CurrentPlayer == "O" && !TicTacToe.GameOver)
            {
                MakeComputerMove();
                return;
            }

            // otherwise just update the status text
            SetStatus(TicTacToe.CurrentPlayer == "X"
                ? "Player 1's turn (X)."
                : "Player 2's turn (O).");
        }

        /// <summary>
        /// This handles the computer's move.
        /// the AI picks a smart spot and we update the board.
        /// </summary>
        private void MakeComputerMove()
        {
            var move = TicTacToeAI.ChooseMove(TicTacToe.Board, "O", "X");

            if (move.row == -1)
                return; // no available moves

            if (!TicTacToe.TryMakeMove(move.row, move.col))
                return;

            // update UI with AI move
            dLabels[(move.row, move.col)].Content = TicTacToe.CurrentPlayer;

            // check win
            if (TicTacToe.IsWinningMove())
            {
                HighlightWinningMove();
                RefreshStats();
                SetStatus("Computer Wins!");
                bHasGameStarted = false;
                return;
            }

            // check tie
            if (TicTacToe.IsTie())
            {
                RefreshStats();
                SetStatus("It's a tie!");
                bHasGameStarted = false;
                return;
            }

            // switch back to human player
            TicTacToe.SwitchPlayer();
            SetStatus("Player 1's turn (X).");
        }

        /// <summary>
        /// changes the background color of the 3 winning squares.
        /// </summary>
        private void HighlightWinningMove()
        {
            foreach (var cell in TicTacToe.WinningCells)
            {
                dLabels[cell].Background = Brushes.Yellow;
            }
        }
        /// <summary>
        /// Clears the board visually and resets colors.
        /// </summary>
        private void ResetBoardUI()
        {
            foreach (var kvp in dLabels)
            {
                kvp.Value.Content = "";
                kvp.Value.Background = Brushes.LightGray;
            }
        }
        /// <summary>
        /// Updates the win and tie numbers on the screen.
        /// </summary>
        private void RefreshStats()
        {
            lblP1Wins.Content = $"Player 1 Wins: {TicTacToe.Player1Wins}";
            lblP2Wins.Content = $"Player 2 Wins: {TicTacToe.Player2Wins}";
            lblTies.Content = $"Ties: {TicTacToe.Ties}";
        }
        /// <summary>
        /// Updates the message shown in the status box.
        /// </summary>
        private void SetStatus(string message)
        {
            lblStatus.Content = message;
        }
    }
}