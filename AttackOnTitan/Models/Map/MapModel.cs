using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AttackOnTitan.Models
{
    public class MapModel
    {
        public readonly int _columnsCount;
        public readonly int _rowsCount;

        public UnitModel SelectedUnit;

        private MapCellModel[,] _mapCells;

        public MapCellModel this[int x, int y]
        {
            get => _mapCells[x, y];
        }

        public MapModel(int columnsCount, int rowsCount)
        {
            _columnsCount = columnsCount;
            _rowsCount = rowsCount;

            InitializeMap();
        }

        private void InitializeMap()
        {
            _mapCells = new MapCellModel[_columnsCount, _rowsCount];

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

        public void SetCellOpacity(MapCellModel mapCell, float opacity) =>
            GameModel.OutputActions.Enqueue(new(OutputActionType.ChangeCellOpacity,
                null, new(mapCell.X, mapCell.Y, opacity)));

        public void SetUnselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.3f);

        public void SetPreselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.65f);

        public void SetSelectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 1f);
    }
}
