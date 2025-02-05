using Microsoft.Xna.Framework;
using Alakoz.GameObjects;
using Alakoz.GameInfo;

namespace Alakoz.Collision
{
    public class PlayerHurtbox : Hurtbox{
        Player playerRef;
        Vector2 nextPosition;
        public PlayerHurtbox(Player newOwner, Vector2 newPosition, float newWidth, float newHeight) : base(newOwner, newPosition, newWidth, newHeight)
        { 
            type = TCollision.HURTBOX;
            owner = newOwner;
            playerRef = newOwner;
            
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
        public override bool OnCollision(CollisionObject currObject)  
        {
            nextPosition = playerRef.position + offset + currentStep*(playerRef.velocity / velocitySteps);

            switch (currObject.type)
            {
                case TCollision.HITBOX:
                    return hitboxCollision((Hitbox)currObject);
                case TCollision.GROUND:
                    return groundCollision((Ground)currObject);
                case TCollision.PLATFORM:
                    return platformCollision((Platform)currObject);
                case TCollision.DOOR:
                    return doorCollision( (Doorbox) currObject);
                default:
                    return false;
            }
        }
        #endregion

        #region // ========== Collisions ========== //

        #region // -----  Hitbox 
        public bool hitboxCollision(Hitbox currObject)
        {   
            bool[] intersecting = CollisionShape.testIntersection(
                playerRef.position.X, playerRef.nextPosition.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );

            if (intersecting[0] && currObject.active) {
                if (!currObject.isColliding(this) && !currObject.isIgnore(this)) // To prevent hitting multiple times
                {
                    playerRef.health -= currObject.damage;
                    playerRef.hit = true;
                    playerRef.KB = currObject.knockback;
                    playerRef.applyKB = true;
                    playerRef.hitstun = currObject.hitstun;
                    resetDimensions();
                    if (playerRef.health <= 0){
                        playerRef.hitStop = (int)THitstop.KILL;
                        currObject.setHitstop((int)THitstop.KILL);
                    } 
                    else {
                        playerRef.hitStop = currObject.hitstop;
                        currObject.setHitstop(currObject.hitstop);
                    }
                    currObject.addCollision(this);
                }
            }
            
            if ((playerRef.hitstun == 0 || !currObject.active) && currObject.isColliding(this)){
                currObject.removeCollision(this);
            }
            return intersecting[0];
        }
        
        #endregion
        #region // -----  Ground 
        public override bool groundCollision(Ground currObject) 
        {
            bool[] intersectionList = CollisionShape.testIntersection(
                nextPosition.X, nextPosition.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );

            // Intersections depending on the side
            bool leftintersection = intersectionList[1]; 
            bool rightintersection = intersectionList[2];
            bool topintersection = intersectionList[3];
            bool bottomintersection = intersectionList[4];

            float heightOffset = currObject.top - height;
            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */
            // ------------- Horizontal Intersections
            if (leftintersection) 
            {

                if ( playerRef.position.Y > heightOffset && playerRef.position.Y < currObject.bottom) // This checks if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    playerRef.position.X =  currObject.left - width;
                    playerRef.velocity.X = 0;

                    if (playerRef.move_right || playerRef.currentState == TState.WALLCLING || playerRef.currentState == TState.WALLJUMP) playerRef.wallCollide = true; // For wall related interactions
                }
                
            } else if (rightintersection)
            {
                // Offset the player horizontally
                if ( playerRef.position.Y > heightOffset && playerRef.position.Y < currObject.bottom) // This check if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    playerRef.position.X =  currObject.right;
                    playerRef.velocity.X = 0;

                    if (playerRef.move_left || playerRef.currentState == TState.WALLCLING || playerRef.currentState == TState.WALLJUMP ) playerRef.wallCollide = true; // for wall related interactions
                };
            };
            
            // --------------- Vertical Intersections
            if ( topintersection )  
            {   
               if ((playerRef.position.Y + height) <= currObject.position.Y)
                { 
                    // Reset ground values
				    playerRef.grounded = true;
                    
                    // // Modify the acccleration based on whether or not its grounded
				    // playerRef.acceleration = playerRef.groundAccel;
				    // playerRef.speed = playerRef.groundSpeed;
				    
                    // Offset the player vertically
                    playerRef.jumping = false;
                    playerRef.velocity.Y = 0;
                    playerRef.position.Y =  currObject.top - playerRef.hurtboxHeight;
                }
                // ############################################################# NEED TO FIX BOTTOM COLLISION 
                // There is a bug where, if the wall is thin enough, left and right intersections take precedence over bottom collision. 
            } else if ( bottomintersection )  // Player intersects the bottom of the currObject
            {
                if (playerRef.position.Y >= (currObject.position.Y + currObject.height))
                {
                    playerRef.velocity.Y = 0;
                    playerRef.position.Y = currObject.bottom;
                }
            };
            return intersectionList[0];
        }
        
        #endregion
        #region // -----  Platform

        /* Platforms only detect top collision when interacting with a hurtbox */
        public override bool platformCollision(Platform currObject)
        {
            bool topIntersection = CollisionShape.topIntersection(
                nextPosition.X, nextPosition.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );
            /*
            Note that monogame places (0,0) at the top left of the screen and increases right and downwards 
            */

            // --------------- Vertical Intersections
            if ( topIntersection ) 
            {
               if ((playerRef.position.Y + height) <= currObject.position.Y)
                { 
                    // Offset the player vertically
                    playerRef.numJumps = 1;
				    playerRef.grounded = true;
				    playerRef.jumping = false;
                    playerRef.velocity.Y = 0;
                    playerRef.position.Y =  currObject.top - playerRef.hurtboxHeight;
                }
            }
            return topIntersection;
        }
        
        #endregion
        #region // -----  Door
        public bool doorCollision( Doorbox currObject )
        {
            bool intersecting = CollisionShape.insideIntersection(
                playerRef.position.X, playerRef.position.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"    
            );

            if (intersecting && playerRef.interacting && !playerRef.beginInteract)
            {
                playerRef.beginInteract = true;
                playerRef.interactFunction += currObject.owner.sendState;
                currObject.owner.open = true;
            }
            return intersecting;
        }
        #endregion

        #endregion
    }
}