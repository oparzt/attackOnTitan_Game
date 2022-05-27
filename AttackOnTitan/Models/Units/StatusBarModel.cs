namespace AttackOnTitan.Models
{
    public class StatusBarModel
    {
        public void ClearStatusBar()
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateUnitStatusBar,
                UnitStatus = new string[] {}
            });
        }
        
        public void UpdateStatusBar(string name, float energy, float gas)
        {
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateUnitStatusBar,
                UnitStatus = new string[]
                {
                    name,
                    $"Энергия {energy}",
                    $"Газ {gas}"
                }
            });
        }
    }
}