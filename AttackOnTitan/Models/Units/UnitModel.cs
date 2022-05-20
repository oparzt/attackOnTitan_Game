using System;
using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public enum UnitCommandType
    {
        Attack,
        Refuel,
        Fly,
        Walk,
        OpenBuildMenu,
        Build,
        ExitBuildMenu,
    }

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
        public bool OpenMenu;
        public bool IsEnemy => UnitType == UnitType.Titan;

        public int MaxEnergy = 10;
        public int Energy = 10;

        public readonly UnitType UnitType;

        private static readonly Dictionary<UnitCommandType, Dictionary<bool, CommandInfo>> CommandInfoByTypes = new()
        {
            [UnitCommandType.Attack] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.Attack, true, "AttackIcon"),
                [false] = new(UnitCommandType.Attack, false, "AttackIconHalf")
            },
            [UnitCommandType.Refuel] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.Refuel, true, "RefuelingIcon"),
                [false] = new(UnitCommandType.Refuel, false, "RefuelingIconHalf")
            },
            [UnitCommandType.Fly] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.Fly, true, "GasIcon"),
                [false] = new(UnitCommandType.Fly, false, "GasIconHalf")
            },
            [UnitCommandType.Walk] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.Walk, true, "WalkIcon"),
                [false] = new(UnitCommandType.Walk, false, "WalkIconHalf")
            },
            [UnitCommandType.OpenBuildMenu] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.OpenBuildMenu, true, "BuildingIcon"),
                [false] = new(UnitCommandType.OpenBuildMenu, false, "BuildingIconHalf")
            },
            [UnitCommandType.ExitBuildMenu] = new Dictionary<bool, CommandInfo>
            {
                [true] = new(UnitCommandType.ExitBuildMenu, true, "ExitIcon"),
                [false] = new(UnitCommandType.ExitBuildMenu, false, "ExitIconHalf")
            }
        };

        private static readonly Dictionary<UnitType, string> TexturesByUnitTypes = new()
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
        
        public void UpdateCommandsBar()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCommandsBar,
                CommandInfos = GetCommandInfos().ToArray()
            });
        }

        public void OpenBuildMenu(Dictionary<ResourceType, int> resourceCount)
        {
            var possibleBuildingOnCell = CurCell.GetPossibleBuildingInCell();
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateBuilderChoose,
                OutputBuildingInfo = new OutputBuildingInfo
                {
                    BackgroundTextureName = "BuilderCard",
                    BuildingInfos = possibleBuildingOnCell,
                    BuildingTexturesName = possibleBuildingOnCell
                        .Select(buildingInfo => CurCell.GetBuildingTextureName(buildingInfo.BuildingType))
                        .ToArray(),
                    NotAvailableResource = possibleBuildingOnCell
                        .Select(buildingInfo => GetNotAvailableResourceForBuildingType(buildingInfo, resourceCount))
                        .ToArray()
                }
            });

            OpenMenu = true;
            UpdateCommandsBar();
        }

        public void CloseBuildMenu()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateBuilderChoose,
                OutputBuildingInfo = new OutputBuildingInfo
                {
                    BackgroundTextureName = "BuilderCard",
                    BuildingInfos = new BuildingInfo[] {}
                }
            });
            OpenMenu = false;
            UpdateCommandsBar();
        }

        private HashSet<ResourceType> GetNotAvailableResourceForBuildingType(BuildingInfo buildingInfo, Dictionary<ResourceType, int> resourceCount)
        {
            return buildingInfo.Price
                .Where(pair => pair.Value > resourceCount[pair.Key])
                .Select(pair => pair.Key)
                .ToHashSet();
        }
        
        private void SetOpacity(float opacity) =>
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.ChangeUnitOpacity,
                UnitInfo = new UnitInfo(ID)
                {
                    Opacity = opacity
                }
            });
        
        private IEnumerable<CommandInfo> GetCommandInfos()
        {
            if (OpenMenu)
            {
                yield return CommandInfoByTypes[UnitCommandType.ExitBuildMenu][true];
                yield break;
            }
            
            switch (UnitType)
            {
                case UnitType.Scout:
                case UnitType.Garrison:
                case UnitType.Police:
                case UnitType.Cadet:
                    yield return CommandInfoByTypes[UnitCommandType.Attack][CurCell.IsEnemyInCell()];
                    yield return CommandInfoByTypes[IsFly ? UnitCommandType.Walk : UnitCommandType.Fly][true];
                    yield return CommandInfoByTypes[UnitCommandType.Refuel][false];
                    break;
                case UnitType.Builder:
                    yield return CommandInfoByTypes[UnitCommandType.OpenBuildMenu][CurCell.GetPossibleBuildingTypesInCell().Any()];
                    break;
                case UnitType.Titan:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
