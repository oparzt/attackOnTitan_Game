using System;
using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class TitanPath
    {
        private readonly GameModel _gameModel;
        private static readonly Random Random = new();
        
        public TitanPath(GameModel gameModel) => _gameModel = gameModel;

        public void TitanStep(UnitModel unitModel)
        {
            unitModel.Energy -= unitModel.GetEnergyCost();

            if (unitModel.TitanTargetType == TitanTargetType.None)
                SetTitanTargetType(unitModel);
            SetTitanTarget(unitModel);
            TitanStepToTarget(unitModel);

            if (unitModel.TitanTarget == unitModel.CurCell ||
                unitModel.TitanTargetType == TitanTargetType.Attack)
            {
                unitModel.TitanTarget = null;
                unitModel.TitanTargetType = TitanTargetType.None;
            }

            if (unitModel.TitanTarget == unitModel.CurCell)
                unitModel.Energy = 0;

            CheckForDestroy(unitModel);
        }

        public void TitanStepToTarget(UnitModel unitModel)
        {
            var possibleNextCells = GetPossibleNearCellsPositions(unitModel);

            if (possibleNextCells.Length == 0)
                return;

            var nextCell = possibleNextCells[Random.Next(0, possibleNextCells.Length)];
            var position = nextCell.IsExistEmptyPositionInCell()
                ? nextCell.MoveUnitToTheCell(unitModel)
                : nextCell.MoveUnitToBorder(unitModel, unitModel.CurCell);

            InitMoveUnit(unitModel, nextCell, position);
        }

        public void SetTitanTargetType(UnitModel unitModel) =>
            unitModel.TitanTargetType = unitModel.CurCell.BuildingType == BuildingType.OuterNone ? 
                TitanTargetType.OuterGate : 
                unitModel.CurCell.NearCells.Keys.Any(cell => cell.GetAllUnitInCell(false).Any()) ?
                TitanTargetType.Attack : 
                TitanTargetType.InnerGate;

        public void SetTitanTarget(UnitModel unitModel)
        {
            unitModel.TitanTarget = unitModel.TitanTargetType == TitanTargetType.OuterGate ? 
                _gameModel.Map.OuterGates[Random.Next(0, _gameModel.Map.OuterGates.Length)] :
                unitModel.TitanTargetType == TitanTargetType.InnerGate ?
                _gameModel.Map.InnerGates[Random.Next(0, _gameModel.Map.InnerGates.Length)] :
                unitModel.CurCell.NearCells.Keys.First(cell => cell.GetAllUnitInCell(false).Any());
        }
        
        public MapCellModel[] GetPossibleNearCellsPositions(UnitModel unitModel)
        {
            return unitModel.CurCell.NearCells.Keys
                .Where(cell => 
                    unitModel.CurCell.GetPossibleTravelModesTo(cell).Contains(TravelMode.TitanRun) &&
                    (cell.IsExistEmptyPositionInCell() || 
                     (cell.GetAllUnitInCell(false).Any() && cell.IsExistEmptyPositionOnBorder(unitModel.CurCell))))
                .OrderBy(cell =>
                    {
                        var diffX = unitModel.TitanTarget.X - cell.X;
                        var diffY = unitModel.TitanTarget.Y - cell.Y;

                        return cell.GetAllUnitInCell(false).Any() ? 
                            0 : Math.Sqrt(diffX * diffX + diffY * diffY);
                    }
                )
                .TakeWhile((pair, pairIndex) => pairIndex < 2)
                .ToArray();
        }

        private void CheckForDestroy(UnitModel unitModel)
        {
            if (!MapCellModel.HousesBuildingTypes.Contains(unitModel.CurCell.BuildingType)) return;
            
            var cell = unitModel.CurCell;
            _gameModel.EconomyModel.UpdateResourceSettings(new Dictionary<ResourceType, float>(),
                BuildingEconomyModel.StepCountDiff[cell.BuildingType], 
                BuildingEconomyModel.LimitDiff[cell.BuildingType], 
                true);
            cell.UpdateBuildingType(cell.NearCells.Keys.Any(nearCell => 
                MapCellModel.HousesBuildingTypes.Contains(nearCell.BuildingType)) ? 
                BuildingType.BetweenHousesNone : 
                BuildingType.None);
            unitModel.Energy = 0;
        }
        
        private void InitMoveUnit(UnitModel unitModel, MapCellModel mapCellModel, Position position)
        {
            unitModel.CurCell.RemoveUnitFromCell(unitModel);
            unitModel.CurCell = mapCellModel;
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.MoveUnit,
                UnitInfo = new UnitInfo(unitModel.ID)
                {
                    X = mapCellModel.X,
                    Y = mapCellModel.Y,
                    Position = position
                }
            });
        }
    }
}