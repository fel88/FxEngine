using System;

namespace FxEngine.Gui
{
    public interface IButton : IGuiElement
    {
        Action Click { get; set; }
        string Caption { get; set; }

    }
}
