using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactGame.Engine.Items
{
    public class ButtonItem : IUnit, Units.IGraphicUnit, Units.IMouseInputUnit, Units.IStringUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; }

        public Dictionary<String, Image> Images = new Dictionary<String, Image>()
        {
            {"default", null },
            {"hover", null },
            {"down", null },
        };

        private Image frame = null;
        public Image Frame => this.frame;
        public void UpdateFrame()
            => this.frame = this.Images["default"];
        public String Text { get; set; }
        public Font Font { get; set; } = new Font("Arial", 18);
        public Brush Brush { get; set; } = new SolidBrush(Color.White);

        public void Draw(Graphics Canvas, PointF Start)
        {
            if (this.Frame != null)
            {
                PointF Position = new PointF(Start.X + this.Location.X, Start.Y + this.Location.Y);
                Canvas.DrawImage(this.Frame, Position.X, Position.Y, this.Size.Width, this.Size.Height);
                var TextSize = TextRenderer.MeasureText(this.Text, this.Font);
                Canvas.DrawString(this.Text, this.Font, this.Brush, new PointF(Position.X + this.Size.Width / 2 - TextSize.Width / 2, Position.Y + this.Size.Height / 2 - TextSize.Height / 2));
            }
        }

        public void MouseEnter()
        {
            this.frame = this.Images["hover"];
            this.OnMouseEnter?.Invoke();
        }
        public void MouseLeave()
        {
            this.frame = this.Images["default"];
            this.OnMouseLeave?.Invoke();
        }
        public void MouseDown()
        {
            this.frame = this.Images["down"];
            this.OnMouseDown?.Invoke();
        }
        public virtual void MouseUp()
        {
            this.frame = this.Images["hover"];
            this.OnMouseUp?.Invoke();
        }

        public event Action OnMouseEnter;
        public event Action OnMouseLeave;
        public event Action OnMouseDown;
        public event Action OnMouseUp;
    }
}
