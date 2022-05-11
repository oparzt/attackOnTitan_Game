using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace AttackOnTitan.Models
{
    public class GameModel : IDisposable
    {
        private Thread _modelThread;

        private bool _killThread;

        public static ConcurrentQueue<InputAction> InputActions = new();
        public static ConcurrentQueue<OutputAction> OutputActions = new();

        private Dictionary<InputActionType, Action<InputAction>> _handlers = new();

        public MapModel Map;
        public MapEventHandler MapEventHandler;

        public UnitModel PreselectedUnit;
        public UnitModel SelectedUnit;
        public UnitPath UnitPath;
        public Dictionary<int, UnitModel> Units = new();

        public UnitEventHandler UnitEventHandler;

        public KeyEventHandler KeyEventHandler;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            Map = new MapModel(columnsMapCount, rowsMapCount);
            UnitPath = new(this);
            var textures = new[]
            {
                "Scout", 
                "Garrison", "Police",
                "Builder", "Cadet", 
                "Titan", "Titan"
            };
            var positions = new[]
            {
                Position.Center, 
                Position.LeftTopBorder, Position.RightTopBorder, 
                Position.LeftBottomBorder, Position.RightBottomBorder,
                Position.TopBorder, Position.BottomBorder,
            };
            
            

            for (var i = 0; i < 7; i++)
            {
                Units[i] = new UnitModel(i, true);
                Units[i].CurCell = Map[2, 2];
                OutputActions.Enqueue(new(OutputActionType.AddUnit, 
                    new(i, 2, 2, positions[i], textures[i], null), 
                    null));
            }

            InitializateHandlers();
        }

        public void InitializateHandlers()
        {
            MapEventHandler = new(this);
            KeyEventHandler = new(this);
            UnitEventHandler = new(this);

            _handlers[InputActionType.None] = action => { };
            _handlers[InputActionType.KeyPressed] = KeyEventHandler.Handle;
            _handlers[InputActionType.SelectMapCell] = MapEventHandler.HandleSelect;
            _handlers[InputActionType.SelectUnit] = UnitEventHandler.HandleSelect;
            _handlers[InputActionType.UnitStopMove] = UnitEventHandler.HandleStopMove;
        }

        public void Run()
        {
            _modelThread = new Thread(Step);
            _modelThread.IsBackground = true;
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
