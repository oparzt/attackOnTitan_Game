using System;

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
            Console.WriteLine("Модель остановили");
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeStepBtnState
            });
        }
    }
}