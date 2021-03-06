using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AttackOnTitan.Components
{
    public class TopBarResourceComponent
    {
        private readonly Texture2D _texture;
        private readonly SpriteFont _font;
        private readonly Vector2 _textOrigin;

        private Rectangle _textureRect;
        private Vector2 _textPosition;
        
        private string _resourceCount = "100";

        public TopBarResourceComponent(Texture2D texture, SpriteFont font, Vector2 textOrigin)
        {
            _texture = texture;
            _font = font;
            _textOrigin = textOrigin;
        }

        public void UpdateResourceCount(string resourceCount) => _resourceCount = resourceCount;
        public void UpdateTextureRect(Rectangle rectangle) => _textureRect = rectangle;
        public void UpdateTextPosition(Vector2 position) => _textPosition = position;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _textureRect, null,
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            
            spriteBatch.DrawString(_font, _resourceCount, _textPosition, 
                Color.White, 0, _textOrigin, 1, SpriteEffects.None, 1);
        } 
    }
}