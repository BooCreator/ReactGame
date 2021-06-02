using ReactGame.Engine.Items;
using ReactGame.Engine.Units;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ReactGame.Levels
{
    public abstract class ALevel : ILevel
    {
        public Boolean IsLoaded { get; set; } = false;
        protected static object locker = new object();
        protected Thread Worker = null;
        protected Thread Loader = null;

        Engine.IUnit HoverUnit = null;
        protected ProgressItem ProgressUnit = null;
        protected Engine.UnitController Units = new Engine.UnitController();

        protected Boolean IsPause = false;
        private LevelSate state = LevelSate.Stop;
        public LevelSate State {
            get => this.state;
            set {
                this.state = value;
            }
        }

        protected Int32 LoadPercent = 0;

        protected Graphics Canvas = null;
        protected Graphics Temp_Canvas = null;
        protected Image frame = null;
        protected Image last_frame = null;
        public Image Frame => this.last_frame;

        public virtual void Free()
        {
            this.Worker?.Abort();
            this.Loader?.Abort();
            this.Units.Clear();
            this.State = LevelSate.Stop;
            this.IsLoaded = false;
            this.IsPause = false;
            this.LoadPercent = 0;
        }

        public virtual void Work()
        {
            while (this.State != LevelSate.End)
            {
                if (!this.IsPause && this.State != LevelSate.Stop && this.State != LevelSate.End)
                {
                    if (this.State == LevelSate.Loading)
                    {
                        this.ProgressUnit.Percent = this.LoadPercent;
                        this.ProgressUnit.Draw(this.Canvas, new PointF());
                    }
                    else
                    {
                        this.Units.DrawAll(this.Canvas, new PointF(), new SizeF(Options.CanvasWidth, Options.CanvasHeight));
                    }

                    this.Temp_Canvas.DrawImage(this.frame, new PointF());
                    //this.last_frame = (Image)this.frame.Clone();
                }
                Thread.Sleep(10);
            }
        }
        public abstract void Loading();

        public virtual void KeyDown(KeyEventArgs Key) { }

        public virtual void KeyUp(KeyEventArgs Key) { }

        public virtual void Load()
        {
            this.frame = new Bitmap(Options.CanvasWidth, Options.CanvasHeight);
            this.last_frame = new Bitmap(Options.CanvasWidth, Options.CanvasHeight);
            this.ProgressUnit = new ProgressItem()
            {
                Size = new SizeF(this.frame.Width - 16, 20),
                Location = new PointF(8, this.frame.Height - 28)
            };
            this.Canvas = Graphics.FromImage(this.frame);
            this.Temp_Canvas = Graphics.FromImage(this.last_frame);
            this.Canvas.SmoothingMode = Options.SmoothingMode;
            this.Worker = new Thread(this.Work);
            this.Loader = new Thread(this.Loading);
            this.Worker.Start();
            this.Loader.Start();
            this.State = LevelSate.Loading;
        }

        public virtual Engine.IUnit MouseDown(MouseButtons Mouse, int X, int Y)
        {
            var Unit = this.Units.GetUnitOn(X, Y, new PointF());
            if (Mouse == MouseButtons.Left)
            {
                var SUnit = Unit as IMouseInputUnit;
                if (SUnit != null)
                    SUnit.MouseDown();
            }
            return Unit;
        }

        public virtual Engine.IUnit MouseMove(int X, int Y)
        {
            var Unit = this.Units.GetUnitOn(X, Y, new PointF());
            if (Unit != this.HoverUnit)
            {
                (this.HoverUnit as IMouseInputUnit)?.MouseLeave();
                this.HoverUnit = Unit;
                (this.HoverUnit as IMouseInputUnit)?.MouseEnter();
            }
            return Unit;
        }

        public virtual Engine.IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = this.Units.GetUnitOn(X, Y, new PointF());
            if (Mouse == MouseButtons.Left)
            {
                var SUnit = Unit as IMouseInputUnit;
                if (SUnit != null)
                    SUnit.MouseUp();
            }
            return Unit;
        }

        public void Pause()
        {
            this.IsPause = true;
        }

        public void Resume()
        {
            this.IsPause = false;
        }
    }
}
