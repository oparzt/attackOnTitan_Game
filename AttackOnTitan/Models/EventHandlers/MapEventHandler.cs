using System;
using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class MapEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<MouseBtn, Action<InputAction>> _selectHandlers = new();

        private MapCellModel _lastNoMouseSelected;


        public MapEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleSelect(InputAction action) =>
            _selectHandlers[action.MouseBtn](action);

        private void InitializeHandlers()
        {
            _selectHandlers[MouseBtn.None] = HandleNoMouseSelect;
            _selectHandlers[MouseBtn.Left] = HandleLeftMouseSelect;
            _selectHandlers[MouseBtn.Right] = HandleRightMouseSelect;
        }

        private void HandleNoMouseSelect(InputAction action)
        {
            var targetCell = GameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];

            if (GameModel.UnitPath.Count != 0)
                GameModel.UnitPath.ExecutePath();
            if (_lastNoMouseSelected is not null)
                GameModel.Map.SetUnselectedOpacity(_lastNoMouseSelected);

            GameModel.Map.SetPreselectedOpacity(targetCell);

            _lastNoMouseSelected = targetCell;
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            if (GameModel.StepEnd || GameModel.BlockClickEvents) return;
            GameModel.BlockClickEvents = true;
            GameModel.SelectedUnit?.SetUnselectedOpacity();
            GameModel.SelectedUnit = null;
            GameModel.UnitPath.SetUnit(null);
            GameModel.CommandModel.ClearCommandBar();
            var mapCell = GameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
            if (mapCell.GetPossibleCreatingUnitTypes().Any())
            {
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.ExecCommand,
                    InputCellInfo = action.InputCellInfo,
                    InputUnitInfo = new InputUnitInfo(-1),
                    InputCommandInfo = new InputCommandInfo(CommandType.OpenCreatingUnitMenu)
                });
            }
            else if (mapCell.BuildingType == BuildingType.Warehouse)
            {
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.ExecCommand,
                    InputCellInfo = action.InputCellInfo,
                    InputUnitInfo = new InputUnitInfo(-1),
                    InputCommandInfo = new InputCommandInfo(CommandType.OpenProductionMenu)
                });
            }

        }

        private void HandleRightMouseSelect(InputAction action)
        {
            if (GameModel.SelectedUnit is null 
                || GameModel.SelectedUnit.UnitType == UnitType.Titan 
                || GameModel.SelectedUnit.Moved)
            {
                HandleNoMouseSelect(action);
                return;
            }
            var targetCell = GameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
            GameModel.Map.SetUnselectedOpacity(targetCell);
            GameModel.UnitPath.Add(targetCell);
        }
    }
}
