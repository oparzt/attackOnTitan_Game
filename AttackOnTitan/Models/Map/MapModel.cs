using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AttackOnTitan.Models
{
    public class MapModel
    {
        public readonly int ColumnsCount;
        public readonly int RowsCount;

        public UnitModel SelectedUnit;

        private MapCellModel[,] _mapCells;

        public MapCellModel this[int x, int y]
        {
            get => _mapCells[x, y];
        }

        public MapModel(int columnsCount, int rowsCount)
        {
            ColumnsCount = columnsCount;
            RowsCount = rowsCount;

            InitializeMap();
        }

        private void InitializeMap()
        {
            _mapCells = new MapCellModel[ColumnsCount, RowsCount];

            for (var x = 0; x < ColumnsCount; x++)
                for (var y = 0; y < RowsCount; y++)
                    _mapCells[x, y] = new(x, y);

            var nearCellsWeights = new Dictionary<NearCellsName, Weight>()
            {
                [NearCellsName.LeftTop] = new(1, 1, NearCellsName.LeftTop),
                [NearCellsName.LeftBottom] = new(1, 1, NearCellsName.LeftBottom),
                [NearCellsName.Top] = new(1, 1, NearCellsName.Top),
                [NearCellsName.Bottom] = new(1, 1, NearCellsName.Bottom),
                [NearCellsName.RightTop] = new(1, 1, NearCellsName.RightTop),
                [NearCellsName.RightBottom] = new(1, 1, NearCellsName.RightBottom)
            };

            for (var x = 0; x < ColumnsCount; x++)
                for (var y = 0; y < RowsCount; y++)
                    _mapCells[x, y].ConnectWithNearCells(_mapCells, ColumnsCount, RowsCount, nearCellsWeights);
        }

        public void SetCellOpacity(MapCellModel mapCell, float opacity) =>
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.ChangeCellOpacity,
                MapCellInfo = new MapCellInfo(mapCell.X, mapCell.Y)
                {
                    Opacity = opacity
                }
            });

        public void SetUnselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.3f);

        public void SetPreselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.65f);

        public void SetSelectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 1f);
    }
}
