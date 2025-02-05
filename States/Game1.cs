using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.States;
using System.Collections.Generic;
using Alakoz.GameObjects;

namespace Alakoz;

public class Game1 : Game
{
    public static Game thisGame;
    private GraphicsDeviceManager _graphics;
    public static SpriteBatch spriteBatch;
    public static GameState testlvl;
    public MainMenu menu;
    public static GameState currState;

    public static SpriteFont FONT;

    #region // ========== GAME1 ========== //
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1080;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content/";
        menu = new MainMenu(Content, _graphics, GraphicsDevice, Window);
        testlvl = new TestLevel(Content, _graphics, GraphicsDevice, Window);
        currState = menu;
        IsMouseVisible = true;
        thisGame = this;
    }
    public static void setGameState(GameState newState){
        // TODO: clean up old game state
        currState = newState;
        currState.Initialize();
        currState.LoadContent();
    } 
    public static List<GameObject> getGameObjects(){return testlvl.allGameObjects;}
    #endregion
        
    #region // ========== INITIALIZING ========== //
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
        currState.Initialize();
        base.Initialize();
    }
    #endregion

    #region // ========== LOADING ========== //
    protected override void LoadContent() // TODO: use this.Content to load your game content here
    {
        FONT = Content.Load<SpriteFont>("Alakoz Content/Fonts/TestFont");
        spriteBatch = new SpriteBatch(GraphicsDevice);
        currState.LoadContent();
    }
    #endregion

    #region // ========== UPDATING ========== //
    protected override void Update(GameTime gameTime) // TODO: Add your update logic here
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        currState.Update(gameTime);
        base.Update(gameTime);
    }
    #endregion

    #region // ========== DRAWING ========== //
    protected override void Draw(GameTime gameTime)
    {
        // spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.transformation);
        GraphicsDevice.Clear(Color.Black);
        currState.Draw(spriteBatch, gameTime);
        base.Draw(gameTime);
       //  spriteBatch.End();
    }
    #endregion
}
