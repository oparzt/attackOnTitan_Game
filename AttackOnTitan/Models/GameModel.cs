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

        public static ConcurrentQueue<InputAction> InputActions = new();
        public static ConcurrentQueue<OutputAction> OutputActions = new();

        private Dictionary<InputActionType, Action<InputAction>> _handlers = new();
        private Dictionary<int, Unit> _units = new();

        private Map _map;

        public GameModel(int columnsMapCount, int rowsMapCount)
        {
            _map = new Map(columnsMapCount, rowsMapCount);
            InitializateHandlers();
        }

        public void InitializateHandlers()
        {
            _handlers[InputActionType.None] = action => { };
            _handlers[InputActionType.KeyPressed] = HandleKeyPressed;
            _handlers[InputActionType.SelectMapCell] = _map.HandleSelect;
            _handlers[InputActionType.SelectUnit] = HandleSelectUnit;
        }

        public void Run()
        {
            _modelThread = new Thread(Step);
            _modelThread.IsBackground = true;
            _modelThread.Start();

            for (var i = 0; i < 3; i++) {
                _units[i] = new Unit(i);
                _map.AddUnit(_units[i], i, 0);
            }

        }

        public void Dispose() => _killThread = true;

        private void Step()
        {
            while (!_killThread)
            {
                while (InputActions.TryDequeue(out var action))
                    _handlers[action.ActionType](action);
                Thread.Sleep(30);
            }

            InputActions.Clear();
            OutputActions.Clear();
        }

        private void HandleKeyPressed(InputAction action)
        {

        }

        private void HandleSelectUnit(InputAction action)
        {
            if (action.MouseBtn == PressedMouseBtn.Left)
            {
                _map.UnselectUnit();
                _map.SelectUnit(_units[action.SelectedUnit.ID]);
            }
        }
    }
}
