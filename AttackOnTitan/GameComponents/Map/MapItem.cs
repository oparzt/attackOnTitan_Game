using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.GameScenes;

namespace AttackOnTitan.GameComponents
{
    public class MapItem : IComponent
    {
        private bool _isVisible = true;
        private Rectangle _destRect;
        private float _opacity = 0.3f;
        private IScene _scene;
        private string _textureName;

        public MapItem(IScene scene, string textureName, int x, int y, Rectangle destRect)
        {
            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (IsComponentOnPosition(new Point(mouseState.X, mouseState.Y)))
                _opacity = 1f;
            else
                _opacity = 0.3f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isVisible)
                spriteBatch.Draw(_scene.Textures[_textureName], _destRect, Color.White * _opacity);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Center).ToVector2().Length();
            return dist <= _destRect.Height / 2;
        }
    }
}
