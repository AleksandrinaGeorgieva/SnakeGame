﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

//todo - save where stopped
//todo - add a game menu
namespace SnakeGame
{
    class Snake
    {
        private static string snakeSign = "*";
        private static string foodSign = "@";
        private static int bodyLength = 5;
        private static int sleepTime = 200;
        private static int currentDirection = 0;
        private static string scoreOutputFileName = "Scores.txt";
        private static Dictionary<string, int> scores = new Dictionary<string, int>();
        private static Queue<Position> snakeElements = new Queue<Position>();
        private static Position food = new Position();

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

        public static List<string> directionHead = new List<string>
        {
            ">", "<", "v", "^"
        };

        public static void InitGame()
        {
            Console.BufferHeight = Console.WindowHeight;
            for (int x = 0; x < bodyLength; x++)
            {
                Position pos = new Position(x, 0);
                snakeElements.Enqueue(pos);
            }            
        }

        public static void CheckForDirectionChange()
        {
            //int currentDirection = 0;
            ConsoleKeyInfo userInput = Console.ReadKey();
            switch (userInput.Key)
            {
                case ConsoleKey.LeftArrow:
                    if(currentDirection != 0)
                    {
                        currentDirection = 1;
                    };
                    break;
                case ConsoleKey.UpArrow:
                    if (currentDirection != 2)
                    {
                        currentDirection = 3;
                    };
                    break;
                case ConsoleKey.DownArrow:
                    if (currentDirection != 3)
                    {
                        currentDirection = 2;
                    };
                    break;
                case ConsoleKey.RightArrow:
                    if (currentDirection != 1)
                    {
                        currentDirection = 0;
                    };
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0); break;
            }
           // return currentDirection;
        }

        private static int CalculatePoints()
        {
            int points = (snakeElements.Count - bodyLength) * 100;
            return Math.Max(points, 0);
        }

        private static void EndGame()
        {
            ReadScores();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game over!");
            int points = CalculatePoints();
            Console.WriteLine("Your points are: {0}", points);
            SaveScore();
            PrintScores();
        }

        private static void ReadScores()
        {
            if (File.Exists(scoreOutputFileName))
            {
                var lines = File
                    .ReadAllLines(scoreOutputFileName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
                string[] temp;
                for (int i = 0; i < lines.Length; i++)
                {
                    temp = lines[i]
                        .Split('-')
                        .ToArray();
                    scores[temp[0].Trim()] = int.Parse(temp[1].Trim());
                }
            }
        }

        private static void PrintScores()
        {
            foreach (var player in scores.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{player.Key}: {player.Value}");
            }
        }

        private static void SaveScore()
        {            
            Console.WriteLine("Enter your name:");
            var name = Console.ReadLine();
            if(name.Length > 0)
            {
                int points = CalculatePoints();
                var maxScore = scores
                    .Max(x => x.Value);
                if (maxScore < points)
                {
                    Console.WriteLine("Highest score!!!");
                }
                var result = new string[] { $"\n{name} - {points}" };

                scores[name] = points;

                File.WriteAllLines(scoreOutputFileName, result);
            }            
        }

        public static bool GenerateNewHead()
        {
            Position snakeHead = snakeElements.Last();
            Position nextDirection = directionOffsets[currentDirection];
            Position snakeNewHead = new Position(snakeHead.X + nextDirection.X, snakeHead.Y + nextDirection.Y);

            if(snakeElements.Contains(snakeNewHead))
            {
                EndGame();
                return false;
            }

            if(snakeNewHead.X < 0)
            {
                snakeNewHead.X = Console.WindowWidth - 1;
            }
            if(snakeNewHead.Y < 0)
            {
                snakeNewHead.Y = Console.WindowHeight - 1;
            }
            if (snakeNewHead.X >= Console.WindowWidth)
            {
                snakeNewHead.X = 0;
            }
            if (snakeNewHead.Y >= Console.WindowHeight)
            {
                snakeNewHead.Y = 0;
            }

            snakeElements.Enqueue(snakeNewHead);
            return true;
        }

        public static void DrawSnake()
        {
            foreach (Position pos in snakeElements)
            {
                Console.SetCursorPosition(pos.X, pos.Y);
                if (snakeElements.Last().Equals(pos))
                {
                    Console.Write(directionHead[currentDirection]);
                }else
                {
                    Console.Write(snakeSign);
                }                
            }
        }

        public static void DrawFood()
        {
            try
            {
                Console.SetCursorPosition(food.X, food.Y);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                food = GenerateFood();
                Console.SetCursorPosition(food.X, food.Y);
            }
            

            Console.Write(foodSign);
        }

        public static Position GenerateFood()
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
            bool success;

            food = GenerateFood();
            InitGame();
            
            while (true)
            {
                Console.Clear();
                DrawSnake();

                //check if food is eaten
                if(snakeElements.Last().X == food.X
                    && snakeElements.Last().Y == food.Y)
                {
                    sleepTime = sleepTime > 10 ? sleepTime - 10 : 10;
                    food = GenerateFood();
                }else
                {
                    snakeElements.Dequeue();
                }

                DrawFood();

                if (Console.KeyAvailable)
                {
                    CheckForDirectionChange();
                }

                success = GenerateNewHead();
                if (!success)
                {
                    break;
                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}
