using System;
using System.Drawing;
using System.Threading;

using ReactGame.Engine.Items;

namespace ReactGame.Levels.Shared
{
    public class MainMenu : ALevel
    {
        public static MainMenu Instance = null;
        public Boolean IsEnd = false;
        public Image Background = null;
        private LabelItem ProfileName = null;
        public MainMenu()
        {
            Instance = this;
        }
        public override void Loading()
        {
            int Chance = new Random().Next(0, 100);
            int Rand = (Chance < 50) ? 1 : 2;
            this.Background = new Bitmap(Environment.CurrentDirectory + $"/Resources/Main-{Rand}.jpg");
            this.LoadPercent += 5;
            Bitmap Button = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_default.png");
            this.LoadPercent += 5;
            Bitmap Button1 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_hover.png");
            this.LoadPercent += 5;
            Bitmap Button2 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_down.png");
            this.LoadPercent += 5;
            Bitmap TextBackground = new Bitmap(Environment.CurrentDirectory + "/Resources/Text_background.png");
            this.LoadPercent += 5;

            float Width = 300;
            float Heigth = 60;
            float Left = Options.CanvasWidth / 2 - Width / 2;
            float Top = Options.CanvasHeight / 2 - Heigth / 2;
            float padding = 12;

            ImageItem Background = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth, Options.CanvasHeight),
                Location = new PointF(),
                Image = this.Background
            };
            this.LoadPercent += 5;

            ButtonItem NewGameButton = new ButtonItem() {
                Text = "Начать игру",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, Top - Heigth - padding),
                Layer = 1
            };
            NewGameButton.Images["default"] = Button;
            NewGameButton.Images["hover"] = Button1;
            NewGameButton.Images["down"] = Button2;
            NewGameButton.OnMouseUp += NewGameButton_OnMouseUp;
            this.LoadPercent += 25;

            ButtonItem ProfileButton = new ButtonItem()
            {
                Text = "Профиль",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, NewGameButton.Location.Y + NewGameButton.Size.Height + padding),
                Layer = 1
            };
            ProfileButton.Images["default"] = Button;
            ProfileButton.Images["hover"] = Button1;
            ProfileButton.Images["down"] = Button2;
            ProfileButton.OnMouseUp += ProfileButton_OnMouseUp;

            this.LoadPercent += 25;

            ButtonItem ExitButton = new ButtonItem()
            {
                Text = "Выйти",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, ProfileButton.Location.Y + ProfileButton.Size.Height + padding),
                Layer = 1
            };
            ExitButton.Images["default"] = Button;
            ExitButton.Images["hover"] = Button1;
            ExitButton.Images["down"] = Button2;
            ExitButton.OnMouseUp += ExitButton_OnMouseUp;

            this.LoadPercent += 25;

            this.ProfileName = new LabelItem()
            {
                Text = Engine.Core.Instance.ProfileName,
                Size = new SizeF(Options.CanvasWidth / 2, 60),
                Location = new PointF(Options.CanvasWidth / 2 - Options.CanvasWidth / 2 / 2, NewGameButton.Location.Y / 2 - 30),
                Image = TextBackground,
                Layer = 3,
                Font = new Font("Verdana", 18),
                Brush = new SolidBrush(Color.Yellow)
            };
            Engine.Core.Instance.ProfileNameChanged += Core_PropfileNameChanged;

            LabelItem WarningInfo = new LabelItem()
            {
                Text = "Без активного профиля прогресс не сохраняется!",
                Size = new SizeF(Options.CanvasWidth - 40, 40),
                Location = new PointF(20, Options.CanvasHeight - 60),
                Image = TextBackground,
                Layer = 3,
                Font = new Font("Verdana", 12),
                Brush = new SolidBrush(Color.White)
            };

            NewGameButton.UpdateFrame();
            ProfileButton.UpdateFrame();
            ExitButton.UpdateFrame();

            this.Units.Add(Background);
            this.Units.Add(ProfileName);
            this.Units.Add(NewGameButton);
            this.Units.Add(ProfileButton);
            this.Units.Add(ExitButton);
            this.Units.Add(WarningInfo);

            this.LoadPercent = 101;

            this.State = LevelSate.Working;
            this.IsLoaded = true;
        }

        private void Core_PropfileNameChanged(String NewName)
        {
            this.ProfileName.Text = NewName;
        }
        private void ProfileButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.State = Engine.CoreState.Profile;
        }

        public override void Work()
        {
            while (!this.IsEnd)
            {
                base.Work();
                Thread.Sleep(100);
            }
        }

        private void NewGameButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            Engine.Core.Instance.State = Engine.CoreState.Game;
        }

        private void ExitButton_OnMouseUp()
        {
            Form1.Instance.Close();
        }

    }

}
