﻿using System;
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

    public enum TravelMode
    {
        None,
        Run,
        BuilderRun,
        Fly
    }

    public class UnitModel
    {
        public readonly int ID;
        public MapCellModel CurCell;

        public bool CanGo = true;
        public bool Moved;
        public bool IsEnemy => UnitType == UnitType.Titan;
        public TravelMode TravelMode = TravelMode.Run;

        public const float MaxEnergy = 10;
        public const float MaxGas = 100; 
        public float Energy = 10;
        public float Gas = 100;

        public readonly Dictionary<TravelMode, float> TravelEnergyCost = new()
        {
            [TravelMode.None] = float.MaxValue,
            [TravelMode.Run] = 1f,
            [TravelMode.BuilderRun] = 2f,
            [TravelMode.Fly] = 0.5f
        };

        public readonly float TravelGasCost = 4f;

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

        public static readonly Dictionary<UnitType, string> UnitTextureNames = new()
        {
            [UnitType.Titan] = "Titan",
            [UnitType.Scout] = "Scout",
            [UnitType.Garrison] = "Garrison",
            [UnitType.Police] = "Police",
            [UnitType.Cadet] = "Cadet",
            [UnitType.Builder] = "Builder"
        };

        public static readonly Dictionary<UnitType, string> UnitNames = new()
        {
            [UnitType.Titan] = "Титаны",
            [UnitType.Scout] = "Разведчики",
            [UnitType.Garrison] = "Гарнизон",
            [UnitType.Police] = "Полиция",
            [UnitType.Cadet] = "Кадеты",
            [UnitType.Builder] = "Строители"
        };
        
        public UnitModel(int id, UnitType unitType)
        {
            ID = id;
            UnitType = unitType;

            if (unitType == UnitType.Builder) TravelMode = TravelMode.BuilderRun;
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
                    TextureName = UnitTextureNames[UnitType],
                    X = mapCell.X,
                    Y = mapCell.Y
                }
            });
        }

        public float GetEnergyCost() => TravelEnergyCost[TravelMode];
        public float GetGasCost() => TravelMode == TravelMode.Fly ? TravelGasCost : 0f;

        public void Refuel() => Gas = MaxGas;

        public bool IsExistTravel(float energyCost, float gasCost)
        {
            switch (TravelMode)
            {
                case TravelMode.Run:
                case TravelMode.BuilderRun:
                    return Energy >= energyCost;
                case TravelMode.Fly:
                    return Energy >= energyCost && Gas >= gasCost;
                case TravelMode.None:
                default:
                    return false;
            }
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
                    yield return CommandInfoByTypes[TravelMode == TravelMode.Fly ? CommandType.Walk : CommandType.Fly][true];
                    yield return CommandInfoByTypes[CommandType.Refuel][CurCell.BuildingType == BuildingType.Barracks];
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
