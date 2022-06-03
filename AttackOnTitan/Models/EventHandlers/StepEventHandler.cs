using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public class StepEventHandler
    {
        private readonly GameModel _gameModel;
        private int _step;

        private Queue<(int, Action)> _wave = new ();

        public StepEventHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
            _wave.Enqueue((25, () => _gameModel.CommandModel.CreateTitans(2)));
            _wave.Enqueue((32, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((39, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((44, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((50, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((52, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((54, () => _gameModel.CommandModel.CreateTitans(3)));
            _wave.Enqueue((60, () => _gameModel.CommandModel.CreateTitans(4)));
            _wave.Enqueue((62, () => _gameModel.CommandModel.CreateTitans(4)));
            _wave.Enqueue((64, () => _gameModel.CommandModel.CreateTitans(4)));
            _wave.Enqueue((70, () => _gameModel.CommandModel.CreateTitans(5)));

            for (var i = 72; i < 2000; i += 3)
                _wave.Enqueue((i, () => _gameModel.CommandModel.CreateTitans(8)));
            
            UpdateStepCount();
            HandleWave();
        }

        private void UpdateStepCount()
        {
            _step++;
            var waveStr = _wave.TryPeek(out var wave) ? 
                $" До следующей атаки {wave.Item1 - _step}" : 
                string.Empty;

            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateGameStepCount,
                StepInfo = $"Ход {_step}.{waveStr}"
            });
        }

        private void HandleWave()
        {
            if (!_wave.TryPeek(out var wave) || wave.Item1 != _step) return;
            
            wave.Item2();
            _wave.Dequeue();
        }

        public void HandleStepBtnPressed(InputAction action)
        {
            UpdateStepCount();
            HandleWave();

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
            _gameModel.EconomyModel.FillResource();
           
            var titans = _gameModel.Units.Values
            .Where(unit => unit.UnitType == UnitType.Titan)
            .ToArray();
            
            RestoreUnitsEnergy(_gameModel.Units.Values);
           
            CheckMapForBattles();

            while (titans.Any(titan => titan.Energy > titan.GetEnergyCost(TravelMode.TitanRun)))
            {
               foreach (var titan in titans.Where(titan => 
                            titan.Energy > titan.GetEnergyCost(TravelMode.TitanRun)))
                   _gameModel.TitanPath.TitanStep(titan);
               
               Thread.Sleep(1000);
            }
            
            CheckMapForBattles();
            CheckMapForDestroyBuildings();
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.StepStart
            });
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });

            CheckForTitanWin();
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

        private void CheckMapForDestroyBuildings()
        {
            var mapCells = _gameModel.Units.Values
                .Where(unit => unit.UnitType == UnitType.Titan)
                .Where(titan => !MapCellModel.WallBuildingTypes.Contains(titan.CurCell.BuildingType))
                .Select(titan => titan.CurCell)
                .ToHashSet();

            foreach (var mapCell in mapCells)
                mapCell.UpdateBuildingType(BuildingType.None);
            
            _gameModel.EconomyModel.UpdateResourceSettings();
        }

        private void CheckForTitanWin()
        {
            if (_gameModel.Units.Values
                .Where(unit => unit.UnitType == UnitType.Titan)
                .Any(unit => _gameModel.Map.InnerGates.Contains(unit.CurCell)))
                GameModel.InputActions.Enqueue(new InputAction
                {
                    ActionType = InputActionType.GameOver,
                    Win = false
                });
        }
    }
}