using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
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
        public Animation sprite;
        public AnimationManager spriteManager;
        public Player owner; // The entity that owns this hurtbox

        public float left {get {return getPosition().X;} set{;}}
        public float right {get {return getPosition().X + width;} set{;}}
        public float top {get {return getPosition().Y;} set{;}}  
        public float bottom {get {return getPosition().Y + height;} set{;}}
 
        public CollisionShape nextBounds {get {return new CollisionShape(owner.nextPosition.X, owner.nextPosition.Y, width, height);} set{;}}
        
        public bool hurtboxVisual {get; set;}
    
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight)
        { 
            offset = Vector2.Zero;
            position = newPosition + offset; // Top left corner + the amount to offset the hurtbox by
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            type = CollisionType.HURTBOX;
            owner = newOwner;
            currBounds = new CollisionShape(left, top, width, height);
        }
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight, bool visual) :this(newOwner, newPosition, newWidth, newHeight)
        { 
            hurtboxVisual = visual;
        }

        // Constructor Overload, this is so that hurtbox sprites can be visualized 
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight, Animation hurtboxSprite, bool visual) :this(newOwner, newPosition, newWidth, newHeight)
        { 
            sprite = hurtboxSprite;
            spriteManager = new AnimationManager(hurtboxSprite, false); // Setting up hitbox visualization
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
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
            width = owner.hurtboxWidth;
            height = owner.hurtboxHeight;
            scale = new Vector2((width / sprite.frameWidth), (height / sprite.frameHeight));
        }
        public override void OnCollision(CollisionObject currObject)  
        { switch (currObject.type)
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
            }
        }

        // ========================================================== UPDATING & DRAWING ==========================================================
        public void Update(GameTime gameTime)
        {
            update_Position(owner.position);
            if (hurtboxVisual) spriteManager.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 newPosition, SpriteEffects spriteEffects)
        {
            // if (hurtboxVisual) spriteManager.Draw(gameTime, spriteBatch, position, scale, spriteEffects);
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

            float heightOffset = currObject.top - height;

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
                    // Console.WriteLine(currObject.hitstun);
                    // Console.WriteLine(currObject.knockback);

                    currObject.append(this);
                }
            }
            
            if (owner.hitstun == 0 && currObject.isColliding(this))
            {
                    currObject.remove(this);
            }
        }

        public void enemyHurtboxCollision(enemyHurtbox enemyHurtbox) {

            Hitbox enemyHitbox = enemyHurtbox.owner.GetHitbox();
            
            bool intersecting = getBounds().isIntersecting(enemyHitbox.getBounds());
            float heightOffset = enemyHitbox.top - height;

            if (intersecting && enemyHitbox.active)
            {
                // Handle the collision
                if (!enemyHitbox.isColliding(this) && !enemyHitbox.isIgnore(this)) // To prevent hitting multiple times
                {
                    owner.health -= enemyHitbox.damage;
                    owner.hit = true;
                    owner.KB = enemyHitbox.knockback;
                    owner.applyKB = true;
                    owner.hitstun = enemyHitbox.hitstun;
                    // Console.WriteLine(currObject.hitstun);
                    // Console.WriteLine(currObject.knockback);

                    enemyHitbox.append(this);
                }
            }
            
            if (owner.hitstun == 0 && enemyHitbox.isColliding(this))
            {
                    enemyHitbox.remove(this);
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

                if ( owner.position.Y > heightOffset  && owner.position.Y < currObject.bottom) // Player approaches from the left
                {
                    owner.position.X =  currObject.left - width;
                    owner.acceleration = 0;
                    
                    if (owner.move_right) owner.wallCollide = true; // For wall related interactions
                }
                
            } else if (rightintersection)
            {
                // Offset the player horizontally
                if ( owner.position.Y >  heightOffset && owner.position.Y < currObject.bottom) // Player appreaches from the right
                {
                    owner.position.X =  currObject.right;
                    owner.acceleration = 0;

                    if (owner.move_left) owner.wallCollide = true; // for wall related interactions
                }
            };
            
            // --------------- Vertical Intersections
            if ( topintersection ) 
            {   
                if (owner.position.Y <=  heightOffset) // player approaches from above
                { 
                    // Offset the player vertically
                    owner.numJumps = 1;
				    owner.grounded = true;
                    
                    // Modify the acccleration based on whether or not its grounded
				    owner.acceleration = owner.groundAccel;
				    owner.speed = owner.groundSpeed;
				    
                    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
                
            } else if ( bottomintersection )  // Player intersects the bottom of the currObject
            {
                if (owner.position.Y > currObject.bottom)
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
    }
    
}