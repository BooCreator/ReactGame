using System;
using System.Drawing;

namespace ReactGame.Engine.Units
{
    public interface IStringUnit
    {
        String Text { get; set; }
        Font Font { get; set; }
        Brush Brush { get; set; }
    }
}
