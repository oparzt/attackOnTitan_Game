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

        public Map(IScene parent, int columnCount, int rowCount, int hexWidth, int hexHeight)
        {
            _mapItems = new MapItem[columnCount, rowCount];
            _camera = new Camera2D(0, 0);
            _columnCount = columnCount;
            _rowCount = rowCount;
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;

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

            Console.WriteLine(_camera.Pos);


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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                null, null, null, null,
                _camera.Transform);
            foreach (var mapItem in _mapItems)
                mapItem.Draw(spriteBatch);
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
    }
}
