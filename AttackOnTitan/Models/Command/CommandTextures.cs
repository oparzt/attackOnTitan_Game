using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class CommandTextures
    {
        public static readonly Dictionary<CommandType, string> CommandTexturesNames = new Dictionary<CommandType, string>()
        {
            [CommandType.Attack] = "AttackIcon",
            [CommandType.AttackDisabled] = "AttackIconHalf",
            [CommandType.Refuel] = "RefuelingIcon",
            [CommandType.RefuelDisabled] = "RefuelingIconHalf",
            [CommandType.Fly] = "GasIcon",
            [CommandType.FlyDisabled] = "GasIconHalf",
            [CommandType.Walk] = "WalkIcon",
            [CommandType.WalkDisabled] = "WalkIconHalf",
            [CommandType.OpenCreatingHouseMenu] = "BuildingIcon",
            [CommandType.OpenCreatingHouseMenuDisabled] = "BuildingIconHalf",
            [CommandType.OpenCreatingUnitMenu] = "EmptyCommand",
            [CommandType.OpenProductionMenu] = "EmptyCommand",
            [CommandType.CreateHouse] = "EmptyCommand",
            [CommandType.CreateUnit] = "EmptyCommand",
            [CommandType.CloseCreatingMenu] = "ExitIcon",
            [CommandType.CloseProductionMenu] = "ExitIcon",
            [CommandType.ChangePeopleAtWork] = "EmptyCommand",
        };
        
    }
}