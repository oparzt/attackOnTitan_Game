using System;
using System.Collections.Generic;

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
        Left,
        Right,
        Top,
        Bottom,
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

        public MapCellModel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void ConnectWithCell(MapCellModel mapCell, int weightForRun, int weightForFly)
        {
            NearCells[mapCell] = new(weightForRun, weightForFly);
        }

        public void ConnectWithCell(MapCellModel mapCell, Weight weight)
        {
            NearCells[mapCell] = weight;
        }

        public void ConnectWithNearCells(MapCellModel[,] mapCells,
            int columnsCount, int rowsCount,
            Dictionary<NearCellsName, Weight> nearCellsWeights)
        {
            var possibleDiffs = X % 2 == 0 ?
                PossibleNearCellsDiffsFromEven :
                PossibleNearCellsDiffsFromOdd;

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
    }

    public struct Weight
    {
        public int WeightForRun;
        public int WeightForFly;

        public Weight(int weightForRun, int weightForFly)
        {
            WeightForRun = weightForRun;
            WeightForFly = weightForFly;
        }
    }
}
