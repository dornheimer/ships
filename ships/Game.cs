using System;
using System.Text;

namespace ships
{
    public class Game
    {
        public static Random Rand { get; set; } = new Random();

        private Board board;
        private int score;
        private int turnCount;
        private int turnsWithoutHit;

        public Game(Board board)
        {
            this.board = board;
            board.PlaceShips();
        }

        public static (int x, int y) ChooseBoardSize()
        {
            Console.WriteLine("Welcome to 'ships'");
            Console.WriteLine("Please select your board size (empty input = default).");

            Console.Write("Board width: ");
            int userWidth = Program.GetUserInt(Board.DefaultWidth);

            Console.Write("Board height: ");
            int userHeight = Program.GetUserInt(Board.DefaultHeight);

            return (userWidth, userHeight);
        }

        public void Play(bool showShips = false, bool showShipArea = false, bool moveShips = true)
        {
            PrintBoard(showShips, showShipArea);
            PrintScore("");

            string message;
            bool playing = true;
            while (playing)
            {
                (int x, int y) = PlayerGuess();
                board.PlayerShots.Add((X: x, Y: y));

                if (board.IsBlocked(x, y))
                {
                    Ship ship = board.GetShipAtCoordinates(x, y);
                    board.RevealShip(ship);
                    score += ship.Size * 100;
                    message = "You hit a ship!";
                    turnsWithoutHit = 0;
                }
                else
                {
                    message = "You missed.";
                    turnsWithoutHit++;
                }

                if (board.ShipsHit == board.Ships.Count)
                {
                    message = "All ships hit! You win!";
                    playing = false;
                }

                if (moveShips)
                    board.MoveShips();

                board.Update(turnsWithoutHit);
                PrintBoard(showShips, showShipArea);
                PrintScore(message);
                turnCount++;
            }
            PrintBoard(true);
        }

        public void PrintBoard(bool showShips = false, bool showShipArea = false)
        {
            Console.Clear();
            int padding = 3;
            PrintLine(padding);

            string xAxis = "|".PadLeft(padding);
            for (int x = 0; x < board.Width; x++)
            {
                xAxis += x.ToString().PadRight(padding);
            }

            xAxis += '|';
            Console.WriteLine(xAxis);
            PrintLine(padding);

            string line;
            for (int y = 0; y < board.Height; y++)
            {
                line = "";
                line += y.ToString().PadRight(padding - 1) + '|';
                for (int x = 0; x < board.Width; x++)
                {
                    if (showShips && (board[x, y] == (int)Board.Status.ShipHidden))
                        line += "+".PadRight(padding);
                    else if (board[x, y] == (int)Board.Status.Shot)
                        line += "*".PadRight(padding);
                    else if (board[x, y] == (int)Board.Status.ShipRevealed)
                        line += "#".PadRight(padding);
                    else if (showShipArea && (board[x, y] == (int)Board.Status.ShipArea))
                        line += ".".PadRight(padding);
                    else if ((turnsWithoutHit > 3) && (board[x, y] == (int)Board.Status.Hint))
                        line += "?".PadRight(padding);
                    else
                        line += "~".PadRight(padding);
                }

                line += '|';
                Console.WriteLine(line);
            }
            PrintLine(padding);
        }

        public void PrintLine(int padding)
        {
            string line = "|".PadLeft(padding);
            for (int x = 0; x < board.Width; x++)
            {
                line += "---";
            }

            line += '|';
            Console.WriteLine(line);
        }

        public void PrintScore(string message, int padding = 3)
        {
            double percentageHit = (board.PlayerShots.Count > 0)
                ? ((double)board.ShipsHit / board.PlayerShots.Count)
                : 0;

            PrintScoreLine(message, padding);
            PrintScoreLine("", padding);
            PrintScoreLine($"Score {score}", padding);
            PrintScoreLine($"Ships Hit {board.ShipsHit}/{board.Ships.Count}", padding);
            PrintScoreLine($"Hits/Shots {board.ShipsHit}/{board.PlayerShots.Count} ({(percentageHit).ToString("0.0%")})", padding);

            PrintLine(padding);
        }

        private void PrintScoreLine(string str, int padding)
        {
            int linePadding = board.Width * 3 + padding;

            string output = "|".PadLeft(padding) + str;
            Console.WriteLine(output.PadRight(linePadding) + '|');
        }

        private (int x, int y) PlayerGuess()
        {
            PrintScoreLine("Make a guess:", 3);
            Console.Write("|".PadLeft(3));
            string guess = Console.ReadLine();

            while (guess.Length < 3)
                guess = Console.ReadLine();

            string[] coordinates = guess.Split(',');

            int x = Convert.ToInt32(coordinates[0]);
            int y = Convert.ToInt32(coordinates[1]);

            return (x, y);
        }
    }
}
