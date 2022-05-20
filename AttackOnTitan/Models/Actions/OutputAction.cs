using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public enum OutputActionType
    {
        AddUnit,
        MoveUnit,
        RemoveUnit,
        StopUnit,
        ChangeUnitOpacity,

        InitializeMap,
        ChangeTextureIntoCell,
        ClearTextureIntoCell,
        ChangeCellOpacity,
        
        AddResource,
        UpdateResourceCount,
        ChangeStepBtnState,
        
        UpdateCommandsBar,
        
        UpdateNoServicedZoneForMap,
        
        UpdateBuilderChoose

        //ChangeUnitTexture,
        //ChangeUnitText,
        //ChangeCellTexture
    }

    public class OutputAction
    {
        public OutputActionType ActionType;

        public UnitInfo UnitInfo;
        public MapCellInfo MapCellInfo;
        public ResourceInfo ResourceInfo;
        public CommandInfo[] CommandInfos;
        public NoServicedZone NoServicedZone;
        public OutputBuildingInfo OutputBuildingInfo;
    }

    public class UnitInfo
    {
        public readonly int ID;

        public int X;
        public int Y;
        public Position Position;

        public float Opacity;

        public string TextureName;
        public string UnitText;

        public UnitInfo(int id)
        {
            ID = id;
        }
    }

    public class MapCellInfo
    {
        public readonly int X;
        public readonly int Y;

        public float Opacity;
        public string TextureName;

        public MapCellInfo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class ResourceInfo
    {
        public readonly ResourceType ResourceType;

        public string TextureName;
        public Point TextureSize;
        public string Count;

        public ResourceInfo(ResourceType resourceType)
        {
            ResourceType = resourceType;
        }
    }

    public class CommandInfo
    {
        public readonly UnitCommandType UnitCommandType;
        public readonly bool IsAvailable;
        public readonly string TextureName;

        public CommandInfo(UnitCommandType unitCommandType, bool isAvailable, string textureName)
        {
            UnitCommandType = unitCommandType;
            IsAvailable = isAvailable;
            TextureName = textureName;
        }
    }

    public class OutputBuildingInfo
    {
        public BuildingInfo[] BuildingInfos;
        public string[] BuildingTexturesName;
        public string BackgroundTextureName;
        public HashSet<ResourceType>[] NotAvailableResource;
    }
}
