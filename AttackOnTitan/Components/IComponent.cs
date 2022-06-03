using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public interface IComponent
    {
        void Update(GameTime gameTime, MouseState mouseState);
        void SetFont(SpriteFont font, Vector2 origin);
        void Draw(SpriteBatch spriteBatch);
        bool IsComponentOnPosition(Point point);
    }
}
