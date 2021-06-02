using System;
using System.Drawing;

namespace ReactGame.Engine
{
    public enum UnitStates
    {
        Visible,
        Hidden
    }

    public interface IUnit
    {
        UnitStates State { get; set; }
        PointF Location { get; set; }
        SizeF Size { get; set; }
        Int32 Layer { get; set; }
        void Draw(Graphics Canvas, PointF Start);
    }
}
