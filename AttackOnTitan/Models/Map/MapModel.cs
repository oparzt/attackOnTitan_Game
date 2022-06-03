using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class MapModel: IEnumerable<MapCellModel>
    {
        public readonly int ColumnsCount;
        public readonly int RowsCount;

        private MapCellModel[,] _mapCells;

        public MapCellModel this[int x, int y] => _mapCells[x, y];
        public MapCellModel[] OuterGates;
        public MapCellModel[] InnerGates;

        public MapModel(int columnsCount, int rowsCount)
        {
            ColumnsCount = columnsCount;
            RowsCount = rowsCount;

            InitializeMap();
        }

        private void InitializeMap()
        {
            _mapCells = new MapCellModel[ColumnsCount, RowsCount];
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.InitializeMap,
                MapCellInfo = new MapCellInfo(ColumnsCount, RowsCount)
                {
                    TextureName = MapTextures.SimpleHexagon
                }
            });

            for (var x = 0; x < ColumnsCount; x++)
            for (var y = 0; y < RowsCount; y++)
                _mapCells[x, y] = new MapCellModel(x, y);
            
            for (var x = 0; x < ColumnsCount; x++)
            for (var y = 0; y < RowsCount; y++)
                _mapCells[x, y].ConnectWithNearCells(_mapCells, ColumnsCount, RowsCount);
        }

        public void InitializeBuildings(Dictionary<BuildingType, (int, int)[]> buildings)
        {
            foreach (var (buildingType, buildingCoords) in buildings)
            foreach (var (x, y) in buildingCoords)
                _mapCells[x, y].UpdateBuildingType(buildingType);
            
            if (buildings.TryGetValue(BuildingType.HiddenNone, out var hiddenBuildingsCoords))
                foreach (var (x, y) in hiddenBuildingsCoords)
                    SetHidden(_mapCells[x, y]);

            OuterGates = buildings[BuildingType.OuterGates]
                .Select(coords => _mapCells[coords.Item1, coords.Item2])
                .ToArray();
            
            InnerGates = buildings[BuildingType.InnerGates]
                .Select(coords => _mapCells[coords.Item1, coords.Item2])
                .ToArray();
        }

        private static void SetHidden(MapCellModel mapCell)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.SetCellHidden,
                MapCellInfo = new MapCellInfo(mapCell.X, mapCell.Y)
            });
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureOverCell,
                MapCellInfo = new MapCellInfo(mapCell.X, mapCell.Y)
                {
                    TextureName = MapTextures.HexagonTextureNames[BuildingType.HiddenNone]
                }
            });
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

        public static void SetHexTextureFor(IEnumerable<MapCellModel> mapCellModels, bool clear)
        {
            foreach (var mapCellModel in mapCellModels)
            {
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ChangeTextureOverCell,
                    MapCellInfo = new MapCellInfo(mapCellModel.X, mapCellModel.Y)
                    {
                        TextureName = clear ?
                            MapTextures.SimpleHexagon :
                            MapTextures.HexagonTextureNames[mapCellModel.BuildingType]
                    }
                });
            }
        }

        public IEnumerator<MapCellModel> GetEnumerator()
        {
            if (_mapCells is null) yield break;
            for (var x = 0; x < ColumnsCount; x++)
            for (var y = 0; y < RowsCount; y++)
                yield return this[x, y];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
