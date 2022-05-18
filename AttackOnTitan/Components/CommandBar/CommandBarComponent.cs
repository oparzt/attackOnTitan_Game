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
        private readonly IScene _scene;
        
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        private readonly Dictionary<CommandType, CommandBarItemComponent> _commandBarItems = new();

        public CommandBarComponent(IScene parent, int viewportWidth, int viewportHeight)
        {
            _scene = parent;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        public void UpdateCommands(CommandInfo[] commandInfos)
        {
            var startX = _viewportWidth / 2
                         - (commandInfos.Length % 2 == 1 ? 30 : -8)
                         - commandInfos.Length / 2 * 76;
            var i = 0;
            
            _commandBarItems.Clear();
            
            foreach (var commandInfo in commandInfos)
            {
                _commandBarItems[commandInfo.CommandType] = new CommandBarItemComponent(commandInfo.CommandType,
                    commandInfo.IsAvailable,
                    _scene.Textures[commandInfo.TextureName],
                    new Rectangle(startX + i * 76,
                        _viewportHeight - 70, 60, 60));
                i++;
            }
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