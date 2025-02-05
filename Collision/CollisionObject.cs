using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Alakoz.Animate;
using Alakoz.GameInfo;
using Alakoz.GameObjects;
using System.Linq;
using MonoGame.Extended;

namespace Alakoz.Collision
{
    public abstract class CollisionObject
    {
        #region  // ----- Physics Variables ----- //
        public Vector2 position {get; set;}
        public Vector2 origin {get; set;}
        public float width {get; set;}
        public float height {get; set;}
        public float velocitySteps = 1; // For fast motions ( > 16px). Indicates the number of parts the path is broken into
        public float currentStep = 1;
        #endregion

        #region  // ----- Collision Variables ----- //
        public bool active;
        public bool linked = false;
        public TCollision type {get; set;}
        public List<CollisionObject> collidingObjects = new List<CollisionObject>(); // To prevent the hitbox from colliding with the same object numerous times in a single frame
        public List<CollisionObject> ignoreObjects = new List<CollisionObject>(); // To prevent the hitbox from ever colliding with certain objects (Like itself) 
        public List<CollisionObject> linkedObjects = new List<CollisionObject>(); // Added for Player, can generalize later
        public List<CollisionObject> oldCollidingObjects = new List<CollisionObject>(); // Added for Player, can generalize later
        #endregion

        #region // ----- Animation Variables ----- //
        public Animation sprite;
        public static Dictionary<TCollision, Animation> CollisionSprites = GameAsset.CollisionAnimations;
        #endregion
        
        #region // ========== Drawing ========= //
        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects, Color color, Vector2 position, float newWidth, float newHeight){
            sprite = CollisionSprites[type];
            Vector2 scale = new Vector2(newWidth / sprite.Width, newHeight / sprite.Height);

            if (active){
                spriteBatch.Draw(
                    sprite.Sprite,
                    position,
                    new Rectangle(
                        0,
                        0,
                        sprite.Width,
                        sprite.Height),
                    color,
                    0f,
                    Vector2.Zero,
                    scale,
                    spriteEffects,
                    0f);
            }
        }

        public virtual void DrawCollision(SpriteBatch spriteBatch, SpriteEffects spriteEffects, Color color)
        {
            // sprite = CollisionSprites[type];
            // Vector2 scale = new Vector2((width / sprite.Width), (height / sprite.Height));
            // spriteBatch.FillRectangle(position.X, position.Y, width, height, color);
            if (active) spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, width, height), color, 1, 0);
        }

        #endregion
        
        #region // ========== Helpers ========== //
        public virtual bool OnCollision(CollisionObject currObject) 
        {
            if (!active) return false;
            switch (currObject.type)
            {
                case TCollision.GROUND:
                    return groundCollision((Ground)currObject);
                case TCollision.PLATFORM:
                    return platformCollision((Platform)currObject);
                default: 
                    return false;
            }
        }

        public virtual Vector2 getPosition() { return position; }
        public virtual CollisionShape getBounds() { return new CollisionShape(getPosition().X, getPosition().Y, width, height); }
        
        public bool isColliding(CollisionObject currObject) { return collidingObjects.Contains(currObject); }
        public bool isIgnore(CollisionObject currObject) { return ignoreObjects.Contains(currObject);}
        public bool isLinked(CollisionObject currObject) { return linkedObjects.Contains(currObject);}
        
        public void addCollision(CollisionObject currObject) 
        {
            if (currObject.linked) foreach(CollisionObject link in currObject.linkedObjects) if (!collidingObjects.Contains(link)) collidingObjects.Add(link);
            if (!isColliding(currObject) ) 
            {   
                collidingObjects.Add(currObject);
            }
        }
        public void addIgnore(CollisionObject toIgnore) { if (!isIgnore(toIgnore) ) ignoreObjects.Add(toIgnore);}
        public void addLink(CollisionObject newObject) 
        { 
            if (linkedObjects.Contains(newObject)) return;
            
            linkedObjects.Union(newObject.linkedObjects); // Combine both links
            newObject.linkedObjects.Union(linkedObjects); // Combine both links

            linkedObjects.Add(newObject);
            newObject.linkedObjects.Add(this);

            newObject.oldCollidingObjects = newObject.collidingObjects; // Store the original list of collisions
            newObject.oldCollidingObjects.Clear(); // Clear the original list

            newObject.collidingObjects = collidingObjects; // Store the shared reference to all collisions in this group
            newObject.linked = true;
            linked = true;
        }
        
        public void removeCollision(CollisionObject currObject) { collidingObjects.Remove(currObject); }
        public void removeIgnore(CollisionObject toIgnore) { ignoreObjects.Remove(toIgnore); }
        public void removeLink() 
        {
            collidingObjects = oldCollidingObjects;

            foreach (CollisionObject link in linkedObjects) link.linkedObjects.Remove(this);
            linkedObjects.Clear();
            linked = false;
        }

        public void clearCollision(){ collidingObjects.Clear();}
        #endregion
        
        #region // ========== Collisions ========== //
        public abstract bool groundCollision(Ground currObject);
    
        public abstract bool platformCollision(Platform currObject);
        #endregion
    }
}