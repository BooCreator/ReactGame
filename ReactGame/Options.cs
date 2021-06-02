using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ReactGame
{
    public static class Options
    {
        public static Int32 MaxFPS = 75;
        public static Int32 CanvasWidth = 640;
        public static Int32 CanvasHeight = 480;
        public static SmoothingMode SmoothingMode = SmoothingMode.HighQuality;
        public static Dictionary<String, Dictionary<String, Dictionary<String, Int32>>> Level = new Dictionary<String, Dictionary<String, Dictionary<String, Int32>>>()
        {
            { "normal",
                new Dictionary<String, Dictionary<String, Int32>>()
                {
                    { "level1", 
                        new Dictionary<String, Int32>()
                        {
                            { "cols", 4 },
                            { "rows", 4 }
                        }
                    },
                    { "level2",
                        new Dictionary<String, Int32>()
                        {
                            { "sec_res", 3 },
                            { "pops", 3 }
                        }
                    },
                    { "level3",
                        new Dictionary<String, Int32>()
                        {
                            { "sec_res", 5 },
                        }
                    },
                    { "level4",
                        new Dictionary<String, Int32>()
                        {
                            { "sec_res", 5 },
                            { "view_items", 5 },
                            { "cols", 4 },
                            { "rows", 4 }
                        }
                    },
                    { "level5",
                        new Dictionary<String, Int32>()
                        {
                            { "laser_time", 800 },
                            { "max_shots", 6 },
                        }
                    }
                }
            },
        };
    }

    public static class Global
    {
        public static char getChar(KeyEventArgs e)
        {
            int keyValue = e.KeyValue;
            if (keyValue >= (int)Keys.A && keyValue <= (int)Keys.Z || keyValue >= (int)Keys.D1 && keyValue <= (int)Keys.D9)
            {
                return (!e.Shift) ? (char)(keyValue + 32) : (char)keyValue;
            }
            return default;
        }
    }

}
