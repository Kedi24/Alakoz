using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.GameObjects;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace Alakoz.GameInfo
{
    public class Camera : GameObject
    {
        public GameObject target;
        public OrthographicCamera camera;

        public Matrix transformation;
        private Matrix inverseMatrix;
        public Vector2 WindowPosition;
        public int width;
        public int height;


        #region // ------ Movement ------ //
        public bool move_left = false;
        public bool move_right = false;
        public bool move_up = false;
        public bool move_down = false;
        public bool zoomIn = false;
        public bool zoomOut = false;
        public bool unrestricted = false;
        #endregion
        
        #region // ------ Physics ------ //
        public float speed = 1f;
        public float velocityMax = 10f;
        #endregion
        
        #region // ------ Input ------ //
        bool prevKeyDown = false; // Just to check if camera modes are activated or not
        bool currKeyDown = false; // Just to check if camera modes are activated or not
        private bool prevKeyC = false;
        private bool currKeyC = false;
        #endregion
        
        #region // ------ Debug ------ //
        public Matrix debugMatrix; // For debug
        public string matrixMSG;
        public string positionMSG;
        public SpriteFont displayFONT;
        #endregion
        
        #region // ========== CONSTRUCTORS ========== //
        public Camera(GameWindow gameWindow, GraphicsDevice graphicsDevice, int camWidth, int camHeight){
            var viewportAdapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, camWidth, camHeight);
            camera = new OrthographicCamera(viewportAdapter);
            width = camWidth;
            height = camHeight;
            velocity = Vector2.Zero;
            transformation = camera.GetViewMatrix();
            inverseMatrix = camera.GetInverseViewMatrix();
            gameWindow.ClientSizeChanged += adjustBounds;
            
            camera.Zoom = 2.2f;
        }
        #endregion
       
        #region // ========== HELPERS ========== //
        public void resetZoom()
        {
            Vector2 worldPos = camera.Position;
            Vector2 screenPos = camera.WorldToScreen(camera.Position);
            MouseState mouse = Mouse.GetState();
            Vector2 MousePos = new Vector2(mouse.Position.X, mouse.Position.Y);
            
            // Console.WriteLine("REFERENCE | TARGET");
            // Console.WriteLine("----------------------------------------------------");
            // Console.WriteLine("  Monitor | Window  ----> Position: " + WindowPosition.ToString());
            // Console.WriteLine("----------------------------------------------------");
            
            // Console.WriteLine("  Window  | Mouse   ----> Position: " + MousePos);
            // Console.WriteLine("  World   | Mouse   ----> Position: " + camera.ScreenToWorld(MousePos));
            // Console.WriteLine("----------------------------------------------------");

            // Console.WriteLine("  Window  | Camera   ----> Position: " + screenPos);
            // Console.WriteLine("  World   | Camera   ----> Position: " + worldPos);
            // Console.WriteLine(camera.Position);
            // Console.WriteLine("---------------------- DONE -------------------------");

        }
        public void setWindowPosition(Vector2 WindowPos) {WindowPosition = WindowPos;}
        public void setTarget(int targetID = -1) {
            var objects = Game1.getGameObjects();
            var tIndex = targetID < 0? 
                objects.FindIndex((GameObject gObject) => {return gObject.id == target.id;}) + 1 :
                objects.FindIndex((GameObject gObject) => {return gObject.id == targetID;});

            target = tIndex <= 0?
                objects[0] :
                objects[tIndex >= objects.Count? 0 : tIndex];
        }
        public float Width(){return width*inverseMatrix.M11;}
        public float Height(){return height*inverseMatrix.M11;}

        public float Left(){return inverseMatrix.M41;}
        public float Right(){return inverseMatrix.M41 + Width();}
        public float Top(){return inverseMatrix.M42;}
        public float Bottom(){return inverseMatrix.M42 + Height();}

        public float _LeftBound(){return Left() + Width()*0.45f;}
        public float _RightBound(){return Right() - Width()*0.45f;}
        public float _TopBound(){return Top() + Height()*0.3f;}
        public float _BottomBound(){return Bottom() - Height()*0.3f;}

        public float _CenterY(){return Top() + Height()*0.6f;}
        public Vector2 Scale(){return new Vector2(inverseMatrix.M11, inverseMatrix.M22);}

        private void adjustBounds(object sender, EventArgs args){
            GameWindow window = (GameWindow) sender;
            // TODO: Adjust camera size to fit window
        }
        
        #endregion
        
        #region // ========== PHYSICS ========== //
        
        #region // ----- Basic
        private void restrictedMove(){ 
            if (target == null) return;
            
            if (target.direction < 0){ // Left
                move_left = true;
                move_right = false;
            } 
            else if (target.direction > 0){// Right
                move_left = false;
                move_right = true;
            }

            if (target.origin.Y < _TopBound()){ // Top
                move_up = true;
                move_down = false;
            } 
            else if (target.origin.Y > _BottomBound()){// Bottom
                move_up = false;
                move_down = true;
            }
            else {
                move_up= false;
                move_down = false;
            }
        }
        private void unrestrictedMove()
        {
            // Horizontal Movements
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) 
            {
                move_left = true;
                move_right = false;
            }
            else if ( Keyboard.GetState().IsKeyDown(Keys.Right) )
            {
                move_left = false;
                move_right = true;
            }
            else 
            {
                move_left = false;
                move_right = false;
            }

            // Vertical Movements
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) 
            {
                move_up = true;
                move_down = false;
            }
            else if ( Keyboard.GetState().IsKeyDown(Keys.Down) )
            {
                move_up = false;
                move_down = true;
            }
            else 
            {
                move_up = false;
                move_down = false;
            }
        }
        #endregion
        
        #endregion
        
        #region // ========== UPDATE ========== //
        public void Update(){
            update_input();
            update_state();
            update_physics();   
        }

        public override void update_input()
        {
            prevKeyDown = currKeyDown;
            currKeyDown = Keyboard.GetState().IsKeyDown(Keys.Tab); 
            
            prevKeyC = currKeyC;
            currKeyC = Keyboard.GetState().IsKeyDown(Keys.C); 

            // Zoom in and out 
            if ( Keyboard.GetState().IsKeyDown(Keys.Z)) {
                zoomIn = true;
                zoomOut = false;
            } 
            else if ( Keyboard.GetState().IsKeyDown(Keys.X)){
                zoomIn = false;
                zoomOut = true;
            }
            else {
                zoomIn = false;
                zoomOut = false;
            }

            if (prevKeyDown && !currKeyDown) unrestricted = !unrestricted;
            if (prevKeyC && !currKeyC) setTarget();
            
            if (!unrestricted) // Player cannot move the camera freely
            {
                restrictedMove();
                return;
            }
            else // Player can move the camera freely
            {
                unrestrictedMove();
            }
            
 
        }

        public override void update_state(){
            // 
            // Handle Zoom
            if (zoomIn) camera.ZoomIn(0.1f);
            else if (zoomOut) camera.ZoomOut(0.1f);
            
            // For free camera movemet based on user input
            if (unrestricted && target != null){
                if (move_right) velocity.X = approach(velocity.X, velocityMax, speed);
                else if (move_left) velocity.X = approach(velocity.X, -velocityMax, speed);
                else velocity.X = approach(velocity.X, 0, speed);
                
                if (move_up) velocity.Y = approach(velocity.Y, -velocityMax, speed);
                else if (move_down) velocity.Y = approach(velocity.Y, velocityMax, speed);  
                else velocity.Y = approach(velocity.Y, 0, speed);    
            }
            else{ // Camera movement based on player position
                if ( move_left){
                    float distance = target.origin.X - _RightBound(); // distance between target and right bound
                    if (Math.Abs(distance) < 3) {
                        velocity.X = approach(velocity.X, target.velocity.X, speed);
                        move_left = false;  
                        move_right = false;   
                    }
                    else velocity.X = distance/32 + target.velocity.X;
                } 
                else if (move_right) {
                    float distance = target.origin.X - _LeftBound(); // distance between target and left bound
                    if (Math.Abs(distance) < 3) {
                        velocity.X = approach(velocity.X, target.velocity.X, speed);
                        move_left = false;  
                        move_right = false; 
                    }
                    else velocity.X = distance/32 + target.velocity.X;
                }

                float distanceY = target.origin.Y - _CenterY(); // distance between target and camera origin
                if (move_up){
                    if (Math.Abs(distanceY) < 3) velocity.Y = approach(velocity.Y, target.velocity.Y, speed*0.5f);
                    else velocity.Y = distanceY/32 + target.velocity.Y;
                }
                else if (move_down){
                    if (Math.Abs(distanceY) < 3) velocity.Y = approach(velocity.Y, target.velocity.Y, speed*0.5f);  
                    else velocity.Y = distanceY/32 + target.velocity.Y;
                }
                else {
                    float distance = target.origin.Y - _CenterY();
                    if (target.velocity.Y > 3f) velocity.Y = distance/32 + (1.2f*target.velocity.Y);
                    else velocity.Y = distance/32 + (0.9f*target.velocity.Y);
                }
            }
        }

        public override void update_physics(){

            if (target.hitStop <= 0) camera.Position += velocity;
            transformation = camera.GetViewMatrix();
            inverseMatrix = camera.GetInverseViewMatrix();
        }

        #endregion
        
        #region // ========== DRAW ========== //
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){
            // For Debugging
            // drawBounds(spriteBatch);
            // drawDebug(spriteBatch);    
            spriteBatch.DrawString(
                displayFONT,
                ">>>>> DEBUG CONTROLS" + "\n" +
                "Swap Mode | TAB" + "\n" +
                "Swap Target | C" + "\n" +
                "Frame Advance | SPACE" + "\n\n" +

                ">>>>> INFO" + "\n" +
                "Zoom Level: " + camera.Zoom + "\n" +
                "Mode: " + (unrestricted ? "FREE" : "FOLLOW") + "\n" +
                "Target Type: " + target.type + "\n" +
                "Target ID: " + target.id + "\n", 
                position: new Vector2(Left()+Width()*0.01f, Top() + 0.45f*Height()),
                color: Color.MonoGameOrange,
                rotation: 0f, 
                origin: Vector2.Zero, 
                scale: Scale(), 
                effects: SpriteEffects.None, 
                layerDepth: 0f
            );
            
            if (target.type == TObject.PLAYER){
                Player temp = (Player)target;
                temp.drawUI(spriteBatch, this);
            }  
        }
        public void drawBounds(SpriteBatch spriteBatch){
            spriteBatch.DrawRectangle(_LeftBound(), Top(), 1f, Height(), Color.Bisque);
            spriteBatch.DrawRectangle(_RightBound(), Top(), 1f, Height(), Color.Bisque);
            spriteBatch.DrawRectangle(Left(), _TopBound(), Width(), 1f, Color.MonoGameOrange);
            spriteBatch.DrawRectangle(Left(), _BottomBound(), Width(), 1f, Color.MonoGameOrange);

            Color tempColor = Color.Yellow;
            if (target.origin.X > _LeftBound() && 
                target.origin.X < _RightBound() &&
                target.origin.Y > _TopBound() && 
                target.origin.Y < _BottomBound()) tempColor = Color.Green;

            spriteBatch.DrawCircle(
                target.origin.X, 
                target.origin.Y-1f, 
                3f, 
                10, 
                tempColor, 
                thickness:1
            );
        }
        public void drawDebug(SpriteBatch spriteBatch)
        {
            debugMatrix = camera.GetInverseViewMatrix();
			matrixMSG = "Matrix (Inverse)-------------------"
                    + "\nX -> " + debugMatrix.M11 + ", " + debugMatrix.M12 + ", " + debugMatrix.M13 + ", " + debugMatrix.M14
                    + "\nY -> " + debugMatrix.M21 + ", " + debugMatrix.M22 + ", " + debugMatrix.M23 + ", " + debugMatrix.M24
                    + "\nZ ->" + debugMatrix.M31 + ", " + debugMatrix.M32 + ", " + debugMatrix.M33 + ", " + debugMatrix.M34
                    + "\nT -> " + debugMatrix.M41 + ", " + debugMatrix.M42 + ", " + debugMatrix.M43 + ", " + debugMatrix.M44
                    + "\n\nMatrix (Normal)-------------------"
                    + "\nX -> " + transformation.M11 + ", " + transformation.M12 + ", " + transformation.M13 + ", " + transformation.M14
                    + "\nY -> " + transformation.M21 + ", " + transformation.M22 + ", " + transformation.M23 + ", " + transformation.M24
                    + "\nZ ->" + transformation.M31 + ", " + transformation.M32 + ", " + transformation.M33 + ", " + transformation.M34
                    + "\nT -> " + transformation.M41 + ", " + transformation.M42 + ", " + transformation.M43 + ", " + transformation.M44;
                    
            positionMSG = 
                    "Camera Position: "+ camera.Position
                    + "\n Velocity: " + velocity
                    + "\n Left: " + move_left
                    + "\n Right: " + move_right
                    + "\n Up: " + move_up
                    + "\n Down: " + move_down
                    + "\n Translation: " + debugMatrix.Translation;

            // spriteBatch.DrawRectangle(new RectangleF(debugMatrix.Translation.X,debugMatrix.Translation.Y, width*debugMatrix.M11, height*debugMatrix.M22), 
                                    // Color.DarkSeaGreen, 10*debugMatrix.M11);
            // spriteBatch.DrawRectangle(new RectangleF(target.position.X, target.position.Y, 20*debugMatrix.M11, 20*debugMatrix.M22), 
            //                         Color.DarkSeaGreen, 3*debugMatrix.M11);                        

            // spriteBatch.DrawRectangle(new RectangleF(positionFZ.X, positionFZ.Y, absWidthFZ, absHeightFZ), 
            //                         Color.Purple, 10*debugMatrix.M11);

            spriteBatch.DrawRectangle(Left(), Top(), Width(), Height(), Color.Purple);

            spriteBatch.DrawString(displayFONT, matrixMSG, new Vector2(debugMatrix.M41, debugMatrix.M42), 
                                    Color.Red, 0f, Vector2.One, 
                                    Scale(), SpriteEffects.None, 0f);
            
            spriteBatch.DrawString(displayFONT, positionMSG, new Vector2(debugMatrix.M41, debugMatrix.M42 + ((height-200) * debugMatrix.M11)), 
                                    Color.Blue, 0f, Vector2.One, 
                                    Scale(), SpriteEffects.None, 0f);
        }
        
        #endregion
    }
}