using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactGame.Engine.Items
{
    public class GridItem : IUnit
    {
        public UnitStates State { get; set; } = UnitStates.Visible;
        public PointF Location { get; set; }
        private SizeF size = new SizeF();
        public SizeF Size { 
            get => this.size; 
            set {
                this.size = value;
                if (this.Cols > 0)
                    this.ColumnWidth = this.Size.Width / this.Cols;
                if (this.Rows > 0)
                    this.RowHeight = this.Size.Height / this.Rows;
            }
        }
        public int Layer { get; set; }

        private Int32 cols = 0;
        public Int32 Cols { 
            get => this.cols; 
            set {
                if(this.Cols > 0)
                    this.ColumnWidth = this.Size.Width / this.Cols;
                this.cols = value;
            }
        }
        private Int32 rows = 0;
        public Int32 Rows
        {
            get => this.rows;
            set
            {
                if (this.Rows > 0)
                    this.RowHeight = this.Size.Height / this.Rows;
                this.rows = value;
            }
        }
        public Pen Border = new Pen(Color.White, 2);
        public Single ColumnWidth { get; private set; } = 0;
        public Single RowHeight { get; private set; } = 0;

        public virtual void Draw(Graphics Canvas, PointF Start)
        {
            PointF First = this.Location;
            
            for(int i = 1; i < Cols; i++)
            {
                First = new PointF(First.X + ColumnWidth, First.Y);
                Canvas.DrawLine(this.Border, new PointF(First.X, First.Y + 12), new PointF(First.X, this.Size.Height + First.Y - 12));
            }
            First = this.Location;
            for (int i = 1; i < Rows; i++)
            {
                First = new PointF(First.X, First.Y + RowHeight);
                Canvas.DrawLine(this.Border, new PointF(First.X + 12, First.Y), new PointF(this.Size.Width + First.X - 12, First.Y));
            }
            //Canvas.DrawRectangle(new Pen(Color.Yellow, 1), new Rectangle(new Point((int)this.Location.X, (int)this.Location.Y), new Size((int)this.Size.Width, (int)this.Size.Height)));
        }
    }
}
