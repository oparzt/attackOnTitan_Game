using System;
using System.Collections.Generic;
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
        private MapCellComponent[,] _mapItems;
        private Rectangle[,] _grassRects;
        private Camera2D _camera;

        private int _cellsColumnCount;
        private int _cellsRowCount;

        private int _grassColumnCount;
        private int _grassRowCount;

        private float HexWidth => _hexWidth * _camera.Zoom;
        private float HexHeight => _hexHeight * _camera.Zoom;

        private float GrassWidth => _grassWidth * _camera.Zoom;
        private float GrassHeight => _grassHeight * _camera.Zoom;

        private readonly int _hexWidth;
        private readonly int _hexHeight;

        private readonly int _grassWidth = 200;
        private readonly int _grassHeight = 200;

        private Rectangle _cellsInViewport;
        private Rectangle _grassInViewport;
        
        private Rectangle _unitTextureRect;
        private readonly int _unitHitRadius;

        private readonly Dictionary<int, UnitComponent> _units = new();
        private readonly HashSet<UnitComponent> _movedUnits = new();

        private readonly Dictionary<NoServicedZoneLocation, Rectangle[]> _noServicedZones = new();

        private MouseBtn _lastMouseBtn;
        private bool _unitWasPressed;
        private bool _mapCellWasPressed;

        public MapComponent(int hexWidth, int hexHeight, int unitWidth, int unitHeight)
        {
            _hexWidth = hexWidth;
            _hexHeight = hexHeight;

            _unitTextureRect = new Rectangle(0, 0, unitWidth, unitHeight);
            _unitHitRadius = unitWidth / 6 * 5 / 2;
        }

        #region InitializationMethods

        private void InitializeMap(string defaultTextureName)
        {
            var mapWidth = _cellsColumnCount * _hexWidth / 4 * 3 + _hexWidth / 4;
            var mapHeight = _cellsRowCount * _hexHeight + _hexHeight / 2;

            var viewportWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewportHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            _camera = new Camera2D(0, 0, viewportWidth, viewportHeight, mapWidth, mapHeight);
            _mapItems = new MapCellComponent[_cellsColumnCount, _cellsRowCount];
            
            for (var row = 0; row < _cellsRowCount; row++)
                for (var column = 0; column < _cellsColumnCount; column++)
                {
                    _mapItems[column, row] = new MapCellComponent(column, row, SceneManager.Textures[defaultTextureName],
                        new Rectangle(
                            column * _hexWidth / 4 * 3,
                            row * _hexHeight + (column % 2 == 1 ? _hexHeight / 2 : 0) + _hexHeight / 4,
                            _hexWidth, _hexHeight));
                    _mapItems[column, row].CreatePositionsRectangles(_unitTextureRect.Size);
                }
            
            InitializeGrass(mapWidth, mapHeight);
        }

        private void InitializeGrass(int mapWidth, int mapHeight)
        {
            _grassColumnCount = mapWidth / _grassWidth + 2;
            _grassRowCount = mapHeight / _grassHeight + 2;

            _grassRects = new Rectangle[_grassColumnCount, _grassRowCount];

            for (var x = 0; x < _grassColumnCount; x++)
            for (var y = 0; y < _grassRowCount; y++)
                _grassRects[x, y] = new Rectangle(x * _grassWidth, y * _grassHeight, _grassWidth, _grassHeight);
        }
        
        #endregion

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            foreach (var unit in _movedUnits)
                unit.Update(gameTime, mouseState);
            
            if (_noServicedZones.Values
                .SelectMany(noServicedZone => noServicedZone)
                .Any(noServicedRect => noServicedRect.Contains(mouseState.Position)))
                return;
            
            _camera.Update(gameTime, mouseState);

            var mouseBtn = mouseState.GetPressedMouseBtn();

            if (_camera.MatrixWasUpdated)
            {
                _cellsInViewport = GetRangeIntoViewport(HexWidth * 0.75f, HexHeight, _cellsColumnCount, _cellsRowCount);
                _grassInViewport = GetRangeIntoViewport(GrassWidth, GrassHeight, _grassColumnCount, _grassRowCount); 
            }

            if (_camera.MatrixWasUpdated)
            {
                _unitWasPressed = false;
                _mapCellWasPressed = false;
                _lastMouseBtn = MouseBtn.None;
                _camera.MatrixWasUpdated = false;
                return;
            }
            
            
            var selectedMapItem = FindMapItemUnderCursor();
            var selectedUnitItem = FindUnitItemUnderCursor();

            if (selectedUnitItem is not null)
                HandleMouseOnUnit(selectedUnitItem, mouseBtn);
            else
                InitiateUnselectUnitAction();

            if (selectedMapItem is not null)
                HandleMouseBtnClickOnMapCell(selectedMapItem, mouseBtn);

            if (mouseBtn == MouseBtn.None) _lastMouseBtn = MouseBtn.None;
        }

        private void HandleMouseOnUnit(UnitComponent unitItem, MouseBtn mouseBtn)
        {
            if (_unitWasPressed)
            {
                if ((mouseBtn == MouseBtn.None && _lastMouseBtn != MouseBtn.None) ||
                    (mouseBtn != MouseBtn.None && _lastMouseBtn != mouseBtn))
                {
                    InitiateSelectUnitAction(unitItem, _lastMouseBtn);
                    _unitWasPressed = false;
                }
            }
            else
            {
                if (mouseBtn == MouseBtn.None)
                    InitiateSelectUnitAction(unitItem, mouseBtn);
                else
                {
                    _unitWasPressed = true;
                    _lastMouseBtn = mouseBtn;
                }
            }
        }

        private void HandleMouseBtnClickOnMapCell(MapCellComponent mapItem, MouseBtn mouseBtn)
        {
            if (_mapCellWasPressed)
            {
                if ((mouseBtn == MouseBtn.None && _lastMouseBtn != MouseBtn.None) ||
                    (mouseBtn != MouseBtn.None && _lastMouseBtn != mouseBtn))
                {
                    InitiateSelectMapCellAction(mapItem, _lastMouseBtn);
                    _mapCellWasPressed = false;
                }
            }
            else
            {
                if (mouseBtn != MouseBtn.Left)
                    InitiateSelectMapCellAction(mapItem, mouseBtn);
                else
                {
                    _mapCellWasPressed = true;
                    _lastMouseBtn = mouseBtn;
                }
            }
        }

        private void InitiateSelectMapCellAction(MapCellComponent mapItem, MouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.SelectMapCell,
                InputCellInfo = new InputCellInfo(mapItem.X, mapItem.Y),
                MouseBtn = mouseBtn
            });

        private void InitiateSelectUnitAction(UnitComponent unitItem, MouseBtn mouseBtn) =>
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.SelectUnit,
                InputUnitInfo = new InputUnitInfo(unitItem.ID),
                MouseBtn = mouseBtn
            });
        
        private void InitiateUnselectUnitAction() =>
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UnselectUnit
            });
        

        #region OutputActionHandlers

        public void InitializeMap(MapCellInfo mapCellInfo)
        {
            _cellsColumnCount = mapCellInfo.X;
            _cellsRowCount = mapCellInfo.Y;
            InitializeMap(mapCellInfo.TextureName);
        }
        
        public void AddUnit(UnitInfo unitInfo)
        {
            _units[unitInfo.ID] = new UnitComponent(unitInfo.ID, unitInfo.TextureName,
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
            var unit = _movedUnits.FirstOrDefault(movedUnit => unitInfo.ID == movedUnit.ID);
            if (unit is not null)
                _movedUnits.Remove(unit);
        }

        public void ChangeUnitOpacity(UnitInfo unitInfo)
        {
            if (!_units.TryGetValue(unitInfo.ID, out var unit)) return;
            unit.SetOpacity(unitInfo.Opacity);
        }

        public void ChangeTextureIntoCell(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].UpdateHouseTexture(SceneManager.Textures[mapCellInfo.TextureName]);
        }

        public void ChangeTextureOverCell(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].UpdateHexagonTexture(SceneManager.Textures[mapCellInfo.TextureName]);
        }

        public void ClearTextureIntoCell(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].ClearHouseTexture();
        }

        public void ChangeCellOpacity(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].SetOpacity(mapCellInfo.Opacity);
        }
        
        public void SetCellHidden(MapCellInfo mapCellInfo)
        {
            _mapItems[mapCellInfo.X, mapCellInfo.Y].SetHidden();
        }

        public void UpdateNoServicedZone(NoServicedZone noServicedZone)
        {
            _noServicedZones[noServicedZone.Location] = noServicedZone.Zones;
        }
        
        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                null, null, null, null, _camera.Transform);
            
            for (var x = _cellsInViewport.Left; x < _cellsInViewport.Right; x++)
            for (var y = _cellsInViewport.Top; y < _cellsInViewport.Bottom; y++)
                _mapItems[x, y].Draw(spriteBatch);

            foreach (var unit in _units.Values)
                unit.Draw(spriteBatch);  
            
            // spriteBatch.Draw(SceneManager.Textures["Wall"], new Rectangle(0, 0, 3840, 2160), 
            //     null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);            
            spriteBatch.Draw(SceneManager.Textures["Wall"], new Rectangle(0, 0, 3930, 3240), 
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            
            for (var i = _grassInViewport.Left; i < _grassInViewport.Right; i++)
            for (var n = _grassInViewport.Top; n < _grassInViewport.Bottom; n++)
                spriteBatch.Draw(SceneManager.Textures["Grass"], _grassRects[i, n], 
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

        private Rectangle GetRangeIntoViewport(float width, float height, int columnCount, int rowCount)
        {
            var viewWidth = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width;
            var viewHeight = SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height;

            var intendedLeftColumn = (int)(-_camera.Pos.X / width);
            var intendedRightColumn = (int)((-_camera.Pos.X + viewWidth) / width);
            
            var intendedTopRow = (int)(-_camera.Pos.Y / height);
            var intendedBottomRow = (int)((-_camera.Pos.Y + viewHeight) / height);
            
            var leftBorder = intendedLeftColumn > 0 ? intendedLeftColumn - 1 : 0;
            var rightBorder = intendedRightColumn >= columnCount ? columnCount : intendedRightColumn + 1;
            
            var topBorder = intendedTopRow > 0 ? intendedTopRow - 1 : 0;
            var bottomBorder = intendedBottomRow >= rowCount ? rowCount : intendedBottomRow + 1;

            return new Rectangle(leftBorder, topBorder, 
                rightBorder - leftBorder,
                bottomBorder - topBorder);
        }
    }
}
