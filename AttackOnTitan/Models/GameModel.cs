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

        private readonly Dictionary<InputActionType, Action<InputAction>> _handlers = new();

        public readonly MapModel Map;

        public UnitModel PreselectedUnit;
        public UnitModel SelectedUnit;
        public readonly UnitPath UnitPath;
        public readonly Dictionary<int, UnitModel> Units = new();
        
        public bool StepEnd = false;
        public bool BlockClickEvents = false;

        private UnitEventHandler _unitEventHandler;
        private MapEventHandler _mapEventHandler;
        private KeyEventHandler _keyEventHandler;
        private StepEventHandler _stepEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new UnitPath(this);
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

            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddResource,
                ResourceInfo = new ResourceInfo(ResourceType.Coin)
                {
                    Count = "100",
                    TextureName = "Coin",
                    TextureSize = new Point(29, 20)
                }
            });
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddResource,
                ResourceInfo = new ResourceInfo(ResourceType.Log)
                {
                    Count = "100",
                    TextureName = "Log",
                    TextureSize = new Point(48, 24)
                }
            });
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddResource,
                ResourceInfo = new ResourceInfo(ResourceType.Stone)
                {
                    Count = "100",
                    TextureName = "Stone",
                    TextureSize = new Point(52, 30)
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(2, 3)
                {
                    TextureName = "House1"
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(3, 3)
                {
                    TextureName = "House2"
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(4, 3)
                {
                    TextureName = "House3"
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(2, 4)
                {
                    TextureName = "Centre"
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(3, 4)
                {
                    TextureName = "Barracks"
                }
            });
            
            OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.ChangeTextureIntoCell,
                MapCellInfo = new MapCellInfo(4, 4)
                {
                    TextureName = "Warehouse"
                }
            });

            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            _mapEventHandler = new MapEventHandler(this);
            _keyEventHandler = new KeyEventHandler(this);
            _unitEventHandler = new UnitEventHandler(this);
            _stepEventHandler = new StepEventHandler(this);

            _handlers[InputActionType.None] = _ => { };
            _handlers[InputActionType.KeyPressed] = _keyEventHandler.Handle;
            _handlers[InputActionType.SelectMapCell] = _mapEventHandler.HandleSelect;
            _handlers[InputActionType.SelectUnit] = _unitEventHandler.HandleSelect;
            _handlers[InputActionType.UnitStopMove] = _unitEventHandler.HandleStopMove;
            _handlers[InputActionType.UnitCommand] = _unitEventHandler.HandleCommand;
            _handlers[InputActionType.UnselectUnit] = _unitEventHandler.HandleUnselect;
            _handlers[InputActionType.StepBtnPressed] = _stepEventHandler.HandleStepBtnPressed;
            _handlers[InputActionType.UpdateWasEnd] = action => BlockClickEvents = false;
            _handlers[InputActionType.UpdateNoServicedZones] = action =>
                OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.UpdateNoServicedZoneForMap,
                    NoServicedZone = action.NoServicedZone
                });
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
