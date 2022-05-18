using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CommandBarItemComponent
    {
        private readonly Texture2D _texture;
        private readonly CommandType _commandType;
        private Rectangle _textureRect;
        private bool _wasPressed;
        public readonly bool IsAvailable;

        public CommandBarItemComponent(CommandType commandType, bool isAvailable, 
            Texture2D texture, Rectangle textureRect)
        {
            _commandType = commandType;
            IsAvailable = isAvailable;
            _texture = texture;
            _textureRect = textureRect;
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var contains = _textureRect.Contains(mouseState.Position);
            var pressed = mouseState.LeftButton == ButtonState.Pressed;
            
            if (_wasPressed)
            {
                if (contains)
                {
                    if (pressed) return;
                    
                    _wasPressed = false;
                    GameModel.InputActions.Enqueue(new InputAction()
                    {
                        ActionType = InputActionType.UnitCommand,
                        UnitCommandInfo = new UnitCommandInfo(_commandType)
                    });
                }
                else
                    _wasPressed = false;
            }
            else
                _wasPressed = pressed && contains;
        }


        public void Draw(SpriteBatch spriteBatch) =>
            spriteBatch.Draw(_texture, _textureRect, Color.White);
    }
}