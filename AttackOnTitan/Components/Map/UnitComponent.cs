using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;
using AttackOnTitan.Models;

namespace AttackOnTitan.Components
{
    public class UnitComponent
    {
        public readonly int ID;

        private Rectangle _destRect;
        private int _hitRadius;
        private float _opacity = 0.65f;
        private IScene _scene;
        private string _textureName;

        // Сколько пикселей пройдет за одну миллисекунду;
        private float _speed = 0.25f;

        // Сколько пикселей пройдет за одну миллисекунду по осям;
        private float _speedX;
        private float _speedY;

        private Queue<MapCellComponent> _targetCells = new();
        private Queue<Rectangle> _targetPositions = new();
        private Point _targetPoint;

        private float _curX;
        private float _curY;

        private bool _isMove;

        public UnitComponent(IScene scene, int id, string textureName, Rectangle destRect, int hitRadius)
        {
            ID = id;

            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;

            _hitRadius = hitRadius;
        }

        public void Update(GameTime gameTime, MouseState mouseState) {
            var elapsed = gameTime.ElapsedGameTime.Milliseconds;

            if (!_isMove)
            {
                if (_targetPositions.TryDequeue(out var targetPosition))
                {
                    _targetPoint = targetPosition.Location;
                    
                    _curX = _destRect.Location.X;
                    _curY = _destRect.Location.Y;
                    
                    var diffLengthInPoint = _targetPoint - _destRect.Location;
                    var diffLength = Math.Sqrt(diffLengthInPoint.X * diffLengthInPoint.X + diffLengthInPoint.Y * diffLengthInPoint.Y);
                    var time = diffLength / _speed;

                    _speedX = time != 0 ? (float)(diffLengthInPoint.X / time) : 0;
                    _speedY = time != 0 ? (float)(diffLengthInPoint.Y / time) : 0;

                    _isMove = true;
                }
                else
                {
                    GameModel.InputActions.Enqueue(new InputAction
                    {
                        ActionType = InputActionType.UnitStopMove,
                        SelectedUnit = new SelectedUnit(ID)
                    });
                    return;
                };
            }

            _curX += _speedX * elapsed;
            _curY += _speedY * elapsed;
            var newLocation = new Point((int)_curX, (int)_curY);
            var diff = _targetPoint - newLocation;

            if (Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y) <= 5)
            {
                _destRect.Location = _targetPoint;
                _isMove = false;
            } else
            {
                _destRect.Location = newLocation;
            }
        }

        public void Move(Rectangle position) =>
            _targetPositions.Enqueue(position);

        public void SetOpacity(float opacity) =>
            _opacity = opacity;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scene.Textures[_textureName], _destRect, null, 
                Color.White * _opacity, 0f, Vector2.Zero, 
                SpriteEffects.None, 1f);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Center).ToVector2().Length();
            return dist <= _hitRadius;
        }
    }
}
