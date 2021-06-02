using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactGame.Engine.Items
{
    public class LabelItem : IUnit, Units.IStringUnit, Units.IGraphicUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; }

        private Image frame = null;
        public Image Frame => this.frame;
        public Image Image { set => this.frame = value; }
        public PointF TextPosition { get; set; } = new PointF(-1, -1);
        public String Text { get; set; }
        public Font Font { get; set; } = new Font("Arial", 18);
        public Brush Brush { get; set; } = new SolidBrush(Color.White);

        public void Draw(Graphics Canvas, PointF Start)
        {
            PointF Position = new PointF(Start.X + this.Location.X, Start.Y + this.Location.Y);
            if (this.Frame != null)
            {
                Canvas.DrawImage(this.Frame, Position.X, Position.Y, this.Size.Width, this.Size.Height);
            }
            var TextSize = TextRenderer.MeasureText(this.Text, this.Font);
            PointF Point = new PointF(
                (this.TextPosition.X < 0) ?
                    Position.X + this.Size.Width / 2 - TextSize.Width / 2 
                    : this.TextPosition.X,
                (this.TextPosition.Y < 0) ?
                    Position.Y + this.Size.Height / 2 - TextSize.Height / 2
                    : this.TextPosition.Y
                );
            Canvas.DrawString(this.Text, this.Font, this.Brush, Point);
        }
    }
}
