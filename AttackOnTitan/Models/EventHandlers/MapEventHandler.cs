using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class MapEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<PressedMouseBtn, Action<InputAction>> _handlers = new();

        private MapCellModel _lastNoMouseSelected;


        public MapEventHandler(GameModel gameModel)
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
            var targetCell = GameModel.Map[action.SelectedCell.X, action.SelectedCell.Y];

            if (GameModel.UnitPath.Count != 0)
                GameModel.UnitPath.ExecutePath();
            if (_lastNoMouseSelected is not null)
                GameModel.Map.SetUnselectedOpacity(_lastNoMouseSelected);
            if (GameModel.PreselectedUnit is not null)
                GameModel.PreselectedUnit.SetUnselectedOpacity();

            GameModel.Map.SetPreselectedOpacity(targetCell);

            _lastNoMouseSelected = targetCell;
        }

        private void HandleLeftMouseSelect(InputAction action)
        {
            if (GameModel.SelectedUnit is not null) {
                GameModel.SelectedUnit.SetUnselectedOpacity();
                GameModel.UnitPath.SetUnit(null);
            }
            GameModel.SelectedUnit = null;
        }

        private void HandleRightMouseSelect(InputAction action)
        {
            if (GameModel.SelectedUnit is null || GameModel.SelectedUnit.Moved)
            {
                HandleNoMouseSelect(action);
                return;
            }
            var targetCell = GameModel.Map[action.SelectedCell.X, action.SelectedCell.Y];
            GameModel.Map.SetUnselectedOpacity(targetCell);
            GameModel.UnitPath.Add(targetCell);
        }
    }
}
