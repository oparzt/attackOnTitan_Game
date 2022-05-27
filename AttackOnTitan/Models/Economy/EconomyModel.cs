using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class EconomyModel
    {
        private GameModel GameModel;

        public readonly Dictionary<ResourceType, float> ResourceCount = new()
        {
            [ResourceType.Coin] = 100,
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100,
            [ResourceType.People] = 100
        };

        public readonly Dictionary<ResourceType, float> ResourceCountInStep = new()
        {
            [ResourceType.Coin] = 10,
            [ResourceType.Log] = 0,
            [ResourceType.Stone] = 0,
            [ResourceType.People] = 0
        };

        public readonly Dictionary<ResourceType, float> ResourceLimit = new()
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

        public readonly Dictionary<ResourceType, int> PeopleAtWork = new()
        {
            [ResourceType.Log] = 0,
            [ResourceType.Stone] = 0,
        };

        public static readonly Dictionary<ResourceType, float> PeopleAtWorkMadeResource = new()
        {
            [ResourceType.Log] = 1,
            [ResourceType.Stone] = 1,
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
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.InitializeProductionMenu,
                ProductionInfo = new ProductionInfo
                {
                    BackgroundTextureName = "BuilderCard",
                    ResourceTexturesName = ResourceTexturesName
                }
            });
        }

        public void UpdateResourceSettings(Dictionary<ResourceType, float> countDiff, 
            Dictionary<ResourceType, float> stepCountDiff,
            Dictionary<ResourceType, float> limitDiff)
        {
            foreach (var resCountPair in countDiff)
                ResourceCount[resCountPair.Key] += resCountPair.Value;
            foreach (var resCountPair in countDiff)
                ResourceCount[resCountPair.Key] = ResourceCount[resCountPair.Key] < 0 ? 0 : ResourceCount[resCountPair.Key];
            foreach (var resCountPair in stepCountDiff)
                ResourceCountInStep[resCountPair.Key] += resCountPair.Value;
            foreach (var resCountPair in limitDiff)
                ResourceLimit[resCountPair.Key] += resCountPair.Value;
            UpdateResourceView();
        }

        public void ChangePeopleAtWork((ResourceType, int) peopleDiff)
        {
            var peopleAtWorkLimit = ResourceCount[ResourceType.People];
            var allPeopleAtWork = PeopleAtWork.Sum(peopleOnRes => peopleOnRes.Value);
            
            if (allPeopleAtWork + peopleDiff.Item2 > peopleAtWorkLimit ||
                PeopleAtWork[peopleDiff.Item1] + peopleDiff.Item2 < 0)
                return;

            PeopleAtWork[peopleDiff.Item1] += peopleDiff.Item2;
            ResourceCountInStep[peopleDiff.Item1] += PeopleAtWorkMadeResource[peopleDiff.Item1] * peopleDiff.Item2;
            
            UpdateResourceView();
        }

        public void FillResource()
        {
            foreach (var resCountPair in ResourceCountInStep)
                ResourceCount[resCountPair.Key] += resCountPair.Value;
            foreach (var resLimitPair in ResourceLimit)
                    ResourceCount[resLimitPair.Key] = ResourceCount[resLimitPair.Key] >= resLimitPair.Value ? 
                        resLimitPair.Value : ResourceCount[resLimitPair.Key];
            UpdateResourceView();
        }
        
        public void UpdateResourceView()
        {
            var resInformation = new Dictionary<ResourceType, string>();
            
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
                
                resInformation[resCountPair.Key] = resCountPair.Value + 
                    (ResourceLimit.TryGetValue(resCountPair.Key, out var limit) ? $"/{limit}" : "") +
                    (ResourceCountInStep.TryGetValue(resCountPair.Key, out var countInStep) ? $" ({countInStep})" : "");
            }

            var peopleAtWorkLimit = ResourceCount[ResourceType.People];
            var allPeopleAtWork = PeopleAtWork.Sum(peopleOnRes => peopleOnRes.Value);
            var canUpdateProductionResource = new Dictionary<ResourceType, string>();
            var canUpdateProduction = new List<(bool, bool)>();

            foreach (var peopleAtWork in PeopleAtWork)
            {
                canUpdateProductionResource[peopleAtWork.Key] = peopleAtWork.Value.ToString();
                canUpdateProduction.Add((peopleAtWork.Value > 0, allPeopleAtWork < peopleAtWorkLimit));
            }
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.UpdateProductionMenu,
                ProductionInfo = new ProductionInfo
                {
                    ResourceInformation = resInformation,
                    CanUpdateProductionResource = canUpdateProductionResource,
                    CanUpdateProduction = canUpdateProduction.ToArray(),
                    PeopleAtWork = (ResourceType.People, $"{allPeopleAtWork}/{peopleAtWorkLimit} на работе")
                }
            });
        }
    }
}