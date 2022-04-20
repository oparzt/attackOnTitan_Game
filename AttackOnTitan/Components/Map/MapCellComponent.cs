using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;

namespace AttackOnTitan.Components
{
    public class MapCellComponent : IComponent
    {
        public readonly int X;
        public readonly int Y;

        private Rectangle _destRect;
        private float _opacity = 0.3f;
        private IScene _scene;
        private string _textureName;

        public MapCellComponent(IScene scene, string textureName, int x, int y, Rectangle destRect)
        {
            X = x;
            Y = y;

            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;
        }

        public void Update(GameTime gameTime, MouseState mouseState) {}

        public void SetOpacity(float opacity) => _opacity = opacity;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scene.Textures[_textureName], _destRect, Color.White * _opacity);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Center).ToVector2().Length();
            return dist <= _destRect.Height / 2;
        }

        public Point GetCenter()
        {
            return _destRect.Center;
        }
    }
}
