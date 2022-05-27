using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class UnitEconomyModel
    {
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, float>> CountDiff = new()
        {
            [UnitType.Builder] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Police] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Scout] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -10
            }
        };

        public static readonly Dictionary<UnitType, Dictionary<ResourceType, float>> StepCountDiff = new()
        {
            [UnitType.Builder] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, float>
            {
                [ResourceType.Coin] = -1
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
            [UnitType.Builder] = new Dictionary<ResourceType, float> {},
            [UnitType.Cadet] = new Dictionary<ResourceType, float> {},
            [UnitType.Garrison] = new Dictionary<ResourceType, float> {},
            [UnitType.Police] = new Dictionary<ResourceType, float> {},
            [UnitType.Scout] = new Dictionary<ResourceType, float> {}
        };
    }
}