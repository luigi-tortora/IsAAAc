using System;
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
            
            Console.SetWindowPosition(0,0);
            
            Console.WindowWidth = Isaaac.width;
            Console.WindowHeight = Isaaac.height;
            
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();
                    
            Isaaac.PrintRoom(1, 1, true, true, true, true);
            /*Isaaac.PrintRoom(33, 1, false, false, true, true);
            Isaaac.PrintRoom(1, 17, true, true, false, false);
            Isaaac.PrintRoom(33, 17, true, false, false, true);*/

            int xBb = 3;
            int yBb = 3;
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
                            xBb += 2;
                            break;
                        }

                        case ConsoleKey.LeftArrow:
                        {
                            xBb -= 2;
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
        WIDTH: 240 HEIGHT: 63
        */
        public const int width = 66;
        public const int height = 16;
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
            int widthRoom = width - 2;
            int heightRoom = height - 2;

            Write("╔", xOffset, yOffset);
            
            for (int i = 0; i < widthRoom; i++)
            {
                Write("═", xOffset + 1 + i, yOffset);

                for (int j = 0; j < widthRoom; j++)
                {
                    Write("═", xOffset + 1 + i, yOffset + heightRoom);
                }
            }

            Write("╗", xOffset + widthRoom, yOffset);

            for (int i = 0; i < heightRoom; i++)
            {
                Write("║", xOffset, yOffset + 1 + i);

                for (int j = 0; j < heightRoom; j++)
                {
                    Write("║", xOffset + widthRoom, yOffset + 1 + i);
                }
            }

            Write("╚", xOffset, yOffset + heightRoom);         
            Write("╝", xOffset + widthRoom, yOffset + heightRoom);     

            int xDoor = (widthRoom - 8) / 2;  
            int yDoor = (heightRoom - 4) / 2;  

            if (up)
            {
                Write("════════", xOffset + xDoor, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Write("════════", xOffset + xDoor, yOffset + heightRoom, ConsoleColor.Black);
            }

            if (right) 
            {
                Write("║", xOffset + widthRoom, yOffset + yDoor, ConsoleColor.Black); 
                Write("║", xOffset + widthRoom, yOffset + yDoor + 1, ConsoleColor.Black);                 
                Write("║", xOffset + widthRoom, yOffset + yDoor + 2, ConsoleColor.Black);
                Write("║", xOffset + widthRoom, yOffset + yDoor + 3, ConsoleColor.Black);
            }

            if (left) 
            {
                Write("║", xOffset, yOffset + yDoor, ConsoleColor.Black);
                Write("║", xOffset, yOffset + yDoor + 1, ConsoleColor.Black); 
                Write("║", xOffset, yOffset + yDoor + 2, ConsoleColor.Black);                 
                Write("║", xOffset, yOffset + yDoor + 3, ConsoleColor.Black);
            }
        }
    }
}
