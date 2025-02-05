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
    public class Hitbox : CollisionObject{
        public Vector2 scale;
        public GameObject owner = null; // The entity that owns this hurtbox

        public float left {get {return position.X;} set{}}
        public float right {get {return position.X + width;} set{}}
        public float top {get {return position.Y;} set{}}
        public float bottom {get {return position.Y + height;} set{}}

        public CollisionShape bounds {get; set;}
        
        public Vector2 knockback;
        public int damage {get; set;}
        public int activeFrames = 0;
        public int hitstun {get; set;}
        public int hitstop;
        public bool hitboxVisual = false;
    
        public Hitbox(Vector2 newPosition, float newWidth, float newHeight, int newActiveFrames, Vector2 newKB, int newDamage, int newHitstun, THitstop newHitstop = THitstop.LIGHT, GameObject newOwner = null){
            owner = newOwner;
            
            type = TCollision.HITBOX;
            position = newPosition; // Top left corner
            width = newWidth;
            height = newHeight;
            origin = new Vector2(newWidth / 2, newHeight / 2);
            scale = Vector2.One; // Default scaling

            activeFrames = newActiveFrames;
            knockback = newKB;
            damage = newDamage;
            hitstun = newHitstun;
            hitstop = (int)newHitstop;
            active = true;

            collidingObjects = new List<CollisionObject>();   
            ignoreObjects = new List<CollisionObject>();         
            bounds = new CollisionShape(left, top, width, height);

            sprite = CollisionSprites[TCollision.HITBOX];
        }
        #region // ========== Helpers ========== //
        public void update_Position(Vector2 updatedPosition) { position = updatedPosition; }
        public void setHitstop(int amount){ if (owner == null) return; owner.hitStop = amount;}
        public void setParameters(float newX, float newY, float newWidth, float newHeight, int newActiveFrames, float newKBX, float newKBY, 
        int newDamage, int newHitstun, THitstop newHitstop = THitstop.LIGHT)
        {
            position = new Vector2(newX, newY);
            width = newWidth;
            height = newHeight;
            activeFrames = newActiveFrames;
            knockback.X = newKBX;
            knockback.Y = newKBY;
            damage = newDamage;
            hitstun = newHitstun;
            hitstop = (int)newHitstop;
        }

        public override bool OnCollision(CollisionObject currObject){
            switch (currObject.type)
            {
                // case TCollision.ENEMYHURTBOX:
                //     enemyHurtboxCollision( (enemyHurtbox) currObject);
                //     break;
                default:
                    return base.OnCollision(currObject);
            }
        }
        #endregion
        
        #region // ========== Collisions ========== //
        public void enemyHurtboxCollision(EnemyHurtbox currObject) {

            if (owner == null || owner.type != TObject.PLAYER) return;
            bool intersecting = getBounds().isIntersecting(currObject.getBounds());

            if (intersecting && currObject.active){
                // Handle the collision
                if (!isColliding(currObject) && !isIgnore(currObject)) // To prevent hitting multiple times
                {
                    owner.applyAttackBounce = true;
                }
            }
        }
                public override bool groundCollision(Ground currObject){
            return false;
        }
    
        public override bool platformCollision(Platform currObject){
            return false;
        }
        #endregion
    }
}