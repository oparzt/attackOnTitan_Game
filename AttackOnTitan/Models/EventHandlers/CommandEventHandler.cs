using System;
using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public enum CommandType
    {
        Attack,
        Fly,
        Walk,
        Refuel,
        OpenCreatingHouseMenu,
        OpenCreatingUnitMenu,
        OpenProductionMenu,
        CreateHouse,
        CreateUnit,
        CloseCreatingMenu,
        CloseProductionMenu,
        ChangePeopleAtWork
    }
    
    public class CommandEventHandler
    {
        public readonly GameModel GameModel;
        private readonly Dictionary<CommandType, Action<InputAction, UnitModel, MapCellModel>> _commandHandlers = new();

        public CommandEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
            InitializeHandlers();
        }

        public void HandleCommand(InputAction action)
        {
            GameModel.BlockClickEvents = true;
            var unitFounded = GameModel.Units.TryGetValue(action.InputUnitInfo.ID, out var unitModel);
            var mapCell = GameModel.Map[action.InputCellInfo.X, action.InputCellInfo.Y];
            _commandHandlers[action.InputCommandInfo.CommandType](action, unitModel, mapCell);
        }
        
        private void InitializeHandlers()
        {
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
        }

        private void HandleAttackCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
        }

        private void HandleFlyCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                GameModel.CommandModel.FlyOrWalk(unitModel,true);
        }
        
        private void HandleWalkCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                GameModel.CommandModel.FlyOrWalk(unitModel, false);
        }

        private void HandleRefuelCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
        }
        
        private void HandleOpenCreatingHouseMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                GameModel.CommandModel.OpenCreatingMenu(unitModel, mapCellModel, true);
        }

        private void HandleOpenCreatingUnitMenuCommand(InputAction action, UnitModel unitModel,MapCellModel mapCellModel) =>
            GameModel.CommandModel.OpenCreatingMenu(unitModel, mapCellModel, false);

        private void HandleOpenProductionMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            GameModel.CommandModel.OpenProductionMenu(mapCellModel);    
        
        private void HandleCreateHouseCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel)
        {
            if (unitModel is not null)
                GameModel.CommandModel.CreateHouse(unitModel, mapCellModel, action.InputCommandInfo);
            else
            {
                GameModel.CommandModel.ClearCommandBar();
                GameModel.CommandModel.ClearCreatingChoose();
            }
        }

        private void HandleCreateUnitCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            GameModel.CommandModel.CreateUnit(unitModel, mapCellModel, action.InputCommandInfo);

        private void HandleChangePeopleAtWorkCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            GameModel.EconomyModel.ChangePeopleAtWork(action.InputCommandInfo.PeopleDiff);
        
        private void HandleExitCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            GameModel.CommandModel.CloseCreatingMenu(unitModel, mapCellModel);

        private void HandleExitProductionMenuCommand(InputAction action, UnitModel unitModel, MapCellModel mapCellModel) =>
            GameModel.CommandModel.CloseProductionMenu();
    }
}