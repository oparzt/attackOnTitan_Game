using System;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Models
{
    public enum InputActionType
    {
        None,
        KeyPressed,
        SelectMapCell,
        SelectUnit
    }

    public enum PressedMouseBtn
    {
        None,
        Left,
        Right
    }

    public struct InputAction
    {
        public InputActionType ActionType;

        public Keys Key;
        public PressedMouseBtn MouseBtn;

        public SelectedCell SelectedCell;
        public SelectedUnit SelectedUnit;
    }

    public struct SelectedCell
    {
        public int X;
        public int Y;
    }

    public struct SelectedUnit
    {
        public int ID;
    }
}
