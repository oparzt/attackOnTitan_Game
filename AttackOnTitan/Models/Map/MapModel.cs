using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class MapModel
    {
        public readonly int ColumnsCount;
        public readonly int RowsCount;

        private MapCellModel[,] _mapCells;

        public MapCellModel this[int x, int y] => _mapCells[x, y];
        public MapCellModel[] OuterGates;
        public MapCellModel[] InnerGates;

        public MapModel(int columnsCount, int rowsCount, Dictionary<BuildingType, (int, int)[]> buildings)
        {
            ColumnsCount = columnsCount;
            RowsCount = rowsCount;

            InitializeMap(buildings);
        }

        private void InitializeMap(Dictionary<BuildingType, (int, int)[]> buildings)
        {
            _mapCells = new MapCellModel[ColumnsCount, RowsCount];
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.InitializeMap,
                MapCellInfo = new MapCellInfo(ColumnsCount, RowsCount)
            });

            for (var x = 0; x < ColumnsCount; x++)
            for (var y = 0; y < RowsCount; y++)
                _mapCells[x, y] = new MapCellModel(x, y);
            
            for (var x = 0; x < ColumnsCount; x++)
            for (var y = 0; y < RowsCount; y++)
                _mapCells[x, y].ConnectWithNearCells(_mapCells, ColumnsCount, RowsCount);

            foreach (var (buildingType, buildingCoords) in buildings)
            foreach (var (x, y) in buildingCoords)
                _mapCells[x, y].UpdateBuildingType(buildingType);

            OuterGates = buildings[BuildingType.OuterGates]
                .Select(coords => _mapCells[coords.Item1, coords.Item2])
                .ToArray();
            
            InnerGates = buildings[BuildingType.InnerGates]
                .Select(coords => _mapCells[coords.Item1, coords.Item2])
                .ToArray();
        }

        public static void SetHidden(MapCellModel mapCell)
        {
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.SetCellHidden,
                MapCellInfo = new MapCellInfo(mapCell.X, mapCell.Y)
            });
            SetCellOpacity(mapCell, 0);
        }
        
        public static void SetUnselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.3f);

        public static void SetPreselectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 0.65f);

        public static void SetSelectedOpacity(MapCellModel mapCell) =>
            SetCellOpacity(mapCell, 1f);
        
        private static void SetCellOpacity(MapCellModel mapCell, float opacity) =>
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.ChangeCellOpacity,
                MapCellInfo = new MapCellInfo(mapCell.X, mapCell.Y)
                {
                    Opacity = opacity
                }
            });
    }
}
