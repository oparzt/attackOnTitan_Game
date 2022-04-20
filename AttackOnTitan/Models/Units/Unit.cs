using System;

namespace AttackOnTitan.Models
{
    public class Unit
    {
        public readonly int ID;
        public MapCell CurCell;

        public Unit(int id)
        {
            ID = id;
        }
    }
}
