using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class BuildingEconomyModel
    {
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> CountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -10
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -10
            },
            [BuildingType.House1] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -10
            },
            [BuildingType.None] = new Dictionary<ResourceType, int> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, int> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -10
            },
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1000,
                [ResourceType.Log] = -1000,
                [ResourceType.Stone] = -1000
            },
            [BuildingType.OpenedGates] = new Dictionary<ResourceType, int> {}
        };

        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> StepCountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, int> {},
            [BuildingType.Centre] = new Dictionary<ResourceType, int> {},
            [BuildingType.House1] = new Dictionary<ResourceType, int> {},
            [BuildingType.None] = new Dictionary<ResourceType, int> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, int> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, int> {},
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, int> {},
            [BuildingType.OpenedGates] = new Dictionary<ResourceType, int> {}
        };
        
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, int>> LimitDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 15
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 15
            },
            [BuildingType.House1] = new Dictionary<ResourceType, int>
            {
                [ResourceType.People] = 30
            },
            [BuildingType.None] = new Dictionary<ResourceType, int> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, int> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Log] = 100,
                [ResourceType.Stone] = 100
            },
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, int> {},
            [BuildingType.OpenedGates] = new Dictionary<ResourceType, int> {}
        };
    }
}