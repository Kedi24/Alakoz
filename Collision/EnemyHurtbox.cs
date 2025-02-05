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
    public class EnemyHurtbox : Hurtbox{
        Vector2 nextPosition;
        new Enemy owner; // The entity that owns this hurtbox
        public EnemyHurtbox(Enemy newOwner, Vector2 newPosition, float newWidth, float newHeight): base(newOwner, newPosition, newWidth, newHeight)
        { 
            position = newPosition; // Top left corner + the amount to offset the hurtbox by
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            type = TCollision.ENEMYHURTBOX;
            owner = newOwner;
        }

        #region // ========== Helpers ========== //       
        public override Vector2 getPosition() { return owner.position; }
        public override CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }
        public override bool OnCollision(CollisionObject currObject)  {
            nextPosition = owner.position + currentStep*(owner.velocity / velocitySteps);

            switch (currObject.type)
            {
                case TCollision.HITBOX:
                    return hitboxCollision((Hitbox)currObject);
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
        public bool hitboxCollision(Hitbox currObject){   
            bool[] intersecting = CollisionShape.testIntersection(
                owner.position.X, owner.position.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );

            if (intersecting[0] && currObject.active){
                if (!currObject.isColliding(this) && !currObject.isIgnore(this)) // To prevent hitting multiple times
                {
                    owner.health -= currObject.damage;
                    owner.hit = true;
                    owner.KB = currObject.knockback;
                    owner.applyKB = true;
                    owner.hitstun = currObject.hitstun;
                    // resetDimensions();
                    if (owner.health <= 0){
                        owner.hitStop = (int)THitstop.KILL;
                        currObject.setHitstop((int)THitstop.KILL);
                    } 
                    else {
                        owner.hitStop = currObject.hitstop;
                        currObject.setHitstop(currObject.hitstop);
                    }
                    currObject.addCollision(this);
                }
            }
            
            if ((owner.hitstun == 0 || !currObject.active) && currObject.isColliding(this)){
                currObject.removeCollision(this);
            }
            return intersecting[0];
        }

        public override bool groundCollision(Ground currObject) 
        {
            bool[] intersectionList = CollisionShape.testIntersection(
                nextPosition.X, nextPosition.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );
            bool leftintersection = intersectionList[1]; 
            bool rightintersection = intersectionList[2];
            bool topintersection = intersectionList[3];
            bool bottomintersection = intersectionList[4];

            float heightOffset = currObject.top - height;

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
                if ( owner.position.Y > heightOffset && owner.position.Y < currObject.bottom) // This check if the player is approaching from above/below, if so, prioritize vertical intersections
                {
                    owner.position.X =  currObject.right;
                    owner.velocity.X = 0;
                };
            };
            
            // --------------- Vertical Intersections
            if ( topintersection )  
            {   
                if ((owner.position.Y + height) <= currObject.position.Y)
                { 
				    owner.grounded = true;
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
            } else if ( bottomintersection )  // Player intersects the bottom of the currObject
            {
                if (owner.position.Y >= (currObject.position.Y + currObject.height))
                {
                    owner.velocity.Y = 0;
                    owner.position.Y = currObject.bottom;
                }
            };
            return intersectionList[0];
        }
        
        public override bool platformCollision(Platform currObject)
        {
            bool topIntersection = CollisionShape.topIntersection(
                nextPosition.X, nextPosition.Y, width, height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );

            if ( topIntersection ) 
            {
                if ((owner.position.Y + height) <= currObject.position.Y)
                { 
				    owner.grounded = true;
                    owner.velocity.Y = 0;
                    owner.position.Y =  currObject.top - owner.hurtboxHeight;
                }
            }
            return topIntersection;
        }
        #endregion
    }
    
}