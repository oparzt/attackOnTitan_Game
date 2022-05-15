using System;

namespace AttackOnTitan.Models
{
    public class UnitModel
    {
        public readonly int ID;
        public MapCellModel CurCell;

        public bool CanGo = true;
        public bool IsFly;
        public bool Moved;
        public bool Enemy;

        public int MaxEnergy = 10;
        public int Energy = 10;

        public UnitModel(int id, bool enemy = false)
        {
            ID = id;
            Enemy = enemy;
        }

        public void SetOpacity(float opacity) =>
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.ChangeUnitOpacity,
                UnitInfo = new UnitInfo(ID)
                {
                    Opacity = opacity
                }
            });

        public void SetUnselectedOpacity() =>
            SetOpacity(0.65f);

        public void SetPreselectedOpacity() =>
            SetOpacity(0.8f);

        public void SetSelectedOpacity() =>
            SetOpacity(1f);


    }
}
