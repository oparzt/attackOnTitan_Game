using System.Collections.Generic;

namespace AttackOnTitan.Models
{
    public class EconomyModel
    {
        private GameModel GameModel;

        public readonly Dictionary<ResourceType, int> ResourceCount = new()
        {
            [ResourceType.Coin] = 100,
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100,
            [ResourceType.People] = 100
        };

        public readonly Dictionary<ResourceType, int> ResourceCountInStep = new()
        {
            [ResourceType.Coin] = 10,
            [ResourceType.Log] = 0,
            [ResourceType.Stone] = 0,
            [ResourceType.People] = 0
        };

        public readonly Dictionary<ResourceType, int> ResourceLimit = new()
        {
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100,
            [ResourceType.People] = 100
        };

        public static readonly Dictionary<ResourceType, string> ResourceTexturesName = new()
        {
            [ResourceType.Coin] = "Coin",
            [ResourceType.Log] = "Log",
            [ResourceType.Stone] = "Stone",
            [ResourceType.People] = "People"
        };

        public EconomyModel(GameModel gameModel)
        {
            GameModel = gameModel;
        }

        public void InitializeResourcePanel()
        {
            foreach (var resCountPair in ResourceCount)
            {
                GameModel.OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.AddResource,
                    ResourceInfo = new ResourceInfo(resCountPair.Key)
                    {
                        Count = resCountPair.Value.ToString(),
                        TextureName = ResourceTexturesName[resCountPair.Key]
                    }
                });
            }
        }

        public void UseResources(Dictionary<ResourceType, int> price)
        {
            foreach (var pricePair in price)
                ResourceCount[pricePair.Key] -= pricePair.Value;
            UpdateResourceView();
        }

        public void UpdateResourceSettings(Dictionary<ResourceType, int> countDiff, 
            Dictionary<ResourceType, int> stepCountDiff,
            Dictionary<ResourceType, int> limitDiff)
        {
            foreach (var resCountPair in countDiff)
                ResourceCount[resCountPair.Key] += resCountPair.Value;
            foreach (var resCountPair in stepCountDiff)
                ResourceCountInStep[resCountPair.Key] += resCountPair.Value;
            foreach (var resCountPair in limitDiff)
                ResourceLimit[resCountPair.Key] += resCountPair.Value;
            UpdateResourceView();
        }

        public void FillResource()
        {
            foreach (var resCountPair in ResourceCountInStep)
                ResourceCount[resCountPair.Key] += resCountPair.Value;
            UpdateResourceView();
        }
        
        public void UpdateResourceView()
        {
            foreach (var resCountPair in ResourceCount)
            {
                GameModel.OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.UpdateResourceCount,
                    ResourceInfo = new ResourceInfo(resCountPair.Key)
                    {
                        Count = resCountPair.Value.ToString()
                    }
                }); 
            }
        }
    }
}