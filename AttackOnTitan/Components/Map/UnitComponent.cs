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
        private float _opacity = 0.65f;
        private IScene _scene;
        private string _textureName;

        // Сколько пикселей пройдет за одну миллисекунду;
        private float _speed = 0.25f;

        // Сколько пикселей пройдет за одну миллисекунду по осям;
        private float _speedX = 0f;
        private float _speedY = 0f;

        private Queue<MapCellComponent> _targetCells = new();
        private MapCellComponent _targetCell;
        private Point _targetPoint;

        private float _curX = 0f;
        private float _curY = 0f;

        private bool _isMove;

        public UnitComponent(IScene scene, int id, string textureName, Rectangle destRect)
        {
            ID = id;

            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;
        }

        public void Update(GameTime gameTime, MouseState mouseState) {
            var elapsed = gameTime.ElapsedGameTime.Milliseconds;

            if (!_isMove)
            {
                if (_targetCells.TryDequeue(out var targetCell))
                {
                    _targetCell = targetCell;
                    _targetPoint = targetCell.GetCenter() - new Point(_destRect.Width / 2, _destRect.Height / 2);

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
                    GameModel.InputActions.Enqueue(new(InputActionType.UnitStopMove, Keys.None, PressedMouseBtn.None,
                        null, new(ID)));
                    return;
                };
            }


            _curX += _speedX * elapsed;
            _curY += _speedY * elapsed;
            var newLocation = new Point((int)_curX, (int)_curY);
            var diff = _targetPoint - newLocation;

            if (Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y) <= 2)
            {
                _destRect.Location = _targetPoint;
                _isMove = false;
            } else
            {
                _destRect.Location = newLocation;
            }
        }

        public void Move(MapCellComponent mapCell)
        {
            _targetCells.Enqueue(mapCell);
        }

        public void SetOpacity(float opacity)
        {
            _opacity = opacity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scene.Textures[_textureName], _destRect, null, Color.White * _opacity, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Center).ToVector2().Length();
            return dist <= _destRect.Height / 2;
        }
    }
}
