using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class EconomyTextures
    {
        public static readonly Dictionary<ResourceType, string> ResourceTexturesName = new ()
        {
            [ResourceType.Coin] = "Coin",
            [ResourceType.Log] = "Log",
            [ResourceType.Stone] = "Stone",
            [ResourceType.People] = "People"
        };
    }
}