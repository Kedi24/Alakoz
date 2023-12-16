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
        public Animation sprite;
        
        // ----- DIMENSIONS ----- //
        public Vector2 sendPosition;
        public Vector2 scale {get; set;}
        public Vector2 offset {get; set;}

        // --------------------------------------------------- CONSTRUCTOR ---------------------------------------------------
        public Doorbox(Door newOwner, Vector2 newPosition, float newWidth, float newHeight)
        {
            type = CollisionType.DOOR;
            owner = newOwner;
            position = newPosition;
            width = newWidth;
            height = newHeight;
            sendPosition = newOwner.endPosition;

            sprite = CollisionSprites[CollisionType.HURTBOX];
            scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
            
            // Just for convenience, the collision bounds will be displayed.
            displayBound = true;
        }
        // --------------------------------------------------- GENERAL ---------------------------------------------------
        /// <summary>
        /// The types of collision the Door can interact with. Parse <paramref name="currObject"/> to see if 
        /// it can interact with the Door.
        /// </summary>
        public override void OnCollision(CollisionObject currObject)
        {
            switch (currObject.type)
            {
                case CollisionType.HURTBOX:
                    hurtboxCollision((Hurtbox)currObject);
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            if (displayBound)
            {
                spriteBatch.Draw(
				sprite.Sprite,
				position,
                new Rectangle(sprite.currentFrame * sprite.frameWidth,
					0,
					sprite.frameWidth,
					sprite.frameHeight),
				Color.White,
				0f,
				Vector2.Zero,
				scale,
				spriteEffects,
				0f) ;
            }
        }


        // --------------------------------------------------- COLLISION ---------------------------------------------------
        public override void groundCollision(Ground currObject)
        {
            throw new NotImplementedException();
        }

        public override void hitboxCollision(Hitbox currObject)
        {
            throw new NotImplementedException();
        }

        public override void hurtboxCollision(Hurtbox currObject)
        {
            if (getBounds().isInside(currObject.getBounds())) owner.hovering = true;
            else owner.hovering = false;
        }

        public override void platformCollision(Platform currObject)
        {
            throw new NotImplementedException();
        }
    }
}