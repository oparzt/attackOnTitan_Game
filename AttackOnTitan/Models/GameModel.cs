using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
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
        public MapCellModel PreSelectedMapCellModel;
        public readonly UnitPath UnitPath;
        public readonly TitanPath TitanPath;
        public readonly PathFinder PathFinder;
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
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new UnitPath(this);
            TitanPath = new TitanPath(this);
            CommandModel = new CommandModel(this);
            EconomyModel = new EconomyModel(this);
            StatusBarModel = new StatusBarModel();
            PathFinder = new PathFinder();


            EconomyModel.InitializeResourcePanel();
            EconomyModel.UpdateResourceView();

            InitializeHandlers();
        }

        public string[] GetTexturesNames()
        {
            var textures = new List<string>();
            textures.AddRange(CommandTextures.CommandTexturesNames.Values);
            textures.AddRange(EconomyTextures.ResourceTexturesName.Values);
            textures.Add(MapTextures.SimpleHexagon);
            textures.AddRange(MapTextures.BuildingTextureNames.Values);
            textures.AddRange(MapTextures.HexagonTextureNames.Values);
            textures.AddRange(UnitTextures.UnitTextureNames.Values);
            return textures.ToArray();
        }

        public void InitMapBuildings()
        {
            var buildings = new Dictionary<BuildingType, (int, int)[]>
            {
                [BuildingType.HiddenNone] = new[]
                {
                    (19, 15), (20, 0), (20, 1), (20, 15),
                    (21, 0), (21, 1), (21, 14), (21, 15),
                    (22, 0), (22, 1), (22, 2), (22, 3), (22, 13), (22, 14), (22, 15)
                },
                [BuildingType.Wall] = new[]
                {
                    (0, 0), (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9),
                    (0, 10), (0, 11), (0, 12), (0, 13), (0, 14), (0, 15),
                    (1, 0), (1, 15), (2, 0), (2, 15),
                    (3, 0), (3, 15), (4, 0), (4, 15),
                    (5, 0), (5, 15), (6, 0), (6, 15),
                    (7, 0), (7, 15), (8, 0), (8, 15),
                    (9, 0), (9, 15), (10, 0), (10, 15),
                    (11, 0), (11, 15), (12, 0), (12, 15),
                    (13, 0), (13, 15), (14, 0), (14, 15),
                    (15, 0), (15, 15), (16, 0), (16, 15),
                    (17, 0), (17, 15), (18, 0), (18, 1), (18, 15),
                    (19, 0), (19, 1), (19, 14), (20, 2), (20, 3),
                    (20, 13), (20, 14),
                    (21, 2), (21, 3), (21, 4), (21, 11), (21, 12), (21, 13),
                    (22, 4), (22, 5), (22, 6), (22, 7), (22, 8), (22, 9), (22, 10), (22, 11), (22, 12),
                },
                [BuildingType.InnerGates] = new[]
                {
                    (2, 7), (2, 8)
                },
                [BuildingType.OuterGates] = new[]
                {
                    (20, 7), (20, 8)
                },
                [BuildingType.NearWall] = new[]
                {
                    (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), (1, 9),
                    (1, 10), (1, 11), (1, 12), (1, 13), (1, 14),
                    (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 9),
                    (2, 10), (2, 11), (2, 12), (2, 13), (2, 14),
                    (3, 1), (3, 14), (4, 1), (4, 14), (5, 1), (5, 14),
                    (6, 1), (6, 14), (7, 1), (7, 14), (8, 1), (8, 14),
                    (9, 1), (9, 14), (10, 1), (10, 14), (11, 1), (11, 14),
                    (12, 1), (12, 14), (13, 1), (13, 14), (14, 1), (14, 14),
                    (15, 1), (15, 14), (16, 1), (16, 14), (17, 1), (17, 13), (17, 14),
                    (18, 2), (18, 3), (18, 13), (18, 14),
                    (19, 2), (19, 3), (19, 11), (19, 12), (19, 13),
                    (20, 4), (20, 5), (20, 6), (20, 9), (20, 10), (20, 11), (20, 12),
                    (21, 5), (21, 6), (21, 7), (21, 8), (21, 9), (21, 10),
                },
                [BuildingType.Centre] = new[]
                {
                    (7, 7)
                },
                [BuildingType.House1] = new[]
                {
                    (6, 7), (6, 8)
                },
                [BuildingType.Barracks] = new[]
                {
                    (9, 7)
                },
                [BuildingType.Warehouse] = new[]
                {
                    (3, 2)
                }
            };
            
            Map.InitializeBuildings(buildings);
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

        public void HandleActions()
        {
            while (InputActions.TryDequeue(out var action)) {
                if (!StepEnd) 
                    _handlers[action.ActionType](action);
                else if (action.ActionType == InputActionType.StepStart)
                    _handlers[action.ActionType](action);
            }
        }

        private void Step()
        {
            while (!_killThread)
            {
                HandleActions();
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
