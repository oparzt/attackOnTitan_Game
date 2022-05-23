using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class UnitEconomyModel
    {
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> CountDiff = new()
        {
            [UnitType.Builder] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Police] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            },
            [UnitType.Scout] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            }
        };

        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> StepCountDiff = new()
        {
            [UnitType.Builder] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Police] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1
            },
            [UnitType.Scout] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -1
            }
        };
        
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> LimitDiff = new()
        {
            [UnitType.Builder] = new Dictionary<ResourceType, int> {},
            [UnitType.Cadet] = new Dictionary<ResourceType, int> {},
            [UnitType.Garrison] = new Dictionary<ResourceType, int> {},
            [UnitType.Police] = new Dictionary<ResourceType, int> {},
            [UnitType.Scout] = new Dictionary<ResourceType, int> {}
        };
    }
}