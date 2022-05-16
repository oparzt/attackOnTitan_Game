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


        private UnitEventHandler _unitEventHandler;
        private MapEventHandler _mapEventHandler;
        private KeyEventHandler _keyEventHandler;
        private StepEventHandler _stepEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new UnitPath(this);
            var textures = new[]
            {
                "Scout",  "Garrison", "Police", "Cadet",
                "Titan", "Titan", "Titan", "Titan"
            };
            var positions = new[]
            {
                Position.TopLeft, Position.TopRight, Position.BottomLeft, Position.BottomRight
            };

            for (var i = 0; i < 4; i++)
            {
                Units[i] = new UnitModel(i)
                {
                    CurCell = Map[2, 2]
                };
                Map[2, 2].UnitsInCell[positions[i]] = Units[i];
                OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.AddUnit,
                    UnitInfo = new UnitInfo(i)
                    {
                        X = 2,
                        Y = 2,
                        Position = positions[i],
                        TextureName = textures[i]
                    }
                });
            }
            
            for (var i = 0; i < 4; i++)
            {
                Units[i + 4] = new UnitModel(i + 4, true)
                {
                    CurCell = Map[4, 2],
                    Enemy = true
                };
                
                Map[4, 2].UnitsInCell[positions[i]] = Units[i + 4];
                
                OutputActions.Enqueue(new OutputAction()
                {
                    ActionType = OutputActionType.AddUnit,
                    UnitInfo = new UnitInfo(i + 4)
                    {
                        X = 4,
                        Y = 2,
                        Position = positions[i],
                        TextureName = textures[i + 4]
                    }
                });
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
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddCommand,
                CommandInfo = new CommandInfo(CommandType.Attack)
                {
                    IsAvailable = true,
                    AvailableTextureName = "AttackIcon",
                    NotAvailableTextureName = "AttackIconHalf"
                }
            });
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddCommand,
                CommandInfo = new CommandInfo(CommandType.Build)
                {
                    IsAvailable = true,
                    AvailableTextureName = "BuildingIcon",
                    NotAvailableTextureName = "BuildingIconHalf"
                }
            });
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddCommand,
                CommandInfo = new CommandInfo(CommandType.Fly)
                {
                    IsAvailable = true,
                    AvailableTextureName = "GasIcon",
                    NotAvailableTextureName = "GasIconHalf"
                }
            });
            
            OutputActions.Enqueue(new OutputAction()
            {
                ActionType = OutputActionType.AddCommand,
                CommandInfo = new CommandInfo(CommandType.Refuel)
                {
                    IsAvailable = true,
                    AvailableTextureName = "RefuelingIcon",
                    NotAvailableTextureName = "RefuelingIconHalf"
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
            _handlers[InputActionType.StepBtnPressed] = _stepEventHandler.HandleStepBtnPressed;
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
            //var stopwatch = new Stopwatch();
            while (!_killThread)
            {
                while (InputActions.TryDequeue(out var action)) {
                    //stopwatch.Restart();
                    if (!StepEnd) _handlers[action.ActionType](action);
                    //Console.WriteLine(stopwatch.ElapsedMilliseconds); 
                }
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
