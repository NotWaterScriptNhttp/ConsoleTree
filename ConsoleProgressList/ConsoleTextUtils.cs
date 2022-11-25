using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ConsoleProgressList
{
    public struct Color
    {
        public int R, G, B;

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
    public struct TextBlock
    {
        private Color _foreground;
        private Color _background;

        public string text;

        public bool useForeColor;
        public bool useBackColor;

        public Color foreground {
            get => _foreground;
            set
            {
                useForeColor = true;
                _foreground = value;
            } 
        }
        public Color background
        {
            get => _background;
            set
            {
                useBackColor = true;
                _background = value;
            }
        }
    }

    public class ConsoleTextUtils
    { 
        [DllImport("kernel32")]
        private static extern bool SetConsoleMode(IntPtr hHandle, int mode);
        [DllImport("kernel32")]
        private static extern bool GetConsoleMode(IntPtr hHandle, out int mode);

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(int hHandle);
        
        private static bool useColors = false;
        private static bool IsTrueColorEnabled = false;

        public static bool NoColorMode { get; private set; } = Environment.GetCommandLineArgs().Contains("--nocol");
        public static bool UseColors
        {
            get
            {
                if (NoColorMode)
                    return false;

                return useColors;
            }
            set
            {
                if (IsTrueColorEnabled)
                {
                    useColors = value;
                    return;
                }

                if (useColors)
                    useColors = value;
            }
        }

        public static void EnableTrueColor()
        {
            if (UseColors)
                return;

            IntPtr ptr = GetStdHandle(-11);
            GetConsoleMode(ptr, out int mode);
            SetConsoleMode(ptr, mode | 0x4);
            IsTrueColorEnabled = true;
            UseColors = true;
        }

        public static string DoString(TextBlock block)
        {
            string colorFormat = "";
            if (UseColors)
            {
                if (block.useForeColor)
                {
                    Color col = block.foreground;
                    colorFormat += $"\x1b[38;2;{col.R};{col.G};{col.B}m";
                }
                if (block.useBackColor)
                {
                    Color col = block.background;
                    colorFormat += $"\x1b[48;2;{col.R};{col.G};{col.B}m";
                }
            }
            else return block.text;

            return $"{colorFormat}{block.text}\x1b[0m";
        }
        public static string DoString(TextBlock[] blocks)
        {
            string output = "";

            foreach (TextBlock block in blocks)
            {
                string colorFormat = "";
                if (UseColors)
                {
                    if (block.useForeColor)
                    {
                        Color col = block.foreground;
                        colorFormat += $"\x1b[38;2;{col.R};{col.G};{col.B}m";
                    }
                    if (block.useBackColor)
                    {
                        Color col = block.background;
                        colorFormat += $"\x1b[48;2;{col.R};{col.G};{col.B}m";
                    }
                }
                else { output += block.text; continue; }

                output += $"{colorFormat}{block.text}\x1b[0m";
            }

            return output;
        }
    }
}
