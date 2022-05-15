using System;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class StepBtnComponent
    {
        private readonly IScene _scene;
        
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private readonly float _fontScale;

        private const string EndText = "Конец хода";
        private const string WaitText = "Подождите";
        private readonly Rectangle _backgroundRect;
        private readonly Vector2 _textPosition;

        private bool _endState = true;
        private bool _wasPressed = false;
        
        public StepBtnComponent(IScene parent, int viewportWidth, int viewportHeight, int font)
        {
            _scene = parent;
            _fontScale = font / 100f;

            _backgroundRect = new Rectangle(viewportWidth - 250, viewportHeight - 40, 
                250, 40);
            _textPosition = new Vector2(viewportWidth - 250 + 70, viewportHeight - 40 + 8);
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var contains = _backgroundRect.Contains(mouseState.Position);
            var pressed = mouseState.LeftButton == ButtonState.Pressed;
            
            if (_wasPressed)
            {
                if (contains)
                {
                    if (!pressed)
                    {
                        _wasPressed = false;
                        GameModel.InputActions.Enqueue(new InputAction()
                        {
                            ActionType = InputActionType.StepBtnPressed
                        });
                    }
                }
                else
                    _wasPressed = false;
            }
            else
                _wasPressed = pressed && contains;
        }

        public void SetFont(SpriteFont font) => _font = font;
        public void SetBackgroundTexture(Texture2D texture) => _backgroundTexture = texture;
        public void ChangeState()
        {
            Console.WriteLine("Было");
            _endState = !_endState;
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _backgroundRect, Color.White);
            spriteBatch.DrawString(_font, _endState ? EndText : WaitText, 
                _textPosition, Color.White, 0, Vector2.Zero, 
                _fontScale, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}