using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class UnitEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<MouseBtn, Action<InputAction>> _selectHandlers = new();
        private readonly Dictionary<UnitCommandType, Action<InputAction>> _commandHandlers = new();

        public UnitEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleCommand(InputAction action) =>
            _commandHandlers[action.UnitCommandInfo.UnitCommandType](action);
        public void HandleSelect(InputAction action) =>
            _selectHandlers[action.MouseBtn](action);

        public void HandleUnselect(InputAction action) =>
            GameModel.PreselectedUnit?.SetUnselectedOpacity();

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
            _selectHandlers[MouseBtn.None] = HandleNoMouseSelect;
            _selectHandlers[MouseBtn.Left] = HandleLeftMouseSelect;
            _selectHandlers[MouseBtn.Right] = HandleRightMouseSelect;
            _commandHandlers[UnitCommandType.Attack] = HandleAttackCommand;
            _commandHandlers[UnitCommandType.Fly] = HandleFlyOrWalkCommand;
            _commandHandlers[UnitCommandType.Refuel] = HandleRefuelCommand;
            _commandHandlers[UnitCommandType.OpenBuildMenu] = HandleOpenBuildMenuCommand;
            _commandHandlers[UnitCommandType.Build] = HandleBuildCommand;
            _commandHandlers[UnitCommandType.ExitBuildMenu] = HandleExitCommand;
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

        private void HandleFlyOrWalkCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }

        private void HandleRefuelCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
        }
        
        private void HandleOpenBuildMenuCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
            GameModel.SelectedUnit?.OpenBuildMenu(GameModel.ResourceCount);
        }

        private void HandleBuildCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;

            foreach (var pricePair in action.UnitCommandInfo.BuildingInfo.Price)
                GameModel.ResourceCount[pricePair.Key] -= pricePair.Value;
            GameModel.UpdateResourceCount();
            
            GameModel.SelectedUnit?.CurCell.UpdateBuildingType(
                action.UnitCommandInfo.BuildingInfo.BuildingType,
                action.UnitCommandInfo.BuildingTextureName);
            GameModel.SelectedUnit?.CloseBuildMenu();
        }
        
        private void HandleExitCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
            GameModel.SelectedUnit?.CloseBuildMenu();
        }
    }
}
