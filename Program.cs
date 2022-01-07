﻿using System;
using System.Threading;

namespace IsAAAc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "IsAAAc";

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            Console.SetWindowPosition(0, 0);

            Console.WindowWidth = Isaaac.Width + 2;
            Console.WindowHeight = Isaaac.Height + 2;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();

            Isaaac.PrintRoom(1, 1, true, true, true, true);
            /*Isaaac.PrintRoom(33, 1, false, false, true, true);
            Isaaac.PrintRoom(1, 17, true, true, false, false);
            Isaaac.PrintRoom(33, 17, true, false, false, true);*/

            int xBb = 2;
            int yBb = 2;
            const string bB = "♦";

            Isaaac.Write(bB, xBb, yBb, ConsoleColor.Cyan);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey cK = Console.ReadKey(true).Key;
                    ThreadPool.QueueUserWorkItem((_) => {while (Console.KeyAvailable) Console.ReadKey(true); });

                    Isaaac.Write(bB, xBb, yBb, ConsoleColor.Black);

                    switch (cK)
                    {
                        case ConsoleKey.UpArrow:
                        {
                            yBb--;

                            break;
                        }

                        case ConsoleKey.DownArrow:
                        {
                            yBb++;

                            break;
                        }

                        case ConsoleKey.RightArrow:
                        {
                            xBb++;

                            break;
                        }

                        case ConsoleKey.LeftArrow:
                        {
                            xBb--;

                            break;
                        }

                        case ConsoleKey.Escape:
                        {
                            return;
                        }
                    }

                    Isaaac.Write(bB, xBb, yBb, ConsoleColor.Cyan);
                }

                Thread.Sleep(33);
            }
        }
    }

    public static class Isaaac
    {
        /*
        LIMITI BUFFER FINESTRA (1920PX * 1080PX) CONSOLE
        WindowWidth: 240 WindowHeight: 63
        */

        public const int Width = 30;
        public const int Height = 10;

        public static void Write(string str, int left, int top, ConsoleColor fColor = ConsoleColor.Gray, ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            Console.ResetColor();
        }

        public static void PrintRoom(int xOffset, int yOffset, bool up, bool right, bool down, bool left) 
        {
            const int DoorWidth = 8;
            const int DoorHeight = DoorWidth / 2;

            Write("╔" + new String('═', Width - 2) + "╗", xOffset, yOffset);
            for (var i = 1; i <= Height - 2; i++)
            {
                Write("║" + new String(' ', Width - 2) + "║", xOffset, yOffset + i);
            }
            Write("╚" + new String('═', Width - 2) + "╝", xOffset, yOffset + Height - 1);

            if (up)
            {
                Write(new String('═', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Write(new String('═', DoorWidth), xOffset + Width / 2 - DoorWidth / 2, yOffset + Height - 1, ConsoleColor.Black);
            }

            if (right)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Write("║", xOffset + Width - 1, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }

            if (left)
            {
                for (int i = 0; i < DoorHeight; i++)
                {
                    Write("║", xOffset, yOffset + Height / 2 - DoorHeight / 2 + i, ConsoleColor.Black);
                }
            }
        }
    }
}
