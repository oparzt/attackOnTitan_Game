using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class BuildingEconomyModel
    {
        public static readonly Dictionary<ResourceType, int> EmptyDictionary = new ();
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> CountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -20,
                [ResourceType.Log] = -20,
                [ResourceType.Stone] = -20
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -20,
                [ResourceType.Log] = -8,
                [ResourceType.Stone] = -16
            },
            [BuildingType.House1] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -20,
                [ResourceType.Log] = -4,
                [ResourceType.Stone] = -12
            },
            [BuildingType.House2] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -20,
                [ResourceType.Log] = -4,
                [ResourceType.Stone] = -12
            },
            [BuildingType.House3] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -20,
                [ResourceType.Log] = -4,
                [ResourceType.Stone] = -12
            },
            [BuildingType.Warehouse] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -2
            },
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -650,
                [ResourceType.Log] = -300,
                [ResourceType.Stone] = -800
            },
            [BuildingType.None] = EmptyDictionary,
            [BuildingType.HiddenNone] = EmptyDictionary,
            [BuildingType.NearWall] = EmptyDictionary,
            [BuildingType.BetweenHousesNone] = EmptyDictionary,
            [BuildingType.Wall] = EmptyDictionary,
            [BuildingType.InnerGates] = EmptyDictionary,
            [BuildingType.OuterGates] = EmptyDictionary,
        };
        
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> StepCountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 1
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 1
            },
            [BuildingType.House1] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 3
            },
            [BuildingType.House2] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 4
            },
            [BuildingType.House3] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 5
            },
            [BuildingType.None] = EmptyDictionary,
            [BuildingType.HiddenNone] = EmptyDictionary,
            [BuildingType.NearWall] = EmptyDictionary,
            [BuildingType.BetweenHousesNone] = EmptyDictionary,
            [BuildingType.Warehouse] = EmptyDictionary,
            [BuildingType.Wall] = EmptyDictionary,
            [BuildingType.InnerGates] = EmptyDictionary,
            [BuildingType.OuterGates] = EmptyDictionary,
            [BuildingType.ClosedGates] = EmptyDictionary,
        };
        
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> LimitDiff = new()
        {
            [BuildingType.Barracks] = EmptyDictionary,
            [BuildingType.Centre] = EmptyDictionary,
            [BuildingType.House1] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 30
            },
            [BuildingType.House2] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 40
            },
            [BuildingType.House3] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 50
            },
            [BuildingType.Warehouse] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Log] = 60,
                [ResourceType.Stone] = 60
            },
            [BuildingType.None] = EmptyDictionary,
            [BuildingType.HiddenNone] = EmptyDictionary,
            [BuildingType.NearWall] = EmptyDictionary,
            [BuildingType.BetweenHousesNone] = EmptyDictionary,
            [BuildingType.Wall] = EmptyDictionary,
            [BuildingType.InnerGates] = EmptyDictionary,
            [BuildingType.OuterGates] = EmptyDictionary,
            [BuildingType.ClosedGates] = EmptyDictionary,
        };
    }
}