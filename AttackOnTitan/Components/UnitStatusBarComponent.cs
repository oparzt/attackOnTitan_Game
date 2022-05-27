using System.Linq;
using AttackOnTitan.Models;
using AttackOnTitan.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AttackOnTitan.Components
{
    public class UnitStatusBarComponent
    {
        private readonly IScene _scene;
        
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private readonly float _fontScale;
        private readonly int _fontSize;

        private readonly int _viewportHeight;
        
        private Rectangle _backgroundRect = Rectangle.Empty;

        private string[] _str = {};
        private Vector2[] _strPos = {};

        
        public UnitStatusBarComponent(IScene parent, int viewportWidth, int viewportHeight, int font)
        {
            _scene = parent;
            _fontScale = font / 100f;
            _fontSize = font;

            _viewportHeight = viewportHeight;
            
            UpdateNoServicedZones();
        }

        public void SetBackgroundTexture(Texture2D texture)
        {
            _backgroundTexture = texture;
        }

        public void SetFont(SpriteFont spriteFont)
        {
            _font = spriteFont;
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
                .Select((str, i) => new Vector2(10, startY + 10 + i * 30))
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
                    Color.White, 0, Vector2.Zero, 
                    _fontScale, SpriteEffects.None, 1); 
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