using System;
using System.Collections.Generic;
using AttackOnTitan.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.Scenes
{
    public interface IScene
    {
        public Dictionary<string, Texture2D> Textures { get; }
        public Dictionary<string, SpriteFont> Fonts { get; }
        public SpriteBatch Sprite { get; }
        public List<IComponent> Components { get; }
    }
}
