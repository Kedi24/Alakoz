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

namespace Alakoz;

public class TestLevel : GameState
{
    #region // ----- Game Info ----- //
    public static TestLevel thisGame;
    public static GameWindow window;
    public static GraphicsDevice graphicsDevice;
    public ContentManager Content;
    private SpriteBatch _spriteBatch;
    private GraphicsDeviceManager _graphics;
    public static int WindowWidth;
    public static int WindowHeight;
    #endregion

    #region // ----- FPS and Timing ----- //
    private float fixedUpdateFreq = (int) (1000/(float)60); // 1000 miliseconds / 30 frames
    private float previousTime = 0;
    private float accumulatedTime = 0.0f;
    private float maxFrameTime = 250;
    public float frameProgress = 0;
    #endregion

    #region // ----- Debugging ----- // 
    bool prevKeyDown = false;
    bool currKeyDown = false;
    bool prevSpace = false;
    bool currSpace = false;
    bool frameAdvance = false;
    #endregion
    
    #region // ----- UI / UX ----- //
    SpriteFont font;
    #endregion

    #region // ----- Game Objects ----- //
    Player player1;
    Camera camera;
    #endregion

    #region // ----- Map ----- //
    Texture2D Background;
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
    #endregion
    
    #region // ----- Collisions ----- //
    List<CollisionObject> allCollisionObjects; // All the collisions to be checked
    int dynamicIndex = 0;
    #endregion

    #region // ========== GAME1 ========== //
    public TestLevel(ContentManager newContent,GraphicsDeviceManager newGraphics, GraphicsDevice newGraphicsDevice, GameWindow Window)
    {
        Content = newContent;
        _graphics = newGraphics;
        graphicsDevice = newGraphicsDevice;
        window = Window;
    }
    #endregion
    
    #region // ========== INITIALIZING ========== //
    public override void Initialize()
    {

        // camera = new Camera(Game1.thisGame.Window, Game1.thisGame.GraphicsDevice, 1080, 720); 
        allGameObjects = new List<GameObject>();
        allCollisionObjects = new List<CollisionObject>();  
    }
    #endregion

    #region // ========== LOADING ========== //
    public override void LoadContent() // TODO: use this.Content to load your game content here
    {
        _spriteBatch = Game1.spriteBatch;
        Console.WriteLine(" >>>>>>>>>> LoadContent: Start");
        Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Sky");
        // Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Dark Wallpaper #1");
        // Background = Content.Load<Texture2D>("Alakoz Content/Backgrounds/Scala Ad Caelum");
        
        font = Content.Load<SpriteFont>("Alakoz Content/Fonts/TestFont");
        camera = new Camera(Game1.thisGame.Window, Game1.thisGame.GraphicsDevice, 1080, 720);
        camera.displayFONT = font;
        // Load Game Object Assests
        Console.WriteLine("Loading Game Assets...");
        GameAsset.LoadPlayerAssets();
        GameAsset.LoadEnemyAssets();
        GameAsset.LoadDoorAssets();
        GameAsset.LoadDebug();
        Door.LoadDoors();

        Console.WriteLine("Loading Map...");
        LoadMap();

        Console.WriteLine(" ---------- LoadContent: OK ----------");
    }
    protected Player LoadPlayer1(Vector2 spawnPoint, int id){
        player1 = new Player(GameAsset.PlayerAnimations, spawnPoint, id);
        player1.stateFONT = font;
        player1.controls.keyboardControls();
        camera.target = player1;
        return player1;
    }
    
    protected Enemy LoadEnemy(Vector2 spawnPoint, int id)
    {

        Enemy enemy = new Enemy(GameAsset.EnemyAnimations, spawnPoint, id);
        
        // ------------------------ Enemy Information Display
        enemy.stateFONT = font;
        // enemy.hurtbox.spriteManager.frameFont = font;

        return enemy;
    }

    protected Door LoadDoor(Vector2 doorPos, float doorWidth, float doorHeight, int doorID, int nextID)
    {
       // Create the new door
        BasicDoor newDoor = new BasicDoor(doorPos, doorWidth, doorHeight, doorID, nextID, GameAsset.DoorAnimations);
        newDoor.stateFONT = font;
        return newDoor;
    }
    protected void LoadMap()
    {
        string testmapDirectory = "Alakoz Content/Maps/TestMaps/";

        // ---------- MAP 1: Desert Level
        // tileMap = new TiledMap(Content.RootDirectory + "/TestMaps/TestLevel - 2.tmx");
        // tileSet = new TiledTileset(Content.RootDirectory + "/TestMaps/Desert_Tileset3.tsx");
        // tilesetTexture = Content.Load<Texture2D>("TestMaps/tmw_desert_spacing");
        // allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + testmapDirectory + "Map - Desert/");

        // ---------- MAP 2: Hitbox Level
        // tileMap = new TiledMap(Content.RootDirectory + testmapDirectory + "Map - Hitbox/Base Level.tmx");
        // tileSet = new TiledTileset(Content.RootDirectory + testmapDirectory + "Map - Hitbox/Base Tileset.tsx");
        // tilesetTexture = Content.Load<Texture2D>(testmapDirectory + "Map - Hitbox/Base_Tileset");
        // allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + testmapDirectory + "Map - Hitbox/");

        // // ---------- MAP 3: Dynamic Level
        tileMap = new TiledMap(Content.RootDirectory + testmapDirectory + "Map - Dynamic/Dynamic Level.tmx"); // Map
        tileSet = new TiledTileset(Content.RootDirectory + testmapDirectory + "Map - Dynamic/Dynamic Tileset.tsx"); // Tileset
        tilesetTexture = Content.Load<Texture2D>(testmapDirectory + "Map - Dynamic/Dynamic_TilesetImage"); // Image
        allTilesets = tileMap.GetTiledTilesets(Content.RootDirectory + testmapDirectory + "Map - Dynamic/");

        tileHeight = tileSet.TileHeight;
        tileWidth = tileSet.TileWidth;

        tilesWide = tileSet.Columns;
        tilesHigh = tileSet.TileCount / tileSet.Columns;

        collisionLayer = tileMap.Layers[1];
        
        collisionLayerObjects = collisionLayer.objects;
        
        LoadMapCollisions();
    }

    // Loads each object from the collision layer of the map and creates corresponding collisionObjects
    protected void LoadMapCollisions()
    {
        // NOTE: The Layers in TILED do not correspond exactly to the tileMap.Layers arrray
        //          TILED | Array Index
        //       TileLayer (0) = Index (0)
        //       Static Layer (1) = Index (3)
        //       Dynamic Layer (2) = Index (2)
        //       Character Layer (3) = Index (1)
        
        // Load all the static collisions from layer[1] first
        Console.WriteLine("Loading Static assests...");
        LoadStaticCollisions();

        // Load all the dynamic collisions from layer[2] next
        Console.WriteLine("Loading Dynamic assests...");
        LoadDynamicCollisions(); 
        
        // Load all the characters from layer[3] next
        Console.WriteLine("Loading Characters...");
        LoadCharacterCollisions();

        // Local functions for clarity
        void LoadStaticCollisions(){
            collisionLayer = tileMap.Layers[3];
            collisionLayerObjects = collisionLayer.objects;
            
            if (collisionLayerObjects.Length <= 0) return;

            // Parsing property information from the object layer to handle collisions
            for (int i = 0; i < collisionLayerObjects.Length; i++)
            {

                TiledObject currentObject = collisionLayerObjects[i]; // Current Object
                TiledProperty currentProperty = currentObject.properties[0]; // Object type

                CollisionObject tempObject; // Current collision Object

                // Ground
                if (currentProperty.value.Equals(TCollision.GROUND.ToString())) tempObject = new Ground(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
                // Platforms
                else if (currentProperty.value.Equals(TCollision.PLATFORM.ToString())) tempObject = new Platform(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
                // Hitboxes
                else if (currentProperty.value.Equals(TCollision.HITBOX.ToString()))
                {
                    float directionX = (float)Convert.ToDouble(currentObject.properties[1].value); // Values are stored as strings so just convert them to floats
                    float directionY = (float)Convert.ToDouble(currentObject.properties[2].value); // Values are stored as strings so just convert them to floats

                    tempObject = new Hitbox(
                        newPosition: new Vector2(currentObject.x, currentObject.y), 
                        newWidth: currentObject.width, 
                        newHeight: currentObject.height, 
                        newActiveFrames: 0, 
                        newKB: new Vector2(directionX, directionY), 
                        newDamage: 5, 
                        newHitstun: 25,
                        newHitstop: THitstop.MEDIUM
                    );

                    tempObject.active = true;
                }
                //Default
                else tempObject = new Ground(new Vector2(currentObject.x, currentObject.y), currentObject.width, currentObject.height);
                allCollisionObjects.Add(tempObject);
            }
            dynamicIndex = allCollisionObjects.Count;
        }
        void LoadDynamicCollisions(){
            collisionLayer = tileMap.Layers[2]; 
            collisionLayerObjects = collisionLayer.objects;
            if (collisionLayerObjects.Length <= 0) return;

            // Parsing property information from the object layer to handle collisions
            for (int i = 0; i < collisionLayerObjects.Length; i++){
                TiledObject currentObject = collisionLayerObjects[i]; // Current Object
                TiledProperty currentProperty = currentObject.properties[0]; // Object type
                GameObject tempObject; // Current collision Object

                // Door
                if (currentProperty.value.Equals(TCollision.DOOR.ToString())) {
                    int sendID = (int)Convert.ToInt32(currentObject.properties[1].value); // ID of the door to send to
                    tempObject = LoadDoor(
                        doorPos: new Vector2(currentObject.x, currentObject.y), 
                        doorWidth: currentObject.width, 
                        doorHeight: currentObject.height, 
                        doorID: currentObject.id, 
                        nextID: sendID
                    );
                }
                else tempObject = LoadDoor(
                    doorPos: new Vector2(currentObject.x, currentObject.y),
                    doorWidth: currentObject.width, 
                    doorHeight: currentObject.height, 
                    doorID: currentObject.id, 
                    nextID: currentObject.id
                );

                allGameObjects.Add(tempObject);
            }
        }
        void LoadCharacterCollisions(){
            collisionLayer = tileMap.Layers[1]; 
            collisionLayerObjects = collisionLayer.objects;
            
            if (collisionLayerObjects.Length <= 0) return;

            // Parsing property information from the object layer to handle collisions
            for (int i = 0; i < collisionLayerObjects.Length; i++){
                TiledObject currentObject = collisionLayerObjects[i]; // Current Object
                TiledProperty currentProperty = currentObject.properties[0]; // Object type
                GameObject tempObject; // Current collision Object

                 // Player
                if (currentProperty.value.Equals(TCollision.PLAYERSPAWN.ToString())) {            
                    tempObject = LoadPlayer1( new Vector2(currentObject.x, currentObject.y), currentObject.id );
                }
                // Enemy
                else if (currentProperty.value.Equals(TCollision.ENEMYSPAWN.ToString())) {           
                    tempObject = LoadEnemy( new Vector2(currentObject.x, currentObject.y), currentObject.id );
                }
                else tempObject = null;
                if (tempObject != null) allGameObjects.Add(tempObject);
           }
        }
    }
    #endregion
    
    #region // ========== UPDATING ========== //

    public void checkCollisions(){
        for (int indexA = dynamicIndex; indexA < allCollisionObjects. Count; indexA++){
            CollisionObject currObject = allCollisionObjects[indexA];
            float steps = currObject.velocitySteps;

            for (int indexB = 0; indexB < allCollisionObjects.Count; indexB++){
                currObject.OnCollision(allCollisionObjects[indexB]);
                allCollisionObjects[indexB].OnCollision(currObject);
            }
            while (steps > 0 ){
                bool collisionOnStep = false; // To check if there is a collision on the step
                for (int indexB = 0; indexB < allCollisionObjects.Count; indexB++){
                    bool res1 = currObject.OnCollision(allCollisionObjects[indexB]);
                    bool res2 = allCollisionObjects[indexB].OnCollision(currObject);
                    collisionOnStep = res1 || res2;
                }

                if (collisionOnStep){ // There was a collision on one of the steps
                    currObject.velocitySteps = 1; // Reset velocity Steps
                    break; // stop looping through steps
                }
                currObject.currentStep += 1;
                steps--;
            }
        }
    }

    public override void Update(GameTime gameTime){
        if (previousTime == 0){
            previousTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
        }

        float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
        float frameTime = now - previousTime;
        if (frameTime > maxFrameTime) frameTime = maxFrameTime;
        previousTime  = now;
        accumulatedTime += frameTime;
        
        while (accumulatedTime >= fixedUpdateFreq){
            // ----- For Frame Advance ----- //
            // Pressing Space will put the game in frame advance mode
            // Use the right arrow key to advance one cycle.
            prevSpace = currSpace;
            currSpace = Keyboard.GetState().IsKeyDown(Keys.Space);
            if (prevSpace && !currSpace) frameAdvance = !frameAdvance;
            if (frameAdvance){
                prevKeyDown = currKeyDown;
                currKeyDown = Keyboard.GetState().IsKeyDown(Keys.Right);
                if (prevKeyDown && !currKeyDown) UpdateObjects();
            }
            else UpdateObjects();
            accumulatedTime -= fixedUpdateFreq;
        }
        frameProgress = accumulatedTime / fixedUpdateFreq;
    }

    private void UpdateObjects(){
        foreach (GameObject entity in allGameObjects){
            entity.update_time();
            entity.update_input();
            entity.update_state();
            foreach (CollisionObject entityCollision in entity.activeCollisions) { if (!allCollisionObjects.Contains(entityCollision)) allCollisionObjects.Add(entityCollision); }
        }
        checkCollisions();

        foreach (GameObject entity in allGameObjects){
            entity.update_physics();
            entity.update_animations();
        }
        camera.Update();
    }
    #endregion

    #region // ========== DRAWING ========== //
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime){
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.transformation);
        spriteBatch.Draw(Background, new Rectangle(-100, -100, 1600*2, 1000*2), Color.White);
        DrawMap(spriteBatch, gameTime);

        // Draw Game Objects
        foreach (GameObject entity in allGameObjects) entity.Draw(gameTime, spriteBatch);
        camera.Draw(gameTime, _spriteBatch);
        spriteBatch.End();
    }

    protected void DrawMap(SpriteBatch spriteBatch, GameTime gameTime){

        // Tiled works in layers. Just to start, looping through the tiles in the FIRST layer.
        TiledLayer layer = tileMap.Layers[0];
        for (var y = 0; y < layer.height; y++){
            for (var x = 0; x < layer.width; x++)
            {
                var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                var gid = layer.data[index]; // The tileset tile index
                var tileX = x * tileMap.TileWidth;
                var tileY = y * tileMap.TileHeight;

                if (gid == 0) continue;
               
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

                // NOTE: This block of code is for tile collisions only. More specificaly, this uses
                //       A different method for parsing collisons. It reads collisions by tiles individually rather
                //       than by objects. uncomment if this method is preferred
                
                // //To access the collision linked to a specific tile. First get the specified tile
                // TiledTile currentTile = tileMap.GetTiledTile(mapTileset, currTileset, gid);
                
                // // Then extract the corresponding object that was tied to the tile
                // TiledObject tileCollisions = currentTile.objects[0];

                spriteBatch.Draw(tilesetTexture, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }
    }
    #endregion
}
