using System;
using System.Drawing;
using System.Threading;

using ReactGame.Engine.Items;

namespace ReactGame.Levels.Shared
{
    public class SubMenu : ALevel
    {
        public Boolean IsEnd = false;
        public override void Loading()
        {
            int Chance = new Random().Next(0, 100);
            int Rand = (Chance < 50) ? 1 : 2;
            Image Back = (Image)MainMenu.Instance.Background.Clone();
            this.LoadPercent += 10;
            Bitmap Button = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_default.png");
            this.LoadPercent += 10;
            Bitmap Button1 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_hover.png");
            this.LoadPercent += 10;
            Bitmap Button2 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_down.png");
            this.LoadPercent += 10;

            float Width = 300;
            float Heigth = 60;
            float Left = Options.CanvasWidth / 2 - Width / 2;
            float Top = Options.CanvasHeight / 2 - Heigth / 2;
            float padding = 12;

            ImageItem Background = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth, Options.CanvasHeight),
                Location = new PointF(),
                Image = Back
            };
            this.LoadPercent += 10;

            ButtonItem MainMenuButton = new ButtonItem()
            {
                Text = "Главное меню",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, Top - Heigth / 2 - padding - Heigth),
                Layer = 1
            };
            MainMenuButton.Images["default"] = Button;
            MainMenuButton.Images["hover"] = Button1;
            MainMenuButton.Images["down"] = Button2;
            MainMenuButton.OnMouseUp += MainMenuButton_OnMouseUp;
            this.LoadPercent += 15;

            ButtonItem ResumeButton = new ButtonItem()
            {
                Text = "Продолжить",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, Top - Heigth / 2),
                Layer = 1
            };
            ResumeButton.Images["default"] = Button;
            ResumeButton.Images["hover"] = Button1;
            ResumeButton.Images["down"] = Button2;
            ResumeButton.OnMouseUp += ResumeButton_OnMouseUp;
            this.LoadPercent += 15;

            ButtonItem ExitButton = new ButtonItem()
            {
                Text = "Выйти из игры",
                Size = new SizeF(Width, Heigth),
                Location = new PointF(Left, ResumeButton.Location.Y + Heigth + padding),
                Layer = 1
            };
            ExitButton.Images["default"] = Button;
            ExitButton.Images["hover"] = Button1;
            ExitButton.Images["down"] = Button2;
            ExitButton.OnMouseUp += ExitButton_OnMouseUp;

            this.LoadPercent += 10;

            MainMenuButton.UpdateFrame();
            ResumeButton.UpdateFrame();
            ExitButton.UpdateFrame();

            this.Units.Add(Background);
            this.Units.Add(ResumeButton);
            this.Units.Add(MainMenuButton);
            this.Units.Add(ExitButton);

            this.LoadPercent = 101;

            this.State = LevelSate.Working;
            this.IsLoaded = true;
        }

        public override void Work()
        {
            while (!this.IsEnd)
            {
                base.Work();
                Thread.Sleep(100);
            }
        }

        private void ResumeButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.State = Engine.CoreState.Game;
        }

        private void MainMenuButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            Engine.Core.Instance.State = Engine.CoreState.MainMenu;
        }

        private void ExitButton_OnMouseUp()
        {
            Form1.Instance.Close();
        }
    }
}
