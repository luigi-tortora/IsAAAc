using System;
using System.Diagnostics;
using System.Threading;

namespace IsAAAc
{
    public class Program
    {
        public const int WindowWidth = 80; // Even.
        public const int WindowHeight = 40; // Even.

        public static void Main(string[] args)
        {
            Console.Title = "IsAAAc";

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            Console.TreatControlCAsInput = true;

            Console.SetWindowPosition(0, 0);

            Console.WindowWidth = WindowWidth;
            Console.WindowHeight = WindowHeight;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();

            Room room = new();

            room.Print(true, true, true, true);

            PlayField playField = new(room);

            playField.PlaceObstacle(count: 10);
            playField.PlacePlayer(id: 0);

            playField.Print();

            Stopwatch sW = new();

            bool exit = false;

            while (!exit)
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
                            playField.MovePlayer(id: 0, Direction.Up);

                            break;
                        }

                        case ConsoleKey.S:
                        {
                            playField.MovePlayer(id: 0, Direction.Down);

                            break;
                        }

                        case ConsoleKey.A:
                        {
                            playField.MovePlayer(id: 0, Direction.Left);

                            break;
                        }

                        case ConsoleKey.D:
                        {
                            playField.MovePlayer(id: 0, Direction.Right);

                            break;
                        }

                        case ConsoleKey.Escape:
                        {
                            exit = true;

                            frameTime = 0;

                            break;
                        }
                    }

                    playField.Print();
                }

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime - (int)sW.ElapsedMilliseconds));
            }

            Console.OutputEncoding = System.Text.Encoding.Default;
            Console.CursorVisible = true;

            Console.TreatControlCAsInput = false;

            Console.Clear();
            Console.ResetColor();
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
        const int DoorWidth = 8;
        const int DoorHeight = DoorWidth / 2;

        public static int Id { get; set; }

        public int Left { get; set; }
        public int Top { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Room()
        {
            Id++;

            Generate();
        }

        private void Generate()
        {
            Random rnd = new();

            do
            {
                Width = rnd.Next(DoorWidth * 3, Program.WindowWidth - 1);
                Height = rnd.Next(DoorHeight * 3, Program.WindowHeight - 1);
            }
            while (Width % 2 != 0 || Height % 2 != 0);

            Left = (Program.WindowWidth - Width) / 2;
            Top = (Program.WindowHeight - Height) / 2;
        }

        public void Print(bool topDoor, bool rightDoor, bool bottomDoor, bool leftDoor) // TODO: Mask or flags enum.
        {
            const ConsoleColor WallColor = ConsoleColor.Gray;

            Program.Write(    "╔" + new String('═', Width - 2) + "╗", Left, Top, WallColor);
            for (var i = 1; i <= Height - 2; i++)
            {
                Program.Write("║" + new String(' ', Width - 2) + "║", Left, Top + i, WallColor);
            }
            Program.Write(    "╚" + new String('═', Width - 2) + "╝", Left, Top + Height - 1, WallColor);

            if (topDoor)
            {
                Program.Write(new String(' ', DoorWidth), Left + Width / 2 - DoorWidth / 2, Top, ConsoleColor.Black);
            }

            if (bottomDoor)
            {
                Program.Write(new String(' ', DoorWidth), Left + Width / 2 - DoorWidth / 2, Top + Height - 1, ConsoleColor.Black);
            }

            if (leftDoor)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Program.Write(" ", Left, Top + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }

            if (rightDoor)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Program.Write(" ", Left + Width - 1, Top + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }
        }
    }

    public class PlayField
    {
        CellInfo[,] _playField;

        Room _room;

        public PlayField(Room room)
        {
            _playField = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playField);

            _room = room;
        }

        private static void Init(CellInfo[,] playField)
        {
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    playField[y, x] = new();
                }
            }
        }

        public void PlaceObstacle(int count)
        {
            Random rnd = new();

            while (count != 0)
            {
                int y = rnd.Next(0, _playField.GetLength(0));
                int x = rnd.Next(0, _playField.GetLength(1));

                if (_playField[y, x].CellType == CellType.Empty)
                {
                    _playField[y, x].Set(cellType: CellType.Obstacle);

                    count--;
                }
            }
        }

        public void PlacePlayer(int id)
        {
            Random rnd = new();

            while (true)
            {
                int y = rnd.Next(0, _playField.GetLength(0));
                int x = rnd.Next(0, _playField.GetLength(1));

                if (_playField[y, x].CellType == CellType.Empty)
                {
                    _playField[y, x].Set(id, CellType.Player);

                    return;
                }
            }
        }

        public void MovePlayer(int id, Direction direction)
        {
            (int y, int x) = FindPlayer(id);

            int yNew = y, xNew = x;

            switch (direction)
            {
                case Direction.Up:
                {
                    yNew--;

                    break;
                }

                case Direction.Down:
                {
                    yNew++;

                    break;
                }

                case Direction.Left:
                {
                    xNew--;

                    break;
                }

                case Direction.Right:
                {
                    xNew++;

                    break;
                }
            }

            if (yNew >= 0 && yNew < _playField.GetLength(0))
            {
                if (xNew >= 0 && xNew < _playField.GetLength(1))
                {
                    if (_playField[yNew, xNew].CellType == CellType.Empty)
                    {
                        _playField[y, x].Set();

                        _playField[yNew, xNew].Set(id, CellType.Player);
                    }
                }
            }
        }

        private (int y, int x) FindPlayer(int id)
        {
            for (int y = 0; y < _playField.GetLength(0); y++)
            {
                for (int x = 0; x < _playField.GetLength(1); x++)
                {
                    if (_playField[y, x].Id == id)
                    {
                        if (_playField[y, x].CellType == CellType.Player)
                        {
                            return (y, x);
                        }
                    }
                }
            }

            throw new Exception(); // TODO: TryFindPlayer().
        }

        private CellInfo[,] _oldPlayField;

        public void Print()
        {
            if (_oldPlayField == null)
            {
                _oldPlayField = new CellInfo[_playField.GetLength(0), _playField.GetLength(1)];
                Init(_oldPlayField);
            }

            lock (_oldPlayField)
            {
                for (int y = 0; y < _playField.GetLength(0); y++)
                {
                    for (int x = 0; x < _playField.GetLength(1); x++)
                    {
                        if (_playField[y, x] != _oldPlayField[y, x])
                        {
                            _oldPlayField[y, x].Set(_playField[y, x]);

                            switch (_playField[y, x].CellType)
                            {
                                case CellType.Obstacle:
                                {
                                    Program.Write("X", x + _room.Left + 1, y + _room.Top + 1, ConsoleColor.Gray);

                                    break;
                                }

                                case CellType.Bullet:
                                {
                                    Program.Write(".", x + _room.Left + 1, y + _room.Top + 1, ConsoleColor.Red);

                                    break;
                                }

                                case CellType.Empty:
                                {
                                    Program.Write(" ", x + _room.Left + 1, y + _room.Top + 1, ConsoleColor.Black);

                                    break;
                                }

                                case CellType.Player:
                                {
                                    Program.Write("♦", x + _room.Left + 1, y + _room.Top + 1, ConsoleColor.Cyan);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class Player // TODO: .
    {
        public const int MaxFireableBullets = 10;

        public int Id { get; set; }
        public int Health { get; set; }
        public int FiredBullets { get; set; }

        public Player(int id, int health)
        {
            Id = id;
            Health = health;
        }
    }
}
