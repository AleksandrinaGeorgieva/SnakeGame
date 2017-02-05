using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnakeGame
{
    class Snake
    {
        private static string snakeSign = "*";
        private static string foodSign = "@";
        private static int bodyLength = 5;
        private static int sleepTime = 200;

        private static Random randomNumberGenerator = new Random();
        public struct Position
        {
            public int X;
            public int Y;
            public Position(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        };

        public static Position[] directionOffsets = new Position[]
            {
                new Position(1, 0), // right
                new Position(-1, 0), // left
                new Position (0, 1), // down
                new Position(0, -1) //top
            };

        public static void InitGame(Queue<Position> q)
        {
            Console.BufferHeight = Console.WindowHeight;
            for (int x = 0; x < bodyLength; x++)
            {
                Position pos = new Position(x, 0);
                q.Enqueue(pos);
            }            
        }

        public static int CheckForDirectionChange()
        {
            int currentDirection = 0;
            ConsoleKeyInfo userInput = Console.ReadKey();
            switch (userInput.Key)
            {
                case ConsoleKey.LeftArrow:
                    currentDirection = 1; break;
                case ConsoleKey.UpArrow:
                    currentDirection = 3; break;
                case ConsoleKey.DownArrow:
                    currentDirection = 2; break;
                case ConsoleKey.RightArrow:
                    currentDirection = 0; break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
            }
            return currentDirection;
        }

        private static void EndGame(Queue<Position> snakeElements)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game over!");
            int points = (snakeElements.Count - bodyLength + 1) * 100;
            Console.WriteLine("Your points are: {0}", points);
        }

        public static bool GenerateNewHead(int direction, Queue<Position> snakeElements)
        {
            Position snakeHead = snakeElements.Last();
            Position nextDirection = directionOffsets[direction];
            Position snakeNewHead = new Position(snakeHead.X + nextDirection.X, snakeHead.Y + nextDirection.Y);

            if(snakeNewHead.X < 0 || 
                snakeNewHead.Y < 0 ||
                snakeNewHead.X >= Console.WindowWidth ||
                snakeNewHead.Y >= Console.WindowHeight ||
                snakeElements.Contains(snakeNewHead))
            {
                EndGame(snakeElements);
                return false;
            }

            snakeElements.Enqueue(snakeNewHead);
            return true;
        }

        public static void DrawSnake(Queue<Position> snakeElements)
        {
            foreach (Position pos in snakeElements)
            {
                Console.SetCursorPosition(pos.X, pos.Y);
                Console.Write(snakeSign);
            }
        }

        public static void DrawFood(Position food)
        {
            Console.SetCursorPosition(food.X, food.Y);
            Console.Write(foodSign);
        }

        public static Position GenerateFood(Queue<Position> snakeElements)
        {
            Position food;
            do
            {
                food = new Position(
                                randomNumberGenerator.Next(0, Console.WindowWidth),
                                randomNumberGenerator.Next(0, Console.WindowHeight)
                            );
            } while (snakeElements.Contains(food));
            
            return food;
        }

        static void Main(string[] args)
        {
            
            int currentDirection = 0;
            bool success;            

            Queue<Position> snakeElements = new Queue<Position>();
            InitGame(snakeElements);
            Position food = GenerateFood(snakeElements);
            while (true)
            {
                Console.Clear();
                DrawSnake(snakeElements);

                //check if food is eaten
                if(snakeElements.Last().X == food.X
                    && snakeElements.Last().Y == food.Y)
                {
                    sleepTime = sleepTime > 10 ? sleepTime - 10 : 10;
                    food = GenerateFood(snakeElements);
                }else
                {
                    snakeElements.Dequeue();
                }

                DrawFood(food);

                if (Console.KeyAvailable)
                {
                    currentDirection = CheckForDirectionChange();
                }

                success = GenerateNewHead(currentDirection, snakeElements);
                if (!success)
                {
                    break;
                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}
