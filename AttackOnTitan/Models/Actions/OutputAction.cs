using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Models
{
    public enum OutputActionType
    {
        AddUnit,
        MoveUnit,
        StopUnit,
        ChangeUnitOpacity,

        ChangeCellOpacity,
        
        AddResource,
        UpdateResourceCount,
        ChangeStepBtnState,

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
}
