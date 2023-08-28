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
    
    public class Platform : CollisionObject{
        public Vector2 position = new Vector2(0f, 0f);
        
        public Vector2 origin;

        public float left {get {return position.X;} set{}}

        public float right {get {return position.X + width;} set{}}

        public float top {get {return position.Y;} set{}}
        
        public float bottom {get {return position.Y + height;} set{}}
        
        public float width {get; set;}
        
        public float height {get; set;}

        public Texture2D sprite;
    
        public Platform(Vector2 newPosition, float newWidth, float newHeight){
            type = PLATFORM;
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

        // ################################################### OLD STUFF JUST INCASE 
        public void OldCollisionCode(Player player)
        {
        // Unlike ground collision, platforms only detect interesctions with the top of the object   

            float heightOffset = top - player.hurtbox.height;

            // Positions of the next position based on velocity, acceleration and hurtbox size
            float pLeft = player.nextPosition.X;
            float pRight = player.nextPosition.X + player.hurtbox.width;
            float pTop = player.nextPosition.Y;
            float pBottom = player.nextPosition.Y + player.hurtbox.height; 

            // Player intersects the top of the object
            bool topIntersection = ( pBottom >=  top && pTop <=  top && pRight >=  left && pLeft <=  right ); 

            if ( topIntersection ) 
            {
                if (player.position.Y <= heightOffset){ // player approaches from above

                    // Offset the player vertically
                    player.numJumps = 1;
				    player.grounded = true;
				    player.jumping = false;
                    player.velocity.Y = 0;
                    player.position.Y =  top - player.hurtbox.height;
                };
            };  
        }
    }        
}
