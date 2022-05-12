using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;
using AttackOnTitan.Models;

namespace AttackOnTitan.Components
{
    public class MapComponent
    {
        private IScene _scene;

        private MapCellComponent[,] _mapItems;
        private Camera2D _camera;

        private int _columnCount;
        private int _rowCount;

        public float HexWidth => _hexWidth * _camera.Zoom;
        public float HexHeight => _hexHeight * _camera.Zoom;

        private int _hexWidth;
        private int _hexHeight;

        private int _leftColumnIntoView;
        private int _rightColumnIntoView;
        private int _topRowIntoView;
        private int _bottomRowIntoView;

        private Rectangle _unitTextureRect;
        private int _unitHitRadius;

        private Dictionary<int, UnitComponent> _units = new();
        private HashSet<UnitComponent> _movedUnits = new();

        public MapComponent(IScene parent, int columnCount, int rowCount,
            int hexWidth, int hexHeight, int unitWidth, int unitHeight)
        {
            _mapItems = new MapCellComponent[columnCount, rowCount];
            _columnCount = columnCount;
            _rowCount = rowCount;
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;
            _scene = parent;

            _unitTextureRect = new Rectangle(0, 0, unitWidth, unitHeight);
            _unitHitRadius = unitWidth / 6 * 5 / 2;

            InitializeMap();
        }

        private void InitializeMap()
        {
            var mapWidth = _columnCount * _hexWidth / 4 * 3 + _hexWidth / 4;
            var mapHeight = _rowCount * _hexHeight + _hexHeight / 2;

            var viewportWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewportHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            _camera = new Camera2D(0, 0, viewportWidth, viewportHeight, mapWidth, mapHeight);

            for (var row = 0; row < _rowCount; row++)
                for (var column = 0; column < _columnCount; column++)
                {
                    _mapItems[column, row] = new MapCellComponent(_scene, "Hexagon", column, row,
                        new Rectangle(
                            column * _hexWidth / 4 * 3,
                            row * _hexHeight + (column % 2 == 1 ? _hexHeight / 2 : 0),
                            _hexWidth, _hexHeight));
                    _mapItems[column, row].CreatePositionsRectangles(_unitTextureRect.Size);
                }
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            _camera.Update(gameTime, mouseState);

            var mouseBtn = GetPressedBtn(mouseState.LeftButton, mouseState.RightButton);

            var selectedMapItem = FindMapItemUnderCursor();
            var selectedUnitItem = FindUnitItemUnderCursor();

            if (selectedMapItem is not null)
                InitiateSelectMapCellAction(selectedMapItem, mouseBtn);

            if (selectedUnitItem is not null)
                InitiateSelectUnitAction(selectedUnitItem, mouseBtn);

            SetItemRangeIntoViewport();

            foreach (var unit in _movedUnits)
                unit.Update(gameTime, mouseState);
        }

        private PressedMouseBtn GetPressedBtn(ButtonState left, ButtonState right) =>
            right == ButtonState.Pressed ? PressedMouseBtn.Right :
            left == ButtonState.Pressed ?PressedMouseBtn.Left : PressedMouseBtn.None;

        private void InitiateSelectMapCellAction(MapCellComponent mapItem, PressedMouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction(new SelectedCell(mapItem.X, mapItem.Y), mouseBtn));

        private void InitiateSelectUnitAction(UnitComponent unitItem, PressedMouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction(new SelectedUnit(unitItem.ID), mouseBtn));


        public void AddUnit(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            _units[unitInfo.ID] = new UnitComponent(_scene, unitInfo.ID, unitInfo.TextureName,
                _mapItems[unitInfo.X, unitInfo.Y].GetPosition(unitInfo.Position),
                _unitHitRadius);
        }

        public void MoveUnit(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            var unit = _units[unitInfo.ID];
            var targetCell = _mapItems[unitInfo.X, unitInfo.Y];

            _movedUnits.Add(unit);
            unit.Move(targetCell);
        }

        public void StopUnit(UnitInfo unitInfo, MapCellInfo mapCellInfo)
        {
            _movedUnits.Remove(_units[unitInfo.ID]);
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

            foreach (var unit in _units.Values)
                unit.Draw(spriteBatch);

            spriteBatch.End();
        }

        private MapCellComponent FindMapItemUnderCursor()
        {
            var intendedColumn = _camera.MousePoint.X / (_hexWidth / 4 * 3);
            var intendedRow = _camera.MousePoint.Y / _hexHeight;
            
            intendedColumn = intendedColumn > 0 ? intendedColumn - 1 : 0;
            intendedRow = intendedRow > 0 ? intendedRow - 1 : 0;

            for (var x = intendedColumn; x < intendedColumn + 2 && x < _columnCount; x++)
            for (var y = intendedRow; y < intendedRow + 2 && y < _rowCount; y++)
                if (_mapItems[x, y].IsComponentOnPosition(_camera.MousePoint))
                    return _mapItems[x, y];

            return null;
        }

        private UnitComponent FindUnitItemUnderCursor() =>
            _units.Values
                .FirstOrDefault(unit => unit.IsComponentOnPosition(_camera.MousePoint));

        private void SetItemRangeIntoViewport()
        {
            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            var intendedLeftColumn = (int)(-_camera.Pos.X / (HexWidth / 4 * 3));
            var intendedRightColumn = (int)((-_camera.Pos.X + viewWidth) / (HexWidth / 4 * 3));
            
            var intendedTopRow = (int)(-_camera.Pos.Y / HexHeight);
            var intendedBottomRow = (int)((-_camera.Pos.Y + viewHeight) / HexHeight);
            
            _leftColumnIntoView = intendedLeftColumn > 0 ? intendedLeftColumn - 1 : 0;
            _rightColumnIntoView = intendedRightColumn >= _columnCount ? _columnCount : intendedRightColumn + 1;
            
            _topRowIntoView = intendedTopRow > 0 ? intendedTopRow - 1 : 0;
            _bottomRowIntoView = intendedBottomRow >= _rowCount ? _rowCount : intendedBottomRow + 1;
        }
    }
}
