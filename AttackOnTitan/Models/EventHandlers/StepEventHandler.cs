using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class StepEventHandler
    {
        public readonly GameModel GameModel;

        public StepEventHandler(GameModel gameModel)
        {
            GameModel = gameModel;
        }

        public void HandleStepBtnPressed(InputAction action)
        {
            GameModel.StepEnd = true;
            GameModel.BlockClickEvents = true;
            GameModel.SelectedUnit?.SetUnselectedOpacity();
            GameModel.SelectedUnit = null;
            GameModel.UnitPath.SetUnit(null);
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
           GameModel.CommandModel.ClearCommandBar();
           GameModel.CommandModel.ClearCreatingChoose();
           GameModel.EconomyModel.FillResource();
           
           RestoreUnitsEnergy(GameModel.Units.Values);

            var units = new List<UnitModel>();
            var enemies = new List<UnitModel>();

            for (var x = 0; x < GameModel.Map.ColumnsCount; x++)
            for (var y = 0; y < GameModel.Map.RowsCount; y++)
            {
                var mapCell = GameModel.Map[x, y];
                units.AddRange(mapCell.GetAllUnitInCell(false));
                enemies.AddRange(mapCell.GetAllUnitInCell(true));
                
                if (units.Count != 0 && enemies.Count != 0)
                    BattleInCell(mapCell, units, enemies);
                units.Clear();
                enemies.Clear();
                if (mapCell.BuildingType != BuildingType.HiddenNone)
                    MapModel.SetUnselectedOpacity(mapCell);
            }
            
            GameModel.StepEnd = false;
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
        }

        private void RestoreUnitsEnergy(IEnumerable<UnitModel> unitModels)
        {
            foreach (var unitModel in unitModels)
                unitModel.Energy = UnitModel.MaxEnergy;
        }

        private void BattleInCell(MapCellModel mapCellModel, List<UnitModel> units, 
            List<UnitModel> enemies)
        {
            var unitsInSafe = units.Count - enemies.Count;
            var deadCount = unitsInSafe > 0 ? enemies.Count : units.Count;

            for (var i = 0; i < deadCount; i++)
            {
                GameModel.Units.Remove(units[i].ID);
                GameModel.Units.Remove(enemies[i].ID);
                mapCellModel.RemoveUnitFromCell(units[i]);
                mapCellModel.RemoveUnitFromCell(enemies[i]);

                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.RemoveUnit,
                    UnitInfo = new UnitInfo(units[i].ID)
                });
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.RemoveUnit,
                    UnitInfo = new UnitInfo(enemies[i].ID)
                });
            }
        }
    }
}