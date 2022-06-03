using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class UnitTextures
    {
        public static readonly Dictionary<UnitType, string> UnitTextureNames = new()
        {
            [UnitType.Titan] = "Titan",
            [UnitType.Scout] = "Scout",
            [UnitType.Garrison] = "Garrison",
            [UnitType.Police] = "Police",
            [UnitType.Cadet] = "Cadet",
            [UnitType.Builder] = "Builder"
        };
    }
}