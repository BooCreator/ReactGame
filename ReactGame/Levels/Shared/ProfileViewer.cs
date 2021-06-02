using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ReactGame.Engine;
using ReactGame.Engine.Items;
using ReactGame.Engine.Units;

namespace ReactGame.Levels.Shared
{
    public class SpecificImageItemWithText : SpecificImageItem, IStringUnit
    {
        public PointF TextPosition { get; set; } = new PointF(-1, -1);
        public String Text { get; set; }
        public Font Font { get; set; } = new Font("Verdana", 18);
        public Brush Brush { get; set; } = new SolidBrush(Color.Yellow);

        public override void Draw(Graphics Canvas, PointF Start)
        {
            base.Draw(Canvas, Start);
            PointF Position = new PointF(Start.X + this.Location.X, Start.Y + this.Location.Y);
            var TextSize = TextRenderer.MeasureText(this.Text, this.Font);
            PointF Point = new PointF(
                (this.TextPosition.X < 0) ?
                    Position.X + this.Size.Width / 2 - TextSize.Width / 2
                    : this.TextPosition.X,
                (this.TextPosition.Y < 0) ?
                    Position.Y + this.Size.Height / 2 - TextSize.Height / 2
                    : this.TextPosition.Y
                );
            Canvas.DrawString(this.Text, this.Font, this.Brush, Point);
        }
    }

    public class SpecificButtonItem : ButtonItem
    {
        public Int32 Index { get; set; } = -1;

        public override void MouseUp()
        {
            base.MouseUp();
            this.OnMouseUpWithIndex?.Invoke(this.Index);
        }

        public event Action<Int32> OnMouseUpWithIndex;
    }

    public class ProfileViewer : ALevel
    {
        public Boolean IsEnd = false;
        private TextItem TextBox = null;

        private Int32 ActiveProfile = -1;
        private List<String> Profiles = new List<String>();
        private L1SpecificGridItem ProfilesList = new L1SpecificGridItem();
        private List<SpecificButtonItem> RemoveButtons = new List<SpecificButtonItem>();
        private Image SelectedItem = null;
        private float MHeight = 0;
        private List<Image> MButtons = new List<Image>();
        private Int32 MaxProfiles = 5;
        public override void Loading()
        {
            Image Back = (Image)MainMenu.Instance.Background.Clone();
            this.LoadPercent += 5;
            Bitmap Button = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_default.png");
            this.LoadPercent += 5;
            Bitmap Button1 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_hover.png");
            this.LoadPercent += 5;
            Bitmap Button2 = new Bitmap(Environment.CurrentDirectory + "/Resources/Button_down.png");

            Bitmap MButton = new Bitmap(Environment.CurrentDirectory + "/Resources/M_button_default.png");
            this.LoadPercent += 5;
            Bitmap MButton1 = new Bitmap(Environment.CurrentDirectory + "/Resources/M_button_hover.png");
            this.LoadPercent += 5;
            Bitmap MButton2 = new Bitmap(Environment.CurrentDirectory + "/Resources/M_button_down.png");

            this.MButtons.Add(MButton);
            this.MButtons.Add(MButton1);
            this.MButtons.Add(MButton2);

            this.LoadPercent += 5;
            Bitmap EndLevel = new Bitmap(Environment.CurrentDirectory + "/Resources/End_level.png");
            this.LoadPercent += 5;
            Bitmap TextField = new Bitmap(Environment.CurrentDirectory + "/Resources/End_background.png");
            this.LoadPercent += 5;
            this.SelectedItem = new Bitmap(Environment.CurrentDirectory + "/Resources/Selection.png");
            this.LoadPercent += 5;

            float Width = 300;
            float Heigth = 60;
            float Left = Options.CanvasWidth / 2 - Width / 2;
            float Top = Options.CanvasHeight / 2 - Heigth / 2;
            float padding = 24;

            ImageItem Background = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth, Options.CanvasHeight),
                Location = new PointF(),
                Image = Back
            };
            this.LoadPercent += 5;
            this.Units.Add(Background);

            this.TextBox = new TextItem()
            {
                Size = new SizeF(Options.CanvasWidth - 120 - Heigth - padding, Heigth),
                Location = new PointF(60, 24),
                Image = TextField,
                Layer = 1
            };
            this.Units.Add(TextBox);

            ButtonItem AddButton = new ButtonItem()
            {
                Text = "+",
                Size = new SizeF(Heigth, Heigth),
                Location = new PointF(this.TextBox.Location.X + this.TextBox.Size.Width + padding, this.TextBox.Location.Y),
                Layer = 1,
            };
            AddButton.Images["default"] = MButton;
            AddButton.Images["hover"] = MButton1;
            AddButton.Images["down"] = MButton2;
            AddButton.OnMouseUp += AddButton_OnMouseUp;
            this.LoadPercent += 5;
            AddButton.UpdateFrame();
            this.Units.Add(AddButton);

            ImageItem ProfileList = new ImageItem()
            {
                Size = new SizeF(Options.CanvasWidth - 120, Options.CanvasHeight - this.TextBox.Location.Y - this.TextBox.Size.Height - padding * 3 - Heigth),
                Location = new PointF(60, this.TextBox.Location.Y + this.TextBox.Size.Height + padding),
                Image = EndLevel,
                Layer = 2
            };
            this.Units.Add(ProfileList);
            this.LoadPercent += 5;

            ButtonItem MainMenuButton = new ButtonItem()
            {
                Text = "Главное меню",
                Size = new SizeF((Options.CanvasWidth - 120 - padding) / 2, Heigth),
                Location = new PointF(60, Options.CanvasHeight - Heigth - padding),
                Layer = 1,
            };
            MainMenuButton.Images["default"] = Button;
            MainMenuButton.Images["hover"] = Button1;
            MainMenuButton.Images["down"] = Button2;
            MainMenuButton.OnMouseUp += MainMenuButton_OnMouseUp;
            this.LoadPercent += 5;

            ButtonItem SelectButton = new ButtonItem()
            {
                Text = "Выбрать",
                Size = MainMenuButton.Size,
                Location = new PointF(MainMenuButton.Location.X + MainMenuButton.Size.Width + padding, Options.CanvasHeight - Heigth - padding),
                Layer = 1,
            };
            SelectButton.Images["default"] = Button;
            SelectButton.Images["hover"] = Button1;
            SelectButton.Images["down"] = Button2;
            SelectButton.OnMouseUp += SelectButton_OnMouseUp;
            this.LoadPercent += 5;

            MainMenuButton.UpdateFrame();
            SelectButton.UpdateFrame();

            this.Units.Add(MainMenuButton);
            this.Units.Add(SelectButton);

            this.Profiles = ProfileWorker.LoadProfiles();

            this.ProfilesList = new L1SpecificGridItem()
            {
                Location = new PointF(ProfileList.Location.X + 6, ProfileList.Location.Y + 36),
                Layer = 3,
                Cols = 1,
                Rows = this.MaxProfiles,
            };
            this.ProfilesList.Size = new SizeF(ProfileList.Size.Width - 60, ProfileList.Size.Height - 36 - 6);

            this.MHeight = this.ProfilesList.RowHeight - 6;
            int sch = 0;
            foreach (var Profile in this.Profiles)
            {
                if (sch >= this.MaxProfiles) break;
                var NewItem = new SpecificImageItemWithText()
                {
                    Text = Profile,
                    Size = new SizeF(this.ProfilesList.ColumnWidth, this.ProfilesList.RowHeight),
                    Location = new PointF(),
                };
                NewItem.Images[true] = SelectedItem;
                NewItem.Images[false] = null;
                this.ProfilesList.Items.Add(NewItem);
                sch++;
            }
            this.ProfilesList.LastResultChanged += ProfilesList_LastResultChanged;
            this.Units.Add(this.ProfilesList);

            for(int i = 0; i < this.MaxProfiles; i++)
            {
                SpecificButtonItem RemButton = new SpecificButtonItem()
                {
                    Text = "x",
                    Size = new SizeF(MHeight, MHeight),
                    Location = new PointF(this.ProfilesList.Location.X + this.ProfilesList.Size.Width, this.ProfilesList.Location.Y + this.ProfilesList.RowHeight * i - (this.ProfilesList.RowHeight - MHeight) / 2 + 4),
                    Layer = 3,
                    Index = i
                };
                RemButton.Images["default"] = MButton;
                RemButton.Images["hover"] = MButton1;
                RemButton.Images["down"] = MButton2;
                RemButton.OnMouseUpWithIndex += RemButton_OnMouseUpWithIndex;
                this.LoadPercent += 5;
                RemButton.UpdateFrame();
                this.Units.Add(RemButton);
                this.RemoveButtons.Add(RemButton);
            }

            this.LoadPercent = 101;

            this.State = LevelSate.Working;
            this.IsLoaded = true;
        }

        private void RemButton_OnMouseUpWithIndex(Int32 Index)
        {
            if (Index > -1 && Index < this.Profiles.Count && ProfileWorker.RemoveProfile(this.Profiles[Index]))
            {
                this.Profiles.RemoveAt(Index);
                this.ProfilesList.Items.RemoveAt(Index);
            }
        }

        private void AddButton_OnMouseUp()
        {
            if (this.Profiles.Count < this.MaxProfiles && this.TextBox.Text.Length > 0)
            {
                if (ProfileWorker.AddProfile(this.TextBox.Text))
                {
                    this.Profiles.Add(this.TextBox.Text);
                    var NewItem = new SpecificImageItemWithText()
                    {
                        Text = this.TextBox.Text,
                        Size = new SizeF(this.ProfilesList.ColumnWidth, this.ProfilesList.RowHeight),
                        Location = new PointF(),
                    };
                    NewItem.Images[true] = this.SelectedItem;
                    NewItem.Images[false] = null;
                    this.ProfilesList.SafeAdd(NewItem);
                    this.TextBox.Text = "";
                }
            }
        }

        private void ProfilesList_LastResultChanged()
        {
            this.ActiveProfile = this.ProfilesList.LastResult;
            foreach (var Item in this.ProfilesList.Items)
            {
                var SItem = Item as SpecificImageItem;
                if (SItem != null)
                    SItem.Value = false;
            }
            var Unit = this.ProfilesList.Items[this.ActiveProfile] as SpecificImageItem;
            if(Unit != null)
                Unit.Value = true;
        }

        private void MainMenuButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            Engine.Core.Instance.State = Engine.CoreState.MainMenu;
        }

        private void SelectButton_OnMouseUp()
        {
            this.State = LevelSate.End;
            Engine.Core.Instance.ToStart();
            if(this.ActiveProfile > -1 && this.ActiveProfile < this.Profiles.Count)
                Engine.Core.Instance.ProfileName = this.Profiles[this.ActiveProfile];
            Engine.Core.Instance.State = Engine.CoreState.MainMenu;
        }

        public override void Work()
        {
            while (!this.IsEnd)
            {
                base.Work();
                Thread.Sleep(100);
            }
        }

        public override void KeyUp(KeyEventArgs Key)
        {
            base.KeyUp(Key);
            this.TextBox.KeyUp(Key);
        }

        public override IUnit MouseUp(MouseButtons Mouse, int X, int Y)
        {
            var Unit = base.MouseUp(Mouse, X, Y);
            if(!this.IsEnd && Unit != null)
            {
                if (Unit == this.ProfilesList)
                    this.ProfilesList.MouseUp(X - (int)this.ProfilesList.Location.X, Y - (int)this.ProfilesList.Location.Y);
            }
            return Unit;
        }

    }
}
