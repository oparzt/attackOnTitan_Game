using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public class StepEventHandler
    {
        private readonly GameModel _gameModel;
        private int _step;

        private Dictionary<int, Action> _wave = new ();

        public StepEventHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
            _wave[25] = () => _gameModel.CommandModel.CreateTitans(2);
            _wave[32] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[39] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[44] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[50] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[52] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[54] = () => _gameModel.CommandModel.CreateTitans(3);
            _wave[60] = () => _gameModel.CommandModel.CreateTitans(4);
            _wave[62] = () => _gameModel.CommandModel.CreateTitans(4);
            _wave[64] = () => _gameModel.CommandModel.CreateTitans(4);
            _wave[70] = () => _gameModel.CommandModel.CreateTitans(5);
            _wave[72] = () => _gameModel.CommandModel.CreateTitans(5);
        }

        public void HandleStepBtnPressed(InputAction action)
        {
            _gameModel.StepEnd = true;
            _gameModel.SelectedUnit?.SetUnselectedOpacity();
            _gameModel.PreselectedUnit?.SetUnselectedOpacity();
            _gameModel.SelectedUnit = null;
            _gameModel.PreselectedUnit = null;
            _gameModel.UnitPath.SetUnit(null);
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
           _gameModel.CommandModel.ClearCommandBar();
           _gameModel.CommandModel.ClearCreatingChoose();
           _step++;
           GameModel.OutputActions.Enqueue(new OutputAction
           {
               ActionType = OutputActionType.UpdateGameStepCount,
               StepCount = _step
           });
           _gameModel.EconomyModel.FillResource();

           if (_wave.TryGetValue(_step, out var wave)) wave();
           
           var titans = _gameModel.Units.Values
               .Where(unit => unit.UnitType == UnitType.Titan)
               .ToArray();
           
           RestoreUnitsEnergy(_gameModel.Units.Values);
           
           CheckMapForBattles();
            

            while (titans.Any(titan => titan.Energy > titan.GetEnergyCost()))
                foreach (var titan in titans.Where(titan => titan.Energy > titan.GetEnergyCost()))
                    _gameModel.TitanPath.TitanStep(titan);
            
            CheckMapForBattles();
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.StepStart
            });
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
            
            if (_gameModel.Units.Values
                .Where(unit => unit.UnitType == UnitType.Titan)
                .Any(unit => _gameModel.Map.InnerGates.Contains(unit.CurCell)))
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.GameOver,
                    Win = false
                });
        }

        private void RestoreUnitsEnergy(IEnumerable<UnitModel> unitModels)
        {
            foreach (var unitModel in unitModels)
                unitModel.Energy = unitModel.MaxEnergy;
        }

        private void CheckMapForBattles()
        {
            var mapCells = _gameModel.Units.Values
                .Where(unit => unit.UnitType == UnitType.Titan)
                .Where(titan => titan.CurCell.GetAllUnitInCell(false).Any())
                .Select(titan => titan.CurCell)
                .ToHashSet();

            foreach (var mapCell in mapCells
                .Where(mapCell => mapCell.GetAllUnitInCell(false).Any() && 
                    mapCell.GetAllUnitInCell(true).Any()))
                _gameModel.CommandModel.AttackCommand(mapCell);
        }
    }
}