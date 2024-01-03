using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.GameObjects;
using Alakoz.Collision;
using Alakoz.GameInfo;

using TiledCS;
using Alakoz.States;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Alakoz;
public class MainMenu : GameStates
{
    public static TestLevel thisGame;
    private GraphicsDeviceManager _graphics;
    public GraphicsDevice graphicsDevice;
    public ContentManager Content;
    public GameWindow window;
    public GameStates nextState;
    public static int WindowWidth;
    public static int WindowHeight;
    Texture2D Background;
    Camera camera;
    public MainMenu(ContentManager newContent,GraphicsDeviceManager newGraphics, GraphicsDevice newGraphicsDevice,GameWindow Window)
    {
        Content = newContent;
        _graphics = newGraphics;
        graphicsDevice = newGraphicsDevice;
        window = Window;
    }
    public override void Initialize() 
    {
        camera = new Camera(Game1.thisGame.Window, Game1.thisGame.GraphicsDevice, 1080, 720); 
    }
    public override void LoadContent() 
    {
        Console.WriteLine("Loading Content...");
        Console.WriteLine("Loading Background...");
        Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Sky");
        Console.WriteLine(" -------------------- LoadContent: OK --------------------");
    }
    public override void Update(GameTime gameTime) 
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.V)) {
            Game1.currState = Game1.testlvl;
            Game1.currState.Initialize();
            Game1.currState.LoadContent();
        }
    }
    public override void Draw(SpriteBatch spriteBatch,GameTime gameTime) 
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(Background, new Rectangle(-100, -100, 1600, 1000), Color.White);
        spriteBatch.End();
    }

}