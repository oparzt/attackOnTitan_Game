using System.Collections.Generic;
using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class BuilderChooseItemComponent
    {
        public BuildingInfo BuildingInfo;
        public HashSet<ResourceType> NotAvailableResources;
        public string BuildingName;
        public SpriteFont Font;
        public float FontScale;
        public Texture2D BackgroundTexture;
        public Texture2D BuildingTexture;
        public string BuildingTextureName;
        public Dictionary<ResourceType, Texture2D> NeededResourceTexture;
        public Vector2 BuildingNamePosition;
        public Rectangle BackgroundTextureRect;
        public Rectangle BuildingTextureRect;
        public Dictionary<ResourceType, (Rectangle, Vector2)> NeededResourcePositions;

        private bool _wasPressed;
        
        public void Update(GameTime gameTime, MouseState mouseState)
        {
            if (NotAvailableResources.Count != 0) return;
            
            var contains = BuildingTextureRect.Contains(mouseState.Position);
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
                        UnitCommandInfo = new UnitCommandInfo(UnitCommandType.Build)
                        {
                            BuildingInfo = BuildingInfo,
                            BuildingTextureName = BuildingTextureName
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
            
            spriteBatch.DrawString(Font, BuildingName, BuildingNamePosition, 
                Color.White, 0, Vector2.Zero, FontScale, SpriteEffects.None, 1);
            
            spriteBatch.Draw(BuildingTexture, BuildingTextureRect, null,
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);

            foreach (var price in BuildingInfo.Price)
            {
                spriteBatch.Draw(NeededResourceTexture[price.Key], NeededResourcePositions[price.Key].Item1, 
                    null, Color.White, 0, Vector2.Zero, 
                    SpriteEffects.None, 1);
            
                spriteBatch.DrawString(Font, BuildingInfo.PriceText[price.Key], 
                    NeededResourcePositions[price.Key].Item2, 
                    NotAvailableResources.Contains(price.Key) ? Color.Red : Color.White, 0, 
                    Vector2.Zero, FontScale, SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }
    }
}