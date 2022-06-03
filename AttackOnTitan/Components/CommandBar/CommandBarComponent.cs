using System.Collections.Generic;
using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Components
{
    public class CommandBarComponent
    {
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private readonly Dictionary<CommandType, CommandBarItemComponent> _commandBarItems = new();

        public CommandBarComponent(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void UpdateCommands(OutputAction action)
        {
            var commandInfos = action.CommandInfos;
            var startX = _viewportWidth / 2
                         - (commandInfos.Length % 2 == 1 ? 30 : -8)
                         - commandInfos.Length / 2 * 76;
            var i = 0;
            
            _commandBarItems.Clear();
            
            foreach (var commandInfo in commandInfos)
            {
                _commandBarItems[commandInfo.CommandType] = new CommandBarItemComponent
                {
                    CommandType = commandInfo.CommandType,
                    UnitInfo = action.UnitInfo,
                    MapCellInfo = action.MapCellInfo,
                    IsAvailable = commandInfo.IsAvailable,
                    
                    Texture = SceneManager.Textures[commandInfo.TextureName],
                    TextureRect = new Rectangle(startX + i * 76,
                        _viewportHeight - 70, 60, 60)
                };
                i++;
            }
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.CommandBar)
                {
                    Zones = _commandBarItems.Values
                        .Select(commandBarItem => commandBarItem.TextureRect)
                        .ToArray()
                }
            });
        }
        
        public void ClearCommands(OutputAction action)
        {
            _commandBarItems.Clear();

            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.CommandBar)
                {
                    Zones = new []{Rectangle.Empty}
                }
            });
        }
        
        public void Update(GameTime gameTime, MouseState mouseState)
        {
            foreach (var commandBarItem in _commandBarItems.Values.Where(commandBarItem => commandBarItem.IsAvailable))
                commandBarItem.Update(gameTime, mouseState); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var commandBarItem in _commandBarItems.Values)
                commandBarItem.Draw(spriteBatch);            
            spriteBatch.End();
        }
    }
}