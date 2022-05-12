using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace AttackOnTitan.Models
{
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

        private UnitEventHandler _unitEventHandler;
        private MapEventHandler _mapEventHandler;
        private KeyEventHandler _keyEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new UnitPath(this);
            var textures = new[]
            {
                "Scout",  "Garrison", "Police", "Cadet",
                "Titan", "Titan", "Titan", "Titan", "Titan", "Titan"
            };
            var positions = new[]
            {
                Position.TopLeft, Position.TopRight, Position.BottomLeft, Position.BottomRight,
                Position.LeftTopBorder, Position.RightTopBorder, 
                Position.LeftBottomBorder, Position.RightBottomBorder,
                Position.TopBorder, Position.BottomBorder,
            };

            for (var i = 0; i < 10; i++)
            {
                Units[i] = new UnitModel(i, true);
                Units[i].CurCell = Map[2, 2];
                OutputActions.Enqueue(new(OutputActionType.AddUnit, 
                    new(i, 2, 2, positions[i], textures[i], null), 
                    null));
            }

            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            _mapEventHandler = new MapEventHandler(this);
            _keyEventHandler = new KeyEventHandler(this);
            _unitEventHandler = new UnitEventHandler(this);

            _handlers[InputActionType.None] = _ => { };
            _handlers[InputActionType.KeyPressed] = _keyEventHandler.Handle;
            _handlers[InputActionType.SelectMapCell] = _mapEventHandler.HandleSelect;
            _handlers[InputActionType.SelectUnit] = _unitEventHandler.HandleSelect;
            _handlers[InputActionType.UnitStopMove] = _unitEventHandler.HandleStopMove;
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
                    _handlers[action.ActionType](action);
                    //Console.WriteLine(stopwatch.ElapsedMilliseconds); 
                }
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
