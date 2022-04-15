using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AttackOnTitan.GameComponents
{
    public class Camera2D : IComponent
    {
        public Matrix Transform;
        public Vector3 InitMouseDragPos;
        public Vector3 InitDragPos;
        public Vector3 Pos;
        public Vector3 Center;

        public int LeftBorder;
        public int RightBorder;

        public bool IsDrag = false;

        public Camera2D(int startPosX = 0, int startPosY = 0, int leftBorder = 0, int rightBorder = 0)
        {
            Pos = new Vector3(startPosX, startPosY, 0);
            Center = new Vector3(SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Width * 0.5f,
                SceneManager.GraphicsMgr.GraphicsDevice.Viewport.Height * 0.5f, 0);

            LeftBorder = leftBorder;
            RightBorder = rightBorder;

            UpdateTransformMatrix();
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            var curMousePos = new Vector3(mouseState.X, mouseState.Y, 0);

            if (IsDrag) UpdateMove(curMousePos);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!IsDrag) StartMove(curMousePos);
            }
            else
                IsDrag = false;
        }

        public void StartMove(Vector3 mousePos)
        {
            IsDrag = true;
            InitDragPos = Pos;
            InitMouseDragPos = mousePos;
        }

        public void UpdateMove(Vector3 mousePos)
        {
            Pos = mousePos - InitMouseDragPos + InitDragPos;
            UpdateTransformMatrix();
        }

        public void UpdateTransformMatrix()
        {
            Transform = Matrix.Identity * Matrix.CreateTranslation(Pos);

        }



        public void Draw(SpriteBatch spriteBatch) {}

        public bool IsComponentOnPosition(Point point)
        {
            throw new NotImplementedException();
        }
    }
}
