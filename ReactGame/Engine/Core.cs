using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ReactGame.Engine
{
    public enum CoreState
    {
        MainMenu,
        Submenu,
        Profile,
        Game,
        Stop,
        End
    }


    public class Core
    {
        public static Core Instance = null;
        Thread Main = null;
        Thread FPSCnecker = null;

        private string profile_name = "Выберите профиль";
        public String ProfileName { 
            get => this.profile_name; 
            set {
                this.profile_name = value;
                this.ProfileNameChanged?.Invoke(this.ProfileName);
            }
        }
        public Int32 MaxFPS { get; set; } = Options.MaxFPS;
        private ILevel ActiveLevel = null;
        private ILevel MainMenu { get; set; } = new Levels.Shared.MainMenu();
        private ILevel Profile { get; set; } = new Levels.Shared.ProfileViewer();
        private ILevel Menu { get; set; } = new Levels.Shared.SubMenu();
        private Int32 level = 0;
        private List<ILevel> Levels { get; set; } = new List<ILevel>();

        private CoreState state = CoreState.MainMenu;
        public CoreState State { 
            get => this.state;
            set
            {
                if (value == CoreState.MainMenu && this.MainMenu.IsLoaded)
                    this.MainMenu.State = LevelSate.Working;
                if (value == CoreState.Submenu && this.Menu.IsLoaded)
                    this.Menu.State = LevelSate.Working;
                if (value == CoreState.Profile && this.Profile.IsLoaded)
                    this.Profile.State = LevelSate.Working;
                this.state = value;
            }
        }

        public void ToStart()
        {
            this.level = 0;
            foreach (var level in this.Levels)
                level.Free();
        }

        private Image frame = null;
        public Image Frame { 
            get => this.frame; 
            private set
            {
                this.frame = value;
                this.Frame_Changed?.Invoke();
            }
        }

        private Int32 fps_counter = 0;
        private Int32 fps = 0;
        public Int32 FPS
        {
            get => this.fps;
            private set
            {
                this.fps = value;
                this.FPS_Changed?.Invoke();
            }
        }
        
        String log = "";
        public String Log
        {
            get => this.log;
            private set
            {
                this.log = value;
                this.Log_Changed?.Invoke();
            }
        }
        
        public Core()
        {
            Instance = this;
        }

        public void AddLevel(ILevel Level)
            => this.Levels.Add(Level);
        public void AddLevels(List<ILevel> Levels)
            => this.Levels.AddRange(Levels);
        public void ClearLevels()
            => this.Levels.Clear();

        public Boolean Run()
        {
            if (this.Levels.Count > -1)
            {
                this.State = CoreState.MainMenu;
                this.FPSCnecker = new Thread(this.FPS_Check);
                this.Main = new Thread(this.Work);
                try
                {
                    this.FPSCnecker.Start();
                    this.Main.Start();
                    return true;
                }
                catch (Exception error)
                {
                    this.Log = error.Message;
                    return false;
                }
            } else
            {
                this.Log = "Отсутствуют уровни для игры!";
                return false;
            }
        }
        public void Stop()
        {
            this.State = CoreState.Stop;
            this.Main.Abort();
            this.FPSCnecker.Abort();
            if (this.MainMenu != null)
            {
                this.MainMenu.State = LevelSate.End;
                (this.MainMenu as Levels.Shared.MainMenu).IsEnd = true;
            }
            if (this.Profile != null)
            {
                this.Profile.State = LevelSate.End;
                (this.Profile as Levels.Shared.ProfileViewer).IsEnd = true;
            }
            if (this.Menu != null)
            {
                this.Menu.State = LevelSate.End;
                (this.Menu as Levels.Shared.SubMenu).IsEnd = true;
            }
            foreach (var Item in this.Levels)
                if (Item != null)
                    Item.State = LevelSate.End;
        }

        private void SetActivelevel()
        {
            switch (this.State)
            {
                case CoreState.MainMenu:
                    this.ActiveLevel = this.MainMenu;
                    break;
                case CoreState.Submenu:
                    this.ActiveLevel = this.Menu;
                    break;
                case CoreState.Profile:
                    this.ActiveLevel = this.Profile;
                    break;
                case CoreState.Game:
                    if (level == 0)
                    {
                        this.ActiveLevel = this.Levels[level++];
                    }
                    else
                        this.ActiveLevel = (this.Levels[level - 1].State == LevelSate.End)
                            ? (this.level < this.Levels.Count) ? this.Levels[level++] : null
                            : this.Levels[level - 1];
                    break;
            }
        }

        public void Work()
        {
            long limit = 1000 / this.MaxFPS;
            var Timer = new Stopwatch();
            Timer.Start();
            while (this.State != CoreState.Stop)
            {
                SetActivelevel();
                if(this.ActiveLevel != null)
                {
                    Timer.Restart();
                    if(this.ActiveLevel.State == LevelSate.Stop)
                    {
                        this.ActiveLevel.Load();
                    }

                    this.Frame = this.ActiveLevel.Frame;

                    this.fps_counter++;

                    while (Timer.ElapsedMilliseconds < limit)
                        Thread.Sleep(1);
                }
                else
                    Thread.Sleep(1);
            }
            Timer.Stop();
            this.State = CoreState.End;
        }
        private void FPS_Check()
        {
            while (this.State != CoreState.Stop && this.State != CoreState.End)
            {
                this.FPS = this.fps_counter;
                this.fps_counter = 0;
                Thread.Sleep(1000);
            }
        }

        public void KeyDown(KeyEventArgs eventArgs)
        {
            if (eventArgs.KeyData == Keys.Escape && this.State != CoreState.MainMenu)
            {
                switch (this.State)
                {
                    case CoreState.Profile:
                        this.State = CoreState.MainMenu;
                        break;
                    case CoreState.Submenu:
                        this.State = CoreState.Game;
                        this.Levels[level - 1].Resume();
                        break;
                    case CoreState.Game:
                        this.State = CoreState.Submenu;
                        this.Levels[level - 1].Pause();
                        break;
                }
            }
            else
                this.ActiveLevel?.KeyDown(eventArgs);
        }
        public void KeyUp(KeyEventArgs eventArgs)
        {
            this.ActiveLevel?.KeyUp(eventArgs);
        }

        public void MouseDown(MouseButtons Mouse, Int32 X, Int32 Y)
        {
            this.ActiveLevel?.MouseDown(Mouse, X, Y);
        }
        public void MouseUp(MouseButtons Mouse, Int32 X, Int32 Y)
        {
            this.ActiveLevel?.MouseUp(Mouse, X, Y);
        }
        public void MouseMove(Int32 X, Int32 Y)
        {
            this.ActiveLevel?.MouseMove(X, Y);
        }

        public delegate void SingleEvent();
        public event SingleEvent FPS_Changed;
        public event SingleEvent Frame_Changed;
        public event SingleEvent Log_Changed;

        public Action<String> ProfileNameChanged;
    }
}
