using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
using Alakoz.Collision;
using Alakoz.GameInfo;

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
    Enemy enemy;
    Dictionary<StateType, Animation> player1Animations;
    Dictionary<StateType, Animation> player2Animations;
    Dictionary<StateType, Animation> enemyAnimations;
    
    Vector2 Player1Pos;
    Vector2 Player2Pos;
    Vector2 enemyPos;
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
        Content.RootDirectory = "Content/";
        IsMouseVisible = true;
    }
    // ============================================= INITIALIZING =============================================
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

        allCollisionObjects = new List<CollisionObject>();
        player1Animations = new Dictionary<StateType, Animation>();
        player2Animations = new Dictionary<StateType, Animation>();
        enemyAnimations = new Dictionary<StateType, Animation>();

        SymbolPosition = new Vector2(400, 0);
        Player1Pos = new Vector2(400, 0);
        enemyPos = new Vector2(300, 300);
        // Player2Pos = new Vector2(500, 0);
        
        base.Initialize();
    }


    // ============================================= LOADING =============================================
    protected override void LoadContent() // TODO: use this.Content to load your game content here
    {
        Console.WriteLine("Loading Content...");
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Sky");
        // Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Dark Wallpaper #1");
        // Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Scala Ad Caelum");
        
        // font = Content.Load<SpriteFont>("Alakoz Content/Fonts/TestFont");
        
        Console.WriteLine("Loading Map...");
        LoadMap();

        Console.WriteLine("Loading Character assests...");
        LoadPlayer1();
        LoadEnemy();

        Console.WriteLine(" -------------------- LoadContent: OK --------------------");
    }
    protected void LoadPlayer1()
    {
        string playerDirectory = "Alakoz Content/Species/Player/Rebel_Animations/"; 
        string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";
        string effectDirectory = "Alakoz Content/Effects/General/";
        
        Animation Symbol = new Animation(Content.Load<Texture2D>( enemyDirectory + "ball"), 1);
        Animation playerHurtbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
        Animation playerHitbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

        Animation idle = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Idle"), 40);
        Animation run = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Run"), 20);
        Animation runEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Runstop"), 21, false);

        Animation jumpStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_JumpStart"), 8, false);
        Animation jump = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Jump"), 12);
        Animation falling = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Fall"), 12);
        
        Animation crouchStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchStart"), 4, false);
        Animation crouch = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Crouch"), 24);
        Animation crouchEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchEnd"), 6);
        
        Animation dashStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_DashStart"), 6, false);
        Animation dash = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Dash"), 12);
        Animation dashEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_DashEnd"), 14, false, 0.012f);     

        Animation ballStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BallStart"), 4, false);
        Animation ball = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Ball"), 12, true, 0.012f);
        Animation ballEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BallEnd"), 2, false);

        Animation hitStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_HitStart"), 8, false);
        Animation hit = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Hit"), 16);        

        player1Animations.Add(StateType.SYMBOL, Symbol);
        player1Animations.Add(StateType.HURTBOX, playerHurtbox);
        player1Animations.Add(StateType.HITBOX, playerHitbox);
        player1Animations.Add(StateType.IDLE, idle);

        player1Animations.Add(StateType.RUN, run);
        player1Animations.Add(StateType.RUNEND, runEnd);

        player1Animations.Add(StateType.CROUCH, crouch);
        player1Animations.Add(StateType.CROUCHSTART, crouchStart);
        player1Animations.Add(StateType.CROUCHEND, crouchEnd);

        player1Animations.Add(StateType.JUMPSTART, jumpStart);
        player1Animations.Add(StateType.JUMP, jump);
        player1Animations.Add(StateType.FALL, falling);

        player1Animations.Add(StateType.DASH, dash);
        player1Animations.Add(StateType.DASHSTART, dashStart);
        player1Animations.Add(StateType.DASHEND, dashEnd);
 
        player1Animations.Add(StateType.BALL, ball);
        player1Animations.Add(StateType.BALLSTART, ballStart);
        player1Animations.Add(StateType.BALLEND, ballEnd);

        player1Animations.Add(StateType.HIT, hit);
        player1Animations.Add(StateType.HITSTART, hitStart);
        
        player1 = new Player(player1Animations, Player1Pos);

        // THIS IS FOR THE INFORMATION DISPLAY, MOVE INTO PLAYER CLASS LATER
        // player1.stateFONT = font;
        // player1.animManager.frameFont = font;
        // player1.hurtbox.spriteManager.frameFont = font;

        // Setup the controls
        player1.controls.reset();

        // return player1;
    }
    
    protected void LoadEnemy()
    {
        string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";
        string effectDirectory = "Alakoz Content/Effects/General/";

        Animation Symbol = new Animation(Content.Load<Texture2D>( enemyDirectory + "ball"), 1);
        Animation enemyHurtbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
        Animation enemyHitbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

        Animation idle = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Idle"), 32);
        Animation idle2 = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Idle2"), 8);
        Animation run = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Running"), 22);
        Animation runStop = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_RunStop"), 24, false);
        Animation jump = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Jump"), 10, false);
        Animation falling = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Falling"), 12);

        enemyAnimations.Add(StateType.SYMBOL, Symbol);
        enemyAnimations.Add(StateType.HURTBOX, enemyHurtbox);
        enemyAnimations.Add(StateType.HITBOX, enemyHitbox);
        enemyAnimations.Add(StateType.IDLE, idle);
        enemyAnimations.Add(StateType.RUN, run);
        enemyAnimations.Add(StateType.RUNEND, runStop);
        enemyAnimations.Add(StateType.JUMP, jump);
        enemyAnimations.Add(StateType.FALL, falling);
        enemy = new Enemy(enemyAnimations, enemyPos);
        
        // ------------------------ Enemy Information Display
        // enemy.stateFONT = font;
        // enemy.animManager.frameFont = font;
        // enemy.hurtbox.spriteManager.frameFont = font;

    }

    protected void LoadMap()
    {
        string testmapDirectory = "Alakoz Content/Maps/TestMaps/";

        // MAP 1 - Uncomment to load Map 1 and comment map 2
        // tileMap = new TiledMap(Content.RootDirectory + "/TestMaps/TestLevel - 2.tmx");
        // tileSet = new TiledTileset(Content.RootDirectory + "/TestMaps/Desert_Tileset3.tsx");
        // tilesetTexture = Content.Load<Texture2D>("TestMaps/tmw_desert_spacing");

        // allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + "/TestMaps/");

        // MAP 2 - Uncomment to load Map 2 and comment Map 1
        tileMap = new TiledMap(Content.RootDirectory + testmapDirectory + "Map - Hitbox/Base Level.tmx");
        tileSet = new TiledTileset(Content.RootDirectory + testmapDirectory + "Map - Hitbox/Base Tileset.tsx");
        tilesetTexture = Content.Load<Texture2D>(testmapDirectory + "Map - Hitbox/Base_Tileset");

        allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + testmapDirectory + "Map - Hitbox/");

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
            
            // Ground
            if (currentProperty.value.Equals(CollisionType.GROUND.ToString())) tempObject = new Ground(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
            // Platforms
            else if (currentProperty.value.Equals(CollisionType.PLATFORM.ToString())) tempObject = new Platform(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
            // Hitboxes
            else if (currentProperty.value.Equals(CollisionType.HITBOX.ToString()))
            {
                float directionX = (float)Convert.ToDouble(currentObject.properties[1].value); // Values are stored as strings so just convert them to floats
                float directionY = (float)Convert.ToDouble(currentObject.properties[2].value); // Values are stored as strings so just convert them to floats

                tempObject = new Hitbox(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height, 0, new Vector2(directionX, directionY), 5, 25);
                tempObject.active = true;
            }
            //Default
            else tempObject = new Ground(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);

            allCollisionObjects.Add(tempObject);
        }

    }

    // ============================================= UPDATING =============================================

    public void checkCollisions(){
        // Add player1 active collisions
        for (int i = 0; i < player1.activeCollisions.Count; i++)
        {
            if (!allCollisionObjects.Contains(player1.activeCollisions[i])) allCollisionObjects.Add(player1.activeCollisions[i]);
        }
        for (int i = 0; i < enemy.activeCollisions.Count; i++) {
            if (!allCollisionObjects.Contains(enemy.activeCollisions[i])) allCollisionObjects.Add(enemy.activeCollisions[i]);
        }
        // Add the player2 active collisions
        //for (int i = 0; i < player2.activeCollisions.Count; i++)
        //{
        //    if (!allCollisionObjects.Contains(player2.activeCollisions[i])) allCollisionObjects.Add(player2.activeCollisions[i]);
        //}
        
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
        // player2.update_time(gameTime); 
        enemy.update_time(gameTime);
        
        player1.update_input(); // Read the players input
        //player2.update_input(); 
        enemy.update_input();

        player1.update_state(); // Modify their state
        // player2.update_state();
        enemy.update_state(); 

        checkCollisions(); // Check the collisions 

        player1.update_physics(); // set the final position, velocity, and acceleration of the player
        //player2.update_physics(); 
        enemy.update_physics();

        player1.update_animations();// set the corresponding animation
        //player2.update_player1Animations(); 
        enemy.update_animations();

        player1.animManager.Update(gameTime); // update the frame count for the animation
        // player2.animManager.Update(gameTime); 
        enemy.animManager.Update(gameTime);
    
        // player2.Update(gameTime);
        
        base.Update(gameTime);
        
    }

    
    // ============================================= DRAWING =============================================

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(Background, new Rectangle(-100, -100, 1600, 1000), Color.White);

        DrawMap(gameTime);

        player1.Draw(gameTime, _spriteBatch);
        //player2.Draw(gameTime, _spriteBatch);
        enemy.Draw(gameTime, _spriteBatch);

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
