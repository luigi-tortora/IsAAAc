using System;

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
            
            Console.WindowWidth = 65;
            Console.WindowHeight = 35;
            
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.Clear();
        
            PrintRoom(1, 1, false, true, true, false);
            PrintRoom(33, 1, false, false, true, true);
            PrintRoom(1, 17, true, true, false, false);
            PrintRoom(33, 17, true, false, false, true);

            Console.ReadKey();
        }

        private static void Write(string str, int left, int top, ConsoleColor fColor = ConsoleColor.Gray, ConsoleColor bColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            Console.ResetColor();
        }

        private static void PrintRoom(int xOffset, int yOffset, bool up, bool right, bool down, bool left) 
        {
            Write("╔═════════════════════════════╗", xOffset, yOffset);
            Write("║                             ║", xOffset, yOffset + 1); 
            Write("║                             ║", xOffset, yOffset + 2); 
            Write("║                             ║", xOffset, yOffset + 3); 
            Write("║                             ║", xOffset, yOffset + 4); 
            Write("║                             ║", xOffset, yOffset + 5); 
            Write("║                             ║", xOffset, yOffset + 6);
            Write("║                             ║", xOffset, yOffset + 7); 
            Write("║                             ║", xOffset, yOffset + 8);                 
            Write("║                             ║", xOffset, yOffset + 9);
            Write("║                             ║", xOffset, yOffset + 10); 
            Write("║                             ║", xOffset, yOffset + 11); 
            Write("║                             ║", xOffset, yOffset + 12); 
            Write("║                             ║", xOffset, yOffset + 13); 
            Write("║                             ║", xOffset, yOffset + 14); 
            Write("╚═════════════════════════════╝", xOffset, yOffset + 15);

            if (up)
            {
                Write("═══════", xOffset + 12, yOffset, ConsoleColor.Black);
            }

            if (down)
            {
                Write("═══════", xOffset + 12, yOffset + 15, ConsoleColor.Black);
            }

            if (right) 
            {
                Write("║", xOffset + 30, yOffset + 6, ConsoleColor.Black);
                Write("║", xOffset + 30, yOffset + 7, ConsoleColor.Black); 
                Write("║", xOffset + 30, yOffset + 8, ConsoleColor.Black);                 
                Write("║", xOffset + 30, yOffset + 9, ConsoleColor.Black);
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
