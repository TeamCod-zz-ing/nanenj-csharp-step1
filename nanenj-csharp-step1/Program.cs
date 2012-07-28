using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace nanenj_csharp_step1
{
    class Program
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)]
            public char UnicodeChar;
            [FieldOffset(0)]
            public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)]
            public CharUnion Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }


        [STAThread]
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            bool isTimeToQuit = false;

            short xLoc = 43;
            short yLoc = 5;

            SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            CharInfo[] buf = new CharInfo[80 * 25];
            SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 80, Bottom = 25 };

            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            Random r = new Random();
            int dir = r.Next(1, 4);

            while (isTimeToQuit == false)
            {
                if (!h.IsInvalid)
                {
                    for (int i = 0; i < buf.Length; ++i)
                    {
                        buf[i].Attributes = 7;
                        buf[i].Char.AsciiChar = 46;
                        if (i == (yLoc*80)+xLoc)
                        {
                            buf[i].Attributes = 7;
                            buf[i].Char.AsciiChar = 64;
                        }
                    }
                    if (Console.KeyAvailable == true)
                    {
                        cki = Console.ReadKey(true);
                        switch (cki.Key)
                        {
                            case ConsoleKey.LeftArrow:
                                xLoc--;
                                if (xLoc <= 0)
                                    xLoc = 0;
                                break;
                            case ConsoleKey.RightArrow:
                                xLoc++;
                                if (xLoc >= 79)
                                    xLoc = 79;
                                break;
                            case ConsoleKey.UpArrow:
                                yLoc--;
                                if (yLoc <= 0)
                                    yLoc = 0;
                                break;
                            case ConsoleKey.DownArrow:
                                yLoc++;
                                if (yLoc >= 24)
                                    yLoc = 24;
                                break;
                            case ConsoleKey.Escape:
                                isTimeToQuit = true;
                                break;
                            default:
                                break;
                        }
                    }

                    bool b = WriteConsoleOutput(h, buf,
                        new Coord() { X = 80, Y = 25 },
                        new Coord() { X = 0, Y = 0 },
                        ref rect);

                }
            }
        }
    }
}
