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

            playField.PlaceObstacle(count: playersCount * 2);

            for (int id = 0; id < playersCount; id++)
            {
                playField.PlacePlayer(new(id));
            }

            PrintPlayersStats(playField.Players);
            playField.Print();

            Random rndWeight = new();
            Random rndDirection = new();

            Stopwatch sW = new();

            bool exit = false;
            GameOver gameOver = GameOver.None;

            do
            {
                int frameTime = 36; // ms. // Divisible by 3.

                gameOver = GetGameOverState(playField.Players);
                if (gameOver == GameOver.None)
                {
                    sW.Restart();

                    for (int id = playersCount - 1; id > 0; id--)
                    {
                        Action action = playField.Players[id].GetActionByWeight(rndWeight.Next(1, 101));

                        switch (action)
                        {
                            case Action.Move:
                            {
                                Direction direction = (Direction)rndDirection.Next((int)Direction.Up, (int)Direction.Left + 1);

                                playField.MovePlayerById(id, direction);

                                break;
                            }

                            case Action.Fire:
                            {
                                Direction direction = (Direction)rndDirection.Next((int)Direction.Up, (int)Direction.Left + 1);

                                playField.FireBulletById(id, direction);

                                break;
                            }
                        }
                    }

                    PrintPlayersStats(playField.Players);
                    playField.Print();

                    sW.Stop();

                    Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));
                }

                gameOver = GetGameOverState(playField.Players);
                if (gameOver == GameOver.None)
                {
                    sW.Restart();

                    if (Console.KeyAvailable)
                    {
                        ConsoleKey cK = Console.ReadKey(true).Key;
                        ThreadPool.QueueUserWorkItem((_) => { while (Console.KeyAvailable && Console.ReadKey(true).Key == cK); });

                        switch (cK)
                        {
                            case ConsoleKey.W:
                            {
                                playField.MovePlayerById(id: 0, Direction.Up);

                                break;
                            }

                            case ConsoleKey.S:
                            {
                                playField.MovePlayerById(id: 0, Direction.Down);

                                break;
                            }

                            case ConsoleKey.A:
                            {
                                playField.MovePlayerById(id: 0, Direction.Left);

                                break;
                            }

                            case ConsoleKey.D:
                            {
                                playField.MovePlayerById(id: 0, Direction.Right);

                                break;
                            }

                            case ConsoleKey.UpArrow:
                            {
                                playField.FireBulletById(id: 0, Direction.Up);

                                break;
                            }

                            case ConsoleKey.DownArrow:
                            {
                                playField.FireBulletById(id: 0, Direction.Down);

                                break;
                            }

                            case ConsoleKey.LeftArrow:
                            {
                                playField.FireBulletById(id: 0, Direction.Left);

                                break;
                            }

                            case ConsoleKey.RightArrow:
                            {
                                playField.FireBulletById(id: 0, Direction.Right);

                                break;
                            }

                            case ConsoleKey.Escape:
                            {
                                exit = true;

                                frameTime = 0;

                                break;
                            }
                        }
                    }

                    PrintPlayersStats(playField.Players);
                    playField.Print();

                    sW.Stop();

                    Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));
                }

                gameOver = GetGameOverState(playField.Players);
                if (gameOver == GameOver.None)
                {
                    sW.Restart();

                    playField.UpdateBulletsState();

                    PrintPlayersStats(playField.Players);
                    playField.Print();

                    sW.Stop();

                    Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));
                }
                else
                {
                    sW.Restart();

                    playField.RemoveAllBullets();

                    PrintPlayersStats(playField.Players);
                    playField.Print();

                    sW.Stop();

                    Thread.Sleep(Math.Max(0, frameTime / 3 - (int)sW.ElapsedMilliseconds));
                }
            }
            while (!exit && gameOver == GameOver.None);

            if (gameOver == GameOver.YouWin)
            {
                Console.Title = $"{Title} [Game Over: You Win!]";

                Console.ReadKey(true);
            }
            else if (gameOver == GameOver.YouLose)
            {
                Console.Title = $"{Title} [Game Over: You Lose!]";

                Console.ReadKey(true);
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

        public static void PrintPlayersStats(List<Player> players)
        {
            int playerHealth = players[0].Health;

            int totalCpuHealth = 0;

            foreach (Player player in players)
            {
                if (player.Id != 0)
                {
                    totalCpuHealth += player.Health;
                }
            }

            Console.Title = $"{Title} [Player Health: {playerHealth}] [Total Cpu Health: {totalCpuHealth}]";
        }

        public static GameOver GetGameOverState(List<Player> players)
        {
            int playerHealth = players[0].Health;

            int totalCpuHealth = 0;

            foreach (Player player in players)
            {
                if (player.Id != 0)
                {
                    totalCpuHealth += player.Health;
                }
            }

            if (playerHealth == 0)
            {
                return GameOver.YouLose;
            }
            else if (totalCpuHealth == 0)
            {
                return GameOver.YouWin;
            }
            else
            {
                return GameOver.None;
            }
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
        public int BulletDistance { get; private set; }

        public CellInfo(int id = 0, CellType cellType = CellType.Empty, Direction bulletDirection = Direction.None, int bulletDistance = 0)
        {
            Id = id;

            CellType = cellType;

            BulletDirection = bulletDirection;
            BulletDistance = bulletDistance;
        }

        public void Set(int id = 0, CellType cellType = CellType.Empty, Direction bulletDirection = Direction.None, int bulletDistance = 0)
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
        public bool Equals(CellInfo cellInfo)
        {
            return Id == cellInfo.Id && CellType == cellInfo.CellType && BulletDirection == cellInfo.BulletDirection && BulletDistance == cellInfo.BulletDistance;
        }
        public override int GetHashCode() => HashCode.Combine(Id, CellType, BulletDirection, BulletDistance);

        public static bool operator ==(CellInfo lhs, CellInfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(CellInfo lhs, CellInfo rhs) => !lhs.Equals(rhs);
    }

    public class Room
    {
        public const int DoorWidth = 8;
        public const int DoorHeight = DoorWidth / 2;

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
        public const int MaxPlayersCount = 11;

        public List<Player> Players { get; }

        private readonly CellInfo[,] _playField;
        private readonly CellInfo[,] _oldPlayField;

        private readonly Room _room;

        public PlayField(Room room)
        {
            _playField = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_playField);

            _oldPlayField = new CellInfo[room.Height - 2, room.Width - 2];
            Init(_oldPlayField);

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

        public void MovePlayerById(int id, Direction direction)
        {
            if (Players[id].Health == 0)
            {
                return;
            }

            (int y, int x) = GetPlayerPositionById(id);

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
                    CellInfo cellInfo = _playField[y, x];
                    CellInfo cellInfoNew = _playField[yNew, xNew];

                    if (cellInfoNew.CellType == CellType.Empty)
                    {
                        cellInfoNew.Set(cellInfo.Id, CellType.Player);
                        cellInfo.Set();
                    }
                    else if (cellInfoNew.CellType == CellType.Bullet)
                    {
                        Players[cellInfoNew.Id].FiredBullets--;

                        if (cellInfo.Id != cellInfoNew.Id && (cellInfo.Id == 0 || cellInfoNew.Id == 0))
                        {
                            Players[cellInfo.Id].Health = Math.Max(0, Players[cellInfo.Id].Health - Players[cellInfoNew.Id].Damage);

                            if (Players[cellInfo.Id].Health == 0)
                            {
                                cellInfoNew.Set();
                            }
                            else
                            {
                                cellInfoNew.Set(cellInfo.Id, CellType.Player);
                            }
                        }
                        else
                        {
                            cellInfoNew.Set(cellInfo.Id, CellType.Player);
                        }

                        cellInfo.Set();
                    }
                    else if (cellInfoNew.CellType == CellType.Player)
                    {
                        if (cellInfo.Id != cellInfoNew.Id && (cellInfo.Id == 0 || cellInfoNew.Id == 0))
                        {
                            Players[cellInfo.Id].Health = Math.Max(0, Players[cellInfo.Id].Health - Players[cellInfoNew.Id].Damage);

                            if (Players[cellInfo.Id].Health == 0)
                            {
                                cellInfo.Set();
                            }
                        }
                    }
                }
            }
        }

        private (int y, int x) GetPlayerPositionById(int id)
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

            throw new ArgumentException(nameof(id));
        }

        public void FireBulletById(int id, Direction direction)
        {
            if (Players[id].Health == 0)
            {
                return;
            }

            if (Players[id].FiredBullets == Player.MaxFireableBullets)
            {
                return;
            }

            (int y, int x) = GetPlayerPositionById(id);

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
                    CellInfo cellInfo = _playField[y, x];
                    CellInfo cellInfoNew = _playField[yNew, xNew];

                    if (cellInfoNew.CellType == CellType.Empty)
                    {
                        Players[cellInfo.Id].FiredBullets++;

                        cellInfoNew.Set(cellInfo.Id, CellType.Bullet, direction, 1);
                    }
                    else if (cellInfoNew.CellType == CellType.Bullet)
                    {
                        Players[cellInfoNew.Id].FiredBullets--;

                        cellInfoNew.Set();
                    }
                    else if (cellInfoNew.CellType == CellType.Player)
                    {
                        if (cellInfo.Id != cellInfoNew.Id && (cellInfo.Id == 0 || cellInfoNew.Id == 0))
                        {
                            Players[cellInfoNew.Id].Health = Math.Max(0, Players[cellInfoNew.Id].Health - Players[cellInfo.Id].Damage);

                            if (Players[cellInfoNew.Id].Health == 0)
                            {
                                cellInfo.Set();
                            }
                        }
                    }
                }
            }
        }

        public void UpdateBulletsState() // TODO: .
        {
            for (int id = Players.Count; id > 0; id--)
            {
                for (int y = 0; y < _playField.GetLength(0); y++)
                {
                    for (int x = 0; x < _playField.GetLength(1); x++)
                    {
                        if (_playField[y, x].Id == Players[id].Id && _playField[y, x].CellType == CellType.Bullet)
                        {
                            
                        }
                    }
                }
            }
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
            lock (_oldPlayField)
            {
                for (int y = 0; y < _playField.GetLength(0); y++)
                {
                    for (int x = 0; x < _playField.GetLength(1); x++)
                    {
                        if (_playField[y, x] != _oldPlayField[y, x])
                        {
                            _oldPlayField[y, x].Set(_playField[y, x]);

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

        public const int MaxFireableBullets = 10;
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
