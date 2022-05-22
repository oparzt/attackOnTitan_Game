using System;
using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public enum UnitType
    {
        Titan,
        Scout,
        Garrison,
        Police,
        Cadet,
        Builder
    }

    public class UnitModel
    {
        public readonly int ID;
        public MapCellModel CurCell;

        public bool CanGo = true;
        public bool IsFly;
        public bool Moved;
        public bool IsEnemy => UnitType == UnitType.Titan;

        public int MaxEnergy = 10;
        public int Energy = 10;

        public readonly UnitType UnitType;

        public static readonly Dictionary<CommandType, Dictionary<bool, OutputCommandInfo>> CommandInfoByTypes = new()
        {
            [CommandType.Attack] = new Dictionary<bool, OutputCommandInfo>
            {
                [true] = new(CommandType.Attack, true, "AttackIcon"),
                [false] = new(CommandType.Attack, false, "AttackIconHalf")
            },
            [CommandType.Refuel] = new Dictionary<bool, OutputCommandInfo>
            {
                [true] = new(CommandType.Refuel, true, "RefuelingIcon"),
                [false] = new(CommandType.Refuel, false, "RefuelingIconHalf")
            },
            [CommandType.Fly] = new Dictionary<bool, OutputCommandInfo>
            {
                [true] = new(CommandType.Fly, true, "GasIcon"),
                [false] = new(CommandType.Fly, false, "GasIconHalf")
            },
            [CommandType.Walk] = new Dictionary<bool, OutputCommandInfo>
            {
                [true] = new(CommandType.Walk, true, "WalkIcon"),
                [false] = new(CommandType.Walk, false, "WalkIconHalf")
            },
            [CommandType.OpenCreatingHouseMenu] = new Dictionary<bool, OutputCommandInfo>
            {
                [true] = new(CommandType.OpenCreatingHouseMenu, true, "BuildingIcon"),
                [false] = new(CommandType.OpenCreatingHouseMenu, false, "BuildingIconHalf")
            }
        };

        public static readonly Dictionary<UnitType, string> TexturesByUnitTypes = new()
        {
            [UnitType.Titan] = "Titan",
            [UnitType.Scout] = "Scout",
            [UnitType.Garrison] = "Garrison",
            [UnitType.Police] = "Police",
            [UnitType.Cadet] = "Cadet",
            [UnitType.Builder] = "Builder"
        };
        
        public UnitModel(int id, UnitType unitType)
        {
            ID = id;
            UnitType = unitType;
        }

        public void AddUnitToTheMap(MapCellModel mapCell, Position position)
        {
            CurCell = mapCell;
            if (position == Position.Center)
                mapCell.UnitInCenterOfCell = this;
            else
                mapCell.UnitsInCell[position] = this;
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.AddUnit,
                UnitInfo = new UnitInfo(ID)
                {
                    Opacity = 0.65f,
                    Position = position,
                    TextureName = TexturesByUnitTypes[UnitType],
                    X = mapCell.X,
                    Y = mapCell.Y
                }
            });
        }

        public void SetUnselectedOpacity() =>
            SetOpacity(0.65f);

        public void SetPreselectedOpacity() =>
            SetOpacity(0.8f);

        public void SetSelectedOpacity() =>
            SetOpacity(1f);
        
        private void SetOpacity(float opacity) =>
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.ChangeUnitOpacity,
                UnitInfo = new UnitInfo(ID)
                {
                    Opacity = opacity
                }
            });
        
        public IEnumerable<OutputCommandInfo> GetCommandInfos()
        {
            switch (UnitType)
            {
                case UnitType.Scout:
                case UnitType.Garrison:
                case UnitType.Police:
                case UnitType.Cadet:
                    yield return CommandInfoByTypes[CommandType.Attack][CurCell.IsEnemyInCell()];
                    yield return CommandInfoByTypes[IsFly ? CommandType.Walk : CommandType.Fly][true];
                    // yield return CommandInfoByTypes[UnitCommandType.Refuel][false];
                    break;
                case UnitType.Builder:
                    yield return CommandInfoByTypes[CommandType.OpenCreatingHouseMenu][CurCell.GetPossibleCreatingBuildingTypes().Any()];
                    break;
                case UnitType.Titan:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
