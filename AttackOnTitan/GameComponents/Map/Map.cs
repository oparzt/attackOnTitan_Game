using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.GameScenes;

namespace AttackOnTitan.GameComponents
{
    public class Map : IComponent
    {
        private MapItem[,] _mapItems;
        private Camera2D _camera;
        private Queue<MapItem> _selected = new();

        private int _columnCount;
        private int _rowCount;

        private int _hexWidth;
        private int _hexHeight;

        private int _leftColumnIntoView;
        private int _rightColumnIntoView;
        private int _topRowIntoView;
        private int _bottomRowIntoView;

        public Map(IScene parent, int columnCount, int rowCount, int hexWidth, int hexHeight)
        {
            _mapItems = new MapItem[columnCount, rowCount];
            _columnCount = columnCount;
            _rowCount = rowCount;
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;

            var mapWidth = columnCount * hexWidth / 4 * 3  + hexWidth / 4;
            var mapHeight = rowCount * hexHeight + hexHeight / 2;

            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            _camera = new Camera2D(0, 0, mapWidth - viewWidth, mapHeight - viewHeight);

            for (var row = 0; row < rowCount; row++)
            for (var column = 0; column < columnCount; column++)
            {
                _mapItems[column, row] = new MapItem(parent, "Hexagon", column, row,
                    new Rectangle(
                        column * hexWidth / 4 * 3,
                        row * hexHeight + (column % 2 == 1 ? hexHeight / 2 : 0),
                        hexWidth, hexHeight));
            }

        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            _camera.Update(gameTime, mouseState);

            var mousePos = new Point(mouseState.X, mouseState.Y) - new Point((int)_camera.Pos.X, (int)_camera.Pos.Y);

            var selectedItem = FindItemUnderCursor(mousePos);

            if (mouseState.RightButton == ButtonState.Released)
            {
                while (_selected.TryDequeue(out var mapItem))
                    mapItem.SetSelected(false);
                if (selectedItem is not null)
                {
                    selectedItem.SetSelected(true);
                    _selected.Enqueue(selectedItem);
                }
            }

            SetItemRangeIntoViewport();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                null, null, null, null, _camera.Transform);
            for (var x = _leftColumnIntoView; x < _rightColumnIntoView; x++)
            for (var y = _topRowIntoView; y < _bottomRowIntoView; y++)
                _mapItems[x, y].Draw(spriteBatch);
            spriteBatch.End();
        }

        public bool IsComponentOnPosition(Point point)
        {
            throw new NotImplementedException();
        }

        private MapItem FindItemUnderCursor(Point MousePoint)
        {
            var intendedColumn = MousePoint.X / (_hexWidth / 4 * 3);
            var intendedRow = MousePoint.Y / _hexHeight;
            var point = new Point(MousePoint.X, MousePoint.Y);

            intendedColumn = intendedColumn > 0 ? intendedColumn - 1 : 0;
            intendedRow = intendedRow > 0 ? intendedRow - 1 : 0;

            for (var x = intendedColumn; x < intendedColumn + 2 && x < _columnCount; x++)
            for (var y = intendedRow; y < intendedRow + 2 && y < _rowCount; y++)
                if (_mapItems[x, y].IsComponentOnPosition(point))
                    return _mapItems[x, y];

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
