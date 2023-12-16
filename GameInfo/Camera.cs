using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.GameObjects;

using MonoGame.Extended;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using MonoGame.Extended.ViewportAdapters;

namespace Alakoz.GameInfo
{
    public class Camera
    {
        public Matrix transformation;
        public Vector3 position;

        public OrthographicCamera cameraView;
        
        // ========================================== CONSTRUCTOR ==========================================
        public Camera(GameWindow gameWindow, GraphicsDevice graphicsDevice, int width, int height)
        {
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, width, height);
            cameraView = new OrthographicCamera(viewportAdapter);
            cameraView.ZoomIn(1f); // Default zoom
        }
        // ========================================== GENERAL ==========================================

        // ========================================== UPDATING ==========================================
        public void Update(GameTime gameTime, GameObject target)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) cameraView.ZoomIn(0.1f);
            else if (Keyboard.GetState().IsKeyDown(Keys.X)) cameraView.ZoomOut(0.1f);
            else if (Keyboard.GetState().IsKeyDown(Keys.C)) cameraView.Zoom = 0;

            cameraView.LookAt(target.position + new Vector2(17, 22));
            transformation = cameraView.GetViewMatrix();
        }
    }
}