using System;
using System.Collections.Generic;


namespace AttackOnTitan.Models
{
    public class UnitPath
    {
        private readonly GameModel _gameModel;
        public int Count => _pathStack.Count;

        private UnitModel _unit;

        private readonly Stack<MapCellModel> _pathStack = new();
        private readonly Stack<MapCellModel> _endPathStack = new();
        private readonly HashSet<MapCellModel> _pathHash = new();

        private readonly Stack<float> _pathEnergyCosts = new();
        private readonly Stack<float> _pathGasCosts = new();
        
        private MapCellModel _enemyCell;

        public UnitPath(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void SetUnit(UnitModel unit)
        {
            _unit = unit;
            _pathStack.Clear();
            _pathHash.Clear();
            _pathEnergyCosts.Clear();
            _pathGasCosts.Clear();
            _enemyCell = null;
            
            if (_unit is null)
                _gameModel.StatusBarModel.ClearStatusBar();
            else
                _gameModel.StatusBarModel.UpdateStatusBar(UnitModel.UnitNames[_unit.UnitType], 
                    _unit.Energy, _unit.Gas, _unit.UnitDamage);
        }

        public void Add(MapCellModel mapCell)
        {
            if (_pathHash.Contains(mapCell)) RemovePathToTheCell(mapCell);

            var lastCellFound = _pathStack.TryPeek(out var lastCell);
            
            
            if (!lastCellFound)
                AddToPath(_unit.CurCell, 0, 0);
            else
            {
                var curEnergyCost = _pathEnergyCosts.Peek() + _unit.GetEnergyCost();
                var curGasCost = _pathGasCosts.Peek() + _unit.GetGasCost();

                if (!_unit.CanGo || !lastCell.IsExistTravelToCell(mapCell, _unit) ||
                    !_unit.IsExistTravel(curEnergyCost, curGasCost) || _enemyCell is not null
                    || _unit.IsEnemy) return;
                
                var isEnemyCell = mapCell.IsEnemyInCell();

                if (!isEnemyCell)
                {
                    if (mapCell.IsExistEmptyPositionInCell())
                        AddToPath(mapCell, curEnergyCost, curGasCost);
                }
                else if (mapCell.IsExistEmptyPositionOnBorder(lastCell))
                    AddToPath(mapCell, curEnergyCost, curGasCost, true);
            }
            
        }

        private void RemovePathToTheCell(MapCellModel mapCell)
        {
            while (_pathStack.TryPop(out var prevMapCell))
            {
                _pathHash.Remove(prevMapCell);
                _pathEnergyCosts.Pop();
                _pathGasCosts.Pop();
                MapModel.SetUnselectedOpacity(prevMapCell);

                if (_enemyCell is not null && prevMapCell == _enemyCell) _enemyCell = null;
                if (mapCell == prevMapCell) break;
            }

            _pathEnergyCosts.TryPeek(out var energyCost);
            _pathGasCosts.TryPeek(out var gasCost);
            
            _gameModel.StatusBarModel.UpdateStatusBar(UnitModel.UnitNames[_unit.UnitType], 
                _unit.Energy - energyCost, _unit.Gas - gasCost, _unit.UnitDamage);
        }

        private void AddToPath(MapCellModel mapCell, float energyCost, float gasCost, bool enemyCell = false)
        {
            if (enemyCell) _enemyCell = mapCell;
            _pathStack.Push(mapCell);
            _pathEnergyCosts.Push(energyCost);
            _pathGasCosts.Push(gasCost);
            _pathHash.Add(mapCell);
            
            _gameModel.StatusBarModel.UpdateStatusBar(UnitModel.UnitNames[_unit.UnitType], 
                _unit.Energy - energyCost, _unit.Gas - gasCost, _unit.UnitDamage);

            MapModel.SetSelectedOpacity(mapCell);
        }

        public void ExecutePath()
        {
            MapModel.SetUnselectedOpacity(_unit.CurCell);

            _unit.Energy -= _pathEnergyCosts.Peek();
            _unit.Gas -= _pathGasCosts.Peek();

            if (_pathStack.Count > 1)
            {
                _unit.Moved = true;

                var prevMapCell = _unit.CurCell;
                var lastCell = _pathStack.Pop();
                _unit.CurCell.RemoveUnitFromCell(_unit);
                _unit.CurCell = lastCell;
                MapModel.SetUnselectedOpacity(lastCell);

                while (_pathStack.Count > 1)
                {
                    prevMapCell = _pathStack.Pop();
                    MapModel.SetUnselectedOpacity(prevMapCell);
                    _endPathStack.Push(prevMapCell);
                }

                while (_endPathStack.Count != 0)
                {
                    prevMapCell = _endPathStack.Pop();
                    InitMoveUnit(_unit, prevMapCell.X, prevMapCell.Y, Position.Center);
                }
                
                var endPosition = _enemyCell is not null ? 
                    lastCell.MoveUnitToBorder(_unit, prevMapCell) :
                    lastCell.MoveUnitToTheCell(_unit);
                if (_enemyCell is not null)
                    _unit.CanGo = false;
                
                InitMoveUnit(_unit, lastCell.X, lastCell.Y, endPosition);
                _gameModel.CommandModel.UpdateCommandBar(_unit);
            }
            
            SetUnit(_unit);
        }

        public void InitMoveUnit(UnitModel unitModel,  int x, int y, Position position)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.MoveUnit,
                UnitInfo = new UnitInfo(unitModel.ID)
                {
                    X = x,
                    Y = y,
                    Position = position
                }
            });
        }
    }
}