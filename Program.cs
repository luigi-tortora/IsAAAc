using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace IsAAAc
{
    public class Program
    {
        public const string Title = "IsAAAc";

        public const int WindowWidth = 80; // Even.
        public const int WindowHeight = 40; // Even.

        public static void Main(string[] args)
        {
            Console.Title = Title;

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

            room.Print((DoorsState)(new Random().Next(0, 16)));

            PlayField playField = new(room);

            int playersCount = new Random().Next(2, PlayField.MaxPlayersCount + 1);

            playField.TryPlaceObstacle(count: playersCount * PlayField.ObstaclesPerPlayer);

            for (int id = 0; id < playersCount; id++)
            {
                if (id == 0)
                {
                    playField.TryPlacePlayer(new(id, health: 15));
                }
                else
                {
                    playField.TryPlacePlayer(new(id, health: 5));
                }
            }

            PrintPlayersStats(playField.Players);
            playField.Print();

            const int FrameTime = 20 * 3; // ms.

            int frameTime = FrameTime;

            Stopwatch sW = new();

            bool exit = false;

            while (true)
            {
                if (exit || playField.GetGameOverState() != GameOver.None)
                {
                    break;
                }

                sW.Restart();

                for (int id = playersCount - 1; id > 0; id--)
                {
                    Action action = playField.Players[id].GetNextAction();
                    Direction direction = playField.Players[id].GetNextDirection(playField);

                    switch (action)
                    {
                        case Action.Move:
                        {
                            playField.TryMovePlayerById(id, direction);

                            break;
                        }

                        case Action.Fire:
                        {
                            playField.TryFireBulletById(id, direction);

                            break;
                        }
                    }
                }

                PrintPlayersStats(playField.Players, frameTime);
                playField.Print();

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));

                if (exit || playField.GetGameOverState() != GameOver.None)
                {
                    break;
                }

                sW.Restart();

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cKI = Console.ReadKey(true);
                    ThreadPool.QueueUserWorkItem((_) => { while (Console.KeyAvailable && Console.ReadKey(true) == cKI); });

                    switch (cKI.Key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            if (playField.TryMovePlayerById(id: 0, Direction.Up))
                            {
                                BeepAsync(800, 1);
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            if (playField.TryMovePlayerById(id: 0, Direction.Down))
                            {
                                BeepAsync(800, 1);
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            if (playField.TryMovePlayerById(id: 0, Direction.Left))
                            {
                                BeepAsync(800, 1);
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            if (playField.TryMovePlayerById(id: 0, Direction.Right))
                            {
                                BeepAsync(800, 1);
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.UpArrow:
                        {
                            if (playField.TryFireBulletById(id: 0, Direction.Up))
                            {
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.DownArrow:
                        {
                            if (playField.TryFireBulletById(id: 0, Direction.Down))
                            {
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.LeftArrow:
                        {
                            if (playField.TryFireBulletById(id: 0, Direction.Left))
                            {
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.RightArrow:
                        {
                            if (playField.TryFireBulletById(id: 0, Direction.Right))
                            {
                                BeepAsync(800, 1);
                            }

                            break;
                        }

                        case ConsoleKey.OemPlus when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            frameTime = Math.Clamp(frameTime - 3, 3, 999);

                            break;
                        }

                        case ConsoleKey.OemMinus when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            frameTime = Math.Clamp(frameTime + 3, 3, 999);

                            break;
                        }

                        case ConsoleKey.NumPad0 when cKI.Modifiers.HasFlag(ConsoleModifiers.Control):
                        {
                            frameTime = FrameTime;

                            break;
                        }

                        case ConsoleKey.Escape:
                        {
                            exit = true;

                            break;
                        }
                    }
                }

                PrintPlayersStats(playField.Players, frameTime);
                playField.Print();

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));

                if (exit || playField.GetGameOverState() != GameOver.None)
                {
                    break;
                }

                sW.Restart();

                playField.UpdateBulletsState();

                PrintPlayersStats(playField.Players, frameTime);
                playField.Print();

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));
            }

            if (!exit)
            {
                playField.RemoveAllBullets();

                Console.Title = $"{Title} [GameOver: {playField.GetGameOverState()}]";
                playField.Print();

                while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }

            Console.Title = Title;

            Console.OutputEncoding = System.Text.Encoding.Default;
            Console.CursorVisible = true;

            Console.TreatControlCAsInput = false;

            Console.Clear();
            Console.ResetColor();
        }

        public static void PrintPlayersStats(List<Player> players, int frameTime = 0)
        {
            int fps = frameTime != 0 ? (int)(1f / ((float)frameTime / 1000f)) : 0;

            int playerHealth = 0;
            int enemiesHealth = 0;

            int enemies = 0;

            foreach (Player player in players)
            {
                if (player.Id == 0)
                {
                    playerHealth = player.Health;
                }
                else
                {
                    enemiesHealth += player.Health;

                    if (player.Health != 0)
                    {
                        enemies++;
                    }
                }
            }

            Console.Title = $"{Title} [FPS: {fps}] [Player Health: {playerHealth}] [Enemies Health: {enemiesHealth}] [Enemies: {enemies}]";
        }

        public static void Write(string str, int left, int top, ConsoleColor fColor = ConsoleColor.Gray, ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            Console.ResetColor();
        }

        private static void BeepAsync(int frequency = 800, int duration = 200)
        {
            ThreadPool.QueueUserWorkItem((_) => Console.Beep(frequency, duration));
        }
    }

    [Flags]
    public enum DoorsState
    {
        None = 0,

        Up    = 1 << 0,
        Right = 1 << 1,
        Down  = 1 << 2,
        Left  = 1 << 3
    }

    public enum GameOver { None, YouWin, YouLose }

    public enum CellType { Empty, Obstacle, Bullet, Player }

    public enum Action { None, Stay, Move, Fire }
    public enum Direction { None, Up, Right, Down, Left }

    public class CellInfo : IEquatable<CellInfo>
    {
        public int Id { get; private set; }

        public CellType CellType { get; private set; }

        public Direction BulletDirection { get; private set; }
        public int BulletDistance { get; set; }

        public CellInfo(
            int id = 0,
            CellType cellType = CellType.Empty,
            Direction bulletDirection = Direction.None,
            int bulletDistance = 0)
        {
            Id = id;

            CellType = cellType;

            BulletDirection = bulletDirection;
            BulletDistance = bulletDistance;
        }

        public void Set(
            int id = 0,
            CellType cellType = CellType.Empty,
            Direction bulletDirection = Direction.None,
            int bulletDistance = 0)
        {
            Id = id;

            CellType = cellType;

            BulletDirection = bulletDirection;
            BulletDistance = bulletDistance;
        }

        public void Set(CellInfo cellInfo)
        {
            Id = cellInfo.Id;

            CellType = cellInfo.CellType;

            BulletDirection = cellInfo.BulletDirection;
            BulletDistance = cellInfo.BulletDistance;
        }

        public override bool Equals(object obj) => obj is CellInfo cellInfo && Equals(cellInfo);
        public bool Equals(CellInfo cellInfo) =>
            Id == cellInfo.Id &&
            CellType == cellInfo.CellType &&
            BulletDirection == cellInfo.BulletDirection &&
            BulletDistance == cellInfo.BulletDistance;
        public override int GetHashCode() => HashCode.Combine(Id, CellType, BulletDirection, BulletDistance);

        public static bool operator ==(CellInfo lhs, CellInfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(CellInfo lhs, CellInfo rhs) => !lhs.Equals(rhs);
    }

    public class Room
    {
        public const int DoorWidth = 8;
        public const int DoorHeight = 4;

        public static int Id { get; private set; }

        public int Left { get; private set; }
        public int Top { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly Random _rndGenerate;

        public Room()
        {
            Id++;

            _rndGenerate = new();

            Generate();
        }

        private void Generate()
        {
            do
            {
                Width = _rndGenerate.Next(DoorWidth * 3, Program.WindowWidth - 1);
                Height = _rndGenerate.Next(DoorHeight * 3, Program.WindowHeight - 1);
            }
            while (Width % 2 != 0 || Height % 2 != 0);

            Left = (Program.WindowWidth - Width) / 2;
            Top = (Program.WindowHeight - Height) / 2;
        }

        public void Print(DoorsState doorsState)
        {
            const ConsoleColor WallColor = ConsoleColor.Gray;

            Program.Write(    "╔" + new String('═', Width - 2) + "╗", Left, Top, WallColor);
            for (var i = 1; i <= Height - 2; i++)
            {
                Program.Write("║" + new String(' ', Width - 2) + "║", Left, Top + i, WallColor);
            }
            Program.Write(    "╚" + new String('═', Width - 2) + "╝", Left, Top + Height - 1, WallColor);

            if (doorsState.HasFlag(DoorsState.Up))
            {
                Program.Write(new String(' ', DoorWidth), Left + Width / 2 - DoorWidth / 2, Top, ConsoleColor.Black);
            }

            if (doorsState.HasFlag(DoorsState.Down))
            {
                Program.Write(new String(' ', DoorWidth), Left + Width / 2 - DoorWidth / 2, Top + Height - 1, ConsoleColor.Black);
            }

            if (doorsState.HasFlag(DoorsState.Left))
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Program.Write(" ", Left, Top + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }

            if (doorsState.HasFlag(DoorsState.Right))
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
        public const int ObstaclesPerPlayer = 2;

        public const int MaxPlayersCount = 1 + 15;

        public List<Player> Players { get; }

        private readonly CellInfo[,] _playField;
        private readonly CellInfo[,] _playFieldOld;

        private readonly Room _room;

        private readonly Random _rndPlace;

        public PlayField(Room room)
        {
            _playField = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playField);

            _playFieldOld = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playFieldOld);

            _room = room;

            _rndPlace = new();

            Players = new();
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

        public bool TryPlaceObstacle(int count)
        {
            int attempts = _playField.GetLength(0) * _playField.GetLength(1) * count;

            while (count != 0 && attempts-- != 0)
            {
                int y = _rndPlace.Next(0, _playField.GetLength(0));
                int x = _rndPlace.Next(0, _playField.GetLength(1));

                if (_playField[y, x].CellType == CellType.Empty)
                {
                    _playField[y, x].Set(cellType: CellType.Obstacle);

                    count--;
                }
            }

            return attempts != -1;
        }

        public bool TryPlacePlayer(Player player)
        {
            int attempts = _playField.GetLength(0) * _playField.GetLength(1);

            while (attempts-- != 0)
            {
                int y = _rndPlace.Next(0, _playField.GetLength(0));
                int x = _rndPlace.Next(0, _playField.GetLength(1));

                if (_playField[y, x].CellType == CellType.Empty)
                {
                    _playField[y, x].Set(player.Id, CellType.Player);

                    Players.Add(player);

                    break;
                }
            }

            return attempts != -1;
        }

        public bool TryGetPointingDirectionsBySrcAndDstId(int srcId, int dstId, out List<Direction> pointingDirections)
        {
            if (srcId == dstId || Players[srcId].Health == 0 || Players[dstId].Health == 0)
            {
                pointingDirections = default;

                return false;
            }

            (int ySrc, int xSrc) = GetPlayerPositionById(srcId);
            (int yDst, int xDst) = GetPlayerPositionById(dstId);

            pointingDirections = new();

            if (ySrc == yDst)
            {
                if (xSrc < xDst)
                {
                    pointingDirections.Add(Direction.Right);
                }
                else if (xSrc > xDst)
                {
                    pointingDirections.Add(Direction.Left);
                }
            }
            else if (xSrc == xDst)
            {
                if (ySrc < yDst)
                {
                    pointingDirections.Add(Direction.Down);
                }
                else if (ySrc > yDst)
                {
                    pointingDirections.Add(Direction.Up);
                }
            }
            else if (xSrc > xDst && ySrc < yDst) // Q1.
            {
                pointingDirections.Add(Direction.Left);
                pointingDirections.Add(Direction.Down);
            }
            else if (xSrc < xDst && ySrc < yDst) // Q2.
            {
                pointingDirections.Add(Direction.Right);
                pointingDirections.Add(Direction.Down);
            }
            else if (xSrc < xDst && ySrc > yDst) // Q3.
            {
                pointingDirections.Add(Direction.Right);
                pointingDirections.Add(Direction.Up);
            }
            else if (xSrc > xDst && ySrc > yDst) // Q4.
            {
                pointingDirections.Add(Direction.Left);
                pointingDirections.Add(Direction.Up);
            }

            Trace.Assert(pointingDirections.Count != 0);

            return true;
        }

        public bool TryMovePlayerById(int id, Direction direction)
        {
            if (Players[id].Health == 0)
            {
                return false;
            }

            bool result = false;

            (int ySrc, int xSrc) = GetPlayerPositionById(id);

            (int yDst, int xDst) = direction switch
            {
                Direction.Up    => (yDst = ySrc - 1, xSrc),
                Direction.Down  => (yDst = ySrc + 1, xSrc),
                Direction.Left  => (yDst = ySrc, xDst = xSrc - 1),
                Direction.Right => (yDst = ySrc, xDst = xSrc + 1),
                _ => throw new ArgumentException(nameof(direction))
            };

            if (yDst >= 0 && yDst < _playField.GetLength(0) && xDst >= 0 && xDst < _playField.GetLength(1))
            {
                CellInfo cellInfoSrc = _playField[ySrc, xSrc];
                CellInfo cellInfoDst = _playField[yDst, xDst];

                switch (cellInfoDst.CellType)
                {
                    case CellType.Empty:
                    {
                        cellInfoDst.Set(cellInfoSrc.Id, CellType.Player);
                        cellInfoSrc.Set();

                        result = true;

                        break;
                    }

                    case CellType.Bullet:
                    {
                        Trace.Assert(cellInfoSrc.Id != cellInfoDst.Id);

                        Players[cellInfoDst.Id].FiredBullets--;

                        if (cellInfoSrc.Id == 0 || cellInfoDst.Id == 0)
                        {
                            Players[cellInfoSrc.Id].Health = Math.Max(0, Players[cellInfoSrc.Id].Health - Players[cellInfoDst.Id].Damage);

                            if (Players[cellInfoSrc.Id].Health == 0)
                            {
                                cellInfoDst.Set();
                            }
                            else
                            {
                                cellInfoDst.Set(cellInfoSrc.Id, CellType.Player);
                            }
                        }
                        else
                        {
                            cellInfoDst.Set(cellInfoSrc.Id, CellType.Player);
                        }

                        cellInfoSrc.Set();

                        result = true;

                        break;
                    }

                    case CellType.Player:
                    {
                        Trace.Assert(cellInfoSrc.Id != cellInfoDst.Id);

                        if (cellInfoSrc.Id == 0 || cellInfoDst.Id == 0)
                        {
                            Players[cellInfoSrc.Id].Health = Math.Max(0, Players[cellInfoSrc.Id].Health - Players[cellInfoDst.Id].Damage);

                            if (Players[cellInfoSrc.Id].Health == 0)
                            {
                                cellInfoSrc.Set();
                            }
                        }

                        break;
                    }
                }
            }

            return result;
        }

        public bool TryFireBulletById(int id, Direction direction)
        {
            if (Players[id].Health == 0)
            {
                return false;
            }

            if (Players[id].FiredBullets == Player.MaxFireableBullets)
            {
                return false;
            }

            bool result = false;

            (int ySrc, int xSrc) = GetPlayerPositionById(id);

            (int yDst, int xDst) = direction switch
            {
                Direction.Up    => (yDst = ySrc - 1, xSrc),
                Direction.Down  => (yDst = ySrc + 1, xSrc),
                Direction.Left  => (yDst = ySrc, xDst = xSrc - 1),
                Direction.Right => (yDst = ySrc, xDst = xSrc + 1),
                _ => throw new ArgumentException(nameof(direction))
            };

            if (yDst >= 0 && yDst < _playField.GetLength(0) && xDst >= 0 && xDst < _playField.GetLength(1))
            {
                CellInfo cellInfoSrc = _playField[ySrc, xSrc];
                CellInfo cellInfoDst = _playField[yDst, xDst];

                switch (cellInfoDst.CellType)
                {
                    case CellType.Empty:
                    {
                        Players[cellInfoSrc.Id].FiredBullets++;

                        cellInfoDst.Set(cellInfoSrc.Id, CellType.Bullet, direction, bulletDistance: 1);

                        result = true;

                        break;
                    }

                    case CellType.Bullet:
                    {
                        Players[cellInfoDst.Id].FiredBullets--;

                        cellInfoDst.Set();

                        result = true;

                        break;
                    }

                    case CellType.Player:
                    {
                        Trace.Assert(cellInfoSrc.Id != cellInfoDst.Id);

                        if (cellInfoSrc.Id == 0 || cellInfoDst.Id == 0)
                        {
                            Players[cellInfoDst.Id].Health = Math.Max(0, Players[cellInfoDst.Id].Health - Players[cellInfoSrc.Id].Damage);

                            if (Players[cellInfoDst.Id].Health == 0)
                            {
                                cellInfoDst.Set();
                            }

                            result = true;
                        }

                        break;
                    }
                }
            }

            return result;
        }

        private (int y, int x) GetPlayerPositionById(int id)
        {
            for (int y = 0; y < _playField.GetLength(0); y++)
            {
                for (int x = 0; x < _playField.GetLength(1); x++)
                {
                    if (_playField[y, x].Id == id && _playField[y, x].CellType == CellType.Player)
                    {
                        return (y, x);
                    }
                }
            }

            throw new ArgumentException(nameof(id));
        }

        public void UpdateBulletsState()
        {
            static bool IsDirectionH(Direction direction) => direction == Direction.Right || direction == Direction.Left;
            static bool IsDirectionV(Direction direction) => direction == Direction.Up    || direction == Direction.Down;

            for (int id = Players.Count - 1; id >= 0; id--)
            {
                for (int bulletDistance = GetMaxBulletDistanceById(id); bulletDistance > 0; bulletDistance--)
                {
                    for (int ySrc = 0; ySrc < _playField.GetLength(0); ySrc++)
                    {
                        for (int xSrc = 0; xSrc < _playField.GetLength(1); xSrc++)
                        {
                            CellInfo cellInfoSrc = _playField[ySrc, xSrc];

                            if (cellInfoSrc.Id == id && cellInfoSrc.CellType == CellType.Bullet && cellInfoSrc.BulletDistance == bulletDistance)
                            {
                                if (IsDirectionH(cellInfoSrc.BulletDirection) && cellInfoSrc.BulletDistance == Player.MaxBulletDistanceH ||
                                    IsDirectionV(cellInfoSrc.BulletDirection) && cellInfoSrc.BulletDistance == Player.MaxBulletDistanceV)
                                {
                                    Players[cellInfoSrc.Id].FiredBullets--;

                                    cellInfoSrc.Set();

                                    continue;
                                }

                                (int yDst, int xDst) = cellInfoSrc.BulletDirection switch
                                {
                                    Direction.Up    => (yDst = ySrc - 1, xSrc),
                                    Direction.Down  => (yDst = ySrc + 1, xSrc),
                                    Direction.Left  => (yDst = ySrc, xDst = xSrc - 1),
                                    Direction.Right => (yDst = ySrc, xDst = xSrc + 1),
                                    _ => throw new Exception(nameof(cellInfoSrc.BulletDirection))
                                };

                                if (yDst < 0 || yDst >= _playField.GetLength(0) || xDst < 0 || xDst >= _playField.GetLength(1))
                                {
                                    Players[cellInfoSrc.Id].FiredBullets--;

                                    cellInfoSrc.Set();

                                    continue;
                                }

                                CellInfo cellInfoDst = _playField[yDst, xDst];

                                switch (cellInfoDst.CellType)
                                {
                                    case CellType.Empty:
                                    {
                                        cellInfoDst.Set(cellInfoSrc);
                                        cellInfoDst.BulletDistance++;

                                        cellInfoSrc.Set();

                                        break;
                                    }

                                    case CellType.Obstacle:
                                    {
                                        Players[cellInfoSrc.Id].FiredBullets--;

                                        cellInfoSrc.Set();

                                        break;
                                    }

                                    case CellType.Bullet:
                                    {
                                        Trace.Assert(cellInfoSrc.Id != cellInfoDst.Id);

                                        Players[cellInfoSrc.Id].FiredBullets--;
                                        Players[cellInfoDst.Id].FiredBullets--;

                                        cellInfoSrc.Set();
                                        cellInfoDst.Set();

                                        break;
                                    }

                                    case CellType.Player:
                                    {
                                        Trace.Assert(cellInfoSrc.Id != cellInfoDst.Id);

                                        Players[cellInfoSrc.Id].FiredBullets--;

                                        if (cellInfoSrc.Id == 0 || cellInfoDst.Id == 0)
                                        {
                                            Players[cellInfoDst.Id].Health = Math.Max(0, Players[cellInfoDst.Id].Health - Players[cellInfoSrc.Id].Damage);

                                            if (Players[cellInfoDst.Id].Health == 0)
                                            {
                                                cellInfoDst.Set();
                                            }
                                        }

                                        cellInfoSrc.Set();

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetMaxBulletDistanceById(int id)
        {
            int maxBulletDistance = 0;

            for (int y = 0; y < _playField.GetLength(0); y++)
            {
                for (int x = 0; x < _playField.GetLength(1); x++)
                {
                    CellInfo cellInfo = _playField[y, x];

                    if (cellInfo.Id == id && cellInfo.CellType == CellType.Bullet)
                    {
                        maxBulletDistance = Math.Max(maxBulletDistance, cellInfo.BulletDistance);
                    }
                }
            }

            return maxBulletDistance;
        }

        public void RemoveAllBullets()
        {
            for (int y = 0; y < _playField.GetLength(0); y++)
            {
                for (int x = 0; x < _playField.GetLength(1); x++)
                {
                    if (_playField[y, x].CellType == CellType.Bullet)
                    {
                        _playField[y, x].Set();
                    }
                }
            }
        }

        public GameOver GetGameOverState()
        {
            int playerHealth = 0;
            int enemiesHealth = 0;

            foreach (Player player in Players)
            {
                if (player.Id == 0)
                {
                    playerHealth = player.Health;
                }
                else
                {
                    enemiesHealth += player.Health;
                }
            }

            if (playerHealth == 0)
            {
                return GameOver.YouLose;
            }
            else if (enemiesHealth == 0)
            {
                return GameOver.YouWin;
            }
            else
            {
                return GameOver.None;
            }
        }

        public void Print()
        {
            lock (_playFieldOld)
            {
                for (int y = 0; y < _playField.GetLength(0); y++)
                {
                    for (int x = 0; x < _playField.GetLength(1); x++)
                    {
                        if (_playField[y, x] != _playFieldOld[y, x])
                        {
                            _playFieldOld[y, x].Set(_playField[y, x]);

                            CellInfo cellInfo = _playField[y, x];

                            switch (cellInfo.CellType)
                            {
                                case CellType.Obstacle:
                                {
                                    Program.Write(Obstacle.GetString(), x + _room.Left + 1, y + _room.Top + 1, Obstacle.GetColor());

                                    break;
                                }

                                case CellType.Bullet:
                                {
                                    Program.Write(Bullet.GetString(), x + _room.Left + 1, y + _room.Top + 1, Bullet.GetColorById(cellInfo.Id));

                                    break;
                                }

                                case CellType.Empty:
                                {
                                    Program.Write(" ", x + _room.Left + 1, y + _room.Top + 1, ConsoleColor.Black);

                                    break;
                                }

                                case CellType.Player:
                                {
                                    Program.Write(Player.GetStringById(cellInfo.Id), x + _room.Left + 1, y + _room.Top + 1, Player.GetColorById(cellInfo.Id));

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class Player
    {
        public const int Defaulthealth = 10;
        public const int DefaultDamage = 1;

        public const int MaxFireableBullets = 5;
        public const int MaxBulletDistanceH = 20;
        public const int MaxBulletDistanceV = 20;

        public int Id { get; }

        public int Health { get; set; }
        public int Damage { get; }

        public int FiredBullets { get; set; }

        private readonly Random _rndWeightGenerate;

        private readonly Random _rndWeightNext;
        private readonly Random _rndDirection;

        private (int stayWeight, int moveWeight, int fireWeight) _actionWeights;
        private (int randomWeight, int pointingWeight) _directionWeights;

        public Player(int id, int health = Defaulthealth, int damage = DefaultDamage)
        {
            Id = id;

            Health = health;
            Damage = damage;

            if (id != 0)
            {
                _rndWeightGenerate = new();

                GenerateActionWeights();
                GenerateDirectionWeights();

                _rndWeightNext = new();
                _rndDirection = new();
            }
        }

        private void GenerateActionWeights()
        {
            do
            {
                _actionWeights.stayWeight = _rndWeightGenerate.Next(1, 101);
                _actionWeights.moveWeight = _rndWeightGenerate.Next(1, 101);
                _actionWeights.fireWeight = _rndWeightGenerate.Next(1, 101);
            }
            while (_actionWeights.stayWeight + _actionWeights.moveWeight + _actionWeights.fireWeight != 100);
        }

        private void GenerateDirectionWeights()
        {
            do
            {
                _directionWeights.randomWeight   = _rndWeightGenerate.Next(1, 101);
                _directionWeights.pointingWeight = _rndWeightGenerate.Next(1, 101);
            }
            while (_directionWeights.randomWeight + _directionWeights.pointingWeight != 100);
        }

        public Action GetNextAction()
        {
            Action action = Action.None;

            int weight = _rndWeightNext.Next(1, 101);

            if (weight >= 1 && weight <= _actionWeights.stayWeight)
            {
                action = Action.Stay;
            }
            else if (weight > _actionWeights.stayWeight && weight <= _actionWeights.stayWeight + _actionWeights.moveWeight)
            {
                action = Action.Move;
            }
            else if (weight > _actionWeights.stayWeight + _actionWeights.moveWeight && weight <= 100)
            {
                action = Action.Fire;
            }

            Trace.Assert(action != Action.None);

            return action;
        }

        public Direction GetNextDirection(PlayField playField)
        {
            Direction direction = Direction.None;

            int weight = _rndWeightNext.Next(1, 101);

            if (weight >= 1 && weight <= _directionWeights.randomWeight)
            {
                direction = (Direction)_rndDirection.Next((int)Direction.Up, (int)Direction.Left + 1);
            }
            else if (weight > _directionWeights.randomWeight && weight <= 100)
            {
                if (playField.TryGetPointingDirectionsBySrcAndDstId(srcId: Id, dstId: 0, out List<Direction> pointingDirections))
                {
                    if (pointingDirections.Count == 1)
                    {
                        direction = pointingDirections[0];
                    }
                    else if (pointingDirections.Count == 2)
                    {
                        direction = pointingDirections[_rndDirection.Next(0, 2)];
                    }
                }
                else
                {
                    direction = (Direction)_rndDirection.Next((int)Direction.Up, (int)Direction.Left + 1);
                }
            }

            Trace.Assert(direction != Direction.None);

            return direction;
        }

        public static string GetStringById(int id)
        {
            return id == 0 ? "◊" : "☼";
        }

        public static ConsoleColor GetColorById(int id)
        {
            return id == 0 ? ConsoleColor.Cyan : ConsoleColor.Red;
        }
    }

    public static class Bullet
    {
        public static string GetString()
        {
            return "∙"; // Alternative: "•".
        }

        public static ConsoleColor GetColorById(int id)
        {
            return id == 0 ? ConsoleColor.DarkCyan : ConsoleColor.DarkRed;
        }
    }

    public static class Obstacle
    {
        public static string GetString()
        {
            return "X";
        }

        public static ConsoleColor GetColor()
        {
            return ConsoleColor.DarkGray;
        }
    }
}