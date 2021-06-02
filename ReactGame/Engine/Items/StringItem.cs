using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReactGame.Engine.Items
{
    public class StringItem : IUnit, Units.IStringUnit
    {
        private string text = "";
        public String Text { 
            get => this.text; 
            set {
                this.text = value;
                this.SetSize();
            }
        }
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; }

        public Font Font { get; set; } = new Font("Arial", 18);
        public Brush Brush { get; set; } = new SolidBrush(Color.Black);

        public StringItem(String Text)
        {
            this.Text = Text;
        }

        public void Draw(Graphics Canvas, PointF Start)
        {
            PointF Position = new PointF(Start.X + this.Location.X, Start.Y + this.Location.Y);
            Canvas.DrawString(this.Text, this.Font, this.Brush, Position);
        }

        private void SetSize()
        {
           this.Size = TextRenderer.MeasureText(this.Text, this.Font);
        }

    }
}
