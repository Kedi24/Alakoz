using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.GameObjects;
using Alakoz.GameInfo;

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

        public GameObject owner = null; // The entity that owns this hurtbox

        public float left {get {return position.X;} set{}}
        public float right {get {return position.X + width;} set{}}
        public float top {get {return position.Y;} set{}}
        public float bottom {get {return position.Y + height;} set{}}

        public CollisionShape bounds {get; set;}
        
        public Vector2 knockback;
        public int damage {get; set;}
        public int activeFrames = 0;
        public int hitstun {get; set;}
        public int hitstop;

        public List<CollisionObject> collidingObjects;  // To prevent the hitbox from colliding with the same object numerous times in a single frame
        public List<CollisionObject> ignoreObjects; // To prevent the hitbox from ever colliding with certain objects 

        public AnimationManager animManager;
        public bool hitboxVisual = false;

        // -------------- CONSTANTS
        public static readonly Hitbox Zero = new Hitbox(Vector2.Zero, 0f, 0f, 0, Vector2.Zero, 0, 0);
        public const int LIGHT = 3;
        public const int MEDIUM = 7;
        public const int HEAVY = 12;
        public const int KILL = 16;
    
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight, int newActiveFrames, Vector2 newKB, int newDamage, int newHitstun, int newHitstop = LIGHT, GameObject newOwner = null){
            owner = newOwner;
            
            type = CollisionType.HITBOX;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            activeFrames = newActiveFrames;
            knockback = newKB;
            damage = newDamage;
            hitstun = newHitstun;
            active = true;

            collidingObjects = new List<CollisionObject>();   
            ignoreObjects = new List<CollisionObject>();         
            bounds = new CollisionShape(left, top, width, height);

            sprite = CollisionSprites[CollisionType.HITBOX];
        }
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight, int newActiveFrames, Vector2 newKB, int newDamage, int newHitstun, bool visual) 
        :this(newPosition, newWidth, newHeight, newActiveFrames, newKB, newDamage, newHitstun)
        {
            hitboxVisual = visual;
        }
        
        // ========================================================== GENERAL ==========================================================
        public bool isColliding(CollisionObject currObject) { return collidingObjects.Contains(currObject);}
        public bool isIgnore(CollisionObject currObject) { return ignoreObjects.Contains(currObject);}
        
        public void addCollision(CollisionObject currObject) { collidingObjects.Add(currObject);}
        public void addIgnore(CollisionObject toIgnore) { ignoreObjects.Add(toIgnore);}

        public void removeCollision(CollisionObject currObject) { collidingObjects.Remove(currObject); }
        public void removeIgnore(CollisionObject toIgnore) { ignoreObjects.Remove(toIgnore); }

        public void update_Position(Vector2 updatedPosition) { position = updatedPosition; }
        public void setHitstop(int amount){ if (owner == null) return; owner.hitStop = amount;}
        public void setVelocityY(float amount) { if (owner == null) return; owner.velocity.Y = 0;}
        public void setParameters(float newX, float newY, float newWidth, float newHeight, int newActiveFrames, float newKBX, float newKBY, int newDamage, int newHitstun, int newHitstop = LIGHT)
        {
            position.X = newX;
            position.Y = newY;
            width = newWidth;
            height = newHeight;
            activeFrames = newActiveFrames;
            knockback.X = newKBX;
            knockback.Y = newKBY;
            damage = newDamage;
            hitstun = newHitstun;
            hitstop = newHitstop;
        }

        // ========================================================== UPDATING & DRAWING ==========================================================

        public void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            base.Draw(spriteBatch, SpriteEffects.None, Color.White, position, width, height);
        }

        public override void OnCollision(CollisionObject currObject)
        {
             // Use the switch statement to add any collisions unique to the hitbox. Otherwise, just call the base function
            // to prevent redundant code
            switch (currObject.type)
            {
                case CollisionType.ENEMYHURTBOX:
                    enemyHurtboxCollision( (enemyHurtbox) currObject);
                    break;
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

        public void enemyHurtboxCollision(enemyHurtbox currObject) {

            if (owner == null || owner.type != GameObjectType.PLAYER) return;
            bool intersecting = getBounds().isIntersecting(currObject.getBounds());

            if (intersecting && currObject.active)
            {
                // Handle the collision
                if (!isColliding(currObject) && !isIgnore(currObject)) // To prevent hitting multiple times
                {
                    owner.applyAttackBounce = true;
                }
            }
        }
    }
}