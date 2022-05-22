using System;
using System.Collections.Generic;


namespace AttackOnTitan.Models
{
    public class UnitPath
    {
        public readonly GameModel GameModel;
        public int Count { get => _pathStack.Count; }

        private UnitModel _unit;

        private Stack<MapCellModel> _pathStack = new();
        private Stack<MapCellModel> _endPathStack = new();
        private Stack<int> _pathCosts = new();
        private HashSet<MapCellModel> _pathHash = new();
        private MapCellModel _enemyCell;

        private int _pathCost;

        public UnitPath(GameModel gameModel)
        {
            GameModel = gameModel;
        }

        public void SetUnit(UnitModel unit)
        {
            _unit = unit;
            _pathStack.Clear();
            _pathHash.Clear();
            _pathCosts.Clear();

            _pathCost = 0;
        }

        public void Add(MapCellModel mapCell)
        {
            if (_pathHash.Contains(mapCell)) RemovePathToTheCell(mapCell);

            var lastCellFound = _pathStack.TryPeek(out var lastCell);
            
            if (!lastCellFound)
                AddToPath(_unit.CurCell, 0);
            else if (_unit.CanGo
                && lastCell.TryGetCost(mapCell, _unit, out var cost)
                && (_pathCost + cost) <= _unit.Energy
                && _enemyCell is null)
            {
                var isEnemyCell = mapCell.IsEnemyInCell();

                if (!isEnemyCell)
                {
                    if (mapCell.IsExistEmptyPositionInCell())
                        AddToPath(mapCell, cost);
                }
                else if (mapCell.IsExistEmptyPositionOnBorder(lastCell))
                    AddToPath(mapCell, cost, true);
            }
        }

        private void RemovePathToTheCell(MapCellModel mapCell)
        {
            while (_pathStack.TryPop(out var prevMapCell))
            {
                _pathHash.Remove(prevMapCell);
                _pathCosts.Pop();
                GameModel.Map.SetUnselectedOpacity(prevMapCell);

                if (_enemyCell is not null && prevMapCell == _enemyCell) _enemyCell = null;
                if (mapCell == prevMapCell) break;
            }

            _pathCost = _pathCosts.TryPeek(out var lastCost) ? lastCost : 0;
        }

        private void AddToPath(MapCellModel mapCell, int cost, bool enemyCell = false)
        {
            if (enemyCell) _enemyCell = mapCell;
            _pathCost += cost;

            _pathStack.Push(mapCell);
            _pathCosts.Push(_pathCost);
            _pathHash.Add(mapCell);

            GameModel.Map.SetSelectedOpacity(mapCell);
        }

        public void ExecutePath()
        {
            GameModel.Map.SetUnselectedOpacity(_unit.CurCell);

            if (_pathStack.Count > 1)
            {
                _unit.Moved = true;
                _unit.Energy -= _pathCost;

                var prevMapCell = _unit.CurCell;
                var lastCell = _pathStack.Pop();
                _unit.CurCell.RemoveUnitFromCell(_unit);
                _unit.CurCell = lastCell;
                GameModel.Map.SetUnselectedOpacity(lastCell);

                while (_pathStack.Count > 1)
                {
                    prevMapCell = _pathStack.Pop();
                    GameModel.Map.SetUnselectedOpacity(prevMapCell);
                    _endPathStack.Push(prevMapCell);
                }

                while (_endPathStack.Count != 0)
                {
                    prevMapCell = _endPathStack.Pop();
                    GameModel.OutputActions.Enqueue(new OutputAction()
                    {
                        ActionType = OutputActionType.MoveUnit,
                        UnitInfo = new UnitInfo(_unit.ID)
                        {
                            X = prevMapCell.X,
                            Y = prevMapCell.Y,
                            Position = Position.Center
                        }
                    });
                }
                
                var endPosition = _enemyCell is not null ? 
                    lastCell.MoveUnitToBorder(_unit, prevMapCell) :
                    lastCell.MoveUnitToTheCell(_unit);
                if (_enemyCell is not null)
                    _unit.CanGo = false;
                
                GameModel.OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.MoveUnit,
                    UnitInfo = new UnitInfo(_unit.ID)
                    {
                        X = lastCell.X,
                        Y = lastCell.Y,
                        Position = endPosition
                    }
                });
                
                GameModel.CommandModel.UpdateCommandBar(_unit);
            }
            
            _pathStack.Clear();
            _pathHash.Clear();
            _pathCosts.Clear();
            _pathCost = 0;
            _enemyCell = null;
        }
    }
}