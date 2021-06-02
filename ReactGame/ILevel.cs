using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReactGame
{
    public enum LevelSate
    {
        Stop,
        Loading,
        Working,
        End
    }
    public interface ILevel
    {
        Boolean IsLoaded { get; set; }
        LevelSate State { get; set; }
        void Load();
        Image Frame { get; }

        void Pause();
        void Resume();

        void Free();

        void KeyUp(KeyEventArgs Key);
        void KeyDown(KeyEventArgs Key);

        Engine.IUnit MouseUp(MouseButtons Mouse, Int32 X, Int32 Y);
        Engine.IUnit MouseDown(MouseButtons Mouse, Int32 X, Int32 Y);
        Engine.IUnit MouseMove(Int32 X, Int32 Y);
    }
}
