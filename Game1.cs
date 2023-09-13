using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Species;
using Alakoz.Collision;

using TiledCS;

namespace Alakoz;

public class Game1 : Game
{
    // ----- OTHER ----- //
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    SpriteFont font;

    // ----- PLAYER & BACKGROUND ----- //
    Texture2D Background;
    Player player1;
    Player player2;
    Dictionary<string, Animation> animations;
    Vector2 Player1Pos;
    Vector2 Player2Pos;
    Vector2 SymbolPosition;
    
    // ----- MAP ----- //
    TiledMap tileMap;
    TiledTileset tileSet;
    Dictionary<int, TiledTileset> allTilesets;
    Texture2D tilesetTexture;
    int tileWidth;
    int tileHeight;
    int tilesWide;
    int tilesHigh;
    TiledLayer collisionLayer;
    TiledObject[] collisionLayerObjects;
    
    // ----- COLLISIONS ----- //
    List<CollisionObject> allCollisionObjects; // All the collisions to be checked

     //============================================= GAME1 =============================================
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1080;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    // ============================================= INITIALIZING =============================================
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

        allCollisionObjects = new List<CollisionObject>();
        animations = new Dictionary<string, Animation>();
        SymbolPosition = new Vector2(400, 0);
        Player1Pos = new Vector2(400, 0);
        Player2Pos = new Vector2(500, 0);
        

        base.Initialize();
    }


    // ============================================= LOADING =============================================
    protected override void LoadContent() // TODO: use this.Content to load your game content here
    {
        Console.WriteLine("Loading Content...");
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Background = Content.Load<Texture2D>("Sky");
        font = Content.Load<SpriteFont>("TestFont");
        
        Console.WriteLine("Loading Map...");
        LoadMap();

        Console.WriteLine("Loading Character assests...");
        LoadCharacter();

        Console.WriteLine(" -------------------- LoadContent: OK --------------------");
    }
    protected void LoadCharacter(){
        
        Animation Symbol = new Animation(Content.Load<Texture2D>("Base_Animations/ball"), 1);
        Animation playerHurtbox = new Animation(Content.Load<Texture2D>("Base_Animations/Hurtbox"), 1);
        Animation playerHitbox = new Animation(Content.Load<Texture2D>("Base_Animations/Hitbox"), 1);

        Animation idle = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Idle"), 32);
        Animation idle2 = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Idle2"), 8);
        Animation run = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Running"), 22);
        Animation runStop = new Animation(Content.Load<Texture2D>("Base_Animations/Base_RunStop"), 24, false);
        Animation turnaround = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Turnaround"), 12);
        Animation jump = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Jump"), 10, false);
        Animation falling = new Animation(Content.Load<Texture2D>("Base_Animations/Base_Falling"), 12);

        //jump.isLooping = false;
        // runStop.isLooping = false;

        animations.Add("ball", Symbol);
        animations.Add("Hurtbox", playerHurtbox);
        animations.Add("Hitbox", playerHitbox);
        animations.Add("Base_Idle", idle);
        animations.Add("Base_Running", run);
        animations.Add("Base_Turnaround", turnaround);
        animations.Add("Base_RunStop", runStop);
        animations.Add("Base_Jump", jump);
        animations.Add("Base_Falling", falling);

        player1 = new Player(animations, Player1Pos);
        player2 = new Player(animations, Player2Pos);

        // THIS IS FOR THE INFORMATION DISPLAY, MOVE INTO PLAYER CLASS LATER
        player1.stateFONT = font;
        player1.animManager.frameFont = font;
        player1.hurtbox.spriteManager.frameFont = font;

        player2.stateFONT = font;
        player2.animManager.frameFont = font;
        player2.hurtbox.spriteManager.frameFont = font;
        
        Console.WriteLine("Width: " + idle.frameWidth);
        Console.WriteLine("Height: " + idle.frameHeight);

        // Setup the controls
        player1.controls.reset();
        player2.controls.altControls();
    }

    protected void LoadPlayerCollisions()
    {
        allCollisionObjects.Add(player1.hurtbox);
        allCollisionObjects.Add(player2.hurtbox);
    }

    protected void LoadMap(){
        
        // MAP 1 - Uncomment to load Map 1 and comment map 2
        // tileMap = new TiledMap(Content.RootDirectory + "/TestMaps/TestLevel - 2.tmx");
        // tileSet = new TiledTileset(Content.RootDirectory + "/TestMaps/Desert_Tileset3.tsx");
        // tilesetTexture = Content.Load<Texture2D>("TestMaps/tmw_desert_spacing");

        // allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + "/TestMaps/");

        // MAP 2 - Uncomment to load Map 2 and comment Map 1
        tileMap = new TiledMap(Content.RootDirectory + "/TestMaps/Map - Hitbox/Base Level.tmx");
        tileSet = new TiledTileset(Content.RootDirectory + "/TestMaps/Map - Hitbox/Base Tileset.tsx");
        tilesetTexture = Content.Load<Texture2D>("TestMaps/Map - Hitbox/Base_Tileset");

        allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + "/TestMaps/Map - Hitbox/");

        

        tileHeight = tileSet.TileHeight;
        tileWidth = tileSet.TileWidth;

        tilesWide = tileSet.Columns;
        tilesHigh = tileSet.TileCount / tileSet.Columns;

        collisionLayer = tileMap.Layers[1];
        
        collisionLayerObjects = collisionLayer.objects;

        LoadMapCollisions();
    }

    // Loads each object from the collision layer of the map and creates corresponding collisionObjects
    protected void LoadMapCollisions(){
        
        if (collisionLayerObjects.Length <= 0) return;

        // Parsing property information from the object layer to handle collisions
        for (int i = 0; i < collisionLayerObjects.Length; i++) {
            
            TiledObject currentObject = collisionLayerObjects[i];
            TiledProperty currentProperty = currentObject.properties[0];
            Vector2 tempPosition = new Vector2(currentObject.x, currentObject.y);
            CollisionObject tempObject;
            
            
            
            // Create collision objects depending on the type of collision (NOTE: Hitbox is commented out until its fixed)
            switch (currentProperty.value)  
            {
                case CollisionObject.GROUND:
                    tempObject = new Ground(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
                    break;
                case CollisionObject.PLATFORM:
                    tempObject = new Platform(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
                    break;
                case CollisionObject.HITBOX:
                    float directionX = (float) Convert.ToDouble(currentObject.properties[1].value); // Values are stored as strings so just convert them to floats
                    float directionY = (float) Convert.ToDouble(currentObject.properties[2].value); // Values are stored as strings so just convert them to floats

                    tempObject = new Hitbox(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height, 0, new Vector2(directionX, directionY), 5,25);
                    tempObject.active = true;
                    break;
                default:
                    tempObject = new Ground(new Vector2(currentObject.x, currentObject.y),  currentObject.width, currentObject.height);
                    break;
            }

            allCollisionObjects.Add(tempObject);
        }

    }

    // ============================================= UPDATING =============================================

    public void check_Collisions(){
        // Add player1 active collisions
        for (int i = 0; i < player1.activeCollisions.Count; i++)
        {
            if (!allCollisionObjects.Contains(player1.activeCollisions[i])) allCollisionObjects.Add(player1.activeCollisions[i]);
        }
        // Add the player2 active collisions
        for (int i = 0; i < player2.activeCollisions.Count; i++)
        {
            if (!allCollisionObjects.Contains(player2.activeCollisions[i])) allCollisionObjects.Add(player2.activeCollisions[i]);
        }
        
        // Currently this loop does more checks than necessary. Will change so that it only checks unique combinations
        for (int indexA = 0; indexA < allCollisionObjects.Count; indexA++){
             for (int indexB = 0; indexB < allCollisionObjects.Count; indexB++)
             {
                if (indexA != indexB)
                {
                    allCollisionObjects[indexA].OnCollision(allCollisionObjects[indexB]);
                }
            }
        }
    }

    protected override void Update(GameTime gameTime) // TODO: Add your update logic here
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Time is buggy, THe animation speed doubles when both players are in the same state, will fix later
        player1.update_time(gameTime); // update the frame count for the state
        player2.update_time(gameTime); 
        
        player1.update_input(); // Read the players input
        player2.update_input(); 

        player1.update_state(); // Modify their state
        player2.update_state(); 

        check_Collisions(); // Check the collisions 

        player1.update_physics(); // set the final position, velocity, and acceleration of the player
        player2.update_physics(); 

        player1.update_animations();// set the corresponding animation
        player2.update_animations(); 

        player1.animManager.Update(gameTime); // update the frame count for the animation
        player2.animManager.Update(gameTime); 
    
        // player2.Update(gameTime);

        // base.Update(gameTime);
    }

    
    // ============================================= DRAWING =============================================

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(Background, new Rectangle(-100, -100, 1600, 1000), Color.White);

        DrawMap(gameTime);

        player1.Draw(gameTime, _spriteBatch);
        player2.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected void DrawMap(GameTime gameTime){

        // Tiled works in layers. Just to start, looping through the tiles in the FIRST layer.
        TiledLayer layer = tileMap.Layers[0];
        
        
        for (var y = 0; y < layer.height; y++)
        {
            for (var x = 0; x < layer.width; x++)
            {
                var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                var gid = layer.data[index]; // The tileset tile index
                var tileX = x * tileMap.TileWidth;
                var tileY = y * tileMap.TileHeight;

                if (gid == 0)
                {
                    continue;
                }
               
                // Helper method to fetch the right TieldMapTileset instance
                // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                var mapTileset = tileMap.GetTiledMapTileset(gid);

                // Retrieve the actual tileset based on the firstgid property.

                TiledTileset currTileset = allTilesets[mapTileset.firstgid];

                // Use the connection object as well as the tileset to figure out the source rectangle
                var rect = tileMap.GetSourceRect(mapTileset, currTileset, gid);

                // Create destination and source rectangles
                Rectangle source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                Rectangle destination = new Rectangle(tileX, tileY, tileMap.TileWidth, tileMap.TileHeight);
                
                // -------------------------------------------------------------------------------------- 

                // NOTE: This block of code is for collitions only. More specificaly, this uses
                //       A different method for parsing collisons. It reads collisions by tiles individually rather
                //       than by objects. uncomment if this method is preferred
                // //To access the collision linked to a specific tile. First get the specified tile
                // TiledTile currentTile = tileMap.GetTiledTile(mapTileset, currTileset, gid);
                
                // // Then extract the corresponding object that was tied to the tile
                // TiledObject tileCollisions = currentTile.objects[0];



                _spriteBatch.Draw(tilesetTexture, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            }
        }
    }
}
