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
    
    public class enemyHurtbox : CollisionObject{
        public Vector2 origin;
        public Vector2 scale {get; set;}
        public AnimationManager spriteManager;
        public Enemy owner; // The entity that owns this hurtbox

        public float left {get {return getPosition().X;} set{;}}
        public float right {get {return getPosition().X + width;} set{;}}
        public float top {get {return getPosition().Y;} set{;}}  
        public float bottom {get {return getPosition().Y + height;} set{;}}
 
        public CollisionShape nextBounds {get {return new CollisionShape(owner.nextPosition.X, owner.nextPosition.Y, width, height);} set{;}}
        
        public bool hurtboxVisual {get; set;}
    
        public enemyHurtbox(Enemy newOwner, Vector2 newPosition, float newWidth, float newHeight)
        { 
            position = newPosition; // Top left corner + the amount to offset the hurtbox by
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            type = CollisionType.ENEMYHURTBOX;
            owner = newOwner;
            currBounds = new CollisionShape(left, top, width, height);
        }
        public enemyHurtbox(Enemy newOwner, Vector2 newPosition, float newWidth, float newHeight, bool visual) :this(newOwner, newPosition, newWidth, newHeight)
        { 
            hurtboxVisual = visual;
        }

        // ========================================================== GENERAL ==========================================================       
        public override Vector2 getPosition() { return owner.currPosition; }
        public Vector2 getNextPosition() { return owner.nextPosition; }
        public override CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }
        public CollisionShape getNextBounds() {return new CollisionShape(getNextPosition().X, getNextPosition().Y, width, height); }
        public void update_Position(Vector2 updatedPosition) { position = updatedPosition; }
        public override void OnCollision(CollisionObject currObject)  { base.OnCollision(currObject); }

        // ========================================================== UPDATING & DRAWING ==========================================================

        public void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            base.Draw(spriteBatch, SpriteEffects.None, Color.White, position, width, height);
        }

        // ========================================================== COLLISION CODE ==========================================================

        // --------------------------------------------------------- Hurtbox
        public override void hurtboxCollision(Hurtbox currObject)
        {
        }

        // --------------------------------------------------------- Hitbox 
        public override void hitboxCollision(Hitbox currObject)
        {   
            bool intersecting = getBounds().isIntersecting(currObject.getBounds());

            if (intersecting && currObject.active)
            {
                // Handle the collision
                if (!currObject.isColliding(this) && !currObject.isIgnore(this)) // To prevent hitting multiple times
                {
                    owner.health -= currObject.damage;
                    owner.hit = true;
                    owner.KB = currObject.knockback;
                    owner.applyKB = true;
                    owner.hitstun = currObject.hitstun;
                    owner.applyFall = true;
                    currObject.owner.applyAttackBounce = true;

                    if (owner.health <= 0)
                    {
                        owner.hitStop = Hitbox.KILL;
                        currObject.setHitstop(Hitbox.KILL);
                    } 
                    else 
                    {
                        owner.hitStop = currObject.hitstop;
                        currObject.setHitstop(currObject.hitstop);
                    }
                    
                    currObject.addCollision(this);
                }
            }
            
            if ((owner.hitstun == 0 || !currObject.active) && currObject.isColliding(this))
            {
                    currObject.removeCollision(this);
            }
        }
        
        // --------------------------------------------------------- Ground 
        public override void groundCollision(Ground currObject) 
        {
            // Intersections depending on the side
            bool leftintersection = getNextBounds().leftIntersection(currObject.getBounds());
            bool rightintersection = getNextBounds().rightIntersection(currObject.getBounds());
            bool topintersection = getNextBounds().topIntersection(currObject.getBounds());
            bool bottomintersection = getNextBounds().bottomIntersection(currObject.getBounds());

            float heightOffset = currObject.top - height;

            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */

            // ------------- Horizontal Intersections
            if (leftintersection) 
            {

                if ( owner.position.Y > heightOffset && owner.position.Y < currObject.bottom) // This checks if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    owner.position.X =  currObject.left - width;
                    owner.velocity.X = 0;
                }
                
            } else if (rightintersection)
            {
                // Offset the player horizontally
                if ( owner.currPosition.Y > heightOffset && owner.currPosition.Y < currObject.bottom) // This check if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    owner.position.X =  currObject.right;
                    owner.velocity.X = 0;
                };
            };
            
            // --------------- Vertical Intersections
            if ( topintersection ) 
            {   
                if (owner.position.X > currObject.left - width && owner.position.X < currObject.right) // player approaches from above
                { 
                    // Reset ground values
				    owner.grounded = true;
                    
                    // // Modify the acccleration based on whether or not its grounded
				    // owner.acceleration = owner.groundAccel;
				    // owner.speed = owner.groundSpeed;
				    
                    // Offset the player vertically
                    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - height;
                }
                // ############################################################# NEED TO FIX BOTTOM COLLISION 
            } else if ( bottomintersection )  // Player intersects the bottom of the currObject
            {
                if (owner.position.X > currObject.left - width && owner.position.X < currObject.right)
                {
                    owner.velocity.Y = 0;
                    owner.position.Y = currObject.bottom;
                }
            };
        }
        
        // --------------------------------------------------------- Platform

        /* Platforms only detect top collision when interacting with a hurtbox */
        public override void platformCollision(Platform currObject)
        {
            bool topintersection = getNextBounds().topIntersection(currObject.getBounds());
            float heightOffset = currObject.top - height;
            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */

            // --------------- Vertical Intersections
            if ( topintersection ) 
            {
                if (owner.position.Y <=  heightOffset)// player approaches from above
                { 
                    // Offset the player vertically
                    owner.numJumps = 1;
				    owner.grounded = true;
				    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y = currObject.top - owner.hurtboxHeight;
                }
            }
        }
    }
    
}