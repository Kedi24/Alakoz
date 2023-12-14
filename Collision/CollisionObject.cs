using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Alakoz.Animate;
using Alakoz.GameInfo;

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

        // ----- DISPLAY ----- //
        public bool displayBound = false;
    
        public virtual Vector2 getPosition() { return position; }
        public virtual CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }

        // ----- ANIMATION ----- //
        public static Dictionary<CollisionType, Animation> collisionSprites {get; set;}

        // ========================================== LOADING ==========================================
        public static void LoadVisuals(ContentManager Content)
        {
            string effectDirectory = "Alakoz Content/Effects/General/";

            Animation hurtboxSprite = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation hitboxSprite = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            collisionSprites = new Dictionary<CollisionType, Animation>
            {
                { CollisionType.HURTBOX, hurtboxSprite },
                { CollisionType.HITBOX, hitboxSprite }
            };
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