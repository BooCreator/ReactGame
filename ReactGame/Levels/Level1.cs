using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ReactGame.Engine;
using ReactGame.Engine.Items;
using ReactGame.Engine.Units;

namespace ReactGame.Levels
{

    public class L1SpecificGridItem : Engine.Items.GridItem
    {
        private Boolean isDraw = false;
        private Int32 result = -1;
        public Int32 LastResult { 
            get => this.result; 
            set {
                this.result = value;
                this.LastResultChanged?.Invoke();
            } 
        }
        public List<IUnit> Items { get; } = new List<IUnit>();
        public void SafeAdd(IUnit Item)
        {
            while(this.isDraw) { }
            this.Items.Add(Item);
        }
        public override void Draw(Graphics Canvas, PointF Start)
        {
            base.Draw(Canvas, Start);
            int row = 0;
            int col = 0;
            this.isDraw = true;
            foreach (var Item in Items)
            {
                Item.Draw(Canvas, new PointF(this.Location.X + col * this.ColumnWidth, this.Location.Y + this.RowHeight * row));
                col++;
                if(col >= this.Cols)
                {
                    col = 0;
                    row++;
                }
            }
            this.isDraw = false;
            Thread.Sleep(10);
        }

        public void MouseUp(Int32 X, Int32 Y)
        {
            if(X > -1 && Y > -1)
            {
                int row = (int)(Y / this.RowHeight);
                int col = (int)(X / this.ColumnWidth);
                int id = (int)(this.Cols * row + col);
                if (id < this.Items.Count)
                {
                    var Str = this.Items[id] as IStringUnit;
                    if (Str != null)
                    {
                        this.LastResult = (Int32.TryParse(Str.Text, out int res)) ? res : id;
                    }
                }
            }
        }

        public event Action LastResultChanged;
    }

    public class SpecificImageItem : ImageItem
    {
        private Boolean value = false;
        public Boolean Value {
            get => this.value;
            set
            {
                this.value = value;
                this.Image = this.Images[this.value];
            }
        }
        public Dictionary<Boolean, Image> Images = new Dictionary<Boolean, Image>()
        {
            { true, null },
            { false, null }
        };
    }

    public class Level1 : ALevel
    {
        private Boolean IsEnd = false;
        private Stopwatch Timer = new Stopwatch();
        private List<Int32> Result = new List<Int32>();
        private List<Int32> ClickResult = new List<Int32>();
        private L1SpecificGridItem Grid = null;
        private L1SpecificGridItem Grid2 = null;

        private List<IUnit> toView = new List<IUnit>();
        private List<SpecificImageItem> Stars = new List<SpecificImageItem>();

        private Int32 points = 0;
        private Int32 max_points = 0;
        private Double sec_res = 30;

        private LabelItem Points = null;
        private LabelItem Time = null;
        public override void Loading()
        {
            this.LoadBasic();
            this.LoadEndForm();

            this.LoadPercent = 101;
            this.Timer.Start();
            this.State = LevelSate.Working;
            this.IsLoaded = true;
        }

        private void LoadBasic()
        {
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
                Text = "Выберите все чётные числа в порядке возрастания",
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

            this.Grid = new L1SpecificGridItem()
            {
                Location = GameField.Location,
                Layer = 2,
                Cols = Options.Level["normal"]["level1"]["cols"],
                Rows = Options.Level["normal"]["level1"]["rows"]
            };
            this.sec_res = this.Grid.Cols * this.Grid.Rows;
            this.Grid.Size = GameField.Size;
            for (int i = 1; i <= this.Grid.Cols * this.Grid.Rows; i++)
            {
                this.Grid.Items.Add(new LabelItem()
                {
                    Text = i.ToString(),
                    Size = new SizeF(this.Grid.ColumnWidth, this.Grid.RowHeight),
                    Location = new PointF(),
                    Font = new Font("Verdana", 14),
                    Brush = new SolidBrush(Color.Yellow)
                });
                if (i % 2 == 0)
                {
                    this.Result.Add(i);
                    this.max_points += 20;
                }
            }
            this.Grid.Items.UnSort<IUnit>();
            this.Grid.LastResultChanged += Grid_LastResultChanged;
            this.Units.Add(this.Grid);
            this.LoadPercent += 7;

            this.Grid2 = new L1SpecificGridItem()
            {
                Location = Result.Location,
                Layer = 2,
                Cols = this.Grid.Cols * this.Grid.Rows / 2,
                Rows = 1
            };
            this.Grid2.Size = Result.Size;
            this.Units.Add(this.Grid2);

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

        private void Grid_LastResultChanged()
        {
            int value = this.Grid.LastResult;
            if(value > -1)
            {
                int id = this.ClickResult.FindIndex(x => x == value);
                if (id > -1)
                {
                    this.ClickResult.Remove(value);
                    var Item = this.Grid2.Items.Find(x => (x as LabelItem)?.Text.CompareTo(value.ToString()) == 0);
                    if(Item != null)
                        this.Grid2.Items.Remove(Item);
                    this.points -= 20;
                } 
                else if(this.Grid2.Items.Count < this.Grid2.Cols) 
                {
                    this.ClickResult.Add(value);
                    this.Grid2.SafeAdd(new LabelItem()
                    {
                        Text = value.ToString(),
                        Size = new SizeF(this.Grid2.ColumnWidth, this.Grid2.RowHeight),
                        Location = new PointF(),
                        Font = new Font("Verdana", 12),
                        Brush = new SolidBrush(Color.Yellow)
                    });
                    this.points += 20;
                }
            }
            if(this.ClickResult.Count == this.Result.Count)
            {
                this.Result.Sort();
                bool result = true;
                for (int i = 0; i < this.Result.Count; i++)
                {
                    if(this.Result[i] != this.ClickResult[i])
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                    this.Ending();
            }

        }

        private void Ending()
        {
            this.IsEnd = true;
            this.Timer.Stop();
            foreach(var Item in this.toView)
                Item.State = UnitStates.Visible;
            foreach (var Item in this.Stars)
            {
                Item.State = UnitStates.Visible;
                Item.Value = false;
            }
            double sec = this.Timer.ElapsedMilliseconds / 1000.0;
            this.Time.State = UnitStates.Visible;
            this.Points.State = UnitStates.Visible;
            this.points = (int)(this.points * this.sec_res / sec + 0.5);
            if (this.points * 100.0 / this.max_points > 30)
            {
                this.Stars[0].Value = true;
                if(this.points * 100.0 / this.max_points > 60)
                {
                    this.Stars[1].Value = true;
                    if (this.points * 100.0 / this.max_points > 90)
                    {
                        this.Stars[2].Value = true;
                    }
                }
            }

            this.Points.Text = (this.points).ToString();
            this.Time.Text = Math.Round(sec, 2).ToString() + " c.";

            double[] Old = ProfileWorker.LoadProfileData(Engine.Core.Instance.ProfileName, "level1");
            if(Old?.Length == 2)
            {
                this.Points.Text += $" Max: {Old[0]}";
                this.Time.Text += $" Max: {Math.Round(Old[1])}";
                if(Old[0] < this.points)
                    ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level1", new double[] { this.points, sec });
            } else
            {
                ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level1", new double[] { this.points, sec });
            }
            
        }

        public override void Free()
        {
            base.Free();
            this.Timer.Stop();
            this.Timer.Reset();
            this.Result.Clear();
            this.ClickResult.Clear();
            this.Grid?.Items.Clear();
            this.Grid2?.Items.Clear();
            this.toView.Clear();
            this.Stars.Clear();
            this.points = 0;
            this.max_points = 0;
            this.Points = null;
            this.Time = null;
            this.IsEnd = false;
        }

        public override Engine.IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = base.MouseUp(Mouse, X, Y);
            if (!this.IsEnd && Mouse == MouseButtons.Left)
            {
                this.Grid.MouseUp(X - (int)this.Grid.Location.X, Y - (int)this.Grid.Location.Y);
            }
            return Unit;
        }

    }
}
