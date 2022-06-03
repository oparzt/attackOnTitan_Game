using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public enum CommandType
    {
        Attack,
        AttackDisabled,
        Fly,
        FlyDisabled,
        Walk,
        WalkDisabled,
        Refuel,
        RefuelDisabled,
        OpenCreatingHouseMenu,
        OpenCreatingHouseMenuDisabled,
        OpenCreatingUnitMenu,
        OpenProductionMenu,
        CreateHouse,
        CreateUnit,
        CloseCreatingMenu,
        CloseProductionMenu,
        ChangePeopleAtWork
    }
    
    public class CommandModel
    {
        private readonly GameModel _gameModel;
        private static readonly Random Random = new();
        
        public CommandModel(GameModel gameModel)
        {
            _gameModel = gameModel;
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
                        ObjectTextureName = MapTextures.BuildingTextureNames[buildingType],
                        ObjectResourceDescription = GetObjectResourceDescription(BuildingEconomyModel.CountDiff[buildingType], BuildingEconomyModel.StepCountDiff[buildingType], BuildingEconomyModel.LimitDiff[buildingType]),
                        NotAvailableResource = GetNotAvailableResource(BuildingEconomyModel.CountDiff[buildingType], _gameModel.EconomyModel.ResourceCount)
                    }));
            }
            else
            {
                creatingInfos.AddRange(mapCellModel.GetPossibleCreatingUnitTypes()
                    .Select(unitType => new CreatingInfo
                    {
                        UnitType = unitType,
                        ObjectName = UnitModel.UnitNames[unitType],
                        ObjectTextureName = UnitTextures.UnitTextureNames[unitType],
                        ObjectResourceDescription = GetObjectResourceDescription(UnitEconomyModel.CountDiff[unitType], UnitEconomyModel.StepCountDiff[unitType], UnitEconomyModel.LimitDiff[unitType]),
                        NotAvailableResource = GetNotAvailableResource(UnitEconomyModel.CountDiff[unitType], _gameModel.EconomyModel.ResourceCount)
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
                    new OutputCommandInfo(CommandType.CloseCreatingMenu , true)
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
                    new OutputCommandInfo(CommandType.CloseProductionMenu , true)
                }
            });
        }

        public void CreateHouse(UnitModel unitModel, MapCellModel mapCellModel, 
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            unitModel.BuiltCount++;
            mapCellModel.UpdateBuildingType(commandInfo.CreatingInfo.BuildingType);
            _gameModel.EconomyModel.UseResource(BuildingEconomyModel.CountDiff[commandInfo.CreatingInfo.BuildingType]);
            _gameModel.EconomyModel.UpdateResourceSettings();
            
            if (unitModel.BuiltCount == 3)
            {
                RemoveUnit(unitModel);
                ClearCommandBar();
                _gameModel.UnitPath.SetUnit(null);
            }
            else
            {
                UpdateCommandBar(unitModel);
                _gameModel.UnitPath.SetUnit(unitModel);
            };
        }

        public void CreateUnit(UnitModel unitModel, MapCellModel mapCellModel,
            InputCommandInfo commandInfo)
        {
            CloseCreatingMenu(unitModel, mapCellModel);
            
            var id = GetEmptyIDForUnit();
            _gameModel.Units[id] = new UnitModel(id, commandInfo.CreatingInfo.UnitType);
            _gameModel.Units[id].AddUnitToTheMap(mapCellModel);
            _gameModel.EconomyModel.UseResource(UnitEconomyModel.CountDiff[commandInfo.CreatingInfo.UnitType]);
            _gameModel.EconomyModel.UpdateResourceSettings();
        }

        public void CreateTitans(int titansCount)
        {
            while (titansCount != 0)
            {
                var gates = _gameModel.Map.OuterGates;
                var gateCount = gates.Length > titansCount ? titansCount : gates.Length;

                foreach (var gate in gates.Take(gateCount))
                {
                    var id = GetEmptyIDForUnit();
                    _gameModel.Units[id] = new UnitModel(id, UnitType.Titan);
                    _gameModel.Units[id].AddUnitToTheMap(gate);
                    titansCount--;
                }
            }
        }

        private int GetEmptyIDForUnit()
        {
            for (var i = 0; i < int.MaxValue; i++)
            {
                if (_gameModel.Units.ContainsKey(i)) continue;
                return i;
            }

            throw new IndexOutOfRangeException("Нет места для новых юнитов");
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
            unitModel.IsFly = fly;
            unitModel.SetPossibleTravelModes();
            _gameModel.UnitPath.SetUnit(unitModel);
            UpdateCommandBar(unitModel);
        }

        public void RefuelCommand(UnitModel unitModel)
        {
            unitModel.Refuel();
            _gameModel.UnitPath.SetUnit(unitModel);
            UpdateCommandBar(unitModel);
        }

        public void AttackCommand(MapCellModel mapCellModel)
        {
            var units = mapCellModel
                .GetAllUnitInCell(false)
                .OrderByDescending(unit => unit.UnitDamage)
                .ToList();
            var titans = mapCellModel
                .GetAllUnitInCell(true)
                .OrderByDescending(unit => unit.UnitDamage)
                .ToList();

            while (units.Count != 0 && titans.Count != 0)
            {
                var titan = titans[0];
                var attackUnits = new List<UnitModel>();
                var unitsDamage = 0f;

                foreach (var unit in units)
                {
                    unitsDamage += unit.UnitDamage;
                    attackUnits.Add(unit);
                    if (unitsDamage > titan.UnitDamage)
                        break;
                }
                
                var winBattle = CheckWinBattle(titan.UnitDamage, unitsDamage, 
                    attackUnits.Count, out var deadUnitsCount);

                for (var i = 0; i < deadUnitsCount; i++)
                {
                    var deadUnit = attackUnits[Random.Next(0, attackUnits.Count)];
                    attackUnits.Remove(deadUnit);
                    units.Remove(deadUnit);
                    RemoveUnit(deadUnit);
                }

                if (winBattle)
                {
                    titans.Remove(titan);
                    RemoveUnit(titan);
                }
            }

            foreach (var unitModel in titans.Concat(units))
            {
                mapCellModel.RemoveUnitFromCell(unitModel);
                unitModel.CanGo = true;
            }
            foreach (var unitModel in titans.Concat(units))
            {
                if (mapCellModel.IsExistEmptyPositionInCell())
                {
                    var position = mapCellModel.MoveUnitToTheCell(unitModel);
                    _gameModel.UnitPath.InitMoveUnit(unitModel, mapCellModel.X, mapCellModel.Y, position);
                }
                else
                {
                    var cell = mapCellModel.NearCells.Keys.First(cell => cell.IsExistEmptyPositionInCell());
                    var position = mapCellModel.MoveUnitToTheCell(unitModel);
                    _gameModel.UnitPath.InitMoveUnit(unitModel, cell.X, cell.Y, position);
                }
            }
        }

        private bool CheckWinBattle(float titanDamage, float unitDamage, int unitsCount, out int deadUnitsCount)
        {
            if (unitDamage > titanDamage)
            {
                var isExistDeadUnits = Random.Next(1, 101) < 100 / unitsCount;
                deadUnitsCount = isExistDeadUnits ? Random.Next(0, unitsCount) : 0;
                return true;
            }
            
            deadUnitsCount = Random.Next(0, unitsCount + 1);
            deadUnitsCount = deadUnitsCount > 0 ? deadUnitsCount : 
                    unitsCount == 1 ? 1 : 0;
            return Random.Next(1, 101) > 100 / unitsCount;
        }

        private void RemoveUnit(UnitModel unitModel)
        {
            _gameModel.Units.Remove(unitModel.ID);
            unitModel.CurCell.RemoveUnitFromCell(unitModel);
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.RemoveUnit,
                UnitInfo = new UnitInfo(unitModel.ID)
            });
            _gameModel.EconomyModel.UpdateResourceSettings();
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
                    ResourceTexturesName = EconomyTextures.ResourceTexturesName
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

        private (ResourceType, string)[] GetObjectResourceDescription(Dictionary<ResourceType, int> countDiff, 
            Dictionary<ResourceType, int> stepCountDiff,
            Dictionary<ResourceType, int> limitDiff)
        {
            var objectResourceDescription = countDiff
                .Select(diff => (diff.Key, diff.Value.ToString())).ToList();
            objectResourceDescription.AddRange(stepCountDiff
                .Select(diff => (diff.Key, diff.Value + " за ход")));
            objectResourceDescription.AddRange(limitDiff
                .Select(diff => (diff.Key, diff.Value + " в лимиты")));
            
            return objectResourceDescription.ToArray();
        }
        
        private HashSet<ResourceType> GetNotAvailableResource(Dictionary<ResourceType, int> countDiff, 
            Dictionary<ResourceType, int> resourceCount)
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