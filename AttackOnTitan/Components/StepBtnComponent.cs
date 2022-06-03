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
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private int _fontSize;
        private Vector2 _textOrigin;

        private const string EndText = "Конец хода";
        private const string WaitText = "Подождите";
        private string _curText = EndText;
        private Vector2 _textPosition;
        private Rectangle _backgroundRect;

        private bool _endState = true;
        private bool _wasPressed = false;
        
        public StepBtnComponent(int viewportWidth, int viewportHeight)
        {
            _backgroundRect = new Rectangle(viewportWidth - 250, viewportHeight - 40, 
                250, 40);
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.StepBtn)
                {
                    Zones = new [] {_backgroundRect}
                }
            });
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
                        if (_endState)
                        {
                            GameModel.InputActions.Enqueue(new InputAction()
                            {
                                ActionType = InputActionType.StepBtnPressed
                            });
                        }
                    }
                }
                else
                    _wasPressed = false;
            }
            else
                _wasPressed = pressed && contains;
        }

        public void SetFont(SpriteFont font, int fontSize, Vector2 origin)
        {
            _font = font;
            _fontSize = fontSize;
            _textOrigin = origin;
            UpdateTextPos();
        }
        public void SetBackgroundTexture(Texture2D texture) => _backgroundTexture = texture;
        public void ChangeState()
        {
            _endState = !_endState;
            _curText = _endState ? EndText : WaitText;
            UpdateTextPos();
        }

        private void UpdateTextPos()
        {
            var measuredSize = _font.MeasureString(_curText);
            var diffX = (_backgroundRect.Width - measuredSize.X) / 2;
            var diffY = (_backgroundRect.Height - _fontSize) / 2;
            _textPosition = new Vector2(_backgroundRect.Left + diffX, _backgroundRect.Top + diffY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _backgroundRect, Color.White);
            spriteBatch.DrawString(_font, _curText, 
                _textPosition, Color.White, 0, _textOrigin, 
                1, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}