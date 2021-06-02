using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactGame.Engine
{
    public class UnitController
    {
        private List<IUnit> Items = new List<IUnit>();

        private Int32 MaxLayer = 0;

        public void Add(IUnit Unit)
        { 
            this.Items.Add(Unit);
            if (Unit.Layer > MaxLayer)
                this.MaxLayer = Unit.Layer;
        }
        public void Remove(IUnit Unit)
            => this.Items.Remove(Unit);
        public void Clear()
            => this.Items.Clear();

        private Boolean UnitInField(IUnit Item, PointF Start, SizeF Size)
        {
            float X1 = Start.X + Item.Location.X;
            float X2 = Start.X + Item.Location.X + Item.Size.Width;
            float Y1 = Start.Y + Item.Location.Y;
            float Y2 = Start.Y + Item.Location.Y + Item.Size.Height;

            if (X2 >= 0 && X1 <= Size.Width || X2 >= 0 && X2 <= Size.Width || X1 <= 0 && X2 >= Size.Width)
            {
                if (Y1 >= 0 && Y1 <= Size.Height)
                    return true;
                else if (Y2 >= 0 && Y2 <= Size.Height)
                    return true;
                else if (Y1 <= 0 && Y2 >= Size.Height)
                    return true;
            }

            return false;
        }

        public void DrawAll(Graphics Canvas, PointF Start, SizeF Size)
        {
            for (int i = -1; i < MaxLayer; i++)
            {
                var Units = Items.FindAll(x => x.Layer == (i + 1));
                foreach (var Item in Units)
                {
                    if (Item.State != UnitStates.Hidden && UnitInField(Item, Start, Size))
                        Item.Draw(Canvas, Start);
                }
            }
        }

        public IUnit GetUnitOn(Int32 X, Int32 Y, PointF Start)
        {
            var Units = this.Items.FindAll(x => 
                x.Location.X < X && x.Location.Y < Y &&
                x.Location.X + x.Size.Width > X && x.Location.Y + x.Size.Height > Y
                && x.State == UnitStates.Visible
            );
            if (Units.Count == 1)
                return Units[0];
            if (Units.Count > 1)
            {
                int layer = Units[0].Layer;
                int id = 0;
                for(int i = 0; i < Units.Count; i++)
                {
                    if(Units[i].Layer > layer)
                    {
                        id = i;
                        layer = Units[i].Layer;
                    }
                }
                return Units[id];
            }
            return null;
        }

    }
}
