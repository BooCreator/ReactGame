using System;
using System.Windows.Forms;

namespace ReactGame.Engine.Units
{
    public interface IKeyInputUnit
    {
        void KeyDown(KeyEventArgs Key);
        void KeyUp(KeyEventArgs Key);

        event Action<KeyEventArgs> OnKeyDown;
        event Action<KeyEventArgs> OnKeyUp;
    }
}
