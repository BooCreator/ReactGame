using System;
using System.Drawing;

namespace ReactGame.Engine.Items
{
    public class ProgressItem : IUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; } = 0;

        private Single MaxValue { get; set; } = 100;
        public Single Percent { get; set; }

        public Pen Border = new Pen(Color.Black);
        public Brush Progress = new SolidBrush(Color.YellowGreen);

        public void Draw(Graphics Canvas, PointF Start)
        {
            Canvas.FillRectangle(this.Progress,
                    new Rectangle(new Point((int)this.Location.X, (int)this.Location.Y),
                    new Size((int)(this.Percent / MaxValue * this.Size.Width), (int)this.Size.Height)));
            Canvas.DrawRectangle(this.Border,
                new Rectangle(new Point((int)this.Location.X, (int)this.Location.Y),
                new Size((int)this.Size.Width, (int)this.Size.Height)));
        }
    }
}
