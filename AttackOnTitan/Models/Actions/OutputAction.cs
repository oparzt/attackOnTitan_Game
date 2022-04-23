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
        ChangeUnitOpacity,

        ChangeCellOpacity,

        //ChangeUnitTexture,
        //ChangeUnitText,
        //ChangeCellTexture
    }

    public struct OutputAction
    {
        public OutputActionType ActionType;

        public UnitInfo UnitInfo;
        public MapCellInfo MapCellInfo;
    }

    public struct UnitInfo
    {
        public int ID;

        public int X;
        public int Y;

        public float Opacity;

        public string TextureName;
        public string UnitText;
    }

    public struct MapCellInfo
    {
        public int X;
        public int Y;

        public float Opacity;

        public string TextureName;
    }
}
