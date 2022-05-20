using System.Collections.Generic;
using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class BuilderChooseComponent
    {
        private readonly IScene _scene;
        
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private readonly List<BuilderChooseItemComponent> _builderChooseItems = new();

        public BuilderChooseComponent(IScene parent, int viewportWidth, int viewportHeight)
        {
            _scene = parent;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void UpdateBuildings(OutputBuildingInfo buildingInfo)
        {
            var builderCardTexture = _scene.Textures[buildingInfo.BackgroundTextureName];

            var startX = _viewportWidth / 2
                 - (buildingInfo.BuildingInfos.Length % 2 == 1 ? builderCardTexture.Width / 2 : -8)
                 - buildingInfo.BuildingInfos.Length / 2 * (builderCardTexture.Width + 16);

            var startY = _viewportHeight / 2 - builderCardTexture.Height / 2;

            var font = _scene.Fonts["Medium"];
            
            _builderChooseItems.Clear();

            for (var i = 0; i < buildingInfo.BuildingInfos.Length; i++)
            {
                var curX = startX + i * (builderCardTexture.Width + 16);
                var buildingTexture = _scene.Textures[buildingInfo.BuildingTexturesName[i]];
                var buildingRect = new Rectangle(curX + 7, startY, 185, 160);
                
                _builderChooseItems.Add(new BuilderChooseItemComponent
                {
                    BuildingInfo = buildingInfo.BuildingInfos[i],
                    NotAvailableResources = buildingInfo.NotAvailableResource[i],
                    BuildingName = buildingInfo.BuildingInfos[i].BuildingName,
                    Font = font,
                    FontScale = 0.24f,
                    BackgroundTexture = builderCardTexture,
                    BuildingTexture = buildingTexture,
                    BuildingTextureName = buildingInfo.BuildingTexturesName[i],
                    NeededResourceTexture = buildingInfo.BuildingInfos[i].Price
                        .Select(pair => (pair.Key, 
                            _scene.Textures[GameModel.ResourceTexturesName[pair.Key]]))
                        .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2),
                    BuildingNamePosition = new Vector2(curX + 10, startY + 160),
                    BackgroundTextureRect = new Rectangle(
                        new Point(curX, startY), 
                        builderCardTexture.Bounds.Size),
                    BuildingTextureRect = buildingRect,
                    NeededResourcePositions = buildingInfo.BuildingInfos[i].Price
                        .Select((pair, pairIndex) => 
                            (pair.Key, new Rectangle(curX + 10, startY + 202 + 35 * pairIndex, 30, 30), 
                            new Vector2(curX + 50, startY + 205 + 35 * pairIndex)))
                        .ToDictionary(tuple => tuple.Item1, tuple => (tuple.Item2, tuple.Item3))
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