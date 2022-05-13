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

    public class MapCellModel
    {
        public readonly int X;
        public readonly int Y;

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

        public MapCellModel(int x, int y)
        {
            X = x;
            Y = y;
        }

        // public void ConnectWithCell(MapCellModel mapCell, int weightForRun, int weightForFly)
        // {
        //     NearCells[mapCell] = new(weightForRun, weightForFly);
        // }
        //
        // public void ConnectWithCell(MapCellModel mapCell, Weight weight)
        // {
        //     NearCells[mapCell] = weight;
        // }

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
            .FirstOrDefault(unit => unit.Enemy) is not null || UnitInCenterOfCell?.Enemy is true;

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
                GameModel.OutputActions.Enqueue(new(OutputActionType.MoveUnit,
                    new(UnitInCenterOfCell.ID, X, Y, positionForUnitInCenter), null));
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
