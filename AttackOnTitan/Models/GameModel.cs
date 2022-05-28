﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace AttackOnTitan.Models
{
    public enum ResourceType
    {
        Coin,
        Log,
        Stone,
        People
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

        private readonly Dictionary<InputActionType, Action<InputAction>> _handlers = new();

        public readonly MapModel Map;

        public UnitModel PreselectedUnit;
        public UnitModel SelectedUnit;
        public readonly UnitPath UnitPath;
        public readonly TitanPath TitanPath;
        public readonly StatusBarModel StatusBarModel;
        public readonly CommandModel CommandModel;
        public readonly EconomyModel EconomyModel;
        public readonly Dictionary<int, UnitModel> Units = new();
        
        public bool StepEnd = false;
        public bool BlockClickEvents;

        private UnitEventHandler _unitEventHandler;
        private MapEventHandler _mapEventHandler;
        private KeyEventHandler _keyEventHandler;
        private StepEventHandler _stepEventHandler;
        private CommandEventHandler _commandEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            var buildings = new Dictionary<BuildingType, (int, int)[]>
            {
                [BuildingType.HiddenNone] = new []
                {
                    (0,0), (1,0), (2,0), (3,0), (4,0), (17,0), (18,0), (19,0), (20,0), (21,0), (22,0),
                    (0,10), (1,10), (2,10), (3,10), (4,10), (17,10), (18,10), (19,10), (20,10), (21,10), (22,10)
                },
                [BuildingType.Wall] = new []
                {
                    (5,0), (5,1), (5,2), (5,3), (5,6), (5, 7), (5,8), (5,9), (5,10),
                    (6,0), (6,1), (6,10),
                    (7,0), (7,9), (6,10),
                    (8,0), (8,1), (8,10),
                    (9,0), (9,9), (9,10),
                    (10,0), (10,1), (10,10),
                    (11,0), (11,9), (11,10),
                    (12,0), (12,1), (12,10),
                    (13,0), (13,9), (13,10),
                    (14,0), (14,1), (14,10),
                    (15,0), (15,9), (15,10),
                    (16,0), (16,1), (16,9), (16,10),
                    (17,1), (17,2), (17,8), (17,9),
                    (18,1), (18,2), (18,3), (18,4),
                    (18,6), (18,7), (18,8), (18,9),
                    (19,2), (19,3), (19,6), (19,7),
                },
                [BuildingType.InnerGates] = new []
                {
                    (5,4), (5,5)
                },
                [BuildingType.OuterGates] = new []
                {
                    (18,5)
                },
                [BuildingType.OuterNone] = new []
                {
                    (19,1), (19,4), (19,5), (19,8), (19,9),
                    (20,1), (20,2), (20,3), (20,4), (20,5), (20,6), (20,7), (20,8), (20,9), 
                    (21,1), (21,2), (21,3), (21,4), (21,5), (21,6), (21,7), (21,8), (21,9), 
                    (22,1), (22,2), (22,3), (22,4), (22,5), (22,6), (22,7), (22,8), (22,9)
                },
                [BuildingType.Centre] = new []
                {
                    (3,4)
                },
                [BuildingType.House1] = new []
                {
                    (3,5)
                },
                [BuildingType.Barracks] = new []
                {
                    (2,4)
                },
                [BuildingType.Warehouse] = new []
                {
                    (2,5)
                }
            };
            
            Map = new MapModel(columnsMapCount, rowsMapCount, buildings);
            UnitPath = new UnitPath(this);
            TitanPath = new TitanPath(this);
            CommandModel = new CommandModel(this);
            EconomyModel = new EconomyModel(this);
            StatusBarModel = new StatusBarModel();
            

            EconomyModel.InitializeResourcePanel();
            EconomyModel.UpdateResourceView();
            
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
            _handlers[InputActionType.StepStart] = action => StepEnd = false;
            _handlers[InputActionType.GameOver] = action => OutputActions.Enqueue(new OutputAction
            {
                ActionType = OutputActionType.GameOver,
                Win = action.Win
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
                    if (!StepEnd) 
                        _handlers[action.ActionType](action);
                    else if (action.ActionType == InputActionType.StepStart)
                        _handlers[action.ActionType](action);
                }
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
