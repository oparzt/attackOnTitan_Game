using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AttackOnTitan.Models
{
    public class Map
    {
        public readonly int _columnsCount;
        public readonly int _rowsCount;

        public Unit SelectedUnit;

        private MapCell[,] _mapCells;
        private readonly Stack<MapCell> _path = new();
        private readonly HashSet<MapCell> _pathHash = new();
        private readonly Dictionary<PressedMouseBtn, Action<InputAction>> _handlers = new ();

        private MapCell _lastNoMouseSelected;


        public Map(int columnsCount, int rowsCount)
        {
            _columnsCount = columnsCount;
            _rowsCount = rowsCount;

            InitializeMap();
            InitializeHandlers();
        }

        public void HandleSelect(InputAction action) =>
            _handlers[action.MouseBtn](action);

        public void AddUnit(Unit unit, int x, int y)
        {
            unit.CurCell = _mapCells[x, y];

            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.AddUnit,
                UnitInfo =
                {
                    ID = unit.ID,
                    X = x,
                    Y = y,
                    TextureName = "Ball"
                }
            });
        }

        public void SelectUnit(Unit unit)
        {
            SelectedUnit = unit;

            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeUnitOpacity,
                UnitInfo =
                {
                    ID = SelectedUnit.ID,
                    Opacity = 1f
                }
            });
        }

        public void UnselectUnit()
        {
            if (SelectedUnit is not null)
            {
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ChangeUnitOpacity,
                    UnitInfo =
                    {
                        ID = SelectedUnit.ID,
                        Opacity = 0.65f
                    }
                });
                SelectedUnit = null;
            }
        }

        private void InitializeMap()
        {
            _mapCells = new MapCell[_columnsCount, _rowsCount];

            for (var x = 0; x < _columnsCount; x++)
            for (var y = 0; y < _rowsCount; y++)
                _mapCells[x, y] = new(x, y);

            var nearCellsWeights = new Dictionary<NearCellsName, Weight>()
            {
                [NearCellsName.LeftTop] = new(1, 1),
                [NearCellsName.LeftBottom] = new(1, 1),
                [NearCellsName.Top] = new(1, 1),
                [NearCellsName.Bottom] = new(1, 1),
                [NearCellsName.RightTop] = new(1, 1),
                [NearCellsName.RightBottom] = new(1, 1)
            };

            for (var x = 0; x < _columnsCount; x++)
            for (var y = 0; y < _rowsCount; y++)
                _mapCells[x, y].ConnectWithNearCells(_mapCells, _columnsCount, _rowsCount, nearCellsWeights);
        }

        private void InitializeHandlers()
        {
            _handlers[PressedMouseBtn.None] = HandleNoMouseSelect;
            _handlers[PressedMouseBtn.Left] = HandleLeftMouseSelect;
            _handlers[PressedMouseBtn.Right] = HandleRightMouseSelect;
        }

        private void HandleNoMouseSelect(InputAction action)
        {
            ClearPathOpacity();
            if (_lastNoMouseSelected is not null)
                ClearCellOpacity(_lastNoMouseSelected);

            _lastNoMouseSelected = _mapCells[action.SelectedCell.X, action.SelectedCell.Y];

            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeCellOpacity,
                MapCellInfo =
                {
                    X = action.SelectedCell.X,
                    Y = action.SelectedCell.Y,
                    Opacity = 0.75f
                }
            });
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            UnselectUnit();
        }

        private void HandleRightMouseSelect(InputAction action)
        {
            // TODO: Добавить добавление только прилежащих клеток
            // TODO: Учитывать вес клетки
            // TODO: Остановить любое действие до окончания движения
            if (SelectedUnit is null)
            {
                HandleNoMouseSelect(action);
                return;
            }

            if (_path.Count == 0) AddCellToPath(SelectedUnit.CurCell);

            AddCellToPath(_mapCells[action.SelectedCell.X, action.SelectedCell.Y]);
        }

        private void AddCellToPath(MapCell mapCell)
        {
            if (_pathHash.Contains(mapCell))
            {
                while (_path.TryPop(out var prevMapCell) && mapCell != prevMapCell)
                {
                    _pathHash.Remove(prevMapCell);
                    ClearCellOpacity(prevMapCell);
                }
            }

            _path.Push(mapCell);
            _pathHash.Add(mapCell);

            SetCellOpacity(mapCell, 1f);
        }

        private void SetCellOpacity(MapCell mapCell, float opacity)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeCellOpacity,
                MapCellInfo =
                    {
                        X = mapCell.X,
                        Y = mapCell.Y,
                        Opacity = opacity
                    }
            });
        }

        private void ClearPathOpacity()
        {
            _pathHash.Clear();
            while (_path.TryPop(out var mapCell))
                ClearCellOpacity(mapCell);
        }

        private void ClearCellOpacity(MapCell mapCell)
        {
            SetCellOpacity(mapCell, 0.3f);
            
        }
    }
}
