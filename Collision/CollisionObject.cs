using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Alakoz.Animate;
using Alakoz.GameInfo;
using Alakoz.GameObjects;

namespace Alakoz.Collision
{
    // A representaion for a type of collision
    // Collsions can come in many types, namely
    // - Hitbox 
    // - Hurtbox
    // - Grabbox
    // to name a few.
    
    public abstract class CollisionObject
    {
        // ----- PHYSICS ----- // 
        public Vector2 position;
        public float width {get; set;}
        public float height {get; set;}

        // ----- COLLISION ----- //
        public bool active;
        public CollisionType type {get; set;}
        public CollisionShape currBounds;
        public Animation sprite;

        // ----- DISPLAY ----- //
    
        public virtual Vector2 getPosition() { return position; }
        public virtual CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }

        // ----- ANIMATION ----- //
        public static Dictionary<CollisionType, Animation> CollisionSprites = GameObjectAsset.CollisionAnimations;
        
        // ========================================== DRAWING ==========================================
        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects, Color color, Vector2 position, float newWidth, float newHeight)
        {
            sprite = CollisionSprites[type];
            Vector2 scale = new Vector2((newWidth / sprite.frameWidth), (newHeight / sprite.frameHeight));
            // To draw the next position
            spriteBatch.Draw(
				sprite.Sprite,
				position,
                new Rectangle(0 * sprite.frameWidth,
					0,
					sprite.frameWidth,
					sprite.frameHeight),
				color,
				0f,
				Vector2.Zero,
				scale,
				spriteEffects,
				0f);
        }

        // ========================================== GENERAL ==========================================
        // Finds the corresponding collision method
        public virtual void OnCollision(CollisionObject currObject) 
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
            }

        }
        // ========================================== COLLISION CODE ==========================================
        public abstract void hurtboxCollision(Hurtbox currObject);

        public abstract void hitboxCollision(Hitbox currObject);

        public abstract void groundCollision(Ground currObject);
    
        public abstract void platformCollision(Platform currObject);
    }
}