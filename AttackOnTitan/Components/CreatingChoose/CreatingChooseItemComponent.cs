using System.Collections.Generic;
using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CreatingChooseItemComponent
    {
        public CommandType CommandType;
        public UnitInfo UnitInfo;
        public MapCellInfo MapCellInfo;
        public CreatingInfo CreatingInfo;

        public SpriteFont Font;
        public Vector2 TextOrigin;
        
        public string ObjectName;
        public Vector2 ObjectNamePosition;
        
        public Texture2D ObjectTexture;
        public Rectangle ObjectTextureRect;

        public Texture2D BackgroundTexture;
        public Rectangle BackgroundTextureRect;

        public (ResourceType, Texture2D, string, Rectangle, Vector2)[] ObjectResources;
        public HashSet<ResourceType> NotAvailableResources;

        private bool _wasPressed;

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (NotAvailableResources.Count != 0) return;
            
            var contains = BackgroundTextureRect.Contains(mouseState.Position);
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
                        {
                            CreatingInfo = CreatingInfo
                        }
                    });
                }
                else
                    _wasPressed = false;
            }
            else
                _wasPressed = pressed && contains;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(BackgroundTexture, BackgroundTextureRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            
            spriteBatch.DrawString(Font, ObjectName, ObjectNamePosition, 
                Color.White, 0, TextOrigin, 1f, SpriteEffects.None, 1);
            
            spriteBatch.Draw(ObjectTexture, ObjectTextureRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);

            foreach (var objectResource in ObjectResources)
            {
                spriteBatch.Draw(objectResource.Item2, objectResource.Item4, 
                    null, Color.White, 0, Vector2.Zero, 
                    SpriteEffects.None, 1);
            
                spriteBatch.DrawString(Font, objectResource.Item3, 
                    objectResource.Item5, 
                    NotAvailableResources.Contains(objectResource.Item1) ? Color.Red : Color.White, 0, 
                    TextOrigin, 1f, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }
    }
}