using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttackOnTitan.Models
{
    public class PathFinder
    {
        public Dictionary<MapCellModel, DijkstraPath> Paths = new ();
        
        private UnitModel _unit;
        
        public PathFinder()
        {
            
        }

        public void SetUnit(UnitModel unitModel)
        {
            _unit = unitModel;
            SetPathsByDijkstra(unitModel);
        }
        
        private void SetPathsByDijkstra(UnitModel unitModel)
        {
            var startCell = unitModel.CurCell;
            var toOpenPaths = new HashSet<DijkstraPath> {new (startCell)
            {
                IsEnemyCell = startCell.IsEnemyInCell()
            }};
            
            Paths.Clear();

            while (toOpenPaths.Count != 0)
            {
                var willOpened = GetLowestCostPath(toOpenPaths);

                Paths[willOpened.Cur] = willOpened;

                if (!willOpened.IsEnemyCell)
                    foreach (var newPath in GetPathsToNeighbours(willOpened))
                        toOpenPaths.Add(newPath);

                toOpenPaths.Remove(willOpened);
                toOpenPaths.RemoveWhere(path => Paths.ContainsKey(path.Cur));
            }
        }
        
        private static DijkstraPath GetLowestCostPath(IEnumerable<DijkstraPath> paths)
        {
            return paths
                .OrderBy(path => path.EnergyCost)
                .ThenBy(path => path.GasCost)
                .First();
        }

        private IEnumerable<DijkstraPath> GetPathsToNeighbours(DijkstraPath path)
        {
            var pathCell = path.Cur;
            return pathCell.NearCells.Keys
                .Where(nearCell => !Paths.ContainsKey(nearCell))
                .SelectMany(nearCell =>
                {
                    var isEnemyInCell = nearCell.IsEnemyInCell();
                    if ((isEnemyInCell && nearCell.IsExistEmptyPositionOnBorder(pathCell)) ||
                        (!isEnemyInCell && nearCell.IsExistEmptyPositionInCell()))
                        return pathCell
                            .GetPossibleTravelModesTo(nearCell)
                            .Intersect(_unit.PossibleTravelModes)
                            .Select(travelMode => new DijkstraPath(nearCell, path)
                            {
                                EnergyCost = path.EnergyCost + _unit.GetEnergyCost(travelMode),
                                GasCost = path.GasCost + _unit.GetGasCost(travelMode),
                                IsEnemyCell = isEnemyInCell
                            });
                    return null;
                })
                .Where(curPath => _unit.IsExistTravel(curPath.EnergyCost, curPath.GasCost));
        }
    }
    
    public class DijkstraPath : IEnumerable<MapCellModel>
    {
        public MapCellModel Cur { get; }
        public DijkstraPath Prev { get; }
        public int Count { get; }

        public int EnergyCost { get; set; }
        public int GasCost { get; set; }
        public bool IsEnemyCell { get; set; }

        public DijkstraPath(MapCellModel cur, DijkstraPath prev = null)
        {
            Prev = prev;
            Cur = cur;
            
            Count = (prev?.Count ?? 0) + 1;
        }

        public MapCellModel[] GetPath()
        {
            var path = new List<MapCellModel> {Cur};
            var prev = Prev;

            while (!(prev is null))
            {
                path.Add(prev.Cur);
                prev = prev.Prev;
            }

            path.Reverse();
            return path.ToArray();
        }

        public IEnumerator<MapCellModel> GetEnumerator()
        {
            if (Prev is not null)
                foreach (var cell in Prev)
                    yield return cell;

            yield return Cur;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}