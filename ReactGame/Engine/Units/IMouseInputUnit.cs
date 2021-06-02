
using System;

namespace ReactGame.Engine.Units
{
    public interface IMouseInputUnit
    {
        void MouseEnter();
        void MouseLeave();
        void MouseDown();
        void MouseUp();

        event Action OnMouseEnter;
        event Action OnMouseLeave;
        event Action OnMouseDown;
        event Action OnMouseUp;
    }
}
