using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Snake
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class SnakeGameForm : Form
    {
        private Timer gameTimer;
        private Queue<Position> snake;
        private MoveDirection direction;
        private Position food;
        private int width = 20;
        private int height = 20;
        private int cellSize = 20;
        private Random random = new Random();

        public SnakeGameForm()
        {
            this.Text = "Snake Game";
            this.Width = width * cellSize + 16;
            this.Height = height * cellSize + 39;
            this.DoubleBuffered = true;

            gameTimer = new Timer();
            gameTimer.Interval = 100;
            gameTimer.Tick += GameTimer_Tick;

            this.KeyDown += SnakeGameForm_KeyDown;

            StartNewGame();
        }

        private void StartNewGame()
        {
            snake = new Queue<Position>();
            snake.Enqueue(new Position(width / 2, height / 2));
            direction = MoveDirection.Right;
            PlaceFood();
            gameTimer.Start();
        }

        private void PlaceFood()
        {
            food = new Position(random.Next(width), random.Next(height));
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            Invalidate();
        }

        private void MoveSnake()
        {
            Position head = snake.Peek();
            Position nextHead = new Position(head.X, head.Y);

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
            }

            if (nextHead.X == food.X && nextHead.Y == food.Y)
            {
                snake.Enqueue(nextHead);
                PlaceFood();
            }
            else
            {
                snake.Enqueue(nextHead);
                snake.Dequeue();
            }

            if (snake.Count > 1)
            {
                foreach (var pos in snake)
                {
                    if (pos.X == nextHead.X && pos.Y == nextHead.Y && pos != nextHead)
                    {
                        StartNewGame();
                        return;
                    }
                }
            }
        }

        private void SnakeGameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (direction != MoveDirection.Down) direction = MoveDirection.Up;
                    break;
                case Keys.S:
                    if (direction != MoveDirection.Up) direction = MoveDirection.Down;
                    break;
                case Keys.A:
                    if (direction != MoveDirection.Right) direction = MoveDirection.Left;
                    break;
                case Keys.D:
                    if (direction != MoveDirection.Left) direction = MoveDirection.Right;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            foreach (var pos in snake)
            {
                g.FillRectangle(Brushes.Green, pos.X * cellSize, pos.Y * cellSize, cellSize, cellSize);
            }

            g.FillRectangle(Brushes.Red, food.X * cellSize, food.Y * cellSize, cellSize, cellSize);
        }
    }
}
