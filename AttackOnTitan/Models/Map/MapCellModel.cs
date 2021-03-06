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
        NearWall,
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

        public readonly Dictionary<MapCellModel, NearCellsName> NearCells = new();

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
        // Потенциальные клетки по близости для клеток с нечетным X
        private static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffsFromOdd = new()
        {
            [NearCellsName.LeftTop] = (-1, 0),
            [NearCellsName.LeftBottom] = (-1, 1),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, 0),
            [NearCellsName.RightBottom] = (1, 1)
        };

        // Потенциальные клетки по близости для клеток с четным X
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

        public static readonly HashSet<BuildingType> HousesBuildingTypes = new()
        {
            BuildingType.House1,
            BuildingType.House2,
            BuildingType.House3,
        
            BuildingType.Barracks,
            BuildingType.Warehouse,
            BuildingType.Centre,
        };

        public static readonly HashSet<BuildingType> WallBuildingTypes = new()
        {
            BuildingType.Wall,
            BuildingType.InnerGates,
            BuildingType.OuterGates,
            BuildingType.ClosedGates,
        };
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

                if (!NearCells.ContainsKey(mapCell))
                    NearCells[mapCell] = nearCellName;
            }
        }

        public IEnumerable<TravelMode> GetPossibleTravelModesTo(MapCellModel mapCell)
        {
            if (!NearCells.ContainsKey(mapCell))
                yield break;

            switch (mapCell.BuildingType)
            {
                case BuildingType.None:
                    if (WallBuildingTypes.Contains(BuildingType))
                        yield return TravelMode.Fly;
                    else
                        yield return TravelMode.Run;

                    yield return TravelMode.BuilderRun;
                    yield return TravelMode.TitanRun;
                    if (HousesBuildingTypes.Contains(BuildingType)) yield return TravelMode.Fly;
                    break;
                
                case BuildingType.BetweenHousesNone:
                    yield return TravelMode.Run;
                    yield return TravelMode.BuilderRun;
                    yield return TravelMode.TitanRun;
                    if (HousesBuildingTypes.Contains(BuildingType) ||
                        (BuildingType == BuildingType.BetweenHousesNone &&
                        NearCells.Keys
                            .Intersect(mapCell.NearCells.Keys)
                            .Any(cell => HousesBuildingTypes.Contains(cell.BuildingType))))
                        yield return TravelMode.Fly;
                    break;
                case BuildingType.NearWall:
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
                    else if (mapCell.BuildingType == BuildingType.NearWall)
                        yield return TravelMode.Fly;
                    else
                    {
                        if (mapCell.BuildingType == BuildingType.Barracks) yield return TravelMode.Run;
                        yield return TravelMode.Fly;
                        yield return TravelMode.BuilderRun;
                    }
                    
                    yield return TravelMode.TitanRun;
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
                    yield return TravelMode.TitanRun;
                    break;
                case BuildingType.HiddenNone:
                default:
                    yield return TravelMode.None;
                    yield break;
            }
        }

        public bool IsExistEmptyPositionInCell() => UnitsInCell.Count != 4;

        public bool IsExistTravelToCell(MapCellModel mapCell, TravelMode travelMode) =>
            NearCells.ContainsKey(mapCell) &&
            GetPossibleTravelModesTo(mapCell).Contains(travelMode);
        
        public bool IsEnemyInCell() => UnitsInCell.Values
            .FirstOrDefault(unit => unit.UnitType == UnitType.Titan) is not null || UnitInCenterOfCell?.IsEnemy is true;

        public bool IsExistEmptyPositionOnBorder(MapCellModel mapCell) => NearCells.TryGetValue(mapCell, out var nearCellsName) 
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
            var position = NearCellsNameToPosition[NearCells[mapCell]];
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
            var buildingTextureName = MapTextures.BuildingTextureNames[buildingType];
            BuildingType = buildingType;
            
            if (buildingType == BuildingType.ClosedGates)
            {
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.GameOver,
                    Win = true
                });
            } else if (HousesBuildingTypes.Contains(buildingType))
            {
                foreach (var nearCell in NearCells.Keys
                    .Where(cell => cell.BuildingType == BuildingType.None))
                    nearCell.UpdateBuildingType(BuildingType.BetweenHousesNone);
            } else if (buildingType == BuildingType.None)
            {
                foreach (var nearCell in NearCells.Keys
                             .Where(cell => cell.BuildingType == BuildingType.BetweenHousesNone)
                             .Where(cell => !cell.NearCells.Keys.Any(nearCell => 
                                 HousesBuildingTypes.Contains(nearCell.BuildingType))))
                    nearCell.UpdateBuildingType(BuildingType.None);
            }
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(X, Y)
                {
                    TextureName = buildingTextureName
                }
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
                case BuildingType.Barracks:
                case BuildingType.Warehouse:
                case BuildingType.Centre:
                case BuildingType.Wall:
                case BuildingType.InnerGates:
                case BuildingType.ClosedGates:
                case BuildingType.HiddenNone:
                case BuildingType.House1:
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
