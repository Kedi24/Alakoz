using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.GameInfo;

namespace Alakoz.Collision
{
    // A representaion for a type of collision
    // Collsions can come in many types, namely
    // - Hitbox 
    // - Hurtbox
    // - Grabbox
    // to name a few.
    
    public class Platform : CollisionObject{
        
        public Vector2 origin;

        public float left {get {return position.X;} set{}}

        public float right {get {return position.X + width;} set{}}

        public float top {get {return position.Y;} set{}}
        
        public float bottom {get {return position.Y + height;} set{}}
    
        public Platform(Vector2 newPosition, float newWidth, float newHeight){
            type = CollisionType.PLATFORM;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);

        }

        public override void hurtboxCollision(Hurtbox hurtbox)
        {
            // platforms themselves dont do anything when touched by a hurtbox
        }

        public override void hitboxCollision(Hitbox currObject)
        {

        }

        public override void groundCollision(Ground currObject)
        {

        }
    
        public override void platformCollision(Platform currObject)
        {

        }
    }        
}
