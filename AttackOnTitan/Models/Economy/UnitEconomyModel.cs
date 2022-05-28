using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class UnitEconomyModel
    {
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, float>> CountDiff = new()
        {
            [UnitType.Titan] = new Dictionary<ResourceType, float>(),
            [UnitType.Builder] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -5
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -15
            },
            [UnitType.Police] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -30
            },
            [UnitType.Scout] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -20
            }
        };

        public static readonly Dictionary<UnitType, Dictionary<ResourceType, float>> StepCountDiff = new()
        {
            [UnitType.Titan] = new Dictionary<ResourceType, float>(),
            [UnitType.Builder] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1.5f
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -4f
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Police] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Scout] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1
            }
        };
        
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, float>> LimitDiff = new()
        {
            [UnitType.Titan] = new Dictionary<ResourceType, float>(),
            [UnitType.Builder] = new Dictionary<ResourceType, float> {},
            [UnitType.Cadet] = new Dictionary<ResourceType, float> {},
            [UnitType.Garrison] = new Dictionary<ResourceType, float> {},
            [UnitType.Police] = new Dictionary<ResourceType, float> {},
            [UnitType.Scout] = new Dictionary<ResourceType, float> {}
        };
    }
}