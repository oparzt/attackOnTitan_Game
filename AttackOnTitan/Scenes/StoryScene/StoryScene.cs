using System.Collections.Generic;
using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Scenes
{
    public class StoryScene: DrawableGameComponent, IScene
    {
        public Dictionary<string, Texture2D> Textures { get; }
        public Dictionary<string, SpriteFont> Fonts { get; }
        
        private SpriteBatch Sprite { get; set; }

        private int _curPage = 1;
        private Keys _lastKey = Keys.None;
        
        public StoryScene(Game game) : base(game)
        {
            Textures = new Dictionary<string, Texture2D>();
            Fonts = new Dictionary<string, SpriteFont>();
        }

        protected override void LoadContent()
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            for (var i = 1; i < 14; i++)
                Textures[$"Page{i}"] = Game.Content.Load<Texture2D>($"Textures/Page{i}");
            
            base.LoadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var keyboardPressed = keyboardState.GetPressedKeys();
            var keyPressed = keyboardPressed.Length == 0 ? Keys.None : keyboardPressed[0];

            if (keyPressed != _lastKey && keyPressed == Keys.Enter)
            {
                if (_curPage + 1 == 14)
                {
                    Game.Components.Add(new Level0Scene(Game));
                    Game.Components.Remove(this);
                } else
                    _curPage++;
            }

            _lastKey = keyPressed;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var device = SceneManager.GraphicsMgr.GraphicsDevice;
            var width = device.Viewport.Width;
            var height = width / 16 * 9;
            var y = (device.Viewport.Height - height) / 2;
            
            Sprite.Begin();
            
            Sprite.Draw(Textures[$"Page{_curPage}"], 
                new Rectangle(0, y, width, height), 
                Color.White);
            
            Sprite.End();
            
            base.Draw(gameTime);
        }
    }
}