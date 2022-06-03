using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AttackOnTitan.Components
{
    public class UnitStatusBarComponent
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private int _fontSize;
        private Vector2 _textOrigin;

        private readonly int _viewportHeight;
        
        private Rectangle _backgroundRect = Rectangle.Empty;

        private string[] _str = {};
        private Vector2[] _strPos = {};

        
        public UnitStatusBarComponent(int viewportWidth, int viewportHeight)
        {
            _viewportHeight = viewportHeight;
            
            UpdateNoServicedZones();
        }

        public void SetBackgroundTexture(Texture2D texture)
        {
            _backgroundTexture = texture;
        }

        public void SetFont(SpriteFont spriteFont, int fontSize, Vector2 origin)
        {
            _font = spriteFont;
            _fontSize = fontSize;
            _textOrigin = origin;
        }

        public void UpdateStatusBar(OutputAction action)
        {
            var unitStatus = action.UnitStatus;
            var height = unitStatus.Length * _fontSize + (unitStatus.Length - 1) * 6 + 20;
            var startY = _viewportHeight - height;

            _backgroundRect = unitStatus.Length != 0 ? 
                new Rectangle(0, startY, 250, height) :
                Rectangle.Empty;
            _str = unitStatus;
            _strPos = unitStatus
                .Select((str, i) => new Vector2(10, startY + 10 + i * (_fontSize + 6)))
                .ToArray();
            
            UpdateNoServicedZones();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _backgroundRect, Color.White);
            for (var i = 0; i < _str.Length; i++)
            {
                spriteBatch.DrawString(_font, _str[i], _strPos[i], 
                    Color.White, 0, _textOrigin, 
                    1f, SpriteEffects.None, 1); 
            }
            spriteBatch.End();
        }

        private void UpdateNoServicedZones()
        {
            GameModel.InputActions.Enqueue(new InputAction
            {
                ActionType = InputActionType.UpdateNoServicedZones,
                NoServicedZone = new NoServicedZone(NoServicedZoneLocation.UnitStatusBar)
                {
                    Zones = new [] {_backgroundRect}
                }
            });
        }
    }
}