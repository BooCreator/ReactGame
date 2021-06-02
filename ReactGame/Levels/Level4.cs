﻿using ReactGame.Engine;
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
    public class Level4 : ALevel
    {
        private Boolean IsEnd = false;
        private Stopwatch Timer = new Stopwatch();

        private List<IUnit> toView = new List<IUnit>();
        private List<SpecificImageItem> Stars = new List<SpecificImageItem>();
        private L1SpecificGridItem Grid = null;
        private L1SpecificGridItem Grid2 = null;

        private Int32 ViewItems = 5;
        private Int32 Result = 0;
        private Int32 points = 500;
        private Int32 max_points = 500;
        private Double sec_res = 5;

        private LabelItem Points = null;
        private LabelItem Time = null;

        private List<Int32> Items = new List<Int32>();

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
            this.sec_res = Options.Level["normal"]["level4"]["sec_res"];
            this.ViewItems = Options.Level["normal"]["level4"]["view_items"];
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
                Text = $"Найдите недостающее число",
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
                Cols = Options.Level["normal"]["level4"]["cols"],
                Rows = Options.Level["normal"]["level4"]["rows"]
            };
            this.sec_res = (this.Grid.Cols * this.Grid.Rows) / 3;
            this.Grid.Size = GameField.Size;

            this.Result = new Random().Next(1, this.Grid.Cols * this.Grid.Rows);
            for (int i = 1; i <= this.Grid.Cols * this.Grid.Rows; i++)
            {
                if(i != this.Result)
                    this.Grid.Items.Add(new LabelItem()
                    {
                        Text = i.ToString(),
                        Size = new SizeF(this.Grid.ColumnWidth, this.Grid.RowHeight),
                        Location = new PointF(),
                        Font = new Font("Verdana", 14),
                        Brush = new SolidBrush(Color.Yellow)
                    });
                this.Items.Add(i);
            }
            this.Grid.Items.UnSort();
            this.Units.Add(this.Grid);
            this.LoadPercent += 7;

            this.Grid2 = new L1SpecificGridItem()
            {
                Location = Result.Location,
                Layer = 2,
                Cols = this.ViewItems,
                Rows = 1
            };
            this.Grid2.Size = Result.Size;
            this.Grid2.Items.Add(new LabelItem()
            {
                Text = this.Result.ToString(),
                Size = new SizeF(this.Grid2.ColumnWidth, this.Grid2.RowHeight),
                Location = new PointF(),
                Font = new Font("Verdana", 14),
                Brush = new SolidBrush(Color.Yellow)
            });
            List<int> RandomList = new List<int>();
            this.Items.UnSort();
            int rand = new Random().Next(0, this.Items.Count);
            for (int i = 1; i < this.ViewItems; i++)
            {
                while (RandomList.FindIndex(x => x == rand) > -1) {
                    rand = new Random().Next(0, this.Items.Count);
                }
                RandomList.Add(rand);
                int val = this.Items[rand];
                this.Grid2.Items.Add(new LabelItem()
                {
                    Text = val.ToString(),
                    Size = new SizeF(this.Grid2.ColumnWidth, this.Grid2.RowHeight),
                    Location = new PointF(),
                    Font = new Font("Verdana", 14),
                    Brush = new SolidBrush(Color.Yellow)
                });

            }
            this.Grid2.Items.UnSort();
            this.Grid2.LastResultChanged += Grid_LastResultChanged;
            this.Units.Add(this.Grid2);
            this.LoadPercent += 7;

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

        private void Grid_LastResultChanged()
        {
            if (this.Grid2.LastResult == this.Result)
            {
                this.Ending();
            }
            else
                this.points -= (int)(this.points * 0.80 + 0.5);
        }

        private void Ending()
        {
            this.IsEnd = true;
            this.Timer.Stop();
            foreach (var Item in this.toView)
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
                if (this.points * 100.0 / this.max_points > 60)
                {
                    this.Stars[1].Value = true;
                    if (this.points * 100.0 / this.max_points > 90)
                    {
                        this.Stars[2].Value = true;
                    }
                }
            }

            this.Time.Text = Math.Round(sec, 2).ToString() + " c.";
            this.Points.Text = (this.points).ToString();

            double[] Old = ProfileWorker.LoadProfileData(Engine.Core.Instance.ProfileName, "level4");
            if (Old?.Length == 2)
            {
                this.Points.Text += $" Max: {Old[0]}";
                this.Time.Text += $" Max: {Math.Round(Old[1])}";
                if (Old[0] < this.points)
                    ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level4", new double[] { this.points, sec });
            }
            else
            {
                ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level4", new double[] { this.points, sec });
            }
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

        public override void Free()
        {
            base.Free();
            this.Timer.Stop();
            this.Timer.Reset();
            this.Grid?.Items.Clear();
            this.Grid2?.Items.Clear();
            this.Items.Clear();
            this.toView.Clear();
            this.Stars.Clear();
            this.Points = null;
            this.Time = null;
            this.IsEnd = false;
            this.points = this.max_points;
        }

        public override Engine.IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = base.MouseUp(Mouse, X, Y);
            if (!this.IsEnd && Mouse == MouseButtons.Left && this.Grid2 != null)
            {
                this.Grid2.MouseUp(X - (int)this.Grid2.Location.X, Y - (int)this.Grid2.Location.Y);
            }
            return Unit;
        }
    
    }
}
