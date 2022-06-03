using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AttackOnTitan.Models;

namespace AttackOnTitan_Tests
{
    public class Tests
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

        private readonly PathInfo[] _pathInfosForWallTest = new PathInfo[]
        {
            new PathInfo((1,0), (0,0), new HashSet<TravelMode> {TravelMode.Fly}),
            new PathInfo((1,0), (0,1), new HashSet<TravelMode> {TravelMode.Fly}),
            new PathInfo((1,1), (0,1), new HashSet<TravelMode> {TravelMode.Fly}),
            new PathInfo((0,2), (0,1), new HashSet<TravelMode> {TravelMode.Fly}),
            
            new PathInfo((0,0), (1,0), new HashSet<TravelMode> {TravelMode.Fly, TravelMode.BuilderRun, TravelMode.TitanRun}),
            new PathInfo((0,1), (1,0), new HashSet<TravelMode> {TravelMode.Fly, TravelMode.BuilderRun, TravelMode.TitanRun}),
            new PathInfo((0,1), (1,1), new HashSet<TravelMode> {TravelMode.Fly, TravelMode.BuilderRun, TravelMode.TitanRun}),
            new PathInfo((0,1), (0,2), new HashSet<TravelMode> {TravelMode.Fly, TravelMode.BuilderRun, TravelMode.TitanRun}),
        };
        

        [SetUp]
        public void CreateGameModel()
        {
            _gameModel = new GameModel(30, 30);
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
        public void MapWall_Test()
        {
            _gameModel.Map.InitializeBuildings(_emptyMapBuildings);
            _gameModel.Map.InitializeBuildings(_buildingsForWallTest);

            foreach (var pathInfo in _pathInfosForWallTest)
            {
                var curPossibleTravelModes = _gameModel.Map[pathInfo.From.Item1, pathInfo.From.Item2]
                    .GetPossibleTravelModesTo(_gameModel.Map[pathInfo.To.Item1, pathInfo.To.Item2])
                    .ToHashSet();
                var expPossibleTravelModes = pathInfo.PossibleTravelModes;
                
                Assert.AreEqual(expPossibleTravelModes, curPossibleTravelModes);
            }
        }
    }

    public class PathInfo
    {
        public readonly (int, int) From;
        public readonly (int, int) To;
        public readonly HashSet<TravelMode> PossibleTravelModes;

        public PathInfo((int, int) from, (int, int) to, HashSet<TravelMode> possibleTravelModes)
        {
            From = from;
            To = to;
            PossibleTravelModes = possibleTravelModes;
        }
    }
}