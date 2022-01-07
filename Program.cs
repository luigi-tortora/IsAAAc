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
        public const int width = 65;
        public const int height = 17;
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
            int width = 60;
            int height = 15;
            /*Write("╔═════════════════════════════════════════════════════════════╗", xOffset, yOffset);
            Write("║                                                             ║", xOffset, yOffset + 1); 
            Write("║                                                             ║", xOffset, yOffset + 2); 
            Write("║                                                             ║", xOffset, yOffset + 3); 
            Write("║                                                             ║", xOffset, yOffset + 4); 
            Write("║                                                             ║", xOffset, yOffset + 5); 
            Write("║                                                             ║", xOffset, yOffset + 6);
            Write("║                                                             ║", xOffset, yOffset + 7); 
            Write("║                                                             ║", xOffset, yOffset + 8);                 
            Write("║                                                             ║", xOffset, yOffset + 9);
            Write("║                                                             ║", xOffset, yOffset + 10); 
            Write("║                                                             ║", xOffset, yOffset + 11); 
            Write("║                                                             ║", xOffset, yOffset + 12); 
            Write("║                                                             ║", xOffset, yOffset + 13); 
            Write("║                                                             ║", xOffset, yOffset + 14); 
            Write("╚═════════════════════════════════════════════════════════════╝", xOffset, yOffset + 15);*/

            Write("╔", xOffset, yOffset);
            
            for (int i = 0; i < width; i++)
            {
                Write("═", xOffset + 1 + i, yOffset);

                for (int j = 0; j < width; j++)
                {
                    Write("═", xOffset + 1 + i, yOffset + height);
                }
            }

            Write("╗", xOffset + width, yOffset);

            for (int i = 0; i < height; i++)
            {
                Write("║", xOffset, yOffset + 1 + i);

                for (int j = 0; j < height; j++)
                {
                    Write("║", xOffset + width, yOffset + 1 + i);
                }
            }

            Write("╚", xOffset, yOffset + height);         
            Write("╝", xOffset + width, yOffset + height);         

            if (up)
            {
                Write("═══════", xOffset + 28, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Write("═══════", xOffset + 28, yOffset + 15, ConsoleColor.Black);
            }

            if (right) 
            {
                Write("║", xOffset + width, yOffset + 7, ConsoleColor.Black); 
                Write("║", xOffset + width, yOffset + 8, ConsoleColor.Black);                 
                Write("║", xOffset + width, yOffset + 6, ConsoleColor.Black);
                Write("║", xOffset + width, yOffset + 9, ConsoleColor.Black);
            }

            if (left) 
            {
                Write("║", xOffset, yOffset + 6, ConsoleColor.Black);
                Write("║", xOffset, yOffset + 7, ConsoleColor.Black); 
                Write("║", xOffset, yOffset + 8, ConsoleColor.Black);                 
                Write("║", xOffset, yOffset + 9, ConsoleColor.Black);
            }
        }
    }
}
