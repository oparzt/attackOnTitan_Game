using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class PeopleEconomyModel
    {
        public const int PeopleMakeCoinCoeff = 6;
        
        public static readonly Dictionary<ResourceType, int> PeopleAtWorkMadeResource = new()
        {
            [ResourceType.Log] = 2,
            [ResourceType.Stone] = 1,
        };

        public static readonly Dictionary<ResourceType, int> PeopleAtWorkCoinDiff = new()
        {
            [ResourceType.Log] = -6,
            [ResourceType.Stone] = -8,
        };
    }
}