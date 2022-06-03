using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class CommandEventHandler
    {
        private readonly GameModel _gameModel;
        private readonly Dictionary<CommandType, Action<InputAction, UnitModel, MapCellModel>> _commandHandlers = new();

        public CommandEventHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleCommand(InputAction action)
        {
            _gameModel.BlockClickEvents = true;
            var unitFounded = _gameModel.Units.TryGetValue(action.InputUnitInfo.ID, out var unitModel);
            var mapCell = _gameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
            _commandHandlers[action.InputCommandInfo.CommandType](action, unitModel, mapCell);
        }
        
        private void InitializeHandlers()
        {
            var emptyHandler = new Action<InputAction, UnitModel, MapCellModel>((action, unitModel, mapCellModel) => { });
            _commandHandlers[CommandType.Attack] = HandleAttackCommand;
            _commandHandlers[CommandType.Fly] = HandleFlyCommand;
            _commandHandlers[CommandType.Walk] = HandleWalkCommand;
            _commandHandlers[CommandType.Refuel] = HandleRefuelCommand;
            _commandHandlers[CommandType.OpenCreatingHouseMenu] = HandleOpenCreatingHouseMenuCommand;
            _commandHandlers[CommandType.OpenCreatingUnitMenu] = HandleOpenCreatingUnitMenuCommand;
            _commandHandlers[CommandType.OpenProductionMenu] = HandleOpenProductionMenuCommand;
            _commandHandlers[CommandType.CreateHouse] = HandleCreateHouseCommand;
            _commandHandlers[CommandType.CreateUnit] = HandleCreateUnitCommand;
            _commandHandlers[CommandType.CloseCreatingMenu] = HandleExitCommand;
            _commandHandlers[CommandType.CloseProductionMenu] = HandleExitProductionMenuCommand;
            _commandHandlers[CommandType.ChangePeopleAtWork] = HandleChangePeopleAtWorkCommand;

            _commandHandlers[CommandType.AttackDisabled] = emptyHandler;
            _commandHandlers[CommandType.FlyDisabled] = emptyHandler;
            _commandHandlers[CommandType.WalkDisabled] = emptyHandler;
            _commandHandlers[CommandType.RefuelDisabled] = emptyHandler;
            _commandHandlers[CommandType.OpenCreatingHouseMenuDisabled] = emptyHandler;
        }

        private void HandleAttackCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                _gameModel.CommandModel.AttackCommand(unitModel.CurCell);
        }

        private void HandleFlyCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                _gameModel.CommandModel.FlyOrWalk(unitModel,true);
        }
        
        private void HandleWalkCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                _gameModel.CommandModel.FlyOrWalk(unitModel, false);
        }

        private void HandleRefuelCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            _gameModel.CommandModel.RefuelCommand(unitModel);
        }
        
        private void HandleOpenCreatingHouseMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                _gameModel.CommandModel.OpenCreatingMenu(unitModel, mapCellModel, true);
        }

        private void HandleOpenCreatingUnitMenuCommand(InputAction action, UnitModel unitModel,MapCellModel mapCellModel) =>
            _gameModel.CommandModel.OpenCreatingMenu(unitModel, mapCellModel, false);

        private void HandleOpenProductionMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            _gameModel.CommandModel.OpenProductionMenu(mapCellModel);    
        
        private void HandleCreateHouseCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
            {
                unitModel.Energy = 0;
                _gameModel.CommandModel.CreateHouse(unitModel, mapCellModel, action.InputCommandInfo);
            }
            else
            {
                _gameModel.CommandModel.ClearCommandBar();
                _gameModel.CommandModel.ClearCreatingChoose();
            }
        }

        private void HandleCreateUnitCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            _gameModel.CommandModel.CreateUnit(unitModel, mapCellModel, action.InputCommandInfo);

        private void HandleChangePeopleAtWorkCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            _gameModel.EconomyModel.ChangePeopleAtWork(action.InputCommandInfo.PeopleDiff);
        
        private void HandleExitCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            _gameModel.CommandModel.CloseCreatingMenu(unitModel, mapCellModel);

        private void HandleExitProductionMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            _gameModel.CommandModel.CloseProductionMenu();
    }
}