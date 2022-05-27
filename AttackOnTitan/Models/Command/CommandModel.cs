using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public class CommandModel
    {
        public readonly GameModel GameModel;
        
        public CommandModel(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeCreatingChoose();
        }

        public void OpenCreatingMenu(UnitModel unitModel, MapCellModel mapCellModel, 
            bool buildingMode)
        {
            var creatingInfos = new List<CreatingInfo>();
            if (buildingMode)
            {
                creatingInfos.AddRange(mapCellModel.GetPossibleCreatingBuildingTypes()
                    .Select(buildingType => new CreatingInfo
                    {
                        BuildingType = buildingType,
                        ObjectName = MapCellModel.BuildingNames[buildingType],
                        ObjectTextureName = MapCellModel.BuildingTextureNames[buildingType],
                        ObjectResourceDescription = GetObjectResourceDescription(BuildingEconomyModel.CountDiff[buildingType], BuildingEconomyModel.StepCountDiff[buildingType], BuildingEconomyModel.LimitDiff[buildingType]),
                        NotAvailableResource = GetNotAvailableResource(BuildingEconomyModel.CountDiff[buildingType], GameModel.EconomyModel.ResourceCount)
                    }));
            }
            else
            {
                creatingInfos.AddRange(mapCellModel.GetPossibleCreatingUnitTypes()
                    .Select(unitType => new CreatingInfo
                    {
                        UnitType = unitType,
                        ObjectName = UnitModel.UnitNames[unitType],
                        ObjectTextureName = UnitModel.UnitTextureNames[unitType],
                        ObjectResourceDescription = GetObjectResourceDescription(UnitEconomyModel.CountDiff[unitType], UnitEconomyModel.StepCountDiff[unitType], UnitEconomyModel.LimitDiff[unitType]),
                        NotAvailableResource = GetNotAvailableResource(UnitEconomyModel.CountDiff[unitType], GameModel.EconomyModel.ResourceCount)
                    }));
            }
            
            var unitInfo = new UnitInfo(unitModel?.ID ?? -1);
            var mapCellInfo = new MapCellInfo(mapCellModel.X, mapCellModel.Y);
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCreatingChoose,
                UnitInfo = unitInfo,
                MapCellInfo = mapCellInfo,
                OutputCreatingInfo = new OutputCreatingInfo
                {
                    CommandType = buildingMode ? CommandType.CreateHouse : CommandType.CreateUnit,
                    ObjectsTextureSize = buildingMode ? new Point(185, 160) : new Point(150, 150),
                    CreatingInfos = creatingInfos.ToArray(),
                }
            });
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCommandsBar,
                UnitInfo = unitInfo,
                MapCellInfo = mapCellInfo,
                CommandInfos = new []
                {
                    new OutputCommandInfo(CommandType.CloseCreatingMenu , true, "ExitIcon")
                }
            });
        }

        public void OpenProductionMenu(MapCellModel mapCellModel)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.OpenProductionMenu,
                UnitInfo = new UnitInfo(-1),
                MapCellInfo = new MapCellInfo(mapCellModel.X, mapCellModel.Y)
            });
            
            ClearCommandBar();
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCommandsBar,
                UnitInfo = new UnitInfo(-1),
                MapCellInfo = new MapCellInfo(mapCellModel.X, mapCellModel.Y),
                CommandInfos = new []
                {
                    new OutputCommandInfo(CommandType.CloseProductionMenu , true, "ExitIcon")
                }
            });
        }

        public void CreateHouse(UnitModel unitModel, MapCellModel mapCellModel, 
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            GameModel.EconomyModel.UpdateResourceSettings(
                BuildingEconomyModel.CountDiff[commandInfo.CreatingInfo.BuildingType],
                BuildingEconomyModel.StepCountDiff[commandInfo.CreatingInfo.BuildingType],
                BuildingEconomyModel.LimitDiff[commandInfo.CreatingInfo.BuildingType]);
            
            mapCellModel.UpdateBuildingType(commandInfo.CreatingInfo.BuildingType);
        }

        public void CreateUnit(UnitModel unitModel, MapCellModel mapCellModel,
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            GameModel.EconomyModel.UpdateResourceSettings(
                UnitEconomyModel.CountDiff[commandInfo.CreatingInfo.UnitType],
                UnitEconomyModel.StepCountDiff[commandInfo.CreatingInfo.UnitType],
                UnitEconomyModel.LimitDiff[commandInfo.CreatingInfo.UnitType]);

            for (var i = 0; i < int.MaxValue; i++)
            {
                if (GameModel.Units.ContainsKey(i)) continue;
                
                GameModel.Units[i] = new UnitModel(i, commandInfo.CreatingInfo.UnitType);
                var position = mapCellModel.MoveUnitToTheCell(GameModel.Units[i]);
                GameModel.Units[i].AddUnitToTheMap(mapCellModel, position);
                
                break;
            }
        }

        public void CloseCreatingMenu(UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                UpdateCommandBar(unitModel);
            else
                ClearCommandBar();
            
            ClearCreatingChoose();
        }

        public void CloseProductionMenu()
        {
            ClearCommandBar();
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.CloseProductionMenu
            });
        }

        public void FlyOrWalk(UnitModel unitModel, bool fly)
        {
            unitModel.TravelMode = fly ? TravelMode.Fly : TravelMode.Run;
            UpdateCommandBar(unitModel);
        }

        public void RefuelCommand(UnitModel unitModel)
        {
            unitModel.Refuel();
            UpdateCommandBar(unitModel);
        }

        public void UpdateCommandBar(UnitModel unitModel)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCommandsBar,
                UnitInfo = new UnitInfo(unitModel.ID),
                MapCellInfo = new MapCellInfo(unitModel.CurCell.X, unitModel.CurCell.Y),
                CommandInfos = unitModel.GetCommandInfos().ToArray()
            });
        }
        
        public void ClearCommandBar()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ClearCommandsBar
            });
        }

        private void InitializeCreatingChoose()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.InitializeCreatingChoose,
                OutputCreatingInfo = new OutputCreatingInfo
                {
                    BackgroundTextureName = "BuilderCard",
                    ResourceTexturesName = EconomyModel.ResourceTexturesName
                }
            });
        }

        public void ClearCreatingChoose()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ClearCreatingChoose
            });
        }

        private (ResourceType, string)[] GetObjectResourceDescription(Dictionary<ResourceType, float> countDiff, 
            Dictionary<ResourceType, float> stepCountDiff,
            Dictionary<ResourceType, float> limitDiff)
        {
            var objectResourceDescription = countDiff
                .Select(diff => (diff.Key, diff.Value.ToString())).ToList();
            objectResourceDescription.AddRange(stepCountDiff
                .Select(diff => (diff.Key, diff.Value + " за ход")));
            objectResourceDescription.AddRange(limitDiff
                .Select(diff => (diff.Key, diff.Value + " в лимиты")));
            
            return objectResourceDescription.ToArray();
        }
        
        private HashSet<ResourceType> GetNotAvailableResource(Dictionary<ResourceType, float> countDiff, 
            Dictionary<ResourceType, float> resourceCount)
        {
            return countDiff
                .Where(pair => Math.Abs(pair.Value) > resourceCount[pair.Key])
                .Select(pair => pair.Key)
                .ToHashSet();
        }
    }
    
        
    public class CreatingInfo
    {
        public string ObjectName;
        public string ObjectTextureName;
        public (ResourceType, string)[] ObjectResourceDescription;
        public HashSet<ResourceType> NotAvailableResource;
        public BuildingType BuildingType;
        public UnitType UnitType;
    }

}