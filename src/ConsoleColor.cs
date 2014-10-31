namespace ip4 
{
    using System;
    using System.Runtime.InteropServices;

    [Flags]
    public enum ConsoleForeground: int {
        Blue      = 0x00000001,
        Green     = 0x00000002,
        Red       = 0x00000004,
        Intensity = 0x00000008
    }

    [Flags]
    public enum ConsoleBackground: int {
        Blue      = 0x00000010,
        Green     = 0x00000020,
        Red       = 0x00000040,
        Intensity = 0x00000080
    }


    public class ConsoleColor {
        private int hConsoleHandle;
        private COORD ConsoleOutputLocation;
        private CONSOLE_SCREEN_BUFFER_INFO ConsoleInfo;
        private int OriginalColors;

        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true,
                        CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo",
                        SetLastError = true, CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput,
                         ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", EntryPoint = "SetConsoleTextAttribute",
                        SetLastError = true, CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int SetConsoleTextAttribute(int hConsoleOutput,
                                 int wAttributes);

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD {
            short X;
            short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT {
            short Left;
            short Top;
            short Right;
            short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public int wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }


        public void TextColor(int color) {
            SetConsoleTextAttribute(hConsoleHandle, color);
        }

        public void ResetColor() {
            SetConsoleTextAttribute(hConsoleHandle, OriginalColors);
        }

        public ConsoleColor() {
            ConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
            ConsoleOutputLocation = new COORD();
            hConsoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleScreenBufferInfo(hConsoleHandle, ref ConsoleInfo);
            OriginalColors = ConsoleInfo.wAttributes;
        }
    }
}