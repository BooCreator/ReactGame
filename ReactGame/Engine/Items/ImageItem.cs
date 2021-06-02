using ReactGame.Engine.Units;

using System.Drawing;

namespace ReactGame.Engine.Items
{
    public class ImageItem: IUnit, IGraphicUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; }

        private Image frame = null;
        public Image Frame => this.frame;
        public Image Image { set => this.frame = value; }
        public virtual void Draw(Graphics Canvas, PointF Start)
        {
            if (this.Frame != null)
            {
                PointF Position = new PointF(Start.X + this.Location.X, Start.Y + this.Location.Y);
                Canvas.DrawImage(this.Frame, Position.X, Position.Y, this.Size.Width, this.Size.Height);
            }
        }
    }
}
