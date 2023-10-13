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
	public class Enemy : Species
	{
        		// ------ OTHER ------ //
		public string stateMSG = "";
		public SpriteFont stateFONT { get; set; }

		public float stateTimer;
		public int stateFrame;
		public const float FPS24 = AnimationManager.FPS24;

		public string movementMSG = "";
        public AnimationManager animManager;
        public Dictionary<string, Animation> animDictionary;
        public SpriteEffects flip;

        // ------ MOVEMENT ------- //
        public new Vector2 position;
        public Vector2 startPosiiton;
        public new Vector2 velocity;
        public float distance;
        public float distanceTraveled;
		public new Vector2 currPosition;
		public new Vector2 nextPosition;
		// Override for enemies, friendlies and player// 
		public new float gravity = 0.4f;  
		public new float speed;
		public new float airSpeed = 1f;
		public new float fallSpeed = 1f;
		public new float groundSpeed = 1f;
		// Keep values below for now - change later if needed
		public new float velocityMax = 3f;
		public new float terminalVelocity = 10f;
		public new float accelMax = 10f;
		public new float accelMin = 0.1f;
		public new float acceleration = 0.2f; // Default acceleration
		public new float deceleration = 0.2f; // Default deceleration
		public new float airAccel = 0.2f;
		public new float airDecel = 0.2f;
		public new float groundAccel = 0.3f;
		public new float groundDecel = 0.1f;

		public new int direction = 1;
		public new int numJumps = 1;
		public bool switchero = false;
		public new bool move_left = false;
		public new bool move_right = false;
		public new bool jumping = false;
		public new bool dashing = false;
		public new bool attacking = false;
		public new bool crouching = false;

		public new bool grounded = false;
		public new bool hit = false;

		public new int numDashes;
		public new int dashCooldown;
		public new int hitstun;
		public new int health;

		public new List<CollisionObject> activeCollisions = new List<CollisionObject>();
		
		// ------ COLLISION ------- //

		public new enemyHurtbox hurtbox {get; set;}
		public new Hitbox hitbox {get; set;}
		public new Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public new Vector2 KB; // The knockback from the hitbox that interescts the player
		public new bool applyKB = false;
		float hurtboxWidth = 34;
		float hurtboxHeight = 45;
		public new bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite

		// ------ STATES ------- //
		public new string currentState;
		public new string previousState;

		public new const string IDLE = "idle";

		public new const string JUMP = "jump";
		public new const string AIR = "air";

		public new const string RUN = "run";
		public new const string RUN_END = "run_end";
		public new const string TURNAROUND = "turnaround";
		
		public new const string CROUCH = "crouch";

		public new const string DASH = "dash";

		public new const string ATTACK = "attack";

		public new const string HIT = "hit";

		public new const string SKILL = "skill";
		public Enemy(Dictionary<string, Animation> newSprites ,Vector2 newPosition)
		{
            position = newPosition;
            startPosiiton = newPosition;
            hurtbox = new enemyHurtbox(this, position, hurtboxWidth, hurtboxHeight, newSprites["Hurtbox"], true);
            speed = 2f;
            distanceTraveled = 100f;

            animDictionary = newSprites;
            animManager = new AnimationManager(newSprites["ball"], true);
            animManager.Position = position;

			activeCollisions.Add(hurtbox);
            flip = SpriteEffects.None;
        }

		public Hitbox GetHitbox() {
			Hitbox enemyHitbox = new Hitbox(position, hurtbox.width, hurtbox.height, 1, new Vector2(0,-10), 10, 20);
			enemyHitbox.active = true;
			return enemyHitbox;
		}
        public void set_positions()
		{
			currPosition = position;

			float tempVelocityX = velocity.X + (speed * acceleration);
			float tempVelocityY = velocity.Y + (speed * gravity);

            nextPosition.X = position.X + tempVelocityX;
            nextPosition.Y = position.Y + tempVelocityY;
		}

        public override void update_time(GameTime gameTime)
        {
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stateTimer >= FPS24)
            {
                stateTimer = FPS24 - stateTimer;
                stateFrame++;	
            }
			if (stateFrame >= 240) stateFrame = 0;
        }

        private void move()
        {
            if (direction == 0) 
			{
                if (velocity.X > -velocityMax) {
                    velocity.X -= speed * acceleration;
                }
                else velocity.X = -velocityMax;
                distance -= velocity.X;
            }
            else if (direction == 1) 
			{
                if (velocity.X < velocityMax) {
                    velocity.X += speed * acceleration;
                } 
                else velocity.X = velocityMax;
                distance += velocity.X;
            }
            Console.Write("Distance: " + distance);
            if (Math.Abs(distance) >= distanceTraveled && switchero) {
                if (direction == 0) direction = 1;
                else if (direction == 1) direction = 0;
                distance = 0;
				switchero = false;
            }
			else if (Math.Abs(distance) <= 2) switchero = true;
        }
		private void jump()
        {
		    throw new NotImplementedException();
		}
		
		private void fall()
		{
            if (velocity.Y < terminalVelocity) velocity.Y += fallSpeed * gravity;
            else velocity.Y = terminalVelocity;
		}
		
		private void decelerate()
		{
            throw new NotImplementedException();
		}
		
		private void knockback()
		{
            throw new NotImplementedException();
		} 

		public void air_state()
		{
            throw new NotImplementedException();
		}
		
		public void idle_state()
		{
            throw new NotImplementedException();
		}
		
		public void run_state()
		{
            throw new NotImplementedException();
		}
		
		public void hit_state()
		{
            throw new NotImplementedException();
		}

        public override void update_input()
        {
            throw new NotImplementedException();
        }

        public override void update_state()
        {   
            if (grounded) move();
            fall();
            set_positions();
        }

        public override void update_physics()
        {
            position.Y += velocity.Y ;
            position.X += velocity.X ;
            Console.WriteLine("Velocity: " + velocity);
            hurtbox.update_Position(position);

            if (position.Y >= 1000f || health == 0) 
			{
				position.Y = 0f;
				position.X = 400f; 
				velocity.Y = 0f; 
				acceleration = 0f;
				health = 100;
			}

        }

        public override void update_animations()
        {
            throw new NotImplementedException();
        }
        
        public void Update(GameTime gameTime) { 
            update_physics();
            hurtbox.Update(gameTime);
            update_time(gameTime);
        }

		public void DrawEnemy(GameTime gameTime, SpriteBatch spritebatch)
		{
			// Draw enemy animation
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// Draw the enemy hurtbox, will change later
			hurtbox.Draw(gameTime, spritebatch, position, flip);
		}
        
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

			// // Messages to display game / player information
			// stateMSG = "Current State: " + currentState 
			// + "\nPrevious State: " + previousState
			// + "\nFrame Time (24FPS): " + stateFrame + " Frames" 
			// + "\nGrounded: " + grounded.ToString()
			// + "\nCooldown: " + dashCooldown
			// + "\nHealth: " + health
			// + "\nHitstun: " + hitstun;
			
			// movementMSG = "Position: " + position.ToString() 
			// + "\nVelocity: " + velocity.ToString()
            // + "\nAcceleration: " + acceleration.ToString() 
			// + "\nDirection: " + direction.ToString()
			// + "\nGravity: " + gravity.ToString();
			
			// string inputMSG = 
			// "Left = " + move_left.ToString() 
			// + "\nRight = " + move_right.ToString()
			// + "\nJumping = " + jumping
			// + "\nDashing = " + dashing
			// + "\nCrouching = " + crouching
			// + "\nGrounded = " + grounded
			// + "\nAttacking = " + attacking
			// + "\nHit = " + hit
			// + "\nIgnore Size: " + hitbox.ignoreObjects.Count;
			DrawEnemy(gameTime, spriteBatch);

            // spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(800f, 400), Color.DarkRed);
            // spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			// spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(5f, 550), Color.Orange);

			
        }
    }
}

