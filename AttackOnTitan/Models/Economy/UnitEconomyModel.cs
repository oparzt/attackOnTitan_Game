using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class UnitEconomyModel
    {
        public static readonly Dictionary<ResourceType, int> EmptyDictionary = new ();
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> CountDiff = new()
        {
            [UnitType.Titan] = EmptyDictionary,
            [UnitType.Builder] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -15,
                [ResourceType.People] = -3
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -30,
                [ResourceType.People] = -6
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -45,
                [ResourceType.People] = -7
            },
            [UnitType.Police] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -90,
                [ResourceType.People] = -8
            },
            [UnitType.Scout] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -60,
                [ResourceType.People] = -9
            }
        };

        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> StepCountDiff = new()
        {
            [UnitType.Titan] = EmptyDictionary,
            [UnitType.Builder] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -4
            },
            [UnitType.Cadet] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -4
            },
            [UnitType.Garrison] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -6
            },
            [UnitType.Police] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -8
            },
            [UnitType.Scout] = new Dictionary<ResourceType, int>
            {
                [ResourceType.Coin] = -10
            }
        };
        
        public static readonly Dictionary<UnitType, Dictionary<ResourceType, int>> LimitDiff = new()
        {
            [UnitType.Titan] = EmptyDictionary,
            [UnitType.Builder] = EmptyDictionary,
            [UnitType.Cadet] = EmptyDictionary,
            [UnitType.Garrison] = EmptyDictionary,
            [UnitType.Police] = EmptyDictionary,
            [UnitType.Scout] = EmptyDictionary
        };
    }
}