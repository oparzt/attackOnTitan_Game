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

        //ChangeUnitTexture,
        //ChangeUnitText,
        //ChangeCellTexture
    }

    public class OutputAction
    {
        public readonly OutputActionType ActionType;

        public readonly UnitInfo UnitInfo;
        public readonly MapCellInfo MapCellInfo;

        public OutputAction(OutputActionType actionType, UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            ActionType = actionType;
            UnitInfo = unitInfo;
            MapCellInfo = mapCellInfo;
        }
    }

    public class UnitInfo
    {
        public readonly int ID;

        public readonly int X;
        public readonly int Y;

        public readonly float Opacity;

        public readonly string TextureName;
        public readonly string UnitText;

        public UnitInfo(int id)
        {
            ID = id;
        }

        public UnitInfo(int id, float opacity)
        {
            ID = id;
            Opacity = opacity;
        }

        public UnitInfo(int id, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
        }

        public UnitInfo(int id, string textureName, string unitText)
        {
            ID = id;
            TextureName = textureName;
            UnitText = unitText;
        }

        public UnitInfo(int id, int x, int y, string textureName, string unitText)
        {
            ID = id;
            X = x;
            Y = y;
            TextureName = textureName;
            UnitText = unitText;
        }
    }

    public class MapCellInfo
    {
        public readonly int X;
        public readonly int Y;

        public readonly float Opacity;

        public readonly string TextureName;

        public MapCellInfo(int x, int y, float opacity)
        {
            X = x;
            Y = y;
            Opacity = opacity;
        }

        public MapCellInfo(int x, int y, string textureName)
        {
            X = x;
            Y = y;
            TextureName = textureName;
        }
    }
}
