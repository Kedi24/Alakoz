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
    public class Hurtbox : CollisionObject{
        public Vector2 scale {get; set;}
        public Vector2 offset {get; set;}

        public float left {get {return getPosition().X;} set{;}}
        public float right {get {return getPosition().X + width;} set{;}}
        public float top {get {return getPosition().Y;} set{;}}  
        public float bottom {get {return getPosition().Y + height;} set{;}}
        public GameObject owner; // The entity that owns this hurtbox
    
        public Hurtbox(GameObject newOwner, Vector2 newPosition, float newWidth, float newHeight)
        { 
            type = TCollision.HURTBOX;
            owner = newOwner;
            
            // Dimensions
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            offset = Vector2.Zero; // Hurbox and player position are the same
            position = newPosition + offset; // Top left corner + the amount to offset the hurtbox by
            
            // // Animation
            sprite = CollisionSprites[TCollision.HURTBOX];
        }

        #region // ========== Helpers ========== //    
        public override Vector2 getPosition() { return owner.position  + offset; }
        public Vector2 getNextPosition() { return owner.nextPosition + offset; }
        public override CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }
        public CollisionShape getNextBounds() { return new CollisionShape(getNextPosition().X, getNextPosition().Y, width, height); }
        public void update_Position(Vector2 updatedPosition) { position = updatedPosition + offset; }
        public void changeDimensions(Vector2 newOffset, float newWidth, float newHeight)
        {
            offset = newOffset;
            width = newWidth;
            height = newHeight;
            scale = new Vector2(newWidth / sprite.Width, newHeight / sprite.Height);
        }
        public void resetDimensions()
        {
            offset = Vector2.Zero;
            position = owner.position;
            width = owner.hurtboxWidth;
            height = owner.hurtboxHeight;
            scale = new Vector2(width / sprite.Width, height / sprite.Height);
        }
        public override bool OnCollision(CollisionObject currObject)  
        {
            switch (currObject.type)
            {
                case TCollision.GROUND:
                    return groundCollision((Ground)currObject);
                case TCollision.PLATFORM:
                    return platformCollision((Platform)currObject);
                default:
                    return false;
            }
        }
        #endregion
        
        #region // ========== Collisions ========== //
        public override bool groundCollision(Ground currObject) 
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
            if (leftintersection) {
                if ( owner.position.Y > heightOffset && owner.position.Y < currObject.bottom){ // This checks if the player is approaching from above/below, if so, prioritize vertical intersections
                    owner.position.X =  currObject.left - width;
                    owner.velocity.X = 0;
                }
                
            } else if (rightintersection){
                // Offset the player horizontally
                if ( owner.position.Y > heightOffset && owner.position.Y < currObject.bottom){ // This check if the player is approaching from above/below, if so, prioritize vertical intersections
                    owner.position.X =  currObject.right;
                    owner.velocity.X = 0;
                };
            };
            
            // --------------- Vertical Intersections
            if ( topintersection ){   
                if (owner.position.X > currObject.left - width && owner.position.X < currObject.right){ // player approaches from above
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
                // ############################################################# NEED TO FIX BOTTOM COLLISION 
                // There is a bug where, if the wall is thin enough, left and right intersections take precedence over bottom collision. 
            } else if ( bottomintersection ){  // Player intersects the bottom of the currObject
                if (owner.position.X > currObject.left - width && owner.position.X < currObject.right){
                    owner.velocity.Y = 0;
                    owner.position.Y = currObject.bottom;
                }
            };

            return leftintersection || rightintersection || topintersection || bottomintersection;
        }
        public override bool platformCollision(Platform currObject)
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
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
            }
            return topintersection;
        }
        #endregion
    }
   
}