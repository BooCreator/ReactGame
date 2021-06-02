using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ReactGame
{
    public partial class Form1 : Form
    {
        public static Form1 Instance;
        Engine.Core Engine = null;
        Action ChangeFPS = null;
        Action ChangeFrame = null;

        public Form1()
        {
            InitializeComponent();
            Instance = this;
            Engine = new Engine.Core();
            this.Engine.Frame_Changed += Engine_Frame_Changed;
            this.Engine.FPS_Changed += Engine_FPS_Changed;
            this.Engine.Log_Changed += Engine_Log_Changed;
            this.ChangeFPS = new Action(() => { this.Text = $"FPS: {this.Engine.FPS}"; });
            this.ChangeFrame = new Action(() => { 
                this.pictureBox1.BackgroundImage = this.Engine.Frame; 
                this.pictureBox1.Invalidate(); 
            });
        }

        private void Engine_Log_Changed()
        {
            MessageBox.Show(this.Engine.Log, "Произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Engine_FPS_Changed()
        {
            this.Invoke(this.ChangeFPS);
        }

        private void Engine_Frame_Changed()
        {
            this.pictureBox1.Invoke(this.ChangeFrame);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Engine.AddLevel(new Levels.Level1());
            this.Engine.AddLevel(new Levels.Level2());
            this.Engine.AddLevel(new Levels.Level3());
            this.Engine.AddLevel(new Levels.Level4());
            this.Engine.AddLevel(new Levels.Level5());
            this.Engine.Run();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int X = (int)((e.X * 1.0 / this.pictureBox1.Width) * Options.CanvasWidth);
            int Y = (int)((e.Y * 1.0 / this.pictureBox1.Height) * Options.CanvasHeight);
            this.Engine.MouseDown(e.Button, X, Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int X = (int)((e.X * 1.0 / this.pictureBox1.Width) * Options.CanvasWidth);
            int Y = (int)((e.Y * 1.0 / this.pictureBox1.Height) * Options.CanvasHeight);
            this.Engine.MouseUp(e.Button, X, Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int X = (int)((e.X * 1.0 / this.pictureBox1.Width) * Options.CanvasWidth);
            int Y = (int)((e.Y * 1.0 / this.pictureBox1.Height) * Options.CanvasHeight);
            this.Engine.MouseMove(X, Y);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            this.Engine.KeyDown(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            this.Engine.KeyUp(e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Engine.Stop();
        }

    }
}
