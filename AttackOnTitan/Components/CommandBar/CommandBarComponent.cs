using System.Collections.Generic;
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

        public void ClearCommands() => _commandBarItems.Clear();

        public void AddCommand(CommandInfo commandInfo)
        {
            var commandBarItem = new CommandBarItemComponent(commandInfo.CommandType,
                _scene.Textures[commandInfo.AvailableTextureName],
                _scene.Textures[commandInfo.NotAvailableTextureName]);
            
            commandBarItem.UpdateCommandState(commandInfo.IsAvailable);
            _commandBarItems[commandInfo.CommandType] = commandBarItem;

            var startX = _viewportWidth / 2
                - (_commandBarItems.Count % 2 == 1 ? 30 : -8)
                - _commandBarItems.Count / 2 * 76;
            var i = 0;
            
            foreach (var commandBarItem2 in _commandBarItems.Values)
            {
                commandBarItem2.UpdateTextureRect(new Rectangle(startX + i * 76,
                    _viewportHeight - 70, 60, 60));
                i++;
            }
        }

        public void UpdateCommandState(CommandInfo commandInfo)
        {
            if (_commandBarItems.TryGetValue(commandInfo.CommandType, out var commandBarItem))
                commandBarItem.UpdateCommandState(commandInfo.IsAvailable);
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            foreach (var commandBarItem in _commandBarItems.Values)
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