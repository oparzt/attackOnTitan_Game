using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public class CommandModel
    {
        public readonly GameModel GameModel;
        
        private static readonly OutputCommandInfo[] DefaultCommandInfos = { };
        private static readonly OutputCreatingInfo DefaultOutputCreatingInfo = new()
        {
            BackgroundTextureName = "BuilderCard",
            CreatingInfos = new CreatingInfo[] {}
        };

        public CommandModel(GameModel gameModel)
        {
            GameModel = gameModel;
        }

        public void OpenCreatingMenu(UnitModel unitModel, MapCellModel mapCellModel, 
            bool buildingMode)
        {
            var creatingInfos = buildingMode ? 
                mapCellModel.GetPossibleBuildingInCell() : 
                mapCellModel.GetPossibleUnitInCell();
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
                    BackgroundTextureName = "BuilderCard",
                    CreatingInfos = creatingInfos,
                    NotAvailableResource = creatingInfos
                        .Select(creatingInfo => GetNotAvailableResourceForCreatingInfos(creatingInfo, GameModel.ResourceCount))
                        .ToArray()
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

        public void CreateHouse(UnitModel unitModel, MapCellModel mapCellModel, 
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            
            foreach (var pricePair in commandInfo.CreatingInfo.Price)
                GameModel.ResourceCount[pricePair.Key] -= pricePair.Value;
            GameModel.UpdateResourceCount();
            
            mapCellModel.UpdateBuildingType(
                commandInfo.CreatingInfo.BuildingType,
                commandInfo.BuildingTextureName);
        }

        public void CreateUnit(UnitModel unitModel, MapCellModel mapCellModel,
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            
            foreach (var pricePair in commandInfo.CreatingInfo.Price)
                GameModel.ResourceCount[pricePair.Key] -= pricePair.Value;
            GameModel.UpdateResourceCount();

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

        public void FlyOrWalk(UnitModel unitModel, bool fly)
        {
            unitModel.IsFly = fly;
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
                ActionType = OutputActionType.UpdateCommandsBar,
                CommandInfos = DefaultCommandInfos
            });
        }

        public void ClearCreatingChoose()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateCreatingChoose,
                OutputCreatingInfo = DefaultOutputCreatingInfo
            });
        }
        
        private HashSet<ResourceType> GetNotAvailableResourceForCreatingInfos(CreatingInfo creatingInfo, Dictionary<ResourceType, int> resourceCount)
        {
            return creatingInfo.Price
                .Where(pair => pair.Value > resourceCount[pair.Key])
                .Select(pair => pair.Key)
                .ToHashSet();
        }
    }
}