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
        UnitStopMove,
        UnitCommand,
        StepBtnPressed,
        UpdateWasEnd,
        UpdateNoServicedZones,
    }

    public enum PressedMouseBtn
    {
        None,
        Left,
        Right
    }

    public enum NoServicedZoneLocation
    {
        TopBar,
        CommandBar,
        StepBtn
    }

    public class InputAction
    {
        public InputActionType ActionType;

        public Keys Key;
        public PressedMouseBtn MouseBtn;

        public SelectedCell SelectedCell;
        public SelectedUnit SelectedUnit;
        public UnitCommandInfo UnitCommandInfo;
        public NoServicedZone NoServicedZone;
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
    
    public class UnitCommandInfo
    {
        public readonly CommandType CommandType;

        public UnitCommandInfo(CommandType commandType) =>
            CommandType = commandType;
    }

    public class NoServicedZone
    {
        public readonly NoServicedZoneLocation Location;
        public Rectangle[] Zones;

        public NoServicedZone(NoServicedZoneLocation location)
        {
            Location = location;
        }
    }
}
