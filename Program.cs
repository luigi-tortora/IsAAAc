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

            room.Print(false, false, false, false);

            PlayField playField = new(room);

            int playersCount = new Random().Next(2, PlayField.MaxPlayersCount + 1);

            playField.PlaceObstacle(count: playersCount * PlayField.ObstaclesPerPlayer);

            for (int id = 0; id < playersCount; id++)
            {
                if (id == 0)
                {
                    playField.PlacePlayer(new(id));
                }
                else
                {
                    playField.PlacePlayer(new(id));
                }
            }

            PrintPlayersStats(playField.Players);
            playField.Print();

            Random rndWeight = new();
            Random rndDirection = new();

            Stopwatch sW = new();

            bool exit = false;

            int frameTime = 20 * 3; // ms.

            while (true)
            {
                if (exit || GetGameOverState(playField.Players) != GameOver.None)
                {
                    break;
                }

                sW.Restart();

                for (int id = playersCount - 1; id > 0; id--)
                {
                    Action action = playField.Players[id].GetActionByWeight(rndWeight.Next(1, 101));
                    Direction direction = (Direction)rndDirection.Next((int)Direction.Up, (int)Direction.Left + 1);

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

                if (exit || GetGameOverState(playField.Players) != GameOver.None)
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

                        case ConsoleKey.Escape:
                        {
                            exit = true;

                            break;
                        }

                        case ConsoleKey.OemPlus:
                        {
                            frameTime = Math.Clamp(((frameTime / 3) - 1) * 3, 1, 1000);

                            break;
                        }

                        case ConsoleKey.OemMinus:
                        {
                            frameTime = Math.Clamp(((frameTime / 3) + 1) * 3, 1, 1000);

                            break;
                        }
                    }
                }

                PrintPlayersStats(playField.Players, frameTime);
                playField.Print();

                sW.Stop();

                Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));

                if (exit || GetGameOverState(playField.Players) != GameOver.None)
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

                Console.Title = $"{Title} [GameOver: {GetGameOverState(playField.Players)}]";
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

        public static void Write(string str, int left, int top, ConsoleColor fColor = ConsoleColor.Gray, ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            Console.ResetColor();
        }

        public static void PrintPlayersStats(List<Player> players, int frameTime = 1)
        {
            int fps = (int)(1f / ((float)frameTime / 1000f));

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

        public static GameOver GetGameOverState(List<Player> players)
        {
            int playerHealth = 0;
            int enemiesHealth = 0;

            foreach (Player player in players)
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

        private static void BeepAsync(int frequency = 800, int duration = 200)
        {
            ThreadPool.QueueUserWorkItem((_) => Console.Beep(frequency, duration));
        }
    }

    public enum GameOver { None, YouWin, YouLose }

    public enum CellType { Empty, Obstacle, Bullet, Player }

    public enum Action { Stay, Move, Fire }
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
        public const int ObstaclesPerPlayer = 2;

        public const int MaxPlayersCount = 1 + 20;

        public List<Player> Players { get; }

        private readonly CellInfo[,] _playField;
        private readonly CellInfo[,] _playFieldOld;

        private readonly Room _room;

        public PlayField(Room room)
        {
            _playField = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playField);

            _playFieldOld = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playFieldOld);

            _room = room;

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

        public void PlacePlayer(Player player)
        {
            Random rnd = new();

            while (true)
            {
                int y = rnd.Next(0, _playField.GetLength(0));
                int x = rnd.Next(0, _playField.GetLength(1));

                if (_playField[y, x].CellType == CellType.Empty)
                {
                    _playField[y, x].Set(player.Id, CellType.Player);

                    Players.Add(player);

                    return;
                }
            }
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
                                if (cellInfoSrc.BulletDistance == Player.MaxBulletDistance)
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
        public const int MaxBulletDistance = 20;

        public int Id { get; }

        public int Health { get; set; }
        public int Damage { get; }

        public int FiredBullets { get; set; }

        private (int stayWeight, int moveWeight, int fireWeight) _actionWeights;

        public Player(int id, int health = Defaulthealth, int damage = DefaultDamage)
        {
            Id = id;

            Health = health;
            Damage = damage;

            if (id != 0)
            {
                GenerateActionWeights();
            }
        }

        private void GenerateActionWeights()
        {
            Random rnd = new();

            do
            {
                _actionWeights.stayWeight = rnd.Next(1, 101);
                _actionWeights.moveWeight = rnd.Next(1, 101);
                _actionWeights.fireWeight = rnd.Next(1, 101);
            }
            while (_actionWeights.stayWeight + _actionWeights.moveWeight + _actionWeights.fireWeight != 100);
        }

        public Action GetActionByWeight(int weight)
        {
            if (weight >= 1 && weight <= _actionWeights.stayWeight)
            {
                return Action.Stay;
            }
            else if (weight > _actionWeights.stayWeight && weight <= _actionWeights.stayWeight + _actionWeights.moveWeight)
            {
                return Action.Move;
            }
            else if (weight > _actionWeights.stayWeight + _actionWeights.moveWeight && weight <= 100)
            {
                return Action.Fire;
            }

            throw new ArgumentException(nameof(weight));
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