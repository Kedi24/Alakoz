using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Collision;
using Alakoz.GameInfo;
using Alakoz.GameObjects;

using TiledCS;

namespace Alakoz.GameObjects
{
	public class Enemy : GameObject
	{
        		// ------ OTHER ------ //
		public string stateMSG = "";
		public SpriteFont stateFONT { get; set; }

		public float stateTimer;
		public int stateFrame;

		public string movementMSG = "";
        public SpriteEffects flip;

        // ------ MOVEMENT ------- //
        public Vector2 startPosition;
        public new Vector2 velocity;
        public float distance;
        public float distanceTraveled;
		public Vector2 currPosition;
		public Vector2 nextPosition;
		// Override for enemies, friendlies and player// 
		public float gravity = 0.4f;  
		public float speed;
		public float airSpeed = 1f;
		public float fallSpeed = 1f;
		public float groundSpeed = 1f;
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
		public bool switchero = false;
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
		public int health = 20;
		
		
		// ------ COLLISION ------- //
		public enemyHurtbox hurtbox {get; set;}
		public Hitbox hitbox {get; set;}
		public Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public Vector2 KB; // The knockback from the hitbox that interescts the player
		public bool applyKB = false;
		float hurtboxWidth = 34;
		float hurtboxHeight = 45;
		public bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite

		// ------ ANIMATION ------ //
		public Dictionary<StateType, Animation> animations;
		public StateType currentAnimation;
		public StateType previousAnimation;
		public StateType tempAnimation;
		public ArrayList preAnimations;

		// ------ STATES ------- //
		public StateType currentState;
		public StateType previousState;


		public Enemy(Dictionary<StateType, Animation> newSprites ,Vector2 newPosition)
		{
            position = newPosition;
            startPosition = newPosition;
            speed = 2f;
			distance = 0f;
            distanceTraveled = 100f;

			stateFrame = 0;
			stateTimer = 0f;
			currentState = StateType.AIR;
			previousState = StateType.IDLE;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[StateType.FALL], true);
            animManager.Position = position;
			currentAnimation = StateType.FALL;
			preAnimations = new ArrayList();

			hurtbox = new enemyHurtbox(this, position, hurtboxWidth, hurtboxHeight, true);
			hitbox = new Hitbox(position, hurtboxWidth, hurtboxHeight, 0, new Vector2(0,-10), 10, 20, this);
			hitbox.addIgnore(hurtbox);
			activeCollisions.Add(hurtbox);
			activeCollisions.Add(hitbox);

            flip = SpriteEffects.None;
        }
		// ========================================== PHYSICS ==========================================

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
		private void move()
        {
            if (direction == 0) 
			{
                velocity.X = approach(velocity.X, -velocityMax, speed * acceleration); 
				distance -= velocity.X;
            }
            else if (direction == 1) 
			{
                velocity.X = approach(velocity.X, velocityMax, speed * acceleration);
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
				if (!(move_left || move_right)) velocity.X = approach(velocity.X, 0, speed * deceleration);
			} 
			else
			{
				velocity.X = approach(velocity.X, 0, speed * deceleration);
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

				if (hitStop <= 0 )stateFrame++; // Dont update the frame during hitstop
				else hitStop--;
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

			if (hitStop > 0 ) return;

			fall();
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
			set_positions(); // Update the "position to be drawn" of the player
        }

        public override void update_physics()
        {
			// Dont apply physics during hitstop
			if (hitStop > 0) return;

			// Flipping
            if (direction == 0) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

            position.Y += velocity.Y ;
            position.X += velocity.X ;

			// So that the hurtbox and hitbox follow the enemy
            hurtbox.update_Position(position); 
			hitbox.update_Position(position);

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
        

		// ========================================== DRAWING ==========================================
		public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
		{
			// Pause current frame during hitstop
			if (hitStop > 0) animManager.Pause();
			else animManager.Resume();

			// Draw enemy animation
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// Draw the enemy hurtbox, will change later
			// hurtbox.Draw(spritebatch, flip);
			// hitbox.Draw(spritebatch, flip);

			// Draw debug messages
			// drawDebug(spritebatch);
		}
        
        public void drawDebug(SpriteBatch spriteBatch)
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

            spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(800f, 400), Color.DarkRed);
            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(5f, 550), Color.Orange);

			
        }
    }
}

