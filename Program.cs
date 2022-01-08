using System;
using System.Diagnostics;
using System.Threading;

namespace IsAAAc
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IsAAAc";

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            Console.SetWindowPosition(0, 0);

            Console.WindowWidth = IsAAAc.Width + 2;
            Console.WindowHeight = IsAAAc.Height + 2;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();

            IsAAAc.PrintRoom(1, 1, true, true, true, true);

            CellType[,] playField = new CellType[IsAAAc.Height - 2, IsAAAc.Width - 2];

            IsAAAc.PlaceObstacle(playField, 10);
            IsAAAc.PlacePlayer(playField);

            IsAAAc.PrintPlayField(playField);

            Stopwatch sW = new();

            while (true)
            {
                int frameTime = 33; // ms.

                sW.Restart();

                if (Console.KeyAvailable)
                {
                    ConsoleKey cK = Console.ReadKey(true).Key;
                    ThreadPool.QueueUserWorkItem((_) => { while (Console.KeyAvailable) Console.ReadKey(true); });

                    switch (cK)
                    {
                        case ConsoleKey.W:
                        {
                            IsAAAc.MovePlayer(playField, MoveType.Up);

                            break;
                        }

                        case ConsoleKey.S:
                        {
                            IsAAAc.MovePlayer(playField, MoveType.Down);

                            break;
                        }

                        case ConsoleKey.A:
                        {
                            IsAAAc.MovePlayer(playField, MoveType.Left);

                            break;
                        }

                        case ConsoleKey.D:
                        {
                            IsAAAc.MovePlayer(playField, MoveType.Right);

                            break;
                        }

                        case ConsoleKey.Escape:
                        {
                            frameTime = 0;

                            return;
                        }
                    }

                    IsAAAc.PrintPlayField(playField);
                }

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime - (int)sW.ElapsedMilliseconds));
            }
        }
    }

    public enum CellType { Obstacle = -2, Bullet, Empty, Player }

    public enum MoveType { Up, Right, Down, Left }

    public class IsAAAc
    {
        // WindowWidth (max): 240; WindowHeight (max): 63 (@ 1920px * 1080px; Windows 10).

        public const int Width = 60;
        public const int Height = Width / 3; // Rectangle.
        //public const int Height = Width / 2; // Square.

        public static void PrintRoom(int xOffset, int yOffset, bool up, bool right, bool down, bool left)
        {
            const ConsoleColor WallColor = ConsoleColor.Gray;

            const int DoorWidth = 8;
            const int DoorHeight = DoorWidth / 2;

            Write(    "╔" + new String('═', Width - 2) + "╗", xOffset, yOffset, WallColor);
            for (var i = 1; i <= Height - 2; i++)
            {
                Write("║" + new String(' ', Width - 2) + "║", xOffset, yOffset + i, WallColor);
            }
            Write(    "╚" + new String('═', Width - 2) + "╝", xOffset, yOffset + Height - 1, WallColor);

            if (up)
            {
                Write(new String(' ', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Write(new String(' ', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset + Height - 1, ConsoleColor.Black);
            }

            if (left)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Write(" ", xOffset, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }

            if (right)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Write(" ", xOffset + Width - 1, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }
        }

        public static void PlaceObstacle(CellType[,] playField, int count)
        {
            Random rnd = new();

            while (count != 0)
            {
                int y = rnd.Next(0, playField.GetLength(0));
                int x = rnd.Next(0, playField.GetLength(1));

                if (playField[y, x] == CellType.Empty)
                {
                    playField[y, x] = CellType.Obstacle;

                    count--;
                }
            }
        }

        public static void PlacePlayer(CellType[,] playField)
        {
            Random rnd = new();

            while (true)
            {
                int y = rnd.Next(0, playField.GetLength(0));
                int x = rnd.Next(0, playField.GetLength(1));

                if (playField[y, x] == CellType.Empty)
                {
                    playField[y, x] = CellType.Player;

                    return;
                }
            }
        }

        public static void MovePlayer(CellType[,] playField, MoveType moveType)
        {
            const int yInc = 1, xInc = 1;

            (int y, int x) = FindPlayer(playField);

            int yNew = y, xNew = x;

            switch (moveType)
            {
                case MoveType.Up:
                {
                    yNew = y - yInc;

                    break;
                }

                case MoveType.Down:
                {
                    yNew = y + yInc;

                    break;
                }

                case MoveType.Left:
                {
                    xNew = x - xInc;

                    break;
                }

                case MoveType.Right:
                {
                    xNew = x + xInc;

                    break;
                }
            }

            if (yNew >= 0 && yNew < playField.GetLength(0))
            {
                if (xNew >= 0 && xNew < playField.GetLength(1))
                {
                    if (playField[yNew, xNew] == CellType.Empty)
                    {
                        playField[y, x] = CellType.Empty;
                        playField[yNew, xNew] = CellType.Player;
                    }
                }
            }
        }

        private static (int y, int x) FindPlayer(CellType[,] playField)
        {
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    if (playField[y, x] == CellType.Player)
                    {
                        return (y, x);
                    }
                }
            }

            throw new Exception();
        }

        private static CellType[,] _oldPlayField;

        public static void PrintPlayField(CellType[,] playField)
        {
            _oldPlayField ??= new CellType[playField.GetLength(0), playField.GetLength(1)];

            lock (_oldPlayField)
            {
                for (int y = 0; y < playField.GetLength(0); y++)
                {
                    for (int x = 0; x < playField.GetLength(1); x++)
                    {
                        if (playField[y, x] != _oldPlayField[y, x])
                        {
                            _oldPlayField[y, x] = playField[y, x];

                            switch (playField[y, x])
                            {
                                case CellType.Obstacle:
                                {
                                    Write("X", x + 2, y + 2, ConsoleColor.Gray);

                                    break;
                                }

                                case CellType.Bullet:
                                {
                                    Write(".", x + 2, y + 2, ConsoleColor.Red);

                                    break;
                                }

                                case CellType.Empty:
                                {
                                    Write(" ", x + 2, y + 2, ConsoleColor.Black);

                                    break;
                                }

                                case CellType.Player:
                                {
                                    Write("♦", x + 2, y + 2, ConsoleColor.Cyan);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Write(string str, int left, int top, ConsoleColor fColor = ConsoleColor.Gray, ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            Console.ResetColor();
        }
    }
}
