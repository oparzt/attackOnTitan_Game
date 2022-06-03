using System;
using System.Collections.Generic;
using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CreatingChooseComponent
    {
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private readonly List<CreatingChooseItemComponent> _builderChooseItems = new();
        private Dictionary<ResourceType, Texture2D> _resourceTextures;
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private int _fontSize;
        private Vector2 _textOrigin;

        public CreatingChooseComponent(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void InitializeCreatingChoose(OutputAction action)
        {
            var creatingInfo = action.OutputCreatingInfo;
            _resourceTextures = creatingInfo.ResourceTexturesName
                .Select(pair => new KeyValuePair<ResourceType, Texture2D>(pair.Key, SceneManager.Textures[pair.Value]))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            _backgroundTexture = SceneManager.Textures[creatingInfo.BackgroundTextureName];
        }

        public void SetFont(SpriteFont font, int fontSize, Vector2 origin)
        {
            _font = font;
            _fontSize = fontSize;
            _textOrigin = origin;
        }

        public void UpdateBuildings(OutputAction action)
        {
            var creatingInfo = action.OutputCreatingInfo;
            var creatingInfos = creatingInfo.CreatingInfos;
            var backgroundWidth = _backgroundTexture.Width;

            var startX = _viewportWidth / 2
                 - (creatingInfos.Length % 2 == 1 ? backgroundWidth / 2 : -8)
                 - creatingInfos.Length / 2 * (backgroundWidth + 16);
            var startY = _viewportHeight / 2;
            var objectDiffX = (backgroundWidth - creatingInfo.ObjectsTextureSize.X) / 2;

            for (var i = 0; i < creatingInfos.Length; i++)
            {
                var cardHeight = creatingInfos[i].ObjectResourceDescription.Length * 35 + 202;

                var curX = startX + i * (backgroundWidth + 16);
                var curY = startY - cardHeight / 2;
                
                var objectTextureRect = new Rectangle(new Point(curX + objectDiffX, curY), 
                    creatingInfo.ObjectsTextureSize);
                
                _builderChooseItems.Add(new CreatingChooseItemComponent
                {
                    CommandType = creatingInfo.CommandType,
                    UnitInfo = action.UnitInfo,
                    MapCellInfo = action.MapCellInfo,
                    CreatingInfo = creatingInfos[i],
                    
                    Font = _font,
                    TextOrigin = _textOrigin,
                    
                    ObjectName = creatingInfos[i].ObjectName,
                    ObjectNamePosition = new Vector2(curX + 10, curY + 160),
                    
                    ObjectTexture = SceneManager.Textures[creatingInfos[i].ObjectTextureName],
                    ObjectTextureRect = objectTextureRect,
                    
                    BackgroundTexture = _backgroundTexture,
                    BackgroundTextureRect = new Rectangle(
                        new Point(curX, curY), 
                        new Point(backgroundWidth, cardHeight)),
                    
                    ObjectResources = creatingInfos[i].ObjectResourceDescription
                        .Select((pair, pairIndex) =>
                        {
                            var textureRect = new Rectangle(curX + 10, curY + 200 + 35 * pairIndex, 30, 30);
                            var textPos = new Vector2(curX + 50, curY + 200 + (30 - _fontSize) / 2 + 35 * pairIndex);
                            return (pair.Item1,
                                _resourceTextures[pair.Item1], pair.Item2,
                                textureRect, textPos);
                        })
                        .ToArray(),
                    NotAvailableResources = creatingInfos[i].NotAvailableResource
                });
            }

            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.BuilderChoose)
                {
                    Zones = new [] { new Rectangle(0,0, _viewportWidth, _viewportHeight) }
                }
            });
        }

        public void ClearCreatingChoose(OutputAction action)
        {
            _builderChooseItems.Clear();
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.BuilderChoose)
                {
                    Zones = new [] { Rectangle.Empty }
                }
            });
        }
        
        public void Update(GameTime gameTime, MouseState mouseState)
        {
            foreach (var builderChooseItem in _builderChooseItems)
                builderChooseItem.Update(gameTime, mouseState); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var builderChooseItem in _builderChooseItems)
                builderChooseItem.Draw(spriteBatch);       
        }
        
    }
}