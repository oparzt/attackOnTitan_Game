using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;

namespace AttackOnTitan.Components
{
    public class SimpleButton : IComponent
    {
        private readonly string _text;
        private readonly Vector2 _position;
        private readonly Color _color;

        private SpriteFont _font;
        private Vector2 _origin;
        private Rectangle _clickableArea;
        private float _opacity = 0.8f;

        public event Action OnClick;

        public SimpleButton(string text, Vector2 position, Color color)
        {
            _text = text;
            _position = position;
            _color = color;
        }

        public void SetFont(SpriteFont font, Vector2 origin)
        {
            var measureSize = font.MeasureString(_text);
            var curPos = _position - origin;

            _font = font;
            _origin = origin;
            _clickableArea = new Rectangle(curPos.ToPoint(), measureSize.ToPoint());
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (IsComponentOnPosition(new Point(mouseState.X, mouseState.Y)))
            {
                _opacity = 1f;
                if (mouseState.LeftButton == ButtonState.Pressed && OnClick is not null)
                    OnClick();
            } else
                _opacity = 0.8f;
        }

        public void Draw(SpriteBatch spriteBatch) =>
            spriteBatch.DrawString(_font, _text, _position, _color * _opacity,
                0f, _origin, 1f, SpriteEffects.None, 1);

        public bool IsComponentOnPosition(Point point) =>
            _clickableArea.Contains(point);
    }
}
