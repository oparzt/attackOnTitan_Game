using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CommandBarItemComponent
    {
        public CommandType CommandType;
        public UnitInfo UnitInfo;
        public MapCellInfo MapCellInfo;
        public bool IsAvailable;
        
        public Texture2D Texture;
        public Rectangle TextureRect;

        private bool _wasPressed;

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var contains = TextureRect.Contains(mouseState.Position);
            var pressed = mouseState.LeftButton == ButtonState.Pressed;
            
            if (_wasPressed)
            {
                if (contains)
                {
                    if (pressed) return;
                    
                    _wasPressed = false;
                    GameModel.InputActions.Enqueue(new InputAction()
                    {
                        ActionType = InputActionType.ExecCommand,
                        InputUnitInfo = new InputUnitInfo(UnitInfo.ID),
                        InputCellInfo = new InputCellInfo(MapCellInfo.X, MapCellInfo.Y),
                        InputCommandInfo = new InputCommandInfo(CommandType)
                    });
                }
                else
                    _wasPressed = false;
            }
            else
                _wasPressed = pressed && contains;
        }


        public void Draw(SpriteBatch spriteBatch) =>
            spriteBatch.Draw(Texture, TextureRect, Color.White);
    }
}