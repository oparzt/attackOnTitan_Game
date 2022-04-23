using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;
using AttackOnTitan.Models;

namespace AttackOnTitan.Components.Map
{
    public class MapComponent : IComponent
    {
        private IScene _scene;

        private MapCellComponent[,] _mapItems;
        private Camera2D _camera;

        private int _columnCount;
        private int _rowCount;

        private int _hexWidth;
        private int _hexHeight;

        private int _leftColumnIntoView;
        private int _rightColumnIntoView;
        private int _topRowIntoView;
        private int _bottomRowIntoView;

        private Dictionary<int, UnitComponent> _units = new();

        //private InputAction _prevSelectMapAction = new();
        //private InputAction _prevSelectUnitAction = new();

        public MapComponent(IScene parent, int columnCount, int rowCount, int hexWidth, int hexHeight)
        {
            _mapItems = new MapCellComponent[columnCount, rowCount];
            _columnCount = columnCount;
            _rowCount = rowCount;
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;
            _scene = parent;

            InitializeMap();
        }

        private void InitializeMap()
        {
            var mapWidth = _columnCount * _hexWidth / 4 * 3 + _hexWidth / 4;
            var mapHeight = _rowCount * _hexHeight + _hexHeight / 2;

            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            _camera = new Camera2D(0, 0, mapWidth - viewWidth, mapHeight - viewHeight);

            for (var row = 0; row < _rowCount; row++)
                for (var column = 0; column < _columnCount; column++)
                {
                    _mapItems[column, row] = new MapCellComponent(_scene, "Hexagon", column, row,
                        new Rectangle(
                            column * _hexWidth / 4 * 3,
                            row * _hexHeight + (column % 2 == 1 ? _hexHeight / 2 : 0),
                            _hexWidth, _hexHeight));
                }
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            _camera.Update(gameTime, mouseState);

            var mousePos = new Point(mouseState.X, mouseState.Y) - new Point((int)_camera.Pos.X, (int)_camera.Pos.Y);
            var mouseBtn = GetPressedBtn(mouseState.LeftButton, mouseState.RightButton);

            var selectedMapItem = FindMapItemUnderCursor(mousePos);
            var selectedUnitItem = FindUnitItemUnderCursor(mousePos);

            if (selectedMapItem is not null)
                InitiateSelectMapCellAction(selectedMapItem, mouseBtn);

            if (selectedUnitItem is not null)
                InitiateSelectUnitAction(selectedUnitItem, mouseBtn);
            //else
                //_prevSelectUnitAction = new();

            SetItemRangeIntoViewport();
        }

        private PressedMouseBtn GetPressedBtn(ButtonState left, ButtonState right) =>
            right == ButtonState.Pressed ? PressedMouseBtn.Right :
            left == ButtonState.Pressed ?PressedMouseBtn.Left : PressedMouseBtn.None;

        private void InitiateSelectMapCellAction(MapCellComponent mapItem, PressedMouseBtn mouseBtn)
        {
            var action = new InputAction(new SelectedCell(mapItem.X, mapItem.Y), mouseBtn);

            //if (action.Equals(_prevSelectMapAction)
                //&& action.MouseBtn != PressedMouseBtn.None) return;

            //_prevSelectMapAction = action;
            GameModel.InputActions.Enqueue(action);
        }

        private void InitiateSelectUnitAction(UnitComponent unitItem, PressedMouseBtn mouseBtn)
        {
            var action = new InputAction(new SelectedUnit(unitItem.ID), mouseBtn);
            //if (action.Equals(_prevSelectUnitAction)) return;

            //_prevSelectUnitAction = action;
            GameModel.InputActions.Enqueue(action);
        }


        public void AddUnit(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            var centerCell = _mapItems[unitInfo.X, unitInfo.Y].GetCenter();
            var pos = centerCell - new Point(15, 15);

            _units[unitInfo.ID] = new UnitComponent(_scene, unitInfo.ID, unitInfo.TextureName,
                new Rectangle(pos, new Point(30, 30)));
        }

        public void MoveUnit(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            var centerCell = _mapItems[unitInfo.X, unitInfo.Y].GetCenter();
            var pos = centerCell - new Point(15, 15);

            _units[unitInfo.ID].Move(new Rectangle(pos, new Point(30, 30)));
        }

        public void ChangeUnitOpacity(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            _units[unitInfo.ID].SetOpacity(unitInfo.Opacity);
        }

        public void ChangeCellOpacity(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].SetOpacity(mapCellInfo.Opacity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                null, null, null, null, _camera.Transform);

            for (var x = _leftColumnIntoView; x < _rightColumnIntoView; x++)
            for (var y = _topRowIntoView; y < _bottomRowIntoView; y++)
                _mapItems[x, y].Draw(spriteBatch);

            foreach (var _unit in _units.Values)
                _unit.Draw(spriteBatch);

            spriteBatch.End();
        }

        public bool IsComponentOnPosition(Point point)
        {
            throw new NotImplementedException();
        }

        private MapCellComponent FindMapItemUnderCursor(Point mousePoint)
        {
            var intendedColumn = mousePoint.X / (_hexWidth / 4 * 3);
            var intendedRow = mousePoint.Y / _hexHeight;

            intendedColumn = intendedColumn > 0 ? intendedColumn - 1 : 0;
            intendedRow = intendedRow > 0 ? intendedRow - 1 : 0;

            for (var x = intendedColumn; x < intendedColumn + 2 && x < _columnCount; x++)
            for (var y = intendedRow; y < intendedRow + 2 && y < _rowCount; y++)
                if (_mapItems[x, y].IsComponentOnPosition(mousePoint))
                    return _mapItems[x, y];

            return null;
        }

        private UnitComponent FindUnitItemUnderCursor(Point mousePoint)
        {
            foreach (var _unit in _units.Values)
                if (_unit.IsComponentOnPosition(mousePoint))
                    return _unit;

            return null;
        }

        private void SetItemRangeIntoViewport()
        {
            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            var intendedLeftColumn = (int)-_camera.Pos.X / (_hexWidth / 4 * 3);
            var intendedRightColumn = (int)(-_camera.Pos.X + viewWidth) / (_hexWidth / 4 * 3);

            var intendedTopRow = (int)-_camera.Pos.Y / _hexHeight;
            var intendedBottomRow = (int)(-_camera.Pos.Y + viewHeight) / _hexHeight;

            _leftColumnIntoView = intendedLeftColumn > 0 ? intendedLeftColumn - 1 : 0;
            _rightColumnIntoView = intendedRightColumn >= _columnCount ? _columnCount : intendedRightColumn + 1;

            _topRowIntoView = intendedTopRow > 0 ? intendedTopRow - 1 : 0;
            _bottomRowIntoView = intendedBottomRow >= _rowCount ? _rowCount : intendedBottomRow + 1;
        }
    }
}
