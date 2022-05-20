using System;
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
        House,
        Barracks,
        Warehouse,
        Centre,
        Wall,
        OpenedGates,
        ClosedGates
    }

    public class MapCellModel
    {
        public readonly int X;
        public readonly int Y;
        public BuildingType BuildingType;

        public readonly Dictionary<MapCellModel, Weight> NearCells = new();

        public UnitModel UnitInCenterOfCell;

        public static readonly Position[] PossiblePositionsInCell = { Position.TopLeft, Position.TopRight, 
            Position.BottomLeft, Position.BottomRight};
        public readonly Dictionary<Position, UnitModel> UnitsInCell = new();
        public readonly Dictionary<Position, UnitModel> UnitsOnBorder = new();

        public static readonly Dictionary<NearCellsName, Position> NearCellsNameToPosition = new()
        {
            [NearCellsName.LeftTop] = Position.LeftTopBorder,
            [NearCellsName.Top] = Position.TopBorder,
            [NearCellsName.RightTop] = Position.RightTopBorder,
            [NearCellsName.RightBottom] = Position.RightBottomBorder,
            [NearCellsName.Bottom] = Position.BottomBorder,
            [NearCellsName.LeftBottom] = Position.LeftBottomBorder
        };

        #region PossibleNearCellsDiffs
        public static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffsFromOdd = new()
        {
            [NearCellsName.LeftTop] = (-1, 0),
            [NearCellsName.LeftBottom] = (-1, 1),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, 0),
            [NearCellsName.RightBottom] = (1, 1)
        };

        public static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsDiffsFromEven = new()
        {
            [NearCellsName.LeftTop] = (-1, -1),
            [NearCellsName.LeftBottom] = (-1, 0),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, -1),
            [NearCellsName.RightBottom] = (1, 0)
        };
        
        public Dictionary<NearCellsName, (int, int)> PossibleNearCells => X % 2 == 0 ?
            PossibleNearCellsDiffsFromEven :
            PossibleNearCellsDiffsFromOdd;
        #endregion

        #region Buildings

        private static readonly Dictionary<BuildingType, string[]> TextureNamesByBuildingType = new()
        {
            [BuildingType.None] = null,
            [BuildingType.House] = new[] {"House1", "House2", "House3"},
            [BuildingType.Barracks] = new[] {"Barracks"},
            [BuildingType.Warehouse] = new[] {"Warehouse"},
            [BuildingType.Centre] = new[] {"Centre"},
            [BuildingType.Wall] = null,
            [BuildingType.OpenedGates] = null,
            [BuildingType.ClosedGates] = null
        };

        private static readonly Dictionary<BuildingType, BuildingInfo> BuildingInfoByBuildingType = new()
        {
            [BuildingType.None] = null,
            [BuildingType.House] = new BuildingInfo("Дом", BuildingType.House, 
                new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = 10,
                [ResourceType.Log] = 10,
                [ResourceType.Stone] = 10
            }),
            [BuildingType.Barracks] = new BuildingInfo("Казармы", BuildingType.Barracks, 
                new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = 10,
                [ResourceType.Log] = 10,
                [ResourceType.Stone] = 10
            }),
            [BuildingType.Warehouse] = new BuildingInfo("Склад", BuildingType.Warehouse, 
                new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = 10,
                [ResourceType.Log] = 10,
                [ResourceType.Stone] = 10
            }),
            [BuildingType.Centre] = new BuildingInfo("Администрация", BuildingType.Centre, 
                new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = 10,
                [ResourceType.Log] = 10,
                [ResourceType.Stone] = 10
            }),
            [BuildingType.Wall] = null,
            [BuildingType.OpenedGates] = null,
            [BuildingType.ClosedGates] = null
        };

        #endregion

        private static readonly Random Random = new();
        
        public MapCellModel(int x, int y, BuildingType buildingType = BuildingType.None)
        {
            X = x;
            Y = y;
            UpdateBuildingType(buildingType);
        }

        public void ConnectWithNearCells(MapCellModel[,] mapCells,
            int columnsCount, int rowsCount,
            Dictionary<NearCellsName, Weight> nearCellsWeights)
        {
            var possibleDiffs = PossibleNearCells;

            foreach (var nearCellsWeight in nearCellsWeights)
            {
                var diff = possibleDiffs[nearCellsWeight.Key];
                var coordX = X + diff.Item1;
                var coordY = Y + diff.Item2;

                if (coordX < 0 || coordX >= columnsCount || coordY < 0 || coordY >= rowsCount)
                    continue;

                NearCells[mapCells[coordX, coordY]] = nearCellsWeight.Value;
            }
        }

        public bool TryGetCost(MapCellModel mapCell, UnitModel unitModel, out int cost)
        {
            if (NearCells.TryGetValue(mapCell, out var weight))
            {
                cost = unitModel.IsFly ? weight.WeightForFly : weight.WeightForRun;
                return true;
            }

            cost = 0;
            return false;
        }

        public bool IsExistEmptyPositionInCell() => UnitsInCell.Count != 4;
        
        public bool IsEnemyInCell() => UnitsInCell.Values
            .FirstOrDefault(unit => unit.UnitType == UnitType.Titan) is not null || UnitInCenterOfCell?.IsEnemy is true;

        public bool IsExistEmptyPositionOnBorder(MapCellModel mapCell) => NearCells.TryGetValue(mapCell, out var weight) 
            && !UnitsOnBorder.ContainsKey(NearCellsNameToPosition[weight.NearCellsName]);

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
            var position = NearCellsNameToPosition[NearCells[mapCell].NearCellsName];
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
        
        public void UpdateBuildingType(BuildingType buildingType, string buildingTextureNames = null)
        {
            BuildingType = buildingType;
            var buildingTextureVariants = TextureNamesByBuildingType[buildingType];
            
            if (buildingTextureVariants is null)
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ClearTextureIntoCell,
                    MapCellInfo = new MapCellInfo(X, Y)
                });
            else
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.ChangeTextureIntoCell,
                    MapCellInfo = new MapCellInfo(X, Y)
                    {
                        TextureName = buildingTextureNames ?? 
                            GetBuildingTextureName(buildingType)
                    }
                });
        }

        public BuildingInfo[] GetPossibleBuildingInCell()
        {
            return GetPossibleBuildingTypesInCell()
                .Select(buildingType => BuildingInfoByBuildingType[buildingType])
                .ToArray();
        }

        public string GetBuildingTextureName(BuildingType buildingType)
        {
            var buildingTextureVariants = TextureNamesByBuildingType[buildingType];
            return buildingTextureVariants[Random.Next(0, buildingTextureVariants.Length)];
        }

        public IEnumerable<BuildingType> GetPossibleBuildingTypesInCell()
        {
            switch (BuildingType)
            {
                case BuildingType.None:
                    yield return BuildingType.Centre;
                    yield return BuildingType.House;
                    yield return BuildingType.Barracks;
                    yield return BuildingType.Warehouse;
                    break;
                case BuildingType.House:
                case BuildingType.Barracks:
                case BuildingType.Warehouse:
                case BuildingType.Centre:
                case BuildingType.Wall:
                case BuildingType.OpenedGates:
                case BuildingType.ClosedGates:
                default:
                    break;
            }
        }
    }

    public class BuildingInfo
    {
        public readonly string BuildingName;
        public readonly BuildingType BuildingType;
        public readonly Dictionary<ResourceType, int> Price;
        public readonly Dictionary<ResourceType, string> PriceText = new();

        public BuildingInfo(string buildingName, BuildingType buildingType, Dictionary<ResourceType, int> price)
        {
            BuildingType = buildingType;
            Price = price;
            BuildingName = buildingName;
            
            foreach (var priceKeyValue in price)
            {
                PriceText[priceKeyValue.Key] = priceKeyValue.Value.ToString();
            }
        }
    }

    public struct Weight
    {
        public int WeightForRun;
        public int WeightForFly;
        public NearCellsName NearCellsName;

        public Weight(int weightForRun, int weightForFly, NearCellsName nearCellsName)
        {
            WeightForRun = weightForRun;
            WeightForFly = weightForFly;
            NearCellsName = nearCellsName;
        }
    }
}
