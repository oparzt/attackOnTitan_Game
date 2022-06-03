using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AttackOnTitan.Models;
using NUnit.Framework;

namespace AttackOnTitan_Tests
{
    public class PathFinderTests
    {
        private GameModel _gameModel;

        private Dictionary<BuildingType, (int, int)[]> _emptyMapBuildings;

        private readonly Dictionary<BuildingType, (int, int)[]> _buildingsForWallTest = new Dictionary<BuildingType, (int, int)[]>
        {
            [BuildingType.InnerGates] = new (int, int)[] {},
            [BuildingType.OuterGates] = new (int, int)[] {},
            [BuildingType.Wall] = new []
            {
                (0, 0), (0, 1)
            }
        };

        private PathFinder _pathFinder;
        
        [SetUp]
        public void CreateGameModel()
        {
            _gameModel = new GameModel(30, 30);
            _pathFinder = new PathFinder();
            _emptyMapBuildings = new Dictionary<BuildingType, (int, int)[]>
            {
                [BuildingType.InnerGates] = new (int, int)[] {},
                [BuildingType.OuterGates] = new (int, int)[] {},
            };

            var none = new List<(int, int)>();
            
            for (var x = 0; x < 30; x++)
            for (var y = 0; y < 30; y++)
                none.Add((x, y));

            _emptyMapBuildings[BuildingType.None] = none.ToArray();
        }
        
        [Test]
        public void PathFinder_Test()
        {
            _gameModel.Map.InitializeBuildings(_emptyMapBuildings);

            var unit = new UnitModel(0, UnitType.Cadet);
            unit.AddUnitToTheMap(_gameModel.Map[15, 15]);
            unit.Energy = unit.MaxEnergy;
            var sw = new Stopwatch();

            sw.Start();
            _pathFinder.SetUnit(unit);
            sw.Stop();

            Assert.LessOrEqual(sw.ElapsedMilliseconds, 500);
        }
    }
}