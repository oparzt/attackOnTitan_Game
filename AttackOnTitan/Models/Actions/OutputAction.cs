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
        ChangeTextureOverCell,
        ClearTextureIntoCell,
        ChangeCellOpacity,
        SetCellHidden,
        
        AddResource,
        UpdateResourceCount,
        ChangeStepBtnState,
        
        UpdateCommandsBar,
        UpdateGameStepCount,
        ClearCommandsBar,
        
        UpdateNoServicedZoneForMap,
        
        UpdateCreatingChoose,
        InitializeCreatingChoose,
        ClearCreatingChoose,
        
        InitializeProductionMenu,
        UpdateProductionMenu,
        OpenProductionMenu,
        CloseProductionMenu,
        
        UpdateUnitStatusBar,
        
        GameOver
    }

    public class OutputAction
    {
        public OutputActionType ActionType;

        public string StepInfo;
        public bool Win;
        public UnitInfo UnitInfo;
        public string[] UnitStatus;
        public MapCellInfo MapCellInfo;
        public ResourceInfo ResourceInfo;
        public OutputCommandInfo[] CommandInfos;
        public NoServicedZone NoServicedZone;
        public OutputCreatingInfo OutputCreatingInfo;
        public ProductionInfo ProductionInfo;
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

    public class OutputCommandInfo
    {
        public readonly CommandType CommandType;
        public readonly bool IsAvailable;
        public readonly string TextureName;

        public OutputCommandInfo(CommandType commandType, bool isAvailable)
        {
            CommandType = commandType;
            IsAvailable = isAvailable;
            TextureName = CommandTextures.CommandTexturesNames[commandType];
        }
    }

    public class OutputCreatingInfo
    {
        public CommandType CommandType;
        public Point ObjectsTextureSize;
        public CreatingInfo[] CreatingInfos;
        public string BackgroundTextureName;
        public Dictionary<ResourceType, string> ResourceTexturesName;
    }

    public class ProductionInfo
    {
        public Dictionary<ResourceType, string> ResourceTexturesName;
        public Dictionary<ResourceType, string> ResourceInformation;
        public Dictionary<ResourceType, string> CanUpdateProductionResource;
        public (bool, bool)[] CanUpdateProduction;
        public (ResourceType, string) PeopleAtWork;
        public string BackgroundTextureName;
    }
}
