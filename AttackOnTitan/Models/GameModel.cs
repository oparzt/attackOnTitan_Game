using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;

namespace AttackOnTitan.Models
{
    public enum ResourceType
    {
        Coin,
        Log,
        Stone
    }
    
    public class GameModel : IDisposable
    {
        private Thread _modelThread;
        private bool _killThread;

        public static readonly ConcurrentQueue<InputAction> InputActions = new();
        public static readonly ConcurrentQueue<OutputAction> OutputActions = new();

        public static readonly Dictionary<ResourceType, string> ResourceTexturesName = new()
        {
            [ResourceType.Coin] = "Coin",
            [ResourceType.Log] = "Log",
            [ResourceType.Stone] = "Stone"
        };
        
        public readonly Dictionary<ResourceType, int> ResourceCount = new()
        {
            [ResourceType.Coin] = 100,
            [ResourceType.Log] = 100,
            [ResourceType.Stone] = 100
        };

        private readonly Dictionary<InputActionType, Action<InputAction>> _handlers = new();

        public readonly MapModel Map;

        public UnitModel PreselectedUnit;
        public UnitModel SelectedUnit;
        public readonly UnitPath UnitPath;
        public readonly CommandModel CommandModel;
        public readonly Dictionary<int, UnitModel> Units = new();
        
        public bool StepEnd = false;
        public bool BlockClickEvents = false;

        private UnitEventHandler _unitEventHandler;
        private MapEventHandler _mapEventHandler;
        private KeyEventHandler _keyEventHandler;
        private StepEventHandler _stepEventHandler;
        private CommandEventHandler _commandEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new UnitPath(this);
            CommandModel = new CommandModel(this);
            var unitsTypes = new[]
            {
                UnitType.Scout, UnitType.Builder, UnitType.Titan
            };
            var positions = new[]
            {
                Position.TopLeft, Position.TopRight, Position.BottomLeft, Position.BottomRight
            };

            for (var i = 0; i < unitsTypes.Length; i++)
            {
                Units[i] = new UnitModel(i, unitsTypes[i]);
                Units[i].AddUnitToTheMap(Map[2, 2], positions[i]);
            }

            foreach (var resourceCountPair in ResourceCount)
            {
                OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.AddResource,
                    ResourceInfo = new ResourceInfo(resourceCountPair.Key)
                    {
                        Count = resourceCountPair.Value.ToString(),
                        TextureName = ResourceTexturesName[resourceCountPair.Key],
                    }
                });
            }
            
            
            for (var i = 2; i < 10; i++)
                Map[i, 3].UpdateBuildingType(BuildingType.House);
            
            Map[2, 4].UpdateBuildingType(BuildingType.Centre);
            Map[3, 4].UpdateBuildingType(BuildingType.Barracks);
            Map[4, 4].UpdateBuildingType(BuildingType.Warehouse);
            
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            _mapEventHandler = new MapEventHandler(this);
            _keyEventHandler = new KeyEventHandler(this);
            _unitEventHandler = new UnitEventHandler(this);
            _stepEventHandler = new StepEventHandler(this);
            _commandEventHandler = new CommandEventHandler(this);

            _handlers[InputActionType.None] = _ => { };
            _handlers[InputActionType.KeyPressed] = _keyEventHandler.Handle;
            _handlers[InputActionType.SelectMapCell] = _mapEventHandler.HandleSelect;
            _handlers[InputActionType.SelectUnit] = _unitEventHandler.HandleSelect;
            _handlers[InputActionType.UnitStopMove] = _unitEventHandler.HandleStopMove;
            _handlers[InputActionType.UnselectUnit] = _unitEventHandler.HandleUnselect;
            _handlers[InputActionType.StepBtnPressed] = _stepEventHandler.HandleStepBtnPressed;
            _handlers[InputActionType.UpdateWasEnd] = action => BlockClickEvents = false;
            _handlers[InputActionType.UpdateNoServicedZones] = action =>
                OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.UpdateNoServicedZoneForMap,
                    NoServicedZone = action.NoServicedZone
                });
            _handlers[InputActionType.ExecCommand] = _commandEventHandler.HandleCommand;
        }

        public void Run()
        {
            _modelThread = new Thread(Step)
            {
                IsBackground = true
            };
            _modelThread.Start();
        }

        public void Dispose() => _killThread = true;

        public void UpdateResourceCount()
        {
            foreach (var resCountPair in ResourceCount)
            {
                OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.UpdateResourceCount,
                    ResourceInfo = new ResourceInfo(resCountPair.Key)
                    {
                        Count = resCountPair.Value.ToString()
                    }
                }); 
            }
            
        }

        private void Step()
        {
            while (!_killThread)
            {
                while (InputActions.TryDequeue(out var action)) {
                    _handlers[action.ActionType](action);
                }
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
