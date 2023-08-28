using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Species;


using TiledCS;

namespace Alakoz.Collision
{
    // A representaion for a type of collision
    // Collsions can come in many types, namely
    // - Hitbox 
    // - Hurtbox
    // - Grabbox
    // to name a few.
    
    public class Hitbox : CollisionObject{
        public Vector2 position = new Vector2(0f, 0f);
        
        public Vector2 origin;

        public float left {get {return position.X;} set{}}

        public float right {get {return position.X + width;} set{}}

        public float top {get {return position.Y;} set{}}
        
        public float bottom {get {return position.Y + height;} set{}}
        
        public float width {get; set;}
        
        public float height {get; set;}
        
        public float KB;

        public float damage;

        public float damageMult = 1;

        public Texture2D sprite;
    
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight){
            type = HITBOX;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);

        }

        public override void OnCollision(CollisionObject currObject)
        {
            
            if (damageMult == 1){
                Console.WriteLine("Hitbox");
                damageMult -= 1;
            };
        }

        // --------------------------------------------------- COLLISION CODE ---------------------------------------------------
        public override void hurtboxCollision(Hurtbox currObject)
        {

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