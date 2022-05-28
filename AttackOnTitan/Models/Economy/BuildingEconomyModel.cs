using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class BuildingEconomyModel
    {
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, float>> CountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -10,
                [ResourceType.Stone] = -10
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -4,
                [ResourceType.Stone] = -8
            },
            [BuildingType.House1] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10,
                [ResourceType.Log] = -2,
                [ResourceType.Stone] = -6
            },
            [BuildingType.None] = new Dictionary<ResourceType, float> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, float> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -5,
                [ResourceType.Log] = -5,
                [ResourceType.Stone] = -1
            },
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -650,
                [ResourceType.Log] = -300,
                [ResourceType.Stone] = -800
            }
        };

        public static readonly float PeopleMakeGoldInStep = 0.1f;

        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, float>> StepCountDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 3
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 10
            },
            [BuildingType.House1] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 5
            },
            [BuildingType.None] = new Dictionary<ResourceType, float> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, float> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, float> {},
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, float> {},
            [BuildingType.InnerGates] = new Dictionary<ResourceType, float> {}
        };
        
        public static readonly Dictionary<BuildingType, Dictionary<ResourceType, float>> LimitDiff = new()
        {
            [BuildingType.Barracks] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 15
            },
            [BuildingType.Centre] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 15
            },
            [BuildingType.House1] = new Dictionary<ResourceType, float>
            {
                [ResourceType.People] = 30
            },
            [BuildingType.None] = new Dictionary<ResourceType, float> {},
            [BuildingType.Wall] = new Dictionary<ResourceType, float> {},
            [BuildingType.Warehouse] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Log] = 100,
                [ResourceType.Stone] = 100
            },
            [BuildingType.ClosedGates] = new Dictionary<ResourceType, float> {},
            [BuildingType.InnerGates] = new Dictionary<ResourceType, float> {}
        };
    }
}