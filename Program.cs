
using Practos_9_Zmey;
using System.Diagnostics;
using Zmeika2;
using static System.Console;

namespace Practos_9_Zmey
{
    class Program
    {
        private const int MapWis = 20;
        private const int MapHser = 10;


        private const int ScreenWidth = MapWis * 3;
        private const int ScreenHeight = MapHser * 3;

        private const int FrameMilliseconds = 200;

        private const ConsoleColor BorderColor = ConsoleColor.Gray;

        private const ConsoleColor FoodColor = ConsoleColor.Red;

        private const ConsoleColor BodyColor = ConsoleColor.Green;
        private const ConsoleColor HeadColor = ConsoleColor.DarkYellow;

        private static readonly Random Random = new Random();

        static void Main()
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            
            CursorVisible = false;

            while (true)
            {
                StartGame();
                Thread.Sleep(2000);
                ReadKey();
            }
        }

        static void StartGame()
        {
            int score = 0;
            Clear();
            DrawBoard();
            Zmey zmey = new Zmey(10, 5, HeadColor, BodyColor);

            Pixel food = GenFood(zmey);
            food.Draw();

            Direction currentMovement = Direction.Right;

            int lagMs = 0;
            var sw = new Stopwatch();

            while (true)
            {
                sw.Restart();
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMilliseconds - lagMs)
                {
                    if (currentMovement == oldMovement)
                        currentMovement = ReadMovement(currentMovement);
                }

                sw.Restart();

                if (zmey.Head.X == food.X && zmey.Head.Y == food.Y)
                {
                    zmey.Move(currentMovement, true);
                    food = GenFood(zmey);
                    food.Draw();

                    score++;

                    Task.Run(() => Beep(1200, 200));
                }
                else
                {
                    zmey.Move(currentMovement);
                }

                if (zmey.Head.X == MapWis - 1
                    || zmey.Head.X == 0
                    || zmey.Head.Y == MapHser - 1
                    || zmey.Head.Y == 0
                    || zmey.Body.Any(b => b.X == zmey.Head.X && b.Y == zmey.Head.Y))
                    break;

                lagMs = (int)sw.ElapsedMilliseconds;
            }

                zmey.Clear();
            food.Clear();

            SetCursorPosition(ScreenWidth / 3, ScreenHeight / 2);
            WriteLine($"ИГРА ОКОНЧЕНА, Результат: {score}");

            Task.Run(() => Beep(200, 600));
        }

        static void DrawBoard()
        {
            for (int i = 0; i < MapWis; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHser - 1, BorderColor).Draw();
            }

            for (int i = 0; i < MapHser; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWis - 1, i, BorderColor).Draw();
            }
        }

        static Pixel GenFood(Zmey zmey)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWis - 2), Random.Next(1, MapHser - 2), FoodColor);
            } while (zmey.Head.X == food.X && zmey.Head.Y == food.Y ||
                     zmey.Body.Any(b => b.X == food.X && b.Y == food.Y));

            return food;
        }

        static Direction ReadMovement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;

            ConsoleKey key = ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,

                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };

            return currentDirection;
        }
    }
}



