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

    public class MapCell
    {
        public readonly int X;
        public readonly int Y;

        public readonly Dictionary<MapCell, Weight> NearCells = new();
        public static readonly Dictionary<NearCellsName, (int, int)> PossibleNearCellsCoords = new()
        {
            [NearCellsName.LeftTop] = (-1, -1),
            [NearCellsName.LeftBottom] = (-1, 0),
            [NearCellsName.Top] = (0, -1),
            [NearCellsName.Bottom] = (0, 1),
            [NearCellsName.RightTop] = (1, -1),
            [NearCellsName.RightBottom] = (1, 0)
        };

        public MapCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void ConnectWithCell(MapCell mapCell, int weightForRun, int weightForFly)
        {
            NearCells[mapCell] = new(weightForRun, weightForFly);
        }

        public void ConnectWithCell(MapCell mapCell, Weight weight)
        {
            NearCells[mapCell] = weight;
        }

        public void ConnectWithNearCells(MapCell[,] mapCells,
            int columnsCount, int rowsCount,
            Dictionary<NearCellsName, Weight> nearCellsWeights)
        {
            foreach (var nearCellsWeight in nearCellsWeights)
            {
                var diff = PossibleNearCellsCoords[nearCellsWeight.Key];
                var coordX = X + diff.Item1;
                var coordY = Y + diff.Item2;

                if (coordX < 0 || coordX >= columnsCount || coordY < 0 || coordY >= rowsCount)
                    continue;

                NearCells[mapCells[coordX, coordY]] = nearCellsWeight.Value;
            }
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
