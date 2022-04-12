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
        private Queue<MapItem> _updateQueue = new();

        private int _columnCount;
        private int _rowCount;

        private int _hexWidth;
        private int _hexHeight;

        public Map(IScene parent, int columnCount, int rowCount, int hexWidth, int hexHeight)
        {
            _mapItems = new MapItem[columnCount, rowCount];
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
            while(_updateQueue.TryDequeue(out var mapItem))
                mapItem.Update(gameTime, mouseState);

            UpdateMapItemsUnderCursor(gameTime, mouseState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var mapItem in _mapItems)
                mapItem.Draw(spriteBatch);
            spriteBatch.End();
        }

        public bool IsComponentOnPosition(Point point)
        {
            throw new NotImplementedException();
        }

        private void UpdateMapItemsUnderCursor(GameTime gameTime, MouseState mouseState)
        {
            var intendedColumn = mouseState.X / (_hexWidth / 4 * 3);
            var intendedRow = mouseState.Y / _hexHeight;

            Console.WriteLine($"{intendedColumn}:{intendedRow}");

            for (var x = intendedColumn - 1; x < intendedColumn + 1; x++)
            for (var y = intendedRow - 1; y < intendedRow + 1; y++)
            {
                if (x >= 0 && x < _columnCount && y >= 0 && y < _rowCount)
                {
                    _mapItems[x, y].Update(gameTime, mouseState);
                    _updateQueue.Enqueue(_mapItems[x, y]);
                }
             }
        }
    }
}
