using ReactGame.Engine.Units;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactGame.Engine.Items
{
    class TextItem : IUnit, Units.IStringUnit, Units.IGraphicUnit, Units.IKeyInputUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public Int32 Layer { get; set; }
        private StringBuilder value = new StringBuilder();
        public PointF TextPosition { get; set; } = new PointF(-1, -1);
        public String Text { 
            get => this.value.ToString(); 
            set => this.value = new StringBuilder(value); 
        }
        public Font Font { get; set; } = new Font("Verdana", 18);
        public Brush Brush { get; set; } = new SolidBrush(Color.Yellow);

        private Image frame = null;
        public Image Frame => this.frame;
        public Image Image { set => this.frame = value; }

        public event Action<KeyEventArgs> OnKeyDown;
        public event Action<KeyEventArgs> OnKeyUp;

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

        public void KeyDown(KeyEventArgs Key)
        {
            this.OnKeyDown?.Invoke(Key);
        }

        public void KeyUp(KeyEventArgs Key)
        {
            if (Key.KeyData == Keys.Back)
            {
                if (this.value.Length > 0)
                    this.value.Remove(this.value.Length - 1, 1);
            }
            else
            {
                var Item = Global.getChar(Key);
                if (Item != default && this.value.Length < 18)
                    this.value.Append(Item);
            }
            this.OnKeyUp?.Invoke(Key);
        }
    }
}
