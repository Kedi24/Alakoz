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
using System.Reflection.Metadata.Ecma335;

namespace Alakoz.Collision
{
    // A representaion for a type of collision
    // Collsions can come in many types, namely
    // - Hitbox 
    // - Hurtbox
    // - Grabbox
    // to name a few.
    
    public class Hurtbox : CollisionObject{
        public Vector2 position {get {return owner.position;} set{;}}
        
        public Vector2 origin;

        public Vector2 scale {get; set;}

        public float left {get {return owner.position.X;} set{;}}

        public float right {get {return owner.position.X + width;} set{;}}

        public float top {get {return position.Y;} set{;}}
        
        public float bottom {get {return position.Y + height;} set{;}}
        
        public float width {get; set;}
        
        public float height {get; set;}

        public Animation sprite;

        public AnimationManager spriteManager;

        public Player owner; // The entity that owns this hurtbox
    
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight)
        { 

            position = newPosition; // Top left corner + the amount to offset the hurtbox by
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            type = HURTBOX;
            owner = newOwner;
        }

        // Constructor Overload, this is so that hurtbox sprites can be visualized 
        public Hurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight, Animation hurtboxSprite) :this(newOwner, newPosition, newWidth, newHeight)
        { 
            sprite = hurtboxSprite;
            spriteManager = new AnimationManager(hurtboxSprite, true); // Setting up hitbox visualization
            
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
        }

        public void update_Position(Vector2 updatedPosition)
        {
            position = updatedPosition;
        }

        public override void OnCollision(CollisionObject currObject)
        {
            // Call the parent version to reduce redundant code
            base.OnCollision(currObject);
        }

        public void Update(GameTime gameTime)
        {
            spriteManager.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            spriteManager.Draw(gameTime, spriteBatch, position, scale, spriteEffects);
        }

        
        // --------------------------------------------------- COLLISION CODE ---------------------------------------------------

        public override void hurtboxCollision(Hurtbox currObject)
        {
        }

        // ------------- Hurtbox - Hitbox collision
        public override void hitboxCollision(Hitbox currObject)
        {
            // Hurtbox - Hitbox collision goes here. Speciically what happens to the hurtbox
        }
        
        // ------------- Hurtbox - Ground collision
        public override void groundCollision(Ground currObject) 
        {
            // Positions useful for dealing with edge cases. where the player intersects on a corner, equal amounts on the x and y axis
            float oLeft = currObject.left;
            float oRight = currObject.right;
            float oTop = currObject.top;
            float oBottom = currObject.bottom; 

            float heightOffset = oTop - height;

            // Positions of the next position based on velocity, acceleration and hurtbox size
            float nextLeft = owner.nextPosition.X;
            float nextRight = owner.nextPosition.X + width;
            float nextTop = owner.nextPosition.Y;
            float nextBottom = owner.nextPosition.Y + height; 
            
            // Boolean values for each type of intersection
            
            bool leftIntersection = ( nextRight >= oLeft && nextLeft <=  oLeft &&  nextTop <=  oBottom && nextBottom >=  oTop ); // Player intersects the left of the currObject
            bool rightIntersection = ( nextLeft <= oRight && nextRight >= oRight && nextTop <=  oBottom && nextBottom >=  oTop ); // Player intersects the right of the currObject
            bool topIntersection = ( nextBottom >=  oTop && nextTop <=  oTop && nextRight >=  oLeft && nextLeft <= oRight ); // Player intersects the top of the currObject
            bool bottomIntersection = ( nextTop <=  oBottom && nextBottom >=  oBottom && nextRight >=  oLeft && nextLeft <=  oRight );  // Player intersects the bottom of the currObject

            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */

            // ------------- Horizontal Intersections
            if (leftIntersection) 
            {
                if ( owner.position.Y > ( heightOffset)) // For intersectionsThis means that the player is approaching form below
                {
                    owner.position.X =  oLeft - width;
                    owner.acceleration = 0;
                    // Console.WriteLine("Left Intersection...");
                }
                
            } else if (rightIntersection)
            {
                // Offset the player horizontally
                if ( owner.position.Y >  ( heightOffset)) // This means that the player is approaching from above
                {
                    owner.position.X =  oRight;
                    owner.acceleration = 0;
                    // Console.WriteLine("Right Intersection...");
                }
            };
            
            // --------------- Vertical Intersections
            if ( topIntersection ) 
            {   
                if (owner.position.Y <=  heightOffset) // player approaches from above
                { 
                    // Offset the player vertically
                    owner.numJumps = 1;
				    owner.grounded = true;
				    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y =  oTop - height;
                }
                // ############################################################# NEED TO FIX BOTTOM COLLISION 
            } else if ( bottomIntersection )  // Player intersects the bottom of the currObject
            {
                if (owner.position.Y > oBottom)
                {
                    owner.position.Y = oBottom;
                }
            };
        }

        /* Platforms only detect top collision when interacting with a hurtbox */
        public override void platformCollision(Platform currObject)
        {
            // Positions useful for dealing with edge cases. where the player intersects on a corner, equal amounts on the x and y axis
            float oLeft = currObject.left;
            float oRight = currObject.right;
            float oTop = currObject.top;
            float oBottom = currObject.bottom; 

            float heightOffset = oTop - height;

            // Positions of the next position based on velocity, acceleration and hurtbox size
            float nextLeft = owner.nextPosition.X;
            float nextRight = owner.nextPosition.X + width;
            float nextTop = owner.nextPosition.Y;
            float nextBottom = owner.nextPosition.Y + height; 
            
            // Boolean values for each type of intersection
            bool topIntersection = ( nextBottom >=  oTop && nextTop <=  oTop && nextRight >=  oLeft && nextLeft <= oRight ); // Player intersects the top of the currObject


            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */
            
            // --------------- Vertical Intersections
            if ( topIntersection ) 
            {
                if (owner.position.Y <=  ( heightOffset)){ // player approaches from above

                    // Offset the player vertically
                    owner.numJumps = 1;
				    owner.grounded = true;
				    owner.jumping = false;
                    owner.velocity.Y = 0;
                    owner.position.Y =  oTop - height;
                }
            }
        }
    }
}