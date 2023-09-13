using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Species;


using TiledCS;

namespace Alakoz.Collision
{
    // A representaion for a type of collision
    // Collsions can come in many types, namely
    // - Hitbox 
    // - Hurtbox
    // - Grabbox
    // to name a few.
    
    public class Hitbox : CollisionObject{
        public Vector2 origin;
        public Vector2 scale;

        public Player owner; // The entity that owns this hurtbox

        public float left {get {return position.X;} set{}}
        public float right {get {return position.X + width;} set{}}
        public float top {get {return position.Y;} set{}}
        public float bottom {get {return position.Y + height;} set{}}

        public CollisionShape bounds {get; set;}
        
        public Vector2 knockback {get; set;}
        public int damage {get; set;}
        public float damageMult = 1;
        public int activeFrames = 0;
        public int hitstun {get; set;}

        public List<CollisionObject> collidingObjects;  // To prevent the hitbox from colliding with the same object numerous times in a single frame
        public List<CollisionObject> ignoreObjects; // To prevent the hitbox from ever colliding with certain objects 

        public Animation sprite;
        public AnimationManager spriteManager;

        public bool hitboxVisual = false;
    
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight, int newActiveFrames, Vector2 newKB, int newDamage, int newHitstun){
            type = HITBOX;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            activeFrames = newActiveFrames;
            knockback = newKB;
            damage = newDamage;
            hitstun = newHitstun;

            collidingObjects = new List<CollisionObject>();   
            ignoreObjects = new List<CollisionObject>();         

            bounds = new CollisionShape(left, top, width, height);
        }
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight, int newActiveFrames, Vector2 newKB, int newDamage, int newHitstun, Animation hitboxSprite, bool visual) 
        :this(newPosition, newWidth, newHeight, newActiveFrames, newKB, newDamage, newHitstun)
        {
            sprite = hitboxSprite;
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
            hitboxVisual = visual;

            spriteManager = new AnimationManager(hitboxSprite, false); // Setting up hitbox visualization
        }
        
        // ========================================================== GENERAL ==========================================================
        public bool isColliding(CollisionObject currObject) { return collidingObjects.Contains(currObject);}
        public bool isIgnore(CollisionObject currObject) { return ignoreObjects.Contains(currObject);}

        public void append(CollisionObject currObject) { collidingObjects.Add(currObject); }
        public void addIgnore(CollisionObject toIgnore) { ignoreObjects.Add(toIgnore); }

        public void remove(CollisionObject currObject) { collidingObjects.Remove(currObject); }
        public void removeIgnore(CollisionObject toIgnore) { ignoreObjects.Remove(toIgnore); }

        public void update_Position(Vector2 updatedPosition) { position = updatedPosition; }
        


        // ========================================================== UPDATING & DRAWING ==========================================================

        public void Update(GameTime gameTime)
        {
            if (hitboxVisual) spriteManager.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (hitboxVisual && active) spriteManager.Draw(gameTime, spriteBatch, position, scale, spriteEffects);
        }

        public override void OnCollision(CollisionObject currObject)
        {
            // Use the switch statement to add any collisions unique to the hitbox. Otherwise, just call the base function
            // to prevent redundant code
            switch (currObject)
            {
                default:
                    base.OnCollision(currObject);
                    break;
            }
        }
        // ========================================================== COLLISION CODE ==========================================================
        
        // --------------------------------------------------------- Hurtbox
        public override void hurtboxCollision(Hurtbox currObject)
        {
        }

        // --------------------------------------------------------- Hitbox
        public override void hitboxCollision(Hitbox currObject)
        {
        }

        // --------------------------------------------------------- Ground 
        public override void groundCollision(Ground currObject)
        {
        }

        // --------------------------------------------------------- Platform
        public override void platformCollision(Platform currObject)
        {
        }
    }
}