using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class UnitEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<PressedMouseBtn, Action<InputAction>> _handlers = new();

        public UnitEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void Handle(InputAction action) =>
            _handlers[action.MouseBtn](action);

        private void InitializeHandlers()
        {
            _handlers[PressedMouseBtn.None] = HandleNoMouseSelect;
            _handlers[PressedMouseBtn.Left] = HandleLeftMouseSelect;
            _handlers[PressedMouseBtn.Right] = HandleRightMouseSelect;
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

        private void HandleRightMouseSelect(InputAction action)
        {
            
        }
    }
}
