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
        public readonly InputActionType ActionType;

        public readonly Keys Key;
        public readonly PressedMouseBtn MouseBtn;

        public readonly SelectedCell SelectedCell;
        public readonly SelectedUnit SelectedUnit;

        public InputAction()
        {
            ActionType = InputActionType.None;
        }

        public InputAction(InputActionType actionType, Keys key, PressedMouseBtn pressedMouseBtn,
            SelectedCell selectedCell, SelectedUnit selectedUnit) {
            ActionType = actionType;
            Key = key;
            MouseBtn = pressedMouseBtn;
            SelectedCell = selectedCell;
            SelectedUnit = selectedUnit;
        }


        public InputAction(SelectedCell selectedCell, PressedMouseBtn pressedMouseBtn)
        {
            ActionType = InputActionType.SelectMapCell;
            MouseBtn = pressedMouseBtn;
            SelectedCell = selectedCell;
        }

        public InputAction(SelectedUnit selectedUnit, PressedMouseBtn pressedMouseBtn)
        {
            ActionType = InputActionType.SelectUnit;
            MouseBtn = pressedMouseBtn;
            SelectedUnit = selectedUnit;
        }

        public InputAction(Keys key)
        {
            ActionType = InputActionType.KeyPressed;
            Key = key;
        }
    }

    public class SelectedCell
    {
        public readonly int X;
        public readonly int Y;

        public SelectedCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class SelectedUnit
    {
        public readonly int ID;

        public SelectedUnit(int id) => ID = id;
    }
}
