using ReactGame.Engine;
using ReactGame.Engine.Items;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactGame.Levels
{
    public class RoundItem : IUnit
    {
        private Boolean ToChange = false;
        private Thread Timer = null;
        public UnitStates State { get; set; }
        public PointF Location { get; set; }
        public SizeF Size { get; set; }
        public int Layer { get; set; }
        public Brush Brush { get; set; } = new SolidBrush(Color.Green);
        public Brush NextBrush { get; set; } = new SolidBrush(Color.Yellow);
        public void Start()
        {
            Timer = new Thread(this.ChangeColor);
            Timer.Start();
        }
        private void ChangeColor()
        {
            int sec = new Random().Next(2, 3);
            while (sec > 0)
            {
                this.ToChange = true;
                sec--;
                Thread.Sleep(1000);
            }
            this.Brush = this.NextBrush;
            this.ColorChanged?.Invoke();
            this.ToChange = false;
        }
        public void Draw(Graphics Canvas, PointF Start)
        {
            Canvas.FillEllipse(this.Brush, new RectangleF(this.Location, this.Size));
        }

        public void MouseUp()
        {
            if(!this.ToChange)
                this.OnMouseUp?.Invoke();
        }

        public event Action OnMouseUp;
        public event Action ColorChanged;
    }

    public class Level2 : ALevel
    {
        private Boolean IsEnd = false;
        private Stopwatch Timer = new Stopwatch();
        private List<Double> Result = new List<Double>();
        private L1SpecificGridItem Grid2 = null;

        private List<IUnit> toView = new List<IUnit>();
        private List<SpecificImageItem> Stars = new List<SpecificImageItem>();

        private Int32 points = 0;
        private Int32 count_pops = 3;
        private Int32 max_points = 0;
        private Double sec_res = 0.3;

        private LabelItem Points = null;
        private LabelItem Time = null;

        private RoundItem ItemToClick = null;
        private Int32 clicks = 0;

        private Int32 active = 0;
        private List<Brush> Items = new List<Brush>();
        public override void Loading()
        {
            this.LoadBasic();
            this.LoadEndForm();

            this.LoadPercent = 101;
            this.State = LevelSate.Working;
            this.ItemToClick.Start();
            this.IsLoaded = true;
        }

        private void LoadBasic()
        {
            this.count_pops = Options.Level["normal"]["level2"]["pops"];
            this.sec_res = Options.Level["normal"]["level2"]["sec_res"] / 10.0;
            Bitmap Back = new Bitmap(Environment.CurrentDirectory + $"/Resources/Level_3_background.jpg");
            this.LoadPercent += 5;
            Bitmap Button = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_default.png");
            this.LoadPercent += 5;
            Bitmap Button1 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_hover.png");
            this.LoadPercent += 5;
            Bitmap Button2 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_down.png");
            this.LoadPercent += 5;
            Bitmap TextBackground = new Bitmap(Environment.CurrentDirectory + "/Resources/Text_background.png");
            this.LoadPercent += 5;

            ImageItem Background = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth, Options.CanvasHeight),
                Location = new PointF(),
                Image = Back
            };
            this.Units.Add(Background);
            this.LoadPercent += 3;

            LabelItem Query = new LabelItem()
            {
                Text = "Нажать на кружок при смене цвета",
                Size = new SizeF(Options.CanvasWidth - 48, 48),
                Location = new PointF(24, 24),
                Image = TextBackground,
                Font = new Font("Verdana", 14),
                Layer = 1
            };
            this.Units.Add(Query);
            this.LoadPercent += 3;

            ImageItem Result = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - 48, 48),
                Location = new PointF(24, Options.CanvasHeight - 24 - 48),
                Image = TextBackground,
                Layer = 1
            };
            this.Units.Add(Result);
            this.LoadPercent += 3;

            ImageItem GameField = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - 48, Options.CanvasHeight - 48 - Query.Size.Height - Result.Size.Height - 48),
                Location = new PointF(24, Query.Location.Y + Query.Size.Height + 24),
                Image = TextBackground,
                Layer = 1
            };
            this.Units.Add(GameField);
            this.LoadPercent += 3;

            this.Grid2 = new L1SpecificGridItem()
            {
                Location = Result.Location,
                Layer = 2,
                Cols = this.count_pops,
                Rows = 1
            };
            this.Grid2.Size = Result.Size;
            this.Units.Add(this.Grid2);
            this.max_points = this.count_pops * 20;

            this.ItemToClick = new RoundItem()
            {
                Location = new PointF(GameField.Location.X + GameField.Size.Width / 2 - 30, GameField.Location.Y + GameField.Size.Height / 2 - 30),
                Size = new SizeF(60, 60),
                Layer = 3,
                Brush = new SolidBrush(Color.Yellow)
            };
            this.ItemToClick.ColorChanged += ItemToClick_ColorChanged;
            this.ItemToClick.OnMouseUp += ItemToClick_OnMouseUp;
            this.Units.Add(this.ItemToClick);

            for (int i = 1; i < this.count_pops + 1; i++)
                if (i % 2 == 0)
                    this.Items.Add(new SolidBrush(Color.Yellow));
                else
                    this.Items.Add(new SolidBrush(Color.Green));
            this.ItemToClick.NextBrush = this.Items[this.active];
            this.active++;

            float BWidth = 200;
            float BHeigth = 60;
            float padding = 12;

            ButtonItem MenuButton = new ButtonItem()
            {
                Text = "Главное меню",
                Size = new SizeF(BWidth, BHeigth),
                Location = new PointF(90, Options.CanvasHeight - BHeigth - padding),
                Layer = 6,
                State = UnitStates.Hidden
            };
            MenuButton.Images["default"] = Button;
            MenuButton.Images["hover"] = Button1;
            MenuButton.Images["down"] = Button2;
            MenuButton.OnMouseUp += MenuButton_OnMouseUp;
            this.LoadPercent += 5;

            ButtonItem NextButton = new ButtonItem()
            {
                Text = "Следующий",
                Size = new SizeF(BWidth, BHeigth),
                Location = new PointF(MenuButton.Location.X + MenuButton.Size.Width + 60, Options.CanvasHeight - BHeigth - padding),
                Layer = 6,
                State = UnitStates.Hidden
            };
            NextButton.Images["default"] = Button;
            NextButton.Images["hover"] = Button1;
            NextButton.Images["down"] = Button2;
            NextButton.OnMouseUp += NextButton_OnMouseUp;
            this.LoadPercent += 5;

            NextButton.UpdateFrame();
            MenuButton.UpdateFrame();

            this.Units.Add(NextButton);
            this.Units.Add(MenuButton);
            this.toView.Add(NextButton);
            this.toView.Add(MenuButton);
        }

        private void LoadEndForm()
        {
            Bitmap EndLevel = new Bitmap(Environment.CurrentDirectory + "/Resources/End_level.png");
            this.LoadPercent += 5;
            Bitmap Raiting = new Bitmap(Environment.CurrentDirectory + "/Resources/Raiting.png");
            this.LoadPercent += 5;
            Bitmap TimeRaiting = new Bitmap(Environment.CurrentDirectory + "/Resources/Time_raiting.png");
            this.LoadPercent += 5;
            Bitmap StarOn = new Bitmap(Environment.CurrentDirectory + "/Resources/star_on.png");
            this.LoadPercent += 5;
            Bitmap StarOff = new Bitmap(Environment.CurrentDirectory + "/Resources/star_off.png");
            this.LoadPercent += 5;
            Bitmap End_back = new Bitmap(Environment.CurrentDirectory + "/Resources/End_background.png");

            ImageItem End_background = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth, Options.CanvasHeight),
                Location = new PointF(),
                Image = End_back,
                Layer = 3,
                State = UnitStates.Hidden
            };
            this.Units.Add(End_background);
            this.toView.Add(End_background);

            ImageItem End = new ImageItem()
            {
                Size = new SizeF(EndLevel.Width, EndLevel.Height),
                Location = new PointF(Options.CanvasWidth / 2 - EndLevel.Width / 2, Options.CanvasHeight / 2 - EndLevel.Height / 2),
                Image = EndLevel,
                Layer = 4,
                State = UnitStates.Hidden
            };
            this.Units.Add(End);
            this.toView.Add(End);
            this.LoadPercent += 3;

            SpecificImageItem Star1 = new SpecificImageItem()
            {
                Size = new SizeF(95, 95),
                Location = new PointF(End.Location.X + 67, End.Location.Y + 67),
                Layer = 5,
                State = UnitStates.Hidden
            };
            Star1.Images[true] = StarOn;
            Star1.Images[false] = StarOff;
            this.Units.Add(Star1);
            this.Stars.Add(Star1);
            this.LoadPercent += 3;

            SpecificImageItem Star2 = new SpecificImageItem()
            {
                Size = new SizeF(95, 95),
                Location = new PointF(Star1.Location.X + Star1.Size.Width + 24, End.Location.Y + 67),
                Layer = 5,
                State = UnitStates.Hidden
            };
            Star2.Images[true] = StarOn;
            Star2.Images[false] = StarOff;
            this.Units.Add(Star2);
            this.Stars.Add(Star2);
            this.LoadPercent += 3;

            SpecificImageItem Star3 = new SpecificImageItem()
            {
                Size = new SizeF(95, 95),
                Location = new PointF(Star2.Location.X + Star2.Size.Width + 24, End.Location.Y + 67),
                Layer = 5,
                State = UnitStates.Hidden
            };
            Star3.Images[true] = StarOn;
            Star3.Images[false] = StarOff;
            this.Units.Add(Star3);
            this.Stars.Add(Star3);
            this.LoadPercent += 3;

            this.Points = new LabelItem()
            {
                Text = "",
                Size = new SizeF(End.Size.Width - 67 - 67, 80),
                Location = new PointF(Star1.Location.X, Star3.Location.Y + Star3.Size.Height + 24),
                TextPosition = new PointF(Star1.Location.X + 100, -1),
                Image = Raiting,
                Layer = 5,
                Brush = new SolidBrush(Color.Yellow),
                State = UnitStates.Hidden
            };
            this.Units.Add(Points);
            this.LoadPercent += 3;

            this.Time = new LabelItem()
            {
                Text = "",
                Size = new SizeF(End.Size.Width - 67 - 67, 80),
                Location = new PointF(this.Points.Location.X, this.Points.Location.Y + this.Points.Size.Height + 12),
                TextPosition = this.Points.TextPosition,
                Image = TimeRaiting,
                Layer = 5,
                Brush = new SolidBrush(Color.Yellow),
                State = UnitStates.Hidden
            };
            this.Units.Add(Time);
            this.LoadPercent += 3;

        }

        private void NextButton_OnMouseUp()
        {
            this.State = LevelSate.End;
        }

        private void MenuButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            Engine.Core.Instance.State = Engine.CoreState.MainMenu;
        }

        private void ItemToClick_ColorChanged()
        {
            this.Timer.Restart();
        }

        private void ItemToClick_OnMouseUp()
        {
            this.Timer.Stop();
            double sec = this.Timer.ElapsedMilliseconds / 1000.0;
            this.points += (int)((20 * this.sec_res / sec + 0.5) / this.clicks);
            this.clicks = 0;
            if (this.active < this.Items.Count)
            {
                this.Result.Add(sec);
                this.Grid2.SafeAdd(new LabelItem()
                {
                    Text = Math.Round(sec, 2).ToString(),
                    Size = new SizeF(this.Grid2.ColumnWidth, this.Grid2.RowHeight),
                    Location = new PointF(),
                    Font = new Font("Verdana", 12),
                    Brush = new SolidBrush(Color.Yellow)
                });
                this.ItemToClick.NextBrush = this.Items[this.active];
                this.active++;
                this.ItemToClick.Start();
            }
            else
                this.Ending();
        }

        private void Ending()
        {
            this.IsEnd = true;
            foreach (var Item in this.toView)
                Item.State = UnitStates.Visible;
            foreach (var Item in this.Stars)
            {
                Item.State = UnitStates.Visible;
                Item.Value = false;
            }
            this.Time.State = UnitStates.Visible;
            double avg = 0;
            foreach (var Item in this.Result)
                avg += Item;
            avg /= this.Result.Count;
            this.Points.State = UnitStates.Visible;
            if (this.points * 100.0 / this.max_points > 30)
            {
                this.Stars[0].Value = true;
                if (this.points * 100.0 / this.max_points > 60)
                {
                    this.Stars[1].Value = true;
                    if (this.points * 100.0 / this.max_points > 90)
                    {
                        this.Stars[2].Value = true;
                    }
                }
            }

            this.Time.Text = Math.Round(avg, 2).ToString() + " ср. c.";
            this.Points.Text = (this.points).ToString();

            double[] Old = ProfileWorker.LoadProfileData(Engine.Core.Instance.ProfileName, "level2");
            if (Old?.Length == 2)
            {
                this.Points.Text += $" Max: {Old[0]}";
                this.Time.Text += $" Max: {Math.Round(Old[1])}";
                if (Old[0] < this.points)
                    ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level2", new double[] { this.points, avg });
            }
            else
            {
                ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level2", new double[] { this.points, avg });
            }
        }

        public override void Free()
        {
            base.Free();
            this.Timer.Stop();
            this.Timer.Reset();
            this.Result.Clear();
            this.Items.Clear();
            this.Grid2?.Items.Clear();
            this.toView.Clear();
            this.Stars.Clear();
            this.points = 0;
            this.max_points = 0;
            this.Points = null;
            this.Time = null;
            this.IsEnd = false;
            this.active = 0;
            this.clicks = 0;
        }

        public override Engine.IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = base.MouseUp(Mouse, X, Y);
            if (Unit != null && !this.IsEnd && Mouse == MouseButtons.Left)
            {
                if (Unit == this.ItemToClick)
                {
                    this.clicks++;
                    this.ItemToClick.MouseUp();
                }
            }
            return Unit;
        }

    }
}
