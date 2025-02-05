using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.GameInfo;
using Alakoz.Animate;
using Alakoz.Collision;
using Alakoz.GameObjects;

namespace Alakoz.Collision
{
    // The bounds for a Door
    public class Doorbox : CollisionObject
    {
        public Door owner;
        public Vector2 sendPosition;
        public Vector2 scale {get; set;}
        public Vector2 offset {get; set;}

        public Doorbox(Door newOwner, Vector2 newPosition, float newWidth, float newHeight)
        {
            type = TCollision.DOOR;
            owner = newOwner;
            position = newPosition;
            width = newWidth;
            height = newHeight;
            sendPosition = newOwner.endPosition;

            sprite = CollisionSprites[TCollision.HURTBOX];
            scale = new Vector2(newWidth / sprite.Width, newHeight / sprite.Height);
        }

        public override bool OnCollision(CollisionObject currObject)
        {
            switch (currObject.type)
            {
                case TCollision.HURTBOX:
                    return hurtboxCollision((Hurtbox)currObject);
                default:
                    return false;
            }
        }
        
        #region // ========== Collisions ========== // 
        public override bool groundCollision(Ground currObject)
        {
            throw new NotImplementedException();
        }

        public bool hurtboxCollision(Hurtbox currObject)
        {
            bool collision = CollisionShape.isOutside(      
                owner.position.X, owner.position.Y, owner.width, owner.height, "Rectangle",
                currObject.position.X, currObject.position.Y, currObject.width, currObject.height, "Rectangle"
            );
            owner.hovering = collision;
            // else owner.hovering = false;

            return collision;
        }

        public override bool platformCollision(Platform currObject)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}