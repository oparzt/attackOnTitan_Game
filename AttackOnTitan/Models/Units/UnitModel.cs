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
        InnerGate
    }

    public class UnitModel
    {
        public readonly int ID;
        public MapCellModel CurCell;

        public bool CanGo = true;
        public bool IsFly;
        public bool Moved;
        public bool IsEnemy => UnitType == UnitType.Titan;
        public HashSet<TravelMode> PossibleTravelModes = new HashSet<TravelMode>();

        public MapCellModel TitanTarget;
        public TitanTargetType TitanTargetType;

        private readonly int _unitDamage;
        public int UnitDamage => (UnitType is UnitType.Builder or UnitType.Titan) || Gas >= GetEnergyCost(TravelMode.Fly) ? _unitDamage : 0;
        public readonly int MaxEnergy = 20;
        public readonly int MaxGas = 200; 
        public int Energy;
        public int Gas;
        public int BuiltCount;

        public readonly Dictionary<TravelMode, int> TravelEnergyCost = new()
        {
            [TravelMode.None] = int.MaxValue,
            [TravelMode.Run] = 4,
            [TravelMode.BuilderRun] = 5,
            [TravelMode.Fly] = 2,
            [TravelMode.TitanRun] = 2
        };

        public readonly int TravelGasCost = 10;

        public readonly UnitType UnitType;
        
        public static readonly Dictionary<CommandType, OutputCommandInfo> CommandInfoByTypes = new()
        {
            
            [CommandType.Attack] = new(CommandType.Attack, true),
            [CommandType.AttackDisabled] = new(CommandType.AttackDisabled, false),
            [CommandType.Refuel] = new(CommandType.Refuel, true),
            [CommandType.RefuelDisabled] = new(CommandType.RefuelDisabled, false),
            [CommandType.Fly] = new(CommandType.Fly, true),
            [CommandType.FlyDisabled] = new(CommandType.FlyDisabled, false),
            [CommandType.Walk] = new(CommandType.Walk, true),
            [CommandType.WalkDisabled] = new(CommandType.WalkDisabled, false),
            [CommandType.OpenCreatingHouseMenu] = new(CommandType.OpenCreatingHouseMenu, true),
            [CommandType.OpenCreatingHouseMenuDisabled] = new(CommandType.OpenCreatingHouseMenuDisabled, false)
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

        public static readonly Random Random = new ();
        
        public UnitModel(int id, UnitType unitType) 
        {
            ID = id;
            UnitType = unitType;
            Energy = 0;

            switch (unitType)
            {
                case UnitType.Titan:
                    MaxEnergy = 10;
                    MaxGas = 0;
                    _unitDamage = Random.Next(4, 30);
                    break;
                case UnitType.Cadet:
                    MaxEnergy = 12;
                    MaxGas = 160;
                    _unitDamage = 5;
                    break;
                case UnitType.Scout:
                    MaxEnergy = 18;
                    MaxGas = 160;
                    _unitDamage = 15;
                    break;
                case UnitType.Garrison:
                    MaxEnergy = 16;
                    MaxGas = 160;
                    _unitDamage = 8;
                    break;
                case UnitType.Police:
                    MaxEnergy = 22;
                    MaxGas = 160;
                    _unitDamage = 12;
                    break;
                case UnitType.Builder:
                    MaxEnergy = 12;
                    MaxGas = 0;
                    _unitDamage = 0;
                    break;
                default:
                    break;
            }

            Gas = MaxGas;
            SetPossibleTravelModes();
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
                    TextureName = UnitTextures.UnitTextureNames[UnitType],
                    X = mapCell.X,
                    Y = mapCell.Y
                }
            });
        }
        
        public void Refuel() => Gas = MaxGas;

        public int GetEnergyCost(TravelMode travelMode) => TravelEnergyCost[travelMode];
        public int GetGasCost(TravelMode travelMode) => travelMode == TravelMode.Fly ? TravelGasCost : 0;

        public bool IsExistTravel(float energyCost, float gasCost) =>
            Energy >= energyCost && Gas >= gasCost;

        public void SetPossibleTravelModes()
        {
            PossibleTravelModes.Clear();
            switch (UnitType)
            {
                case UnitType.Scout:
                case UnitType.Garrison:
                case UnitType.Police:
                case UnitType.Cadet:
                    PossibleTravelModes.Add(TravelMode.Run);
                    if (IsFly)
                        PossibleTravelModes.Add(TravelMode.Fly);
                    break;
                case UnitType.Builder:
                    PossibleTravelModes.Add(TravelMode.BuilderRun);
                    break;
                case UnitType.Titan:
                    PossibleTravelModes.Add(TravelMode.TitanRun);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                    yield return CommandInfoByTypes[CurCell.IsEnemyInCell() ? CommandType.Attack : CommandType.AttackDisabled];
                    yield return CommandInfoByTypes[IsFly ? CommandType.Walk : CommandType.Fly];
                    yield return CommandInfoByTypes[CurCell.BuildingType == BuildingType.Barracks ? CommandType.Refuel : CommandType.RefuelDisabled];
                    break;
                case UnitType.Builder:
                    yield return CommandInfoByTypes[CurCell.GetPossibleCreatingBuildingTypes().Any() && Energy != 0 ?
                        CommandType.OpenCreatingHouseMenu : CommandType.OpenCreatingHouseMenuDisabled];
                    break;
                case UnitType.Titan:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
