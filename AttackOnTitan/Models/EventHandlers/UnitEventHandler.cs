using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class UnitEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<MouseBtn, Action<InputAction>> _selectHandlers = new();

        public UnitEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleSelect(InputAction action) =>
            _selectHandlers[action.MouseBtn](action);

        public void HandleUnselect(InputAction action) =>
            GameModel.PreselectedUnit?.SetUnselectedOpacity();

        public void HandleStopMove(InputAction action)
        {
            GameModel.OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.StopUnit,
                UnitInfo = new UnitInfo(action.InputUnitInfo.ID)
            });
            GameModel.Units.TryGetValue(action.InputUnitInfo.ID, out var unit);
            if (unit is not null)
                unit.Moved = false;
        }

        private void InitializeHandlers()
        {
            _selectHandlers[MouseBtn.None] = HandleNoMouseSelect;
            _selectHandlers[MouseBtn.Left] = HandleLeftMouseSelect;
            _selectHandlers[MouseBtn.Right] = HandleRightMouseSelect;
        }

        private void HandleNoMouseSelect(InputAction action)
        {
            if (!GameModel.Units.TryGetValue(action.InputUnitInfo.ID, out var target) || 
                target == GameModel.SelectedUnit ||
                target.IsEnemy)
                return;

            GameModel.PreselectedUnit?.SetUnselectedOpacity();
            target.SetPreselectedOpacity();
            GameModel.PreselectedUnit = target;
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            if (GameModel.StepEnd || 
                GameModel.BlockClickEvents || 
                !GameModel.Units.TryGetValue(action.InputUnitInfo.ID, out var target)) 
                return;
            
            GameModel.BlockClickEvents = true;
            
            GameModel.PreselectedUnit?.SetUnselectedOpacity();
            GameModel.SelectedUnit?.SetUnselectedOpacity();
            GameModel.SelectedUnit = target;
            GameModel.PreselectedUnit = null;
            GameModel.SelectedUnit.SetSelectedOpacity();
            GameModel.CommandModel.ClearCommandBar();
            GameModel.CommandModel.UpdateCommandBar(target); 
            GameModel.UnitPath.SetUnit(target);
        }

        private void HandleRightMouseSelect(InputAction action) {}
    }
}
