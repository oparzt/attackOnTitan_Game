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
        SelectUnit,
        UnitStopMove
    }

    public enum PressedMouseBtn
    {
        None,
        Left,
        Right
    }

    public class InputAction
    {
        public InputActionType ActionType;

        public Keys Key;
        public PressedMouseBtn MouseBtn;

        public SelectedCell SelectedCell;
        public SelectedUnit SelectedUnit;
    }

    public class SelectedCell
    {
        public int X;
        public int Y;

        public SelectedCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class SelectedUnit
    {
        public int ID;

        public SelectedUnit(int id) => ID = id;
    }
}
