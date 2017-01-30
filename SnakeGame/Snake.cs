﻿using System;
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

            int bodyLength = 5;
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

        public static void GenerateNewHead(int direction, Queue<Position> snakeElements)
        {
            Position snakeHead = snakeElements.Last();
            snakeElements.Dequeue();
            Position nextDirection = directionOffsets[direction];
            Position snakeNewHead = new Position(snakeHead.X + nextDirection.X, snakeHead.Y + nextDirection.Y);
            snakeElements.Enqueue(snakeNewHead);
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

        public static Position GenerateFood()
        {
            Position food = new Position(
                randomNumberGenerator.Next(0, Console.WindowWidth),
                randomNumberGenerator.Next(0, Console.WindowHeight)
            );
            return food;
        }

        static void Main(string[] args)
        {
            //https://www.youtube.com/watch?v=dXng0W0R_Ks
            //46:54
            int currentDirection = 0;      
            
            Position food = GenerateFood();

            Queue<Position> snakeElements = new Queue<Position>();
            InitGame(snakeElements);
            while (true)
            {
                Console.Clear();
                DrawSnake(snakeElements);
                DrawFood(food);
                if (Console.KeyAvailable)
                {
                    currentDirection = CheckForDirectionChange();
                }       

                GenerateNewHead(currentDirection, snakeElements);
                Thread.Sleep(100);
            }
        }
    }
}