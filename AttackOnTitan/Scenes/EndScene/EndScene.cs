using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Scenes
{
    public class EndScene: DrawableGameComponent
    {
        public Dictionary<string, Texture2D> Textures { get; } = new();
        public Dictionary<string, SpriteFont> Fonts { get; } = new();
        
        private SpriteBatch Sprite { get; set; }

        private readonly bool _win;
        private Keys _lastKey;
        
        public EndScene(Game game, bool win) : base(game)
        {
            _win = win;
        }

        protected override void LoadContent()
        {
            Sprite = new SpriteBatch(Game.GraphicsDevice);

            Textures["Texture"] = Game.Content.Load<Texture2D>(_win ?
                "Textures/Win" : "Textures/Loss");
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var keyboardPressed = keyboardState.GetPressedKeys();
            var keyPressed = keyboardPressed.Length == 0 ? Keys.None : keyboardPressed[0];

            if (keyPressed != _lastKey && keyPressed == Keys.Enter)
            {
                Game.Components.Add(new StartScene(Game));
                Game.Components.Remove(this);
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
            
            Sprite.Draw(Textures[$"Texture"], 
                new Rectangle(0, y, width, height), 
                Color.White);
            
            Sprite.End();
            
            base.Draw(gameTime);
        }
    }
}