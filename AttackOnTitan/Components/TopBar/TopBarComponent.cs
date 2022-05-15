using System;
using System.Collections.Generic;
using AttackOnTitan.Scenes;
using AttackOnTitan.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AttackOnTitan.Components
{
    public class TopBarComponent
    {
        private readonly IScene _scene;

        private readonly int _width;
        private readonly int _height;
        private readonly int _fontSize;
        
        private SpriteFont _font;
        private Texture2D _backgroundTexture;

        private int _nextResX = 15;

        private readonly Dictionary<ResourceType, TopBarResourceComponent> _resComponents = new();

        public TopBarComponent(IScene parent, int width, int height, int fontSize)
        {
            _scene = parent;
            _width = width;
            _height = height;
            _fontSize = fontSize;
        }

        public void SetFont(SpriteFont spriteFont) => _font = spriteFont;
        public void SetBackgroundTexture(Texture2D texture) => _backgroundTexture = texture;

        public void AddResource(ResourceInfo resInfo)
        {
            var resComponent = new TopBarResourceComponent(_scene.Textures[resInfo.TextureName], _font, _fontSize / 100f);
            
            _resComponents[resInfo.ResourceType] = resComponent;
            
            resComponent.UpdateTextureRect(new Rectangle(new Point(_nextResX, Math.Abs(_height - resInfo.TextureSize.Y) / 2),
                resInfo.TextureSize));
            resComponent.UpdateTextPosition(new Vector2(_nextResX + resInfo.TextureSize.X + 15,
                (_height - _fontSize) / 2f));
            if (resInfo.Count is not null) resComponent.UpdateResourceCount(resInfo.Count);

            _nextResX += resInfo.TextureSize.X + 100;
        }

        public void UpdateResourceCount(ResourceInfo resInfo) => 
            _resComponents[resInfo.ResourceType].UpdateResourceCount(resInfo.Count);
        

        public void Draw(SpriteBatch spriteBatch)
        {
            var limitBackground = _width / _height + 1;
            
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            foreach (var resComponent in _resComponents.Values)
                resComponent.Draw(spriteBatch);

            for (var i = 0; i < limitBackground; i++)
                spriteBatch.Draw(_backgroundTexture, 
                    new Rectangle(_height * i, 0, _height, _height), null,
                    Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}