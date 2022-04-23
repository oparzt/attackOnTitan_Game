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

            for (var i = 0; i < 4; i++)
            {
                Units[i] = new UnitModel(i, true);
                Units[i].CurCell = Map[0, i];
                OutputActions.Enqueue(new OutputAction
                {
                    ActionType = OutputActionType.AddUnit,
                    UnitInfo =
                {
                    ID = i,
                    X = 0,
                    Y = i,
                    TextureName = "Ball"
                }
                });
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
            _handlers[InputActionType.SelectMapCell] = MapEventHandler.Handle;
            _handlers[InputActionType.SelectUnit] = UnitEventHandler.Handle;
        }

        public void Run()
        {
            _modelThread = new Thread(Step);
            _modelThread.IsBackground = true;
            _modelThread.Start();

            //for (var i = 0; i < 3; i++) {
            //    _units[i] = new UnitModel(i);
            //    _map.AddUnit(_units[i], i, 0);
            //}

        }

        public void Dispose() => _killThread = true;

        private void Step()
        {
            var stopwatch = new Stopwatch();
            while (!_killThread)
            {
                while (InputActions.TryDequeue(out var action)) {
                    stopwatch.Restart();
                    _handlers[action.ActionType](action);
                    Console.WriteLine(stopwatch.ElapsedMilliseconds); 
                }
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }
    }
}
