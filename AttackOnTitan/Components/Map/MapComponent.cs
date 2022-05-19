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
        private Rectangle[,] _grassRects;
        private Camera2D _camera;

        private int _cellsColumnCount;
        private int _cellsRowCount;

        private int _grassColumnCount;
        private int _grassRowCount;

        public float HexWidth => _hexWidth * _camera.Zoom;
        public float HexHeight => _hexHeight * _camera.Zoom;

        public float GrassWidth => _grassWidth * _camera.Zoom;
        public float GrassHeight => _grassHeight * _camera.Zoom;

        private int _hexWidth;
        private int _hexHeight;

        private int _mapWidth;
        private int _mapHeight;

        private int _grassWidth = 200;
        private int _grassHeight = 200;

        private int _leftCellsBorder;
        private int _rightCellsBorder;
        private int _topCellsBorder;
        private int _bottomCellsBorder;

        private int _leftGrassBorder;
        private int _rightGrassBorder;
        private int _topGrassBorder;
        private int _bottomGrassBorder;

        private Rectangle _unitTextureRect;
        private int _unitHitRadius;

        private Dictionary<int, UnitComponent> _units = new();
        private HashSet<UnitComponent> _movedUnits = new();

        private Dictionary<NoServicedZoneLocation, Rectangle[]> _noServicedZones = new();

        public MapComponent(IScene parent, int cellsColumnCount, int cellsRowCount,
            int hexWidth, int hexHeight, int unitWidth, int unitHeight)
        {
            _mapItems = new MapCellComponent[cellsColumnCount, cellsRowCount];
            _cellsColumnCount = cellsColumnCount;
            _cellsRowCount = cellsRowCount;
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;
            _scene = parent;

            _unitTextureRect = new Rectangle(0, 0, unitWidth, unitHeight);
            _unitHitRadius = unitWidth / 6 * 5 / 2;

            InitializeMap();
            InitializeGrass();
        }

        private void InitializeMap()
        {
            _mapWidth = _cellsColumnCount * _hexWidth / 4 * 3 + _hexWidth / 4;
            _mapHeight = _cellsRowCount * _hexHeight + _hexHeight / 2;

            var viewportWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewportHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            _camera = new Camera2D(0, 0, viewportWidth, viewportHeight, _mapWidth, _mapHeight);

            for (var row = 0; row < _cellsRowCount; row++)
                for (var column = 0; column < _cellsColumnCount; column++)
                {
                    _mapItems[column, row] = new MapCellComponent(_scene, "Hexagon", column, row,
                        new Rectangle(
                            column * _hexWidth / 4 * 3,
                            row * _hexHeight + (column % 2 == 1 ? _hexHeight / 2 : 0),
                            _hexWidth, _hexHeight));
                    _mapItems[column, row].CreatePositionsRectangles(_unitTextureRect.Size);
                }
        }

        private void InitializeGrass()
        {
            _grassColumnCount = _mapWidth / _grassWidth + 2;
            _grassRowCount = _mapHeight / _grassHeight + 2;

            _grassRects = new Rectangle[_grassColumnCount, _grassRowCount];

            for (var x = 0; x < _grassColumnCount; x++)
            for (var y = 0; y < _grassColumnCount; y++)
                _grassRects[x, y] = new Rectangle(x * _grassWidth, y * _grassHeight, _grassWidth, _grassHeight);
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            _camera.Update(gameTime, mouseState);

            var mouseBtn = GetPressedBtn(mouseState.LeftButton, mouseState.RightButton);

            SetCellsRangeIntoViewport();
            SetGrassRangeIntoViewport();

            foreach (var unit in _movedUnits)
                unit.Update(gameTime, mouseState);

            if (_noServicedZones.Values
                .SelectMany(noServicedZone => noServicedZone)
                .Any(noServicedRect => noServicedRect.Contains(mouseState.Position)))
            {
                return;
            }
            
            var selectedMapItem = FindMapItemUnderCursor();
            var selectedUnitItem = FindUnitItemUnderCursor();
            
            if (selectedUnitItem is not null)
                InitiateSelectUnitAction(selectedUnitItem, mouseBtn);

            if (selectedMapItem is not null)
                InitiateSelectMapCellAction(selectedMapItem, mouseBtn);
        }

        private PressedMouseBtn GetPressedBtn(ButtonState left, ButtonState right) =>
            right == ButtonState.Pressed ? PressedMouseBtn.Right :
            left == ButtonState.Pressed ?PressedMouseBtn.Left : PressedMouseBtn.None;

        private void InitiateSelectMapCellAction(MapCellComponent mapItem, PressedMouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.SelectMapCell,
                SelectedCell = new SelectedCell(mapItem.X, mapItem.Y),
                MouseBtn = mouseBtn
            });

        private void InitiateSelectUnitAction(UnitComponent unitItem, PressedMouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.SelectUnit,
                SelectedUnit = new SelectedUnit(unitItem.ID),
                MouseBtn = mouseBtn
            });


        public void AddUnit(UnitInfo unitInfo)
        {
            _units[unitInfo.ID] = new UnitComponent(_scene, unitInfo.ID, unitInfo.TextureName,
                _mapItems[unitInfo.X, unitInfo.Y].GetPosition(unitInfo.Position),
                _unitHitRadius);
        }

        public void MoveUnit(UnitInfo unitInfo)
        {
            if (!_units.TryGetValue(unitInfo.ID, out var unit)) return;
            var targetPosition = _mapItems[unitInfo.X, unitInfo.Y].GetPosition(unitInfo.Position);

            _movedUnits.Add(unit);
            unit.Move(targetPosition);
        }
        
        public void RemoveUnit(UnitInfo unitInfo)
        {
            _units.Remove(unitInfo.ID);
        }

        public void StopUnit(UnitInfo unitInfo)
        {
            _movedUnits.Remove(_units[unitInfo.ID]);
        }

        public void ChangeUnitOpacity(UnitInfo unitInfo)
        {
            if (!_units.TryGetValue(unitInfo.ID, out var unit)) return;
            unit.SetOpacity(unitInfo.Opacity);
        }

        public void ChangeCellOpacity(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].SetOpacity(mapCellInfo.Opacity);
        }

        public void UpdateNoServicedZone(NoServicedZone noServicedZone)
        {
            _noServicedZones[noServicedZone.Location] = noServicedZone.Zones;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                null, null, null, null, _camera.Transform);
            
            for (var x = _leftCellsBorder; x < _rightCellsBorder; x++)
            for (var y = _topCellsBorder; y < _bottomCellsBorder; y++)
                _mapItems[x, y].Draw(spriteBatch);

            foreach (var unit in _units.Values)
                unit.Draw(spriteBatch);
            
            for (var i = _leftGrassBorder; i < _rightGrassBorder; i++)
            for (var n = _topGrassBorder; n < _bottomGrassBorder; n++)
                spriteBatch.Draw(_scene.Textures["Grass"], _grassRects[i, n], 
                    null, Color.White, 0, Vector2.Zero, 
                    SpriteEffects.None, 0);

            spriteBatch.End();
        }

        private MapCellComponent FindMapItemUnderCursor()
        {
            var intendedColumn = _camera.MousePoint.X / (_hexWidth / 4 * 3);
            var intendedRow = _camera.MousePoint.Y / _hexHeight;
            
            intendedColumn = intendedColumn > 0 ? intendedColumn - 1 : 0;
            intendedRow = intendedRow > 0 ? intendedRow - 1 : 0;

            for (var x = intendedColumn; x < intendedColumn + 2 && x < _cellsColumnCount; x++)
            for (var y = intendedRow; y < intendedRow + 2 && y < _cellsRowCount; y++)
                if (_mapItems[x, y].IsComponentOnPosition(_camera.MousePoint))
                    return _mapItems[x, y];

            return null;
        }

        private UnitComponent FindUnitItemUnderCursor() =>
            _units.Values
                .FirstOrDefault(unit => unit.IsComponentOnPosition(_camera.MousePoint));

        private void SetCellsRangeIntoViewport()
        {
            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            var intendedLeftColumn = (int)(-_camera.Pos.X / (HexWidth / 4 * 3));
            var intendedRightColumn = (int)((-_camera.Pos.X + viewWidth) / (HexWidth / 4 * 3));
            
            var intendedTopRow = (int)(-_camera.Pos.Y / HexHeight);
            var intendedBottomRow = (int)((-_camera.Pos.Y + viewHeight) / HexHeight);
            
            _leftCellsBorder = intendedLeftColumn > 0 ? intendedLeftColumn - 1 : 0;
            _rightCellsBorder = intendedRightColumn >= _cellsColumnCount ? _cellsColumnCount : intendedRightColumn + 1;
            
            _topCellsBorder = intendedTopRow > 0 ? intendedTopRow - 1 : 0;
            _bottomCellsBorder = intendedBottomRow >= _cellsRowCount ? _cellsRowCount : intendedBottomRow + 1;
        }
        
        private void SetGrassRangeIntoViewport()
        {
            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            var intendedLeftColumn = (int)(-_camera.Pos.X / GrassWidth);
            var intendedRightColumn = (int)((-_camera.Pos.X + viewWidth) / GrassWidth);
            
            var intendedTopRow = (int)(-_camera.Pos.Y / GrassHeight);
            var intendedBottomRow = (int)((-_camera.Pos.Y + viewHeight) / GrassHeight);
            
            _leftGrassBorder = intendedLeftColumn > 0 ? intendedLeftColumn - 1 : 0;
            _rightGrassBorder = intendedRightColumn >= _grassColumnCount ? _grassColumnCount : intendedRightColumn + 1;
            
            _topGrassBorder = intendedTopRow > 0 ? intendedTopRow - 1 : 0;
            _bottomGrassBorder = intendedBottomRow >= _grassRowCount ? _grassRowCount : intendedBottomRow + 1;
        }
    }
}
