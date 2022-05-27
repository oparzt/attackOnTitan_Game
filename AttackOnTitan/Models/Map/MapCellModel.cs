using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public enum NearCellsName
    {
        LeftTop,
        LeftBottom,
        Top,
        Bottom,
        RightTop,
        RightBottom
    }

    public enum Position
    {
        Center,
        
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        
        TopBorder,
        BottomBorder,
        LeftTopBorder,
        RightTopBorder,
        LeftBottomBorder,
        RightBottomBorder
    } 
    
    public enum BuildingType
    {
        None,
        HiddenNone,
        BetweenHousesNone,

        House1,
        House2,
        House3,
        
        Barracks,
        Warehouse,
        Centre,
        
        Wall,
        InnerGates,
        OuterGates,
        ClosedGates,
    }

    public class MapCellModel
    {
        public readonly int X;
        public readonly int Y;
        public BuildingType BuildingType;

        private readonly Dictionary<MapCellModel, NearCellsName> _nearCells = new();

        public UnitModel UnitInCenterOfCell;
        public readonly Dictionary<Position, UnitModel> UnitsInCell = new();
        public readonly Dictionary<Position, UnitModel> UnitsOnBorder = new();

        #region Positions
        private static readonly Dictionary<NearCellsName, Position> NearCellsNameToPosition = new()
        {
            [NearCellsName.LeftTop] = Position.LeftTopBorder,
            [NearCellsName.Top] = Position.TopBorder,
            [NearCellsName.RightTop] = Position.RightTopBorder,
            [NearCellsName.RightBottom] = Position.RightBottomBorder,
            [NearCellsName.Bottom] = Position.BottomBorder,
            [NearCellsName.LeftBottom] = Position.LeftBottomBorder
        };
        
        private static readonly Position[] PossiblePositionsInCell = 
        { 
            Position.TopLeft, Position.TopRight, 
            Position.BottomLeft, Position.BottomRight
        };
        #endregion

        #region NearCellsStaticInfo
        private static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffsFromOdd = new()
        {
            [NearCellsName.LeftTop] = (-1, 0),
            [NearCellsName.LeftBottom] = (-1, 1),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, 0),
            [NearCellsName.RightBottom] = (1, 1)
        };

        private static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffsFromEven = new()
        {
            [NearCellsName.LeftTop] = (-1, -1),
            [NearCellsName.LeftBottom] = (-1, 0),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, -1),
            [NearCellsName.RightBottom] = (1, 0)
        };
        
        private Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffs => X % 2 == 0 ?
            PossibleNearCellsDiffsFromEven :
            PossibleNearCellsDiffsFromOdd;
        #endregion

        #region BuildingsInfo
        public static readonly Dictionary<BuildingType, string> BuildingTextureNames = new()
        {
            [BuildingType.House1] = "House1",
            [BuildingType.House2] = "House2",
            [BuildingType.House3] = "House3",
            
            [BuildingType.Barracks] = "Barracks",
            [BuildingType.Warehouse] = "Warehouse",
            [BuildingType.Centre] = "Centre",

            [BuildingType.ClosedGates] = "OuterGates"
        };

        public static readonly Dictionary<BuildingType, string> BuildingNames = new()
        {
            [BuildingType.House1] = "Дом 1",
            [BuildingType.House2] = "Дом 2",
            [BuildingType.House3] = "Дом 3",
            [BuildingType.Barracks] = "Казармы",
            [BuildingType.Warehouse] = "Склад",
            [BuildingType.Centre] = "Администрация",
            [BuildingType.Wall] = "Стена",
            [BuildingType.ClosedGates] = "Ворота"
        };

        private static readonly HashSet<BuildingType> HousesBuildingTypes = new()
        {
            BuildingType.House1,
            BuildingType.House2,
            BuildingType.House3,
        
            BuildingType.Barracks,
            BuildingType.Warehouse,
            BuildingType.Centre,
        };

        private static readonly HashSet<BuildingType> WallBuildingTypes = new()
        {
            BuildingType.Wall,
            BuildingType.InnerGates,
            BuildingType.OuterGates,
            BuildingType.ClosedGates,
        };
        #endregion

        #region PossibleTravelModes

        #endregion
        
        public MapCellModel(int x, int y, BuildingType buildingType = BuildingType.None)
        {
            X = x;
            Y = y;
            UpdateBuildingType(buildingType);
        }

        public void ConnectWithNearCells(MapCellModel[,] mapCells,
            int columnsCount, int rowsCount)
        {
            foreach (var (nearCellName, diff) in PossibleNearCellsDiffs)
            {
                var coordX = X + diff.Item1;
                var coordY = Y + diff.Item2;

                if (coordX < 0 || coordX >= columnsCount || coordY < 0 || coordY >= rowsCount)
                    continue;

                var mapCell = mapCells[coordX, coordY];

                if (!_nearCells.ContainsKey(mapCell))
                    _nearCells[mapCell] = nearCellName;
            }
        }

        private IEnumerable<TravelMode> GetPossibleTravelModesTo(MapCellModel mapCell)
        {
            switch (mapCell.BuildingType)
            {
                case BuildingType.None:
                    if (WallBuildingTypes.Contains(BuildingType))
                        yield return TravelMode.Fly;
                    else
                        yield return TravelMode.Run;

                    yield return TravelMode.BuilderRun;
                    
                    if (HousesBuildingTypes.Contains(BuildingType)) yield return TravelMode.Fly;
                    break;
                
                case BuildingType.BetweenHousesNone:
                    if (!WallBuildingTypes.Contains(BuildingType))
                        yield return TravelMode.Run;
                    yield return TravelMode.BuilderRun;
                    yield return TravelMode.Fly;
                    break;
                case BuildingType.House1:
                case BuildingType.House2:
                case BuildingType.House3:
                case BuildingType.Barracks:
                case BuildingType.Warehouse:
                case BuildingType.Centre:
                    if (HousesBuildingTypes.Contains(BuildingType)) 
                        yield return TravelMode.Run;
                    else if (WallBuildingTypes.Contains(BuildingType))
                        yield return TravelMode.Fly;
                    else
                    {
                        if (mapCell.BuildingType == BuildingType.Barracks) yield return TravelMode.Run;
                        yield return TravelMode.Fly;
                        yield return TravelMode.BuilderRun;
                    }
                    break;
                case BuildingType.Wall:
                    if (WallBuildingTypes.Contains(BuildingType))
                        yield return TravelMode.Run;
                    else 
                        yield return TravelMode.Fly;
                    break;
                case BuildingType.InnerGates:
                case BuildingType.OuterGates:
                case BuildingType.ClosedGates:
                    yield return TravelMode.BuilderRun;
                    yield return TravelMode.Fly;
                    yield return TravelMode.Run;
                    break;
                case BuildingType.HiddenNone:
                default:
                    yield return TravelMode.None;
                    yield break;
            }
        }

        public bool IsExistEmptyPositionInCell() => UnitsInCell.Count != 4;

        public bool IsExistTravelToCell(MapCellModel mapCell, UnitModel unitModel) =>
            _nearCells.ContainsKey(mapCell) &&
            GetPossibleTravelModesTo(mapCell).Contains(unitModel.TravelMode);
        
        public bool IsEnemyInCell() => UnitsInCell.Values
            .FirstOrDefault(unit => unit.UnitType == UnitType.Titan) is not null || UnitInCenterOfCell?.IsEnemy is true;

        public bool IsExistEmptyPositionOnBorder(MapCellModel mapCell) => _nearCells.TryGetValue(mapCell, out var nearCellsName) 
            && !UnitsOnBorder.ContainsKey(NearCellsNameToPosition[nearCellsName]);

        public Position MoveUnitToTheCell(UnitModel unitModel)
        {
            if (UnitInCenterOfCell is null)
            {
                if (UnitsInCell.Count == 0)
                {
                    UnitInCenterOfCell = unitModel;
                    return Position.Center;  
                }
            }
            else
            {
                var positionForUnitInCenter = GetEmptyPositionInCell();
                GameModel.OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.MoveUnit,
                    UnitInfo = new UnitInfo(UnitInCenterOfCell.ID)
                    {
                        X = X,
                        Y = Y,
                        Position = positionForUnitInCenter
                    }
                });
                UnitsInCell[positionForUnitInCenter] = UnitInCenterOfCell;
                UnitInCenterOfCell = null;
            }

            var positionForNewUnit = GetEmptyPositionInCell();
            UnitsInCell[positionForNewUnit] = unitModel;
            return positionForNewUnit;
        }

        public Position MoveUnitToBorder(UnitModel unitModel, MapCellModel mapCell)
        {
            var position = NearCellsNameToPosition[_nearCells[mapCell]];
            UnitsOnBorder[position] = unitModel;
            return position;
        }

        public void RemoveUnitFromCell(UnitModel unitModel)
        {
            if (UnitInCenterOfCell is not null && UnitInCenterOfCell == unitModel)
            {
                UnitInCenterOfCell = null;
                return;
            }

            var position = UnitsInCell
                .Where(curPositionPair => curPositionPair.Value == unitModel).
                Select(curPositionPair => curPositionPair.Key).FirstOrDefault();

            UnitsInCell.Remove(position);
            
            position = UnitsOnBorder
                .Where(curPositionPair => curPositionPair.Value == unitModel).
                Select(curPositionPair => curPositionPair.Key).FirstOrDefault();

            UnitsOnBorder.Remove(position);
        }

        private Position GetEmptyPositionInCell() =>
            PossiblePositionsInCell.FirstOrDefault(position => 
                !UnitsInCell.ContainsKey(position));

        public IEnumerable<UnitModel> GetAllUnitInCell(bool isEnemy)
        {
            if (UnitInCenterOfCell is not null && UnitInCenterOfCell.IsEnemy == isEnemy)
                yield return UnitInCenterOfCell;

            foreach (var unit in UnitsInCell.Values.Where(unit => unit.IsEnemy == isEnemy))
                yield return unit;
            foreach (var unit in UnitsOnBorder.Values.Where(unit => unit.IsEnemy == isEnemy))
                yield return unit;
        }
        
        public void UpdateBuildingType(BuildingType buildingType)
        {
            var textureFound = BuildingTextureNames.TryGetValue(buildingType, out var buildingTextureName);
            BuildingType = buildingType;
            
            if (buildingType == BuildingType.HiddenNone)
                MapModel.SetHidden(this);
            else if (buildingType == BuildingType.ClosedGates)
            {
                
            } else if (HousesBuildingTypes.Contains(buildingType))
            {
                foreach (var nearCell in _nearCells.Keys
                    .Where(cell => cell.BuildingType == BuildingType.None))
                    nearCell.UpdateBuildingType(BuildingType.BetweenHousesNone);
            }
            
            if (textureFound)
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ChangeTextureIntoCell,
                    MapCellInfo = new MapCellInfo(X, Y)
                    {
                        TextureName = buildingTextureName
                    }
                });
            else
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ClearTextureIntoCell,
                    MapCellInfo = new MapCellInfo(X, Y)
                });
        }

        #region PossibleCreating
        public IEnumerable<BuildingType> GetPossibleCreatingBuildingTypes()
        {
            if (IsEnemyInCell())
                yield break;
            switch (BuildingType)
            {
                case BuildingType.None:
                case BuildingType.BetweenHousesNone:
                    yield return BuildingType.Centre;
                    yield return BuildingType.House1;
                    yield return BuildingType.Barracks;
                    yield return BuildingType.Warehouse;
                    break;
                case BuildingType.OuterGates:
                    yield return BuildingType.ClosedGates;
                    break;
                case BuildingType.House1:
                case BuildingType.Barracks:
                case BuildingType.Warehouse:
                case BuildingType.Centre:
                case BuildingType.Wall:
                case BuildingType.InnerGates:
                case BuildingType.ClosedGates:
                case BuildingType.HiddenNone:
                case BuildingType.House2:
                case BuildingType.House3:
                default:
                    break;
            }
        }

        public IEnumerable<UnitType> GetPossibleCreatingUnitTypes()
        {
            if (IsEnemyInCell() || !IsExistEmptyPositionInCell())
                yield break;
            switch (BuildingType)
            {
                case BuildingType.Barracks:
                    yield return UnitType.Cadet;
                    yield return UnitType.Scout;
                    yield return UnitType.Garrison;
                    yield return UnitType.Police;
                    break;
                case BuildingType.Centre:
                    yield return UnitType.Builder;
                    break;
                case BuildingType.OuterGates:
                case BuildingType.None:
                case BuildingType.HiddenNone:
                case BuildingType.Warehouse:
                case BuildingType.Wall:
                case BuildingType.InnerGates:
                case BuildingType.ClosedGates:
                case BuildingType.House1:
                case BuildingType.House2:
                case BuildingType.House3:
                case BuildingType.BetweenHousesNone:
                default:
                    break;
            }
        }
        #endregion
    }
}
