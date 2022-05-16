using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CommandBarItemComponent
    {
        private readonly Texture2D _availableTexture;
        private readonly CommandType _commandType;
        private Rectangle _textureRect;
        private bool _wasPressed;
        private bool _isAvailable;
        private readonly Texture2D _notAvailableTexture;

        public CommandBarItemComponent(CommandType commandType, Texture2D availableTexture, 
            Texture2D notAvailableTexture)
        {
            _commandType = commandType;
            _availableTexture = availableTexture;
            _notAvailableTexture = notAvailableTexture;
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var contains = _textureRect.Contains(mouseState.Position);
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

        public void UpdateTextureRect(Rectangle textureRect) => _textureRect = textureRect;
        public void UpdateCommandState(bool isAvailable) => _isAvailable = isAvailable;

        public void Draw(SpriteBatch spriteBatch) =>
            spriteBatch.Draw(_isAvailable ? _availableTexture : _notAvailableTexture, 
                _textureRect, Color.White);
    }
}