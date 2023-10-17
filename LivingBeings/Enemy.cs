using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
using Alakoz.Collision;
using Alakoz.GameInfo;

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
        public SpriteEffects flip;

        // ------ MOVEMENT ------- //
        public Vector2 startPosition;
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
		public new int health = 20;
		
		
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

		// ------ ANIMATION ------ //
		public Dictionary<StateType, Animation> animations;
		public StateType currentAnimation;
		public StateType previousAnimation;
		public StateType tempAnimation;
		public ArrayList preAnimations;

		// ------ STATES ------- //
		public new StateType currentState;
		public new StateType previousState;


		public Enemy(Dictionary<StateType, Animation> newSprites ,Vector2 newPosition)
		{
            position = newPosition;
            startPosition = newPosition;
            hurtbox = new enemyHurtbox(this, position, hurtboxWidth, hurtboxHeight, newSprites[StateType.HURTBOX], true);
            speed = 2f;
			distance = 0f;
            distanceTraveled = 100f;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[StateType.FALL], true);
            animManager.Position = position;
			currentAnimation = StateType.FALL;
			preAnimations = new ArrayList();

			activeCollisions.Add(hurtbox);
            flip = SpriteEffects.None;
        }

		public Hitbox GetHitbox() {
			Hitbox enemyHitbox = new Hitbox(position, hurtbox.width, hurtbox.height, 1, new Vector2(0,-10), 10, 20);
			enemyHitbox.active = true;
			return enemyHitbox;
		}
		// ========================================== PHYSICS ==========================================

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
		private void move()
        {
            if (direction == 0) 
			{
                if (velocity.X > -velocityMax) velocity.X -= speed * acceleration;
                else velocity.X = -velocityMax;
                
				distance -= velocity.X;
            }
            else if (direction == 1) 
			{
                if (velocity.X < velocityMax) velocity.X += speed * acceleration;
                else velocity.X = velocityMax;
                
				distance += velocity.X;
            }
		}
		private void switchDirection()
		{
			if (direction == 0) direction = 1;
			else direction = 0;
			distance = 0;
			switchero = false;
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
			if (!hit)
			{
				if (!(move_left || move_right))
				{
					if (direction == 0) 
					{
						if (velocity.X < 0) velocity.X += speed * deceleration;
						else velocity.X = 0;
					}
					else if (direction == 1) 
					{
						if (velocity.X > 0) velocity.X -= speed * deceleration;
						else velocity.X = 0;
					}
				}
			} else
			{
				if (velocity.X < 0) 
				{
					velocity.X += speed * deceleration;
					if (velocity.X > 0 )velocity.X = 0;
				}
				else if (velocity.X > 0) 
				{
					velocity.X -= speed * deceleration;
					if (velocity.X < 0 ) velocity.X = 0;
				}
				else velocity.X = 0;
			}
		}
		private void knockback()
		{
            velocity.X = KB.X;
			velocity.Y = KB.Y;
			applyKB = false;
		} 
		
		// ========================================== STATES ==========================================
		public void air_state()
		{
           	if (grounded)
			{
				velocity.Y = 0;
				tempAnimation = StateType.IDLE;
				
				set_state(StateType.IDLE);
				idle_state();
			}
			else tempAnimation = StateType.FALL;
		}
		
		public void idle_state()
		{
            if (!grounded)
			{
				set_state(StateType.AIR);
				tempAnimation = StateType.FALL;
				air_state();
			}
			else 
			{
				tempAnimation = StateType.RUN;
				set_state(StateType.RUN);
				run_state();
			}
		}
		
		public void run_state()
		{
            if (!grounded)
			{
				tempAnimation = StateType.FALL;
				set_state(StateType.AIR);
				air_state();
			}
			else
			{
				tempAnimation = StateType.RUN;
				         
				// Swap directions when max distance has been reached
				if (switchero) switchDirection(); 
				move();
			}
		}
		
		public void hit_state()
		{
            if (hitstun == 0) 
			{
				hit = false;
				tempAnimation = StateType.FALL;
				set_state(StateType.AIR);
				air_state();
				return;
			}
			else
			{
				move_left = false;
				move_right = false;
				jumping = false;
				crouching = false;

				if (applyKB) 
				{
					if (tempAnimation == currentAnimation)  // To make sure the impact frames play when hit again DURING hitstun
					{
						preAnimations = new ArrayList(){StateType.SYMBOL};
						set_preAnimations();
					}
					knockback();
				}
			}
		}

		// ========================================== SETTERS ==========================================
        // Set the current and next positions based on velocity for collision checks
		public void set_positions()
		{
			currPosition = position;

			float tempVelocityX = velocity.X + (speed * acceleration);
			float tempVelocityY = velocity.Y + (speed * gravity);

            nextPosition.X = position.X + tempVelocityX;
            nextPosition.Y = position.Y + tempVelocityY;
		}
       
		/// Changes the enemies current state, setting it to <param name="newState"> while recording the previous state.
		/// Also resets the stateFrame count.
        public void set_state(StateType newState)
        {
			if (currentState != newState)
			{
				previousState = currentState; 
				stateFrame = 0;
			}
            currentState = newState;
        }
		
		/// Play a set of non looping animations. This functions keeps the base looping animations the same
		public void set_preAnimations()
		{
			animManager.Clear(); // Empty the stack for new animations
			animManager.Add( animations[currentAnimation] ); // Add the current state animation to the bottom of the stack

			for (int i = 0; i < preAnimations.Count; i++) // add the transitional animations
			{ 
				if (preAnimations.Count == 0) break;
				animManager.Add( animations[ (StateType) preAnimations[i]] ); 
			}
			animManager.Play(); // Pop and play the animation at the top of the stack

			preAnimations.Clear(); // Clear remaining pre-animations for next game tick
		}
			   
		// ========================================== UPDATING ==========================================
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
		
		public override void update_input()
        {
			if (Math.Abs(distance) >= distanceTraveled) switchero = true;
			else switchero = false;

			if (grounded)
			{
				if (direction == 0)
				{	
					move_left = true; 
					move_right = false;
				} else
				{
					move_left = false; 
					move_right = true;;
				}
			} else 
			{
				move_left = false; 
				move_right = false;
			}
        }

        public override void update_state()
        {   
            if (hit) 
			{	
				tempAnimation = StateType.SYMBOL;
				set_state(StateType.HIT);
				hit_state();			
			}
			switch (currentState)
			{
				case StateType.AIR:
					air_state();
					break;
				case StateType.IDLE:
					idle_state();
					break;
				case StateType.RUN:
					run_state();
					break;
			}
			grounded = false;
			decelerate();
			fall();
			set_positions(); // Update the "position to be drawn" of the player
        }

        public override void update_physics()
        {
			// Flipping
            if (direction == 0) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

            position.Y += velocity.Y ;
            position.X += velocity.X ;

            hurtbox.update_Position(position);

            if (position.Y >= 2000f || health <= 0) 
			{
				position.Y = startPosition.Y;
				position.X = startPosition.X; 
				velocity.Y = 0f; 
				acceleration = 0f;
				health = 20;
			}
			update_cooldowns();

        }

		public void update_cooldowns()
		{
			// -------- Hitstun
			if (hit && hitstun > 0) hitstun -= 1;
			else 
			{
				hitstun = 0;
				hit = false;
			}
			// -------- Health
			if (health < 0) health = 0;
		}
        
		public override void update_animations()
        {
			// Set the main animation to play
			if (currentAnimation != tempAnimation) 
			{
				previousAnimation = currentAnimation;
				currentAnimation = tempAnimation;
				set_preAnimations();
			}	
        }
        
        public void Update(GameTime gameTime) { 
            update_physics();
            hurtbox.Update(gameTime);
            update_time(gameTime);
        }

		// ========================================== DRAWING ==========================================
		public void DrawEnemy(GameTime gameTime, SpriteBatch spritebatch)
		{
			// Draw enemy animation
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// Draw the enemy hurtbox, will change later
			hurtbox.Draw(gameTime, spritebatch, position, flip);
		}
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

			// Messages to display game / player information
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nState Time (24FPS): " + stateFrame
			+ "\nAnimation: " + currentAnimation
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun;
			
			movementMSG = "Position: " + position.ToString() 
			+ "\nHurtbox: " + hurtbox.position.ToString()
			+ "\nVelocity: " + velocity.ToString()
            + "\nAcceleration: " + acceleration.ToString() 
			+ "\nDirection: " + direction.ToString()
			+ "\nDistance: " + distance.ToString()
			+ "\nTraveled: " + distanceTraveled.ToString()
			+ "\nGravity: " + gravity.ToString();
			
			string inputMSG = 
			"Left = " + move_left
			+ "\nRight = " + move_right
			+ "\nJumping = " + jumping
			+ "\nSwitching = " + switchero
			+ "\nGrounded = " + grounded
			+ "\nAttacking = " + attacking
			+ "\nHit = " + hit;
			DrawEnemy(gameTime, spriteBatch);

            // spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(800f, 400), Color.DarkRed);
            // spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			// spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(5f, 550), Color.Orange);

			
        }
    }
}

