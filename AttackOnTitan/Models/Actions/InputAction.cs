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
        UnselectUnit,
        UnitStopMove,
        UnitCommand,
        StepBtnPressed,
        UpdateWasEnd,
        UpdateNoServicedZones,
    }

    public enum NoServicedZoneLocation
    {
        TopBar,
        CommandBar,
        StepBtn,
        BuilderChoose
    }

    public class InputAction
    {
        public InputActionType ActionType;

        public Keys Key;
        public MouseBtn MouseBtn;

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
        public readonly UnitCommandType UnitCommandType;
        public BuildingInfo BuildingInfo;
        public string BuildingTextureName;

        public UnitCommandInfo(UnitCommandType unitCommandType) =>
            UnitCommandType = unitCommandType;
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
