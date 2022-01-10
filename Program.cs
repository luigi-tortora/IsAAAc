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

            Console.WindowWidth = Room.Width + 2;
            Console.WindowHeight = Room.Height + 2;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();

            Room.Print(1, 1, true, true, true, true);

            CellInfo[,] playField = new CellInfo[Room.Height - 2, Room.Width - 2];
            PlayField.Init(playField);

            PlayField.PlaceObstacle(playField, count: 10);
            PlayField.PlacePlayer(playField, id: 0);

            PlayField.Print(playField);

            Stopwatch sW = new();

            while (true)
            {
                int frameTime = 17; // ms.

                sW.Restart();

                if (Console.KeyAvailable)
                {
                    ConsoleKey cK = Console.ReadKey(true).Key;
                    ThreadPool.QueueUserWorkItem((_) => { while (Console.KeyAvailable && Console.ReadKey(true).Key == cK); });

                    switch (cK)
                    {
                        case ConsoleKey.W:
                        {
                            PlayField.MovePlayer(playField, id: 0, Direction.Up);

                            break;
                        }

                        case ConsoleKey.S:
                        {
                            PlayField.MovePlayer(playField, id: 0, Direction.Down);

                            break;
                        }

                        case ConsoleKey.A:
                        {
                            PlayField.MovePlayer(playField, id: 0, Direction.Left);

                            break;
                        }

                        case ConsoleKey.D:
                        {
                            PlayField.MovePlayer(playField, id: 0, Direction.Right);

                            break;
                        }

                        case ConsoleKey.Escape:
                        {
                            frameTime = 0;

                            return;
                        }
                    }

                    PlayField.Print(playField);
                }

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime - (int)sW.ElapsedMilliseconds));
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

    public enum CellType { Empty, Obstacle, Bullet, Player }

    public enum Direction { None, Up, Right, Down, Left }

    public class CellInfo : IEquatable<CellInfo>
    {
        public int Id { get; set; }
        public CellType CellType { get; set; }
        public Direction Direction { get; set; }

        public CellInfo(int id = 0, CellType cellType = CellType.Empty, Direction direction = Direction.None)
        {
            Id = id;
            CellType = cellType;
            Direction = direction;
        }

        public void Set(int id = 0, CellType cellType = CellType.Empty, Direction direction = Direction.None)
        {
            Id = id;
            CellType = cellType;
            Direction = direction;
        }

        public void Set(CellInfo cellInfo)
        {
            Id = cellInfo.Id;
            CellType = cellInfo.CellType;
            Direction = cellInfo.Direction;
        }

        public override bool Equals(object obj) => obj is CellInfo cellInfo && Equals(cellInfo);
        public bool Equals(CellInfo cellInfo) => Id == cellInfo.Id && CellType == cellInfo.CellType && Direction == cellInfo.Direction;
        public override int GetHashCode() => HashCode.Combine(Id, CellType, Direction);

        public static bool operator ==(CellInfo lhs, CellInfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(CellInfo lhs, CellInfo rhs) => !lhs.Equals(rhs);
    }

    public class Room
    {
        public const int Width = 60;
        public const int Height = Width / 3; // Rectangle.
        //public const int Height = Width / 2; // Square.

        public static void Print(int xOffset, int yOffset, bool up, bool right, bool down, bool left)
        {
            const ConsoleColor WallColor = ConsoleColor.Gray;

            const int DoorWidth = 8;
            const int DoorHeight = DoorWidth / 2;

            Program.Write(    "╔" + new String('═', Width - 2) + "╗", xOffset, yOffset, WallColor);
            for (var i = 1; i <= Height - 2; i++)
            {
                Program.Write("║" + new String(' ', Width - 2) + "║", xOffset, yOffset + i, WallColor);
            }
            Program.Write(    "╚" + new String('═', Width - 2) + "╝", xOffset, yOffset + Height - 1, WallColor);

            if (up)
            {
                Program.Write(new String(' ', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Program.Write(new String(' ', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset + Height - 1, ConsoleColor.Black);
            }

            if (left)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Program.Write(" ", xOffset, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }

            if (right)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Program.Write(" ", xOffset + Width - 1, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }
        }
    }

    public class PlayField
    {
        public static void Init(CellInfo[,] playField)
        {
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    playField[y, x] = new();
                }
            }
        }

        public static void PlaceObstacle(CellInfo[,] playField, int count)
        {
            Random rnd = new();

            while (count != 0)
            {
                int y = rnd.Next(0, playField.GetLength(0));
                int x = rnd.Next(0, playField.GetLength(1));

                if (playField[y, x].CellType == CellType.Empty)
                {
                    playField[y, x].Set(cellType: CellType.Obstacle);

                    count--;
                }
            }
        }

        public static void PlacePlayer(CellInfo[,] playField, int id)
        {
            Random rnd = new();

            while (true)
            {
                int y = rnd.Next(0, playField.GetLength(0));
                int x = rnd.Next(0, playField.GetLength(1));

                if (playField[y, x].CellType == CellType.Empty)
                {
                    playField[y, x].Set(id, CellType.Player);

                    return;
                }
            }
        }

        public static void MovePlayer(CellInfo[,] playField, int id, Direction direction)
        {
            const int yInc = 1, xInc = 1;

            (int y, int x) = FindPlayer(playField, id);

            int yNew = y, xNew = x;

            switch (direction)
            {
                case Direction.Up:
                {
                    yNew = y - yInc;

                    break;
                }

                case Direction.Down:
                {
                    yNew = y + yInc;

                    break;
                }

                case Direction.Left:
                {
                    xNew = x - xInc;

                    break;
                }

                case Direction.Right:
                {
                    xNew = x + xInc;

                    break;
                }
            }

            if (yNew >= 0 && yNew < playField.GetLength(0))
            {
                if (xNew >= 0 && xNew < playField.GetLength(1))
                {
                    if (playField[yNew, xNew].CellType == CellType.Empty)
                    {
                        playField[y, x].Set();

                        playField[yNew, xNew].Set(id, CellType.Player);
                    }
                }
            }
        }

        private static (int y, int x) FindPlayer(CellInfo[,] playField, int id)
        {
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    if (playField[y, x].Id == id)
                    {
                        if (playField[y, x].CellType == CellType.Player)
                        {
                            return (y, x);
                        }
                    }
                }
            }

            throw new Exception();
        }

        private static CellInfo[,] _oldPlayField;

        public static void Print(CellInfo[,] playField)
        {
            if (_oldPlayField == null)
            {
                _oldPlayField = new CellInfo[playField.GetLength(0), playField.GetLength(1)];
                PlayField.Init(_oldPlayField);
            }

            lock (_oldPlayField)
            {
                for (int y = 0; y < playField.GetLength(0); y++)
                {
                    for (int x = 0; x < playField.GetLength(1); x++)
                    {
                        if (playField[y, x] != _oldPlayField[y, x])
                        {
                            _oldPlayField[y, x].Set(playField[y, x]);

                            switch (playField[y, x].CellType)
                            {
                                case CellType.Obstacle:
                                {
                                    Program.Write("X", x + 2, y + 2, ConsoleColor.Gray);

                                    break;
                                }

                                case CellType.Bullet:
                                {
                                    Program.Write(".", x + 2, y + 2, ConsoleColor.Red);

                                    break;
                                }

                                case CellType.Empty:
                                {
                                    Program.Write(" ", x + 2, y + 2, ConsoleColor.Black);

                                    break;
                                }

                                case CellType.Player:
                                {
                                    Program.Write("♦", x + 2, y + 2, ConsoleColor.Cyan);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
