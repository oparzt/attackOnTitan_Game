using System;
using System.Collections.Generic;


namespace AttackOnTitan.Models
{
    public class UnitPath
    {
        public GameModel GameModel;
        public int Count { get => _pathStack.Count; }

        private UnitModel _unit;

        private Stack<MapCellModel> _pathStack = new();
        private Stack<MapCellModel> _endPathStack = new();
        private Stack<int> _pathCosts = new();
        private HashSet<MapCellModel> _pathHash = new();

        private int _pathCost = 0;

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
            else if (lastCell.TryGetCost(mapCell, _unit, out var cost)
                && (_pathCost + cost) <= _unit.Energy)
                    AddToPath(mapCell, cost);
        }

        private void RemovePathToTheCell(MapCellModel mapCell)
        {
            while (_pathStack.TryPop(out var prevMapCell))
            {
                _pathHash.Remove(prevMapCell);
                _pathCosts.Pop();
                GameModel.Map.SetUnselectedOpacity(prevMapCell);

                if (mapCell == prevMapCell) break;
            }

            if (_pathCosts.TryPeek(out var lastCost))
                _pathCost = lastCost;
            else
                _pathCost = 0;
        }

        private void AddToPath(MapCellModel mapCell, int cost)
        {
            _pathCost += cost;

            _pathStack.Push(mapCell);
            _pathCosts.Push(_pathCost);
            _pathHash.Add(mapCell);

            GameModel.Map.SetSelectedOpacity(mapCell);
        }

        public void ExecutePath()
        {
            if (_pathStack.TryPeek(out var lastCell))
            {
                _unit.CurCell = lastCell;
                _unit.Moved = true;
                _unit.Energy -= _pathCost;
            }

            while (_pathStack.TryPop(out var prevMapCell))
            {
                GameModel.Map.SetUnselectedOpacity(prevMapCell);
                _endPathStack.Push(prevMapCell);
            }
            while (_endPathStack.TryPop(out var targetCell))
                GameModel.OutputActions.Enqueue(new(OutputActionType.MoveUnit,
                    new(_unit.ID, targetCell.X, targetCell.Y), null));

            _pathHash.Clear();
            _pathCosts.Clear();
            _pathCost = 0;
        }
    }
}