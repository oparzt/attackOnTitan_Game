using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan
{
    public enum MouseBtn
    {
        None,
        Left,
        Right
    }
    
    public static class MouseStateExtension
    {
        public static MouseBtn GetPressedMouseBtn(this MouseState mouseState) =>
            mouseState.RightButton == ButtonState.Pressed ? MouseBtn.Right :
            mouseState.LeftButton == ButtonState.Pressed ? MouseBtn.Left : MouseBtn.None;
    }
}