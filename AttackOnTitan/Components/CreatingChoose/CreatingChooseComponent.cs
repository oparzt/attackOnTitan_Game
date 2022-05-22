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
        private readonly IScene _scene;
        
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private readonly List<CreatingChooseItemComponent> _builderChooseItems = new();

        private readonly Random _random = new();

        public CreatingChooseComponent(IScene parent, int viewportWidth, int viewportHeight)
        {
            _scene = parent;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void UpdateBuildings(OutputAction action)
        {
            var creatingInfo = action.OutputCreatingInfo;
            var backgroundTexture = _scene.Textures[creatingInfo.BackgroundTextureName];
            var (backgroundWidth, backgroundHeight) = (backgroundTexture.Width, backgroundTexture.Height);

            var startX = _viewportWidth / 2
                 - (creatingInfo.CreatingInfos.Length % 2 == 1 ? backgroundWidth / 2 : -8)
                 - creatingInfo.CreatingInfos.Length / 2 * (backgroundWidth + 16);

            var startY = _viewportHeight / 2 - backgroundHeight;

            var objectDiffX = (backgroundWidth - creatingInfo.ObjectsTextureSize.X) / 2;

            var font = _scene.Fonts["Medium"];
            
            _builderChooseItems.Clear();

            for (var i = 0; i < creatingInfo.CreatingInfos.Length; i++)
            {
                var curX = startX + i * (backgroundTexture.Width + 16);
                var objectTextureName = creatingInfo.CreatingInfos[i].PossibleTextures[
                    _random.Next(0, creatingInfo.CreatingInfos[i].PossibleTextures.Length)
                ];
                var objectTextureRect = new Rectangle(new Point(curX + objectDiffX, startY), 
                    creatingInfo.ObjectsTextureSize);
                
                _builderChooseItems.Add(new CreatingChooseItemComponent
                {
                    CreatingInfo = creatingInfo.CreatingInfos[i],
                    CommandType = creatingInfo.CommandType,
                    UnitInfo = action.UnitInfo,
                    MapCellInfo = action.MapCellInfo,
                    
                    Font = font,
                    FontScale = 0.24f,
                    
                    ObjectName = creatingInfo.CreatingInfos[i].ObjectName,
                    ObjectNamePosition = new Vector2(curX + 10, startY + 160),
                    
                    ObjectTexture = _scene.Textures[objectTextureName],
                    ObjectTextureRect = objectTextureRect,
                    ObjectTextureName = objectTextureName,
                    
                    BackgroundTexture = backgroundTexture,
                    BackgroundTextureRect = new Rectangle(
                        new Point(curX, startY), 
                        backgroundTexture.Bounds.Size),
                    
                    NeededResourceTexture = creatingInfo.CreatingInfos[i].Price
                        .Select(pair => (pair.Key, 
                            _scene.Textures[GameModel.ResourceTexturesName[pair.Key]]))
                        .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2),
                    NeededResourcePositions = creatingInfo.CreatingInfos[i].Price
                        .Select((pair, pairIndex) => 
                            (pair.Key, new Rectangle(curX + 10, startY + 202 + 35 * pairIndex, 30, 30), 
                            new Vector2(curX + 50, startY + 205 + 35 * pairIndex)))
                        .ToDictionary(tuple => tuple.Item1, tuple => (tuple.Item2, tuple.Item3)),
                    NotAvailableResources = creatingInfo.NotAvailableResource[i]
                });
            }

            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.BuilderChoose)
                {
                    Zones = new [] { _builderChooseItems.Count == 0 ? 
                        Rectangle.Empty : 
                        new Rectangle(0,0, _viewportWidth, _viewportHeight)}
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