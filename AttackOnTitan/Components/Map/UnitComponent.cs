using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;

namespace AttackOnTitan.Components
{
    public class UnitComponent : IComponent
    {
        public readonly int ID;

        private Rectangle _destRect;
        private float _opacity = 0.65f;
        private IScene _scene;
        private string _textureName;
        private Vector2 _origin;

        public UnitComponent(IScene scene, int id, string textureName, Rectangle destRect)
        {
            ID = id;


            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;
            _origin = new Vector2(destRect.Width, destRect.Height);
        }

        public void Update(GameTime gameTime, MouseState mouseState) {}

        public void Move(Rectangle destRect)
        {
            _destRect = destRect;
        }

        public void SetOpacity(float opacity)
        {
            _opacity = opacity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scene.Textures[_textureName], _destRect, null, Color.White * _opacity, 0f, _origin, SpriteEffects.None, 1f);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Location).ToVector2().Length();
            return dist <= _destRect.Height / 2;
        }
    }
}
