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
    public class Level5 : ALevel
    {
        private Thread Logic = null;

        private Boolean IsEnd = false;
        private Stopwatch Timer = new Stopwatch();

        private List<IUnit> toView = new List<IUnit>();
        private List<SpecificImageItem> Stars = new List<SpecificImageItem>();
        private Int32 Selected_item = 0;
        private List<SpecificImageItem> Items = new List<SpecificImageItem>();

        private List<ImageItem> Lasers = new List<ImageItem>();
        private List<ImageItem> Shots = new List<ImageItem>();


        private Int32 max_shots = 6;
        private LabelItem Points = null;
        private LabelItem Time = null;
        private Int32 points = 0;
        private Int32 max_points = 500;

        public override void Loading()
        {
            this.LoadBasic();
            this.LoadEndForm();

            this.LoadPercent = 101;
            this.Timer.Start();
            this.State = LevelSate.Working;
            this.IsLoaded = true;
            this.Logic = new Thread(this.GameLogic);
            this.Logic.Start();
        }

        private void LoadBasic()
        {
            this.max_shots = Options.Level["normal"]["level5"]["max_shots"];
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

            Bitmap Laser_1 = new Bitmap(Environment.CurrentDirectory + "/Resources/Laser_1.png");
            this.LoadPercent += 5;
            Bitmap Laser_2 = new Bitmap(Environment.CurrentDirectory + "/Resources/Laser_2.png");
            this.LoadPercent += 5;
            Bitmap Lock = new Bitmap(Environment.CurrentDirectory + "/Resources/Lock.png");
            this.LoadPercent += 5;
            Bitmap UnLock = new Bitmap(Environment.CurrentDirectory + "/Resources/UnLock.png");
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
                Text = $"Успейте нажать на указанный элемент",
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
            //this.Units.Add(GameField);
            this.LoadPercent += 3;

            SpecificImageItem One = new SpecificImageItem()
            {
                Size = new SizeF(53, 69),
                Location = new PointF(GameField.Location.X, GameField.Location.Y),
                Layer = 2,
            };
            One.Images[true] = Lock;
            One.Images[false] = UnLock;
            One.Value = false;
            this.Units.Add(One);
            this.Items.Add(One);

            SpecificImageItem Two = new SpecificImageItem()
            {
                Size = new SizeF(53, 69),
                Location = new PointF(GameField.Location.X, GameField.Location.Y + GameField.Size.Height / 2 - 69 / 2),
                Layer = 2,
            };
            Two.Images[true] = Lock;
            Two.Images[false] = UnLock;
            Two.Value = false;
            this.Units.Add(Two);
            this.Items.Add(Two);

            SpecificImageItem Three = new SpecificImageItem()
            {
                Size = new SizeF(53, 69),
                Location = new PointF(GameField.Location.X, GameField.Location.Y + GameField.Size.Height - 69),
                Layer = 2,
            };
            Three.Images[true] = Lock;
            Three.Images[false] = UnLock;
            Three.Value = false;
            this.Units.Add(Three);
            this.Items.Add(Three);

            ImageItem First_laser = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - One.Size.Width, 4),
                Location = new PointF(GameField.Location.X + One.Size.Width, One.Location.Y + One.Size.Height / 2 - 2),
                Image = Laser_1,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(First_laser);
            this.Lasers.Add(First_laser);

            ImageItem Second_laser = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - Two.Size.Width, 4),
                Location = new PointF(GameField.Location.X + Two.Size.Width, Two.Location.Y + Two.Size.Height / 2 - 2),
                Image = Laser_1,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(Second_laser);
            this.Lasers.Add(Second_laser);

            ImageItem Third_laser = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - Three.Size.Width, 4),
                Location = new PointF(GameField.Location.X + Three.Size.Width, Three.Location.Y + Three.Size.Height / 2 - 2),
                Image = Laser_1,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(Third_laser);
            this.Lasers.Add(Third_laser);

            ImageItem First_shot = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - One.Size.Width, 4),
                Location = new PointF(GameField.Location.X + One.Size.Width, One.Location.Y + One.Size.Height / 2 - 2),
                Image = Laser_2,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(First_shot);
            this.Shots.Add(First_shot);

            ImageItem Second_shot = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - Two.Size.Width, 4),
                Location = new PointF(GameField.Location.X + Two.Size.Width, Two.Location.Y + Two.Size.Height / 2 - 2),
                Image = Laser_2,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(Second_shot);
            this.Shots.Add(Second_shot);

            ImageItem Third_shot = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - GameField.Location.X - Three.Size.Width, 4),
                Location = new PointF(GameField.Location.X + Three.Size.Width, Three.Location.Y + Three.Size.Height / 2 - 2),
                Image = Laser_2,
                Layer = 2,
                State = UnitStates.Hidden
            };
            this.Units.Add(Third_shot);
            this.Shots.Add(Third_shot);

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

            this.max_points = this.max_shots * 50;
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

        private void GameLogic()
        {
            for(int i = 0; i < this.max_shots; i++)
            {
                this.Selected_item = new Random().Next(0, 3);
                Thread.Sleep(new Random().Next(1000, 1500));
                while (this.IsPause) { }
                this.Lasers[this.Selected_item].State = UnitStates.Visible;
                Thread.Sleep(Options.Level["normal"]["level5"]["laser_time"]);
                this.Lasers[this.Selected_item].State = UnitStates.Hidden;
                this.Shots[this.Selected_item].State = UnitStates.Visible;
                if (this.Items[this.Selected_item].Value)
                    this.points += 50;
                foreach(var Item in this.Items)
                {
                    if(Item != this.Items[this.Selected_item])
                        if(Item.Value)
                            this.points -= 25;
                }
                Thread.Sleep(200);
                this.Shots[this.Selected_item].State = UnitStates.Hidden;
                Thread.Sleep(500);
                foreach(var Item in this.Items)
                    Item.Value = false;
            }
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
            double sec = this.Timer.ElapsedMilliseconds / 1000.0;
            this.Time.State = UnitStates.Visible;
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

            this.Time.Text = Math.Round(sec, 2).ToString() + " c.";
            this.Points.Text = (this.points).ToString();

            double[] Old = ProfileWorker.LoadProfileData(Engine.Core.Instance.ProfileName, "level5");
            if (Old?.Length == 2)
            {
                this.Points.Text += $" Max: {Old[0]}";
                this.Time.Text += $" Max: {Math.Round(Old[1])}";
                if (Old[0] < this.points)
                    ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level5", new double[] { this.points, sec });
            }
            else
            {
                ProfileWorker.SaveProfileData(Engine.Core.Instance.ProfileName, "level5", new double[] { this.points, sec });
            }
        }

        private void NextButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            Engine.Core.Instance.State = Engine.CoreState.MainMenu;
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
            this.Logic?.Abort();
            this.Timer.Stop();
            this.Timer.Reset();
            this.toView.Clear();
            this.Stars.Clear();
            this.Items.Clear();
            this.Lasers.Clear();
            this.Shots.Clear();
            this.Points = null;
            this.Time = null;
            this.IsEnd = false;
            this.points = 0;
            this.max_points = 0;
        }

        public override IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = base.MouseUp(Mouse, X, Y);
            if(!this.IsEnd && Unit != null)
            {
                var Item = this.Items.Find(x => x == Unit);
                if(Item != null)
                    Item.Value = true;
            }
            return Unit;
        }

    }
}
