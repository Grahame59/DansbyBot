using System;
using System.Collections.Generic;


namespace Snake
{
    public class SnakeGame
    {

    
        // Enum to represent the movement direction
        public enum MoveDirection
        {
            Up,
            Down,
            Left,
            Right,
            None
        }

        // Structure to represent a position on the game board
        public struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            // Method to check if two positions are equal
            public bool Equals(Position other)
            {
                return X == other.X && Y == other.Y;
            }
        }

        // Function to initialize the game board
        public static void InitializeBoard(char[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = 'S';
                }
            }
        }

        // Function to initialize the snake at the center of the board
        public static void InitializeSnake(Queue<Position> snake, int width, int height)
        {
            int startX = width / 2;
            int startY = height / 2;
            snake.Enqueue(new Position(startX, startY));
        }

        // Function to generate a random position for food
        public static Position GenerateFoodPosition(Queue<Position> snake, int width, int height)
        {
            Random random = new Random();
            int x, y;
            do
            {
                x = random.Next(0, width);
                y = random.Next(0, height);
            } 
            while (snake.Contains(new Position(x, y)));
            
                return new Position(x, y);
        }

        // Function to draw the game board
        public static void DrawBoard(char[,] board, Queue<Position> snake, Position food)
        {
            // Clear the board
            Array.Clear(board, 0, board.Length);

            // Draw snake
            foreach (var position in snake)
            {
                board[position.Y, position.X] = '^';
            }

            // Draw food
            board[food.Y, food.X] = 'F';

            // Draw board
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        // Function to get the move direction from user input
        public static MoveDirection GetMoveDirection(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    return MoveDirection.Up;
                case ConsoleKey.DownArrow:
                    return MoveDirection.Down;
                case ConsoleKey.LeftArrow:
                    return MoveDirection.Left;
                case ConsoleKey.RightArrow:
                    return MoveDirection.Right;
                default:
                    return MoveDirection.None;
            }
        }

        // Function to move the snake based on the direction
       public static Position MoveSnake(Queue<Position> snake, MoveDirection direction, int width, int height)
        {
            Position head = snake.Peek();
            Position nextHead = new Position(head.X, head.Y);

            // Calculate the position of the new head based on the direction
            switch (direction)
            {
                case MoveDirection.Up:
                    nextHead.Y = (head.Y - 1 + height) % height;
                    break;
                case MoveDirection.Down:
                    nextHead.Y = (head.Y + 1) % height;
                    break;
                case MoveDirection.Left:
                    nextHead.X = (head.X - 1 + width) % width;
                    break;
                case MoveDirection.Right:
                    nextHead.X = (head.X + 1) % width;
                    break;
                default:
                    break;
            }

            return nextHead;
        }

        // Function to check if the snake collides with walls or itself
        public static bool IsCollision(Position head, Queue<Position> snake, int width, int height)
        {
            // Check collision with walls
            if (head.X < 0 || head.X >= width || head.Y < 0 || head.Y >= height)
                return true;

            // Check collision with itself
            foreach (var segment in snake)
            {
                if (head.Equals(segment) && !head.Equals(snake.Peek()))
                    return true;
            }

            return false;
        }
    }
}

