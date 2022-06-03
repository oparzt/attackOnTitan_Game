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
        private readonly int _width;
        private readonly int _height;
        
        private SpriteFont _font;
        private int _fontSize;
        private Vector2 _textOrigin;
        private Texture2D _backgroundTexture;

        private int _nextResX = 15;
        private string _stepText = String.Empty;
        private Vector2 _textPos;

        private readonly Dictionary<ResourceType, TopBarResourceComponent> _resComponents = new();

        public TopBarComponent(int width, int height)
        {
            _width = width;
            _height = height;
            
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.TopBar)
                {
                    Zones = new [] {new Rectangle(0, 0, width, height)}
                }
            });
        }

        public void SetFont(SpriteFont font, int fontSize, Vector2 origin)
        {
            _font = font;
            _fontSize = fontSize;
            _textOrigin = origin;
        }
        public void SetBackgroundTexture(Texture2D texture) => _backgroundTexture = texture;

        public void AddResource(ResourceInfo resInfo)
        {
            var resTexture = SceneManager.Textures[resInfo.TextureName];
            var resComponent = new TopBarResourceComponent(resTexture, _font, _textOrigin);
            
            _resComponents[resInfo.ResourceType] = resComponent;
            
            resComponent.UpdateTextureRect(new Rectangle(new Point(_nextResX, (_height - resTexture.Height) / 2),
                resTexture.Bounds.Size));
            resComponent.UpdateTextPosition(new Vector2(_nextResX + resTexture.Width + 15,
                (_height - _fontSize) / 2f));
            if (resInfo.Count is not null) resComponent.UpdateResourceCount(resInfo.Count);

            _nextResX += resTexture.Bounds.Size.X + 100;
        }

        public void UpdateResourceCount(ResourceInfo resInfo) => 
            _resComponents[resInfo.ResourceType].UpdateResourceCount(resInfo.Count);

        public void UpdateStepCount(OutputAction action)
        {
            _stepText = action.StepInfo;
            _textPos = new Vector2(_width - _font.MeasureString(action.StepInfo).X - 10, 
                (_height - _fontSize) / 2f);
        }
        
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
            
            spriteBatch.DrawString(_font, _stepText, _textPos, 
                Color.White, 0, _textOrigin, 1f, SpriteEffects.None, 1);

            spriteBatch.End();
        }
    }
}