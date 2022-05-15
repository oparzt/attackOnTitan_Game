using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class UnitEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<PressedMouseBtn, Action<InputAction>> _selectHandlers = new();

        public UnitEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleSelect(InputAction action) =>
            _selectHandlers[action.MouseBtn](action);

        public void HandleStopMove(InputAction action)
        {
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.StopUnit,
                UnitInfo = new UnitInfo(action.SelectedUnit.ID)
            });
            GameModel.Units[action.SelectedUnit.ID].Moved = false;
        }

        private void InitializeHandlers()
        {
            _selectHandlers[PressedMouseBtn.None] = HandleNoMouseSelect;
            _selectHandlers[PressedMouseBtn.Left] = HandleLeftMouseSelect;
            _selectHandlers[PressedMouseBtn.Right] = HandleRightMouseSelect;
        }

        private void HandleNoMouseSelect(InputAction action)
        {
            var target = GameModel.Units[action.SelectedUnit.ID];

            if (target != GameModel.SelectedUnit)
            {
                target.SetPreselectedOpacity();
                GameModel.PreselectedUnit = target;
            }
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            var target = GameModel.Units[action.SelectedUnit.ID];
            GameModel.PreselectedUnit = null;

            if (target != GameModel.SelectedUnit)
            {
                GameModel.SelectedUnit = target;
                GameModel.UnitPath.SetUnit(target);
            }

            GameModel.SelectedUnit.SetSelectedOpacity();
        }

        private void HandleRightMouseSelect(InputAction action) {}
    }
}
