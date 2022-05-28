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

    public enum TravelMode
    {
        None,
        Run,
        BuilderRun,
        Fly,
        TitanRun
    }

    public enum TitanTargetType
    {
        None,
        Attack,
        OuterGate,
        InnerGate
    }

    public class UnitModel
    {
        public readonly int ID;
        public MapCellModel CurCell;

        public bool CanGo = true;
        public bool Moved;
        public bool IsEnemy => UnitType == UnitType.Titan;
        public TravelMode TravelMode = TravelMode.Run;

        public MapCellModel TitanTarget;
        public TitanTargetType TitanTargetType;

        private readonly float _unitDamage;
        public float UnitDamage => UnitType is UnitType.Builder or UnitType.Titan ? _unitDamage : 2;
        public readonly float MaxEnergy = 10;
        public readonly float MaxGas = 100; 
        public float Energy;
        public float Gas;

        public readonly Dictionary<TravelMode, float> TravelEnergyCost = new()
        {
            [TravelMode.None] = float.MaxValue,
            [TravelMode.Run] = 1f,
            [TravelMode.BuilderRun] = 2f,
            [TravelMode.Fly] = 0.5f,
            [TravelMode.TitanRun] = 1f
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

        public static readonly Random Random = new Random();
        
        public UnitModel(int id, UnitType unitType)
        {
            ID = id;
            UnitType = unitType;
            Energy = 0;

            switch (unitType)
            {
                case UnitType.Titan:
                    MaxEnergy = 5;
                    MaxGas = 0;
                    _unitDamage = Random.Next(2, 15);
                    TitanTargetType = TitanTargetType.OuterGate;
                    break;
                case UnitType.Scout:
                    MaxEnergy = 9;
                    MaxGas = 80;
                    _unitDamage = 10;
                    break;
                case UnitType.Garrison:
                    MaxEnergy = 8;
                    MaxGas = 80;
                    _unitDamage = 5;
                    break;
                case UnitType.Police:
                    MaxEnergy = 11;
                    MaxGas = 80;
                    _unitDamage = 7;
                    break;
                case UnitType.Cadet:
                    MaxEnergy = 6;
                    MaxGas = 80;
                    _unitDamage = 4;
                    break;
                case UnitType.Builder:
                    MaxEnergy = 6;
                    MaxGas = 0;
                    _unitDamage = 0;
                    break;
                default:
                    break;
            }

            Gas = MaxGas;

            if (unitType == UnitType.Builder) TravelMode = TravelMode.BuilderRun;
        }

        public void AddUnitToTheMap(MapCellModel mapCell)
        {
            CurCell = mapCell;
            var position = mapCell.MoveUnitToTheCell(this);

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
