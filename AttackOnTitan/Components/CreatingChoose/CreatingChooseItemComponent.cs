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
        public CreatingInfo CreatingInfo;
        public UnitInfo UnitInfo;
        public MapCellInfo MapCellInfo;
        
        public SpriteFont Font;
        public float FontScale;
        
        public string ObjectName;
        public Vector2 ObjectNamePosition;
        
        public Texture2D ObjectTexture;
        public string ObjectTextureName;
        public Rectangle ObjectTextureRect;

        public Texture2D BackgroundTexture;
        public Rectangle BackgroundTextureRect;

        
        public HashSet<ResourceType> NotAvailableResources;
        public Dictionary<ResourceType, Texture2D> NeededResourceTexture;
        public Dictionary<ResourceType, (Rectangle, Vector2)> NeededResourcePositions;

        private bool _wasPressed;
        
        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (NotAvailableResources.Count != 0) return;
            
            var contains = ObjectTextureRect.Contains(mouseState.Position);
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
                            CreatingInfo = CreatingInfo,
                            BuildingTextureName = ObjectTextureName
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
                Color.White, 0, Vector2.Zero, FontScale, SpriteEffects.None, 1);
            
            spriteBatch.Draw(ObjectTexture, ObjectTextureRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);

            foreach (var price in CreatingInfo.Price)
            {
                spriteBatch.Draw(NeededResourceTexture[price.Key], NeededResourcePositions[price.Key].Item1, 
                    null, Color.White, 0, Vector2.Zero, 
                    SpriteEffects.None, 1);
            
                spriteBatch.DrawString(Font, CreatingInfo.PriceText[price.Key], 
                    NeededResourcePositions[price.Key].Item2, 
                    NotAvailableResources.Contains(price.Key) ? Color.Red : Color.White, 0, 
                    Vector2.Zero, FontScale, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }
    }
}