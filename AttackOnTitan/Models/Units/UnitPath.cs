using System;
using System.Collections.Generic;
using System.Linq;


namespace AttackOnTitan.Models
{
    public class UnitPath
    {
        private readonly GameModel _gameModel;
        private UnitModel _unit;
        private DijkstraPath _lastPath;

        public bool CanExecute { get; private set; }

        public UnitPath(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void SetUnit(UnitModel unit)
        {
            _unit = unit;

            MapModel.SetHexTextureFor(_gameModel.PathFinder.Paths.Keys, true);

            if (unit is null)
            {
                _gameModel.StatusBarModel.ClearStatusBar();
            }
            else
            {
                _gameModel.StatusBarModel.UpdateStatusBar(UnitModel.UnitNames[unit.UnitType], 
                    unit.Energy, unit.Gas, unit.UnitDamage);
                
                if (unit.IsEnemy) return;
                _gameModel.PathFinder.SetUnit(unit);
                _lastPath = _gameModel.PathFinder.Paths[unit.CurCell];
                
                MapModel.SetHexTextureFor(_gameModel.PathFinder.Paths.Keys, false);
            }
        }

        public void SetPath(MapCellModel mapCell)
        {
            CanExecute = true;

            ClearLastPath();
            if (_gameModel.PathFinder.Paths.TryGetValue(mapCell, out var path))
                _lastPath = path;
            DrawPath();
                
            _gameModel.StatusBarModel.UpdateStatusBar(UnitModel.UnitNames[_unit.UnitType], 
                _unit.Energy - _lastPath.EnergyCost, _unit.Gas - _lastPath.GasCost, _unit.UnitDamage);
        }

        public void ExecutePath()
        {
            CanExecute = false;
            
            ClearLastPath();

            var preLastCell = _unit.CurCell;
            foreach (var cell in _lastPath.SkipLast(1))
            {
                InitMoveUnit(_unit, cell.X, cell.Y, Position.Center);
                preLastCell = cell;
            }

            var lastCell = _lastPath.Last();

            if (lastCell != _unit.CurCell)
            {
                var endPosition = _lastPath.IsEnemyCell ? 
                    lastCell.MoveUnitToBorder(_unit, preLastCell) :
                    lastCell.MoveUnitToTheCell(_unit);

                if (_lastPath.IsEnemyCell)
                    _unit.CanGo = false;
                _unit.Moved = true;
                _unit.CurCell.RemoveUnitFromCell(_unit);
                _unit.CurCell = lastCell;

                InitMoveUnit(_unit, lastCell.X, lastCell.Y, endPosition);
            }

            _unit.Energy -= _lastPath.EnergyCost;
            _unit.Gas -= _lastPath.GasCost;
            _gameModel.CommandModel.UpdateCommandBar(_unit);
            SetUnit(_unit);
        }

        public void InitMoveUnit(UnitModel unitModel, int x, int y, Position position)
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

        private void ClearLastPath()
        {
            foreach (var cell in _lastPath)
                MapModel.SetUnselectedOpacity(cell);
        }

        private void DrawPath()
        {
            foreach (var cell in _lastPath)
                MapModel.SetSelectedOpacity(cell);
        }
    }
}