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
        public float left {get {return position.X;} set{}}
        public float right {get {return position.X + width;} set{}}
        public float top {get {return position.Y;} set{}}
        public float bottom {get {return position.Y + height;} set{}}
    
        public Platform(Vector2 newPosition, float newWidth, float newHeight){
            type = TCollision.PLATFORM;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
        }
        #region // ========== Collision ========== //
        public override bool groundCollision(Ground currObject)
        {
            return false;
        }
    
        public override bool platformCollision(Platform currObject)
        {
            return false;
        }
        #endregion
    }        
}
