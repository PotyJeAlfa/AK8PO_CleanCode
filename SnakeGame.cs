using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static void Main()
        {
            var game = new SnakeGame(Console.WindowWidth = 32, Console.WindowHeight = 16);
            game.Run();
        }
    }

    public class SnakeGame
    {
        private const int InitialSnakeLength = 5;
        private readonly GameBoard _board;
        private readonly Snake _snake;
        private readonly Berry _berry;
        private Direction _direction = Direction.Right;
        private bool _isGameOver;

        public SnakeGame(int width, int height)
        {
            _board = new GameBoard(width, height);
            _snake = new Snake(width / 2, height / 2, InitialSnakeLength);
            _berry = new Berry(_board);
        }

        public void Run()
        {
            while (!_isGameOver)
            {
                _board.Clear();
                _board.DrawBorder();

                HandleInput();
                _snake.Move(_direction);

                if (_snake.HasCollidedWithWall(_board) || _snake.HasCollidedWithItself())
                {
                    _isGameOver = true;
                    break;
                }

                if (_snake.Head.Equals(_berry.Position))
                {
                    _snake.Grow();
                    _berry.Respawn(_board);
                }

                _board.DrawPixel(_berry.Position, ConsoleColor.Cyan);
                _board.DrawSnake(_snake);
                Thread.Sleep(150);
            }

            ShowGameOver();
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey(true).Key;

            _direction = key switch
            {
                ConsoleKey.UpArrow when _direction != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when _direction != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when _direction != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when _direction != Direction.Left => Direction.Right,
                _ => _direction
            };
        }

        private void ShowGameOver()
        {
            Console.SetCursorPosition(_board.Width / 5, _board.Height / 2);
            Console.WriteLine($"Game over, Score: {_snake.Length}");
        }
    }

    public class Snake
    {
        private readonly List<Pixel> _body = new();
        private int _growPending;

        public Snake(int startX, int startY, int initialLength)
        {
            for (int i = 0; i < initialLength; i++)
            {
                _body.Add(new Pixel(startX - i, startY, ConsoleColor.Green));
            }
        }

        public Pixel Head => _body[^1];
        public int Length => _body.Count;

        public void Move(Direction direction)
        {
            Pixel nextHead = direction switch
            {
                Direction.Up => new Pixel(Head.X, Head.Y - 1, ConsoleColor.Red),
                Direction.Down => new Pixel(Head.X, Head.Y + 1, ConsoleColor.Red),
                Direction.Left => new Pixel(Head.X - 1, Head.Y, ConsoleColor.Red),
                Direction.Right => new Pixel(Head.X + 1, Head.Y, ConsoleColor.Red),
                _ => Head
            };

            _body.Add(nextHead);
            if (_growPending > 0)
                _growPending--;
            else
                _body.RemoveAt(0);
        }

        public void Grow() => _growPending++;

        public bool HasCollidedWithWall(GameBoard board) =>
            Head.X <= 0 || Head.X >= board.Width - 1 || Head.Y <= 0 || Head.Y >= board.Height - 1;

        public bool HasCollidedWithItself()
        {
            for (int i = 0; i < _body.Count - 1; i++)
            {
                if (_body[i].Equals(Head))
                    return true;
            }
            return false;
        }

        public IEnumerable<Pixel> GetBody() => _body;
    }

    public class Berry
    {
        private static readonly Random _random = new();
        public Pixel Position { get; private set; }

        public Berry(GameBoard board)
        {
            Respawn(board);
        }

        public void Respawn(GameBoard board)
        {
            Position = new Pixel(
                _random.Next(1, board.Width - 1),
                _random.Next(1, board.Height - 1),
                ConsoleColor.Cyan
            );
        }
    }

    public class GameBoard
    {
        public int Width { get; }
        public int Height { get; }

        public GameBoard(int width, int height)
        {
            Width = width;
            Height = height;
            Console.SetWindowSize(width, height);
        }

        public void Clear() => Console.Clear();

        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int x = 0; x < Width; x++)
            {
                DrawAt(x, 0, '■');
                DrawAt(x, Height - 1, '■');
            }

            for (int y = 0; y < Height; y++)
            {
                DrawAt(0, y, '■');
                DrawAt(Width - 1, y, '■');
            }
        }

        public void DrawSnake(Snake snake)
        {
            foreach (var segment in snake.GetBody())
            {
                DrawPixel(segment);
            }
        }

        public void DrawPixel(Pixel pixel)
        {
            Console.ForegroundColor = pixel.Color;
            DrawAt(pixel.X, pixel.Y, '■');
        }

        private void DrawAt(int x, int y, char symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }
    }

    public record Pixel(int X, int Y, ConsoleColor Color);

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
