using System.Collections.Generic;
using System.Linq;

namespace AttackOnTitan.Models
{
    public class EconomyModel
    {
        private GameModel _gameModel;

        public readonly Dictionary<ResourceType, int> ResourceCount = new()
        {
            [ResourceType.Coin] = 100,
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100,
            [ResourceType.People] = 100
        };

        private readonly Dictionary<ResourceType, int> _resourceCountInStep = new()
        {
            [ResourceType.Coin] = 0,
            [ResourceType.Log] = 0,
            [ResourceType.Stone] = 0,
            [ResourceType.People] = 0
        };

        private readonly Dictionary<ResourceType, int> _resourceLimit = new()
        {
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100,
            [ResourceType.People] = 100
        };

        private readonly Dictionary<ResourceType, int> _peopleAtWork = new()
        {
            [ResourceType.Log] = 0,
            [ResourceType.Stone] = 0,
        };

        private static readonly ResourceType[] StepCountedResources = {ResourceType.Log, ResourceType.Stone, ResourceType.People, ResourceType.Coin};
        private static readonly ResourceType[] LimitedResources = {ResourceType.Log, ResourceType.Stone, ResourceType.People};

        public EconomyModel(GameModel gameModel)
        {
            _gameModel = gameModel;
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
                        TextureName = EconomyTextures.ResourceTexturesName[resCountPair.Key]
                    }
                });
            }
            
            GameModel.OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.InitializeProductionMenu,
                ProductionInfo = new ProductionInfo
                {
                    BackgroundTextureName = "BuilderCard",
                    ResourceTexturesName = EconomyTextures.ResourceTexturesName
                }
            });
        }

        public void UpdateResourceSettings()
        {
            var countInStep = StepCountedResources.ToDictionary(resType => resType, resType => 0);
            var countLimit = LimitedResources.ToDictionary(resType => resType, resType => 0);

            foreach (var mapCell in _gameModel.Map)
            {
                var curCountInStep = BuildingEconomyModel.StepCountDiff[mapCell.BuildingType];
                var curCountLimit = BuildingEconomyModel.LimitDiff[mapCell.BuildingType];

                foreach (var (resType, resCount) in curCountInStep)
                    countInStep[resType] += resCount;
                foreach (var (resType, resCount) in curCountLimit)
                    countLimit[resType] += resCount;
            }
            
            foreach (var units in _gameModel.Units.Values)
            {
                var curCountInStep = UnitEconomyModel.StepCountDiff[units.UnitType];
                var curCountLimit = UnitEconomyModel.LimitDiff[units.UnitType];

                foreach (var (resType, resCount) in curCountInStep)
                    countInStep[resType] += resCount;
                foreach (var (resType, resCount) in curCountLimit)
                    countLimit[resType] += resCount;
            }

            NormalizePeopleAtWorkCount();

            foreach (var (resType, resCount) in _peopleAtWork)
            {
                countInStep[resType] += resCount * PeopleEconomyModel.PeopleAtWorkMadeResource[resType];
                countInStep[ResourceType.Coin] += resCount * PeopleEconomyModel.PeopleAtWorkCoinDiff[resType];
            }

            countInStep[ResourceType.Coin] += ResourceCount[ResourceType.People] 
                * PeopleEconomyModel.PeopleMakeCoinCoeff;
            
            foreach (var (resType, resCount) in countInStep)
                _resourceCountInStep[resType] = resCount;
            foreach (var (resType, resCount) in countLimit)
                _resourceLimit[resType] = resCount;
            
            foreach (var (resType, resCount) in _resourceLimit)
                ResourceCount[resType] = ResourceCount[resType] >= resCount ? 
                    resCount : ResourceCount[resType];

            UpdateResourceView();
        }

        public void UseResource(Dictionary<ResourceType, int> countDiff)
        {
            foreach (var (resType, resCount) in countDiff)
                ResourceCount[resType] += resCount;
            
            UpdateResourceView();
        }
        
        public void FillResource()
        {
            foreach (var resCountPair in _resourceCountInStep)
                ResourceCount[resCountPair.Key] += resCountPair.Value;
            
            UpdateResourceSettings();
        }

        public void ChangePeopleAtWork((ResourceType, int) peopleDiff)
        {
            var peopleAtWorkLimit = ResourceCount[ResourceType.People];
            var allPeopleAtWork = _peopleAtWork.Sum(peopleOnRes => peopleOnRes.Value);
            
            if (allPeopleAtWork + peopleDiff.Item2 > peopleAtWorkLimit ||
                _peopleAtWork[peopleDiff.Item1] + peopleDiff.Item2 < 0)
                return;

            _peopleAtWork[peopleDiff.Item1] += peopleDiff.Item2;
            UpdateResourceSettings();
        }

        private void NormalizePeopleAtWorkCount()
        {
            var peopleAtWorkLimit = ResourceCount[ResourceType.People];
            var allPeopleAtWork = _peopleAtWork.Sum(peopleOnRes => peopleOnRes.Value);

            while (allPeopleAtWork > peopleAtWorkLimit)
            {
                if (_peopleAtWork[ResourceType.Log] > 0)
                {
                    _peopleAtWork[ResourceType.Log] -= 1;
                    allPeopleAtWork -= 1;
                }
                
                if (allPeopleAtWork <= peopleAtWorkLimit) break;
                
                if (_peopleAtWork[ResourceType.Stone] > 0)
                {
                    _peopleAtWork[ResourceType.Stone] -= 1;
                    allPeopleAtWork -= 1;
                }
            }
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
                    (_resourceLimit.TryGetValue(resCountPair.Key, out var limit) ? $"/{limit}" : "") +
                    (_resourceCountInStep.TryGetValue(resCountPair.Key, out var countInStep) ? $" ({countInStep})" : "");
            }

            var peopleAtWorkLimit = ResourceCount[ResourceType.People];
            var allPeopleAtWork = _peopleAtWork.Sum(peopleOnRes => peopleOnRes.Value);
            var canUpdateProductionResource = new Dictionary<ResourceType, string>();
            var canUpdateProduction = new List<(bool, bool)>();

            foreach (var peopleAtWork in _peopleAtWork)
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