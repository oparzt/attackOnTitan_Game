using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class UnitEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<PressedMouseBtn, Action<InputAction>> _selectHandlers = new();
        private readonly Dictionary<CommandType, Action<InputAction>> _commandHandlers = new();

        public UnitEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleCommand(InputAction action) =>
            _commandHandlers[action.UnitCommandInfo.CommandType](action);
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
            _commandHandlers[CommandType.Attack] = HandleAttackCommand;
            _commandHandlers[CommandType.OpenBuildMenu] = HandleBuildCommand;
            _commandHandlers[CommandType.Fly] = HandleFlyOrWalkCommand;
            _commandHandlers[CommandType.Refuel] = HandleRefuelCommand;
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
            if (GameModel.StepEnd || GameModel.BlockClickEvents) return;
            GameModel.BlockClickEvents = true;
            var target = GameModel.Units[action.SelectedUnit.ID];
            GameModel.PreselectedUnit = null;

            if (target != GameModel.SelectedUnit)
            {
                GameModel.SelectedUnit?.SetUnselectedOpacity();
                GameModel.SelectedUnit = target;
                GameModel.UnitPath.SetUnit(target);
                target.UpdateCommandsBar();
            }
            
            GameModel.SelectedUnit.UpdateCommandsBar();
            GameModel.SelectedUnit.SetSelectedOpacity();
            GameModel.BlockClickEvents = true;
        }

        private void HandleRightMouseSelect(InputAction action) {}
        
        private void HandleAttackCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }

        private void HandleBuildCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }

        private void HandleFlyOrWalkCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }

        private void HandleRefuelCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }
    }
}
