using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.GameScenes;

namespace AttackOnTitan.GameComponents
{
    public class SimpleButton : IComponent
    {
        private IScene _parent;
        private string _font;
        private string _text;
        private Vector2 _position;
        private Rectangle _rectangle;
        private Color _color;
        private float _opacity = 0.8f;

        public event Action OnClick;

        public SimpleButton(IScene parent, string font, string text,
            Vector2 position, Rectangle clickableArea, Color color)
        {
            _parent = parent;
            _font = font;
            _text = text;
            _rectangle = clickableArea;
            _position = position;
            _color = color;
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
            spriteBatch.DrawString(_parent.Fonts[_font], _text, _position, _color * _opacity);

        public bool IsComponentOnPosition(Point point) =>
            _rectangle.Contains(point);
    }
}
