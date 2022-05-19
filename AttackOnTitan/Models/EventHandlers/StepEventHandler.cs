using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

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
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });

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
                GameModel.Map.SetUnselectedOpacity(mapCell);
            }
            
            GameModel.StepEnd = false;
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
        }

        public void BattleInCell(MapCellModel mapCellModel, List<UnitModel> units, 
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