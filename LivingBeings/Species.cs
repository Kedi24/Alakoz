using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
using Alakoz.Collision;


using TiledCS;
namespace Alakoz.LivingBeings
{
	
	public abstract class Species
	{

		// ------ MOVEMENT ------- //
        public Vector2 position;
        public Vector2 velocity;
		public Vector2 currPosition;
		public Vector2 nextPosition;
		// Override for enemies, friendlies and player// 
		public float gravity;  
		public float speed;
		public float airSpeed;
		public float fallSpeed;
		public float groundSpeed;
		// Keep values below for now - change later if needed
		public float velocityMax = 3f;
		public float terminalVelocity = 10f;
		public float accelMax = 10f;
		public float accelMin = 0.1f;
		public float acceleration = 0.2f; // Default acceleration
		public float deceleration = 0.2f; // Default deceleration
		public float airAccel = 0.2f;
		public float airDecel = 0.2f;
		public float groundAccel = 0.3f;
		public float groundDecel = 0.1f;

		public int direction = 1;
		public int numJumps = 1;
		public bool move_left = false;
		public bool move_right = false;
		public bool jumping = false;
		public bool dashing = false;
		public bool attacking = false;
		public bool crouching = false;

		public bool grounded = false;
		public bool hit = false;

		public int numDashes;
		public int dashCooldown;
		public int hitstun;
		public int health;

		public List<CollisionObject> activeCollisions = new List<CollisionObject>();
		
		// ------ COLLISION ------- //

		public Hurtbox hurtbox {get; set;}
		public Hitbox hitbox {get; set;}
		public Hitbox hitbox2 {get; set;}
		public Hitbox hitbox3 {get; set;}
		public Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public Vector2 KB; // The knockback from the hitbox that interescts the player
		public bool applyKB = false;
		public bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite

		// ------ ANIMATION ------ //
		public AnimationManager animManager;

		// ------ STATES ------- //
		public string currentState;
		public string previousState;
		// ========================================== UPDATING ==========================================
        // public abstract void Update();

        // ========================================== DRAWING  ==========================================
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
		// ========================================== ABSTRACT METHODS ==========================================
		public abstract void update_time(GameTime gametime);
		public abstract void update_input();
		public abstract void update_state();
		public abstract void update_physics();
		public abstract void update_animations();	
	}
	
}

