using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public static class MapTextures
    {
        public static readonly Dictionary<BuildingType, string> BuildingTextureNames = new()
        {
            [BuildingType.None] = "EmptyBuilding",
            [BuildingType.HiddenNone] = "EmptyBuilding",
            [BuildingType.NearWall] = "EmptyBuilding",
            [BuildingType.BetweenHousesNone] = "EmptyBuilding",
            
            [BuildingType.House1] = "House1",
            [BuildingType.House2] = "House2",
            [BuildingType.House3] = "House3",
            
            [BuildingType.Barracks] = "Barracks",
            [BuildingType.Warehouse] = "Warehouse",
            [BuildingType.Centre] = "Centre",

            [BuildingType.Wall] = "EmptyBuilding",
            [BuildingType.InnerGates] = "EmptyBuilding",
            [BuildingType.OuterGates] = "EmptyBuilding",
            [BuildingType.ClosedGates] = "OuterGates"
        };
        
        public static readonly Dictionary<BuildingType, string> HexagonTextureNames = new()
        {
            [BuildingType.None] = "Hexagon__None",
            [BuildingType.HiddenNone] = "Hexagon__Hidden",
            [BuildingType.NearWall] = "Hexagon__NearWalls",
            [BuildingType.BetweenHousesNone] = "Hexagon__NearHouses",
            
            [BuildingType.House1] = "Hexagon__Houses",
            [BuildingType.House2] = "Hexagon__Houses",
            [BuildingType.House3] = "Hexagon__Houses",
            
            [BuildingType.Barracks] = "Hexagon__Houses",
            [BuildingType.Warehouse] = "Hexagon__Houses",
            [BuildingType.Centre] = "Hexagon__Houses",

            [BuildingType.Wall] = "Hexagon__Walls",
            [BuildingType.InnerGates] = "Hexagon__Gates",
            [BuildingType.OuterGates] = "Hexagon__Gates",
            [BuildingType.ClosedGates] = "Hexagon__Gates"
        };

        public static readonly string SimpleHexagon = "Hexagon__Simple";
    }
}