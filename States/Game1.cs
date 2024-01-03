using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.States;

namespace Alakoz;

public class Game1 : Game
{
    // ----- OTHER ----- //
    public static Game thisGame;
    private GraphicsDeviceManager _graphics;
    public SpriteBatch _spriteBatch;
    public static GameStates testlvl;
    public MainMenu menu;
    public static GameStates currState;

     //============================================= GAME1 =============================================
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1080;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content/";
        menu = new MainMenu(Content,_graphics,GraphicsDevice,Window);
        testlvl = new TestLevel(Content,_graphics,GraphicsDevice,Window);
        currState = menu;
        IsMouseVisible = true;
        thisGame = this;
    }
    // ============================================= INITIALIZING =============================================
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        // testlvl.Initialize();
        currState.Initialize();
        base.Initialize();
    }

    // ============================================= LOADING =============================================
    protected override void LoadContent() // TODO: use this.Content to load your game content here
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //testlvl.LoadContent();
        currState.LoadContent();
    }
    
    protected override void Update(GameTime gameTime) // TODO: Add your update logic here
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // testlvl.Update(gameTime);
        currState.Update(gameTime);
        base.Update(gameTime);
    }

    
    // ============================================= DRAWING =============================================

    protected override void Draw(GameTime gameTime)
    {
        // _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.transformation);
        GraphicsDevice.Clear(Color.Black);
        // testlvl.Draw(_spriteBatch,gameTime);
        currState.Draw(_spriteBatch,gameTime);
        base.Draw(gameTime);
       //  _spriteBatch.End();
    }
}
