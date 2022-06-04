using System;
using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class MapEventHandler
    {
        private readonly GameModel _gameModel;
        private readonly Dictionary<MouseBtn, Action<InputAction>> _selectHandlers = new();

        public MapEventHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
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
            var targetCell = _gameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];

            if (_gameModel.UnitPath.CanExecute)
                _gameModel.UnitPath.ExecutePath();
            if (_gameModel.PreSelectedMapCellModel is not null)
                MapModel.SetUnselectedOpacity(_gameModel.PreSelectedMapCellModel);

            MapModel.SetPreselectedOpacity(targetCell);

            _gameModel.PreSelectedMapCellModel = targetCell;
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            if (_gameModel.StepEnd || _gameModel.BlockClickEvents) return;
            _gameModel.BlockClickEvents = true;
            _gameModel.SelectedUnit?.SetUnselectedOpacity();
            _gameModel.SelectedUnit = null;
            _gameModel.UnitPath.SetUnit(null);
            _gameModel.CommandModel.ClearCommandBar();
            var mapCell = _gameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
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
            if (_gameModel.SelectedUnit is null 
                || _gameModel.SelectedUnit.UnitType == UnitType.Titan 
                || _gameModel.SelectedUnit.Moved)
            {
                HandleNoMouseSelect(action);
                return;
            }
            var targetCell = _gameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
            MapModel.SetUnselectedOpacity(targetCell);
            _gameModel.UnitPath.SetPath(targetCell);
        }
    }
}
