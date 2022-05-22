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
        StepBtnPressed,
        UpdateWasEnd,
        UpdateNoServicedZones,
        ExecCommand,
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

        public InputCellInfo InputCellInfo;
        public InputUnitInfo InputUnitInfo;
        public InputCommandInfo InputCommandInfo;
        public NoServicedZone NoServicedZone;
    }

    public class InputCellInfo
    {
        public readonly int X;
        public readonly int Y;

        public InputCellInfo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class InputUnitInfo
    {
        public readonly int ID;

        public InputUnitInfo(int id) => ID = id;
    }
    
    public class InputCommandInfo
    {
        public readonly CommandType CommandType;
        public CreatingInfo CreatingInfo;
        public string BuildingTextureName;

        public InputCommandInfo(CommandType commandType) =>
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
