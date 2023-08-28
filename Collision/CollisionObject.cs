using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Species;
using Alakoz.Collision;


using TiledCS;

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
        
        public bool active;
    
        public string type;
    
        public const string HURTBOX = "hurtbox"; // Hurtbox collision type
    
        public const string HITBOX = "hitbox"; // Hitbox collision type
     
        public const string GROUND = "ground"; // Flat ground collision type

        public const string PLATFORM = "platform"; 

        public string getType() 
        {
            return type;
        }

        // Finds the corresponding collision method
        public virtual void OnCollision(CollisionObject currObject) 
        {
            switch (currObject.type)
            {
                case HURTBOX:
                    hurtboxCollision((Hurtbox)currObject);
                    break;
                case HITBOX:
                    hitboxCollision((Hitbox)currObject);
                    break;
                case GROUND:
                    groundCollision((Ground)currObject);
                    break;
                case PLATFORM:
                    platformCollision((Platform)currObject);
                    break;
            }

        }
        // --------------------------------------------------- COLLISION CODE ---------------------------------------------------

        public abstract void hurtboxCollision(Hurtbox currObject);

        public abstract void hitboxCollision(Hitbox currObject);

        public abstract void groundCollision(Ground currObject);
    
        public abstract void platformCollision(Platform currObject);
    }
}