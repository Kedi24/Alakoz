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
    
    public class Hurtbox : CollisionObject{
        public Vector2 origin; 
        public Vector2 scale {get; set;}
        public Vector2 offset {get; set;}
        public Player owner; // The entity that owns this hurtbox

        public float left {get {return getPosition().X;} set{;}}
        public float right {get {return getPosition().X + width;} set{;}}
        public float top {get {return getPosition().Y;} set{;}}  
        public float bottom {get {return getPosition().Y + height;} set{;}}
 
        
        public bool hurtboxVisual {get; set;}
    
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight)
        { 
            type = CollisionType.HURTBOX;
            owner = newOwner;
            
            // Dimensions
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            offset = Vector2.Zero; // Hurbox and player position are the same
            position = newPosition + offset; // Top left corner + the amount to offset the hurtbox by

            // Collision
            currBounds = new CollisionShape(left, top, width, height);
            
            // Animation
            sprite = CollisionSprites[CollisionType.HURTBOX];
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
        }
        
        // Constructor Overload to visualize collision object bounds. 
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight, bool visual) :this(newOwner, newPosition, newWidth, newHeight)
        { 
            hurtboxVisual = visual;
        }

        // ========================================================== GENERAL ==========================================================       
        public override Vector2 getPosition() { return owner.currPosition + offset; } // Current hurtbox Position
        public Vector2 getNextPosition() { return owner.nextPosition + offset; } // Next hurtbox Position
        public override CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); } // Get the bounds based on the current position
        public CollisionShape getNextBounds() {return new CollisionShape(getNextPosition().X, getNextPosition().Y, width, height); } // Get the bounds based on the next position
        public void update_Position(Vector2 updatedPosition) { position = updatedPosition + offset; } // Update hurtbox position
        public void changeDimensions(Vector2 newOffset, float newWidth, float newHeight) // Change hurtbox dimensions
        {
            offset = newOffset;
            width = newWidth;
            height = newHeight;
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
        }
        public void resetDimensions() // Reset hurtbox dimensions
        {
            offset = Vector2.Zero;
            position = owner.position;
            width = owner.hurtboxWidth;
            height = owner.hurtboxHeight;
            scale = new Vector2((width / sprite.frameWidth), (height / sprite.frameHeight));
        }
        public override void OnCollision(CollisionObject currObject)  
        { 
            switch (currObject.type)
            {
                case CollisionType.HURTBOX:
                    hurtboxCollision((Hurtbox)currObject);
                    break;
                case CollisionType.HITBOX:
                    hitboxCollision((Hitbox)currObject);
                    break;
                case CollisionType.GROUND:
                    groundCollision((Ground)currObject);
                    break;
                case CollisionType.PLATFORM:
                    platformCollision((Platform)currObject);
                    break;
                case CollisionType.ENEMYHURTBOX:
                    enemyHurtboxCollision((enemyHurtbox)currObject);
                    break;
                case CollisionType.DOOR:
                    doorCollision( (Doorbox) currObject);
                    break;
            }
        }

        // ========================================================== UPDATING & DRAWING ==========================================================
        public void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
                base.Draw(spriteBatch, SpriteEffects.None, Color.White, position, width, height);
            
                // To draw the NextPosition
                spriteBatch.Draw(
				sprite.Sprite,
				owner.nextPosition,
                new Rectangle(0 * sprite.frameWidth,
					0,
					sprite.frameWidth,
					sprite.frameHeight),
				Color.GreenYellow,
				0f,
				Vector2.Zero,
				scale,
				spriteEffects,
				0f) ;
            
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
                    resetDimensions();
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

        public void enemyHurtboxCollision(enemyHurtbox enemyHurtbox) {

            // Hitbox enemyHitbox = enemyHurtbox.owner.GetHitbox();
            
            // bool intersecting = getBounds().isIntersecting(enemyHitbox.getBounds());

            // if (intersecting && enemyHitbox.active)
            // {
            //     // Handle the collision
            //     if (!enemyHitbox.isColliding(this) && !enemyHitbox.isIgnore(this)) // To prevent hitting multiple times
            //     {
            //         owner.health -= enemyHitbox.damage;
            //         owner.hit = true;
            //         owner.KB = enemyHitbox.knockback;
            //         owner.applyKB = true;
            //         owner.hitstun = enemyHitbox.hitstun;
                    
            //         // if (owner.health <= 0) owner.hitStop = 15; 
            //         // else owner.hitStop = 7;

            //         enemyHitbox.append(this);
            //     }
            // }
            
            // if (owner.hitstun == 0 && enemyHitbox.isColliding(this))
            // {
            //         enemyHitbox.remove(this);
            // }
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

                    if (owner.move_right || owner.currentState == StateType.WALLCLING || owner.currentState == StateType.WALLJUMP) owner.wallCollide = true; // For wall related interactions
                }
                
            } else if (rightintersection)
            {
                // Offset the player horizontally
                if ( owner.currPosition.Y > heightOffset && owner.currPosition.Y < currObject.bottom) // This check if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    owner.position.X =  currObject.right;
                    owner.velocity.X = 0;

                    if (owner.move_left || owner.currentState == StateType.WALLCLING || owner.currentState == StateType.WALLJUMP ) owner.wallCollide = true; // for wall related interactions
                };
            };
            
            // --------------- Vertical Intersections
            if ( topintersection )  
            {   
                if (owner.position.X > currObject.left - width && owner.position.X < currObject.right) // player approaches from above
                { 
                    // Reset ground values
				    owner.grounded = true;
                    
                    // Modify the acccleration based on whether or not its grounded
				    // owner.acceleration = owner.groundAccel;
				    // owner.speed = owner.groundSpeed;
				    
                    // Offset the player vertically
                    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
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
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
            }
        }
    
        // ========================================================== OBJECT COLLISIONS ==========================================================
        
        //--------------------------------------------------------- Door
        public void doorCollision( Doorbox currObject )
        {
            bool intersecting = getBounds().isIntersecting(currObject.getBounds());

            if (intersecting && owner.interacting && !owner.beginInteract)
            {
                owner.beginInteract = true;
                owner.interactFunction += currObject.owner.sendState;
                currObject.owner.open = true;
            }
        }
    }
    
}