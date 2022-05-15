﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AttackOnTitan.Scenes;
using AttackOnTitan.Models;

namespace AttackOnTitan.Components
{
    public class MapCellComponent
    {
        public readonly int X;
        public readonly int Y;

        private Rectangle _destRect;
        private float _opacity = 0.3f;
        private IScene _scene;
        private string _textureName;

        private Dictionary<Position, Rectangle> _positionsRectangles = new();

        public MapCellComponent(IScene scene, string textureName, int x, int y, Rectangle destRect)
        {
            X = x;
            Y = y;

            _scene = scene;
            _textureName = textureName;
            _destRect = destRect;
        }

        public void Update(GameTime gameTime, MouseState mouseState) {}

        public void SetOpacity(float opacity) => _opacity = opacity;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_scene.Textures[_textureName], _destRect, 
                null, Color.White * _opacity, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }

        public bool IsComponentOnPosition(Point point)
        {
            var dist = (point - _destRect.Center).ToVector2().Length();
            return dist <= _destRect.Height / 2;
        }

        public Point GetCenter()
        {
            return _destRect.Center;
        }

        public Rectangle GetPosition(Position position)
        {
            return _positionsRectangles[position];
        }

        public void CreatePositionsRectangles(Point unitTextureSize)
        {
            var halfUnitWidth = new Point(unitTextureSize.X / 2, 0);
            var halfUnitHeight = new Point(0, unitTextureSize.Y / 2);
            var halfUnitSize = halfUnitWidth + halfUnitHeight;

            var halfRectWidth = new Point(_destRect.Width / 2, 0);
            var halfRectHeight = new Point(0, _destRect.Height / 2);
            var halfRectSize = halfRectWidth + halfRectHeight;

            var quarterRectWidth = new Point(_destRect.Width / 4, 0);
            var quarterRectHeight = new Point(0, _destRect.Height / 4);

            var center = _destRect.Center;

            var topBorderCenter = center - halfRectHeight;
            var bottomBorderCenter = center + halfRectHeight;

            var leftTopBorderCenter = new Rectangle(center
                - halfRectSize, quarterRectWidth + halfRectHeight).Center;
            var rightTopBorderCenter = new Rectangle(center - halfRectHeight
                + quarterRectWidth, quarterRectWidth + halfRectHeight).Center;

            var leftBottomBorderCenter = new Rectangle(center
                - halfRectWidth, quarterRectWidth + halfRectHeight).Center;
            var rightBottomBorderCenter = new Rectangle(center
                + quarterRectWidth, quarterRectWidth + halfRectHeight).Center;

            _positionsRectangles[Position.Center] = new Rectangle(center
                - halfUnitSize, unitTextureSize);
            
            _positionsRectangles[Position.TopLeft] = new Rectangle(center 
                - unitTextureSize,
                unitTextureSize);
            _positionsRectangles[Position.TopRight] = new Rectangle(center 
                - new Point(0, unitTextureSize.Y),
                unitTextureSize);
            _positionsRectangles[Position.BottomLeft] = new Rectangle(center
                - new Point(unitTextureSize.X, 0),
                unitTextureSize);
            _positionsRectangles[Position.BottomRight] = new Rectangle(center,
                unitTextureSize);

            _positionsRectangles[Position.TopBorder] = new Rectangle(topBorderCenter
                - halfUnitSize, unitTextureSize);
            _positionsRectangles[Position.BottomBorder] = new Rectangle(bottomBorderCenter
                - halfUnitSize, unitTextureSize);
            _positionsRectangles[Position.LeftTopBorder] = new Rectangle(leftTopBorderCenter
                - halfUnitSize, unitTextureSize);
            _positionsRectangles[Position.RightTopBorder] = new Rectangle(rightTopBorderCenter
                - halfUnitSize, unitTextureSize);
            _positionsRectangles[Position.LeftBottomBorder] = new Rectangle(leftBottomBorderCenter
                - halfUnitSize, unitTextureSize);
            _positionsRectangles[Position.RightBottomBorder] = new Rectangle(rightBottomBorderCenter
                - halfUnitSize, unitTextureSize);

        }
    }
}
