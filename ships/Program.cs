using System;

namespace ships
{
    class Program
    {

        static void Main(string[] args)
        {
            (int width, int height) = Game.ChooseBoardSize();
            Console.SetWindowSize(width*4, height*4);

            Board board = new Board(width, height);
            Game game = new Game(board);
            game.Play(false, false);
        }

        public static int GetUserInt(int defaultValue)
        {
            return int.TryParse(Console.ReadLine(), out int input)
                ? input
                : defaultValue;
        }
    }
}
