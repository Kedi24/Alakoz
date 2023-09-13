using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Species;
using Alakoz.Collision;


using TiledCS;

namespace Alakoz.Species
{
	public class Player
	{
		// ------ OTHER ------ //
		public string stateMSG = "";
		public SpriteFont stateFONT { get; set; }

		public float stateTimer;
		public int stateFrame;
		public const float FPS24 = AnimationManager.FPS24;

		public string movementMSG = "";

		// ------ MOVEMENT ------- //
        public Vector2 position;
        public Vector2 velocity;

		public Vector2 currPosition;
		public Vector2 nextPosition;

		public float gravity = 0.4f;
		public float speed = 2f; // Default speed
		public float airSpeed = 1f;
		public float fallSpeed = 1f;
		public float groundSpeed = 1f;
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

		public int numDashes = 1;
		public int dashCooldown = 30;
		public int hitstun = 0;
		public int health = 100;

		// ------ COLLISION ------ //
		public List<CollisionObject> activeCollisions = new List<CollisionObject>();
		
		public Hurtbox hurtbox {get; set;}
		public Hitbox hitbox {get; set;}
		public Hitbox hitbox2 {get; set;}
		public Hitbox hitbox3 {get; set;}
		public Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public Vector2 KB; // The knockback from the hitbox that interescts the player
		public bool applyKB = false;
		float hurtboxWidth = 34;
		float hurtboxHeight = 45;
		public bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite


		// ------ ANIMATION ------ //
		public Dictionary<string, Animation> animDictionary;
		public AnimationManager animManager;
		public string currentAnimation;

		// ------ SKILLS ------ //


		// ------ SOUND ------ //


		// ------ EFFECTS ------ //
		SpriteEffects flip;


		// ------ INPUTS ------ //
		public Controls controls;


		// ##### States ##### // 
		public string currentState;
		public string previousState;

		public const string IDLE = "idle";

		public const string JUMP = "jump";
		public const string AIR = "air";

		public const string RUN = "run";
		public const string RUN_END = "run_end";
		public const string TURNAROUND = "turnaround";
		
		public const string CROUCH = "crouch";

		public const string DASH = "dash";

		public const string ATTACK = "attack";

		public const string HIT = "hit";

		public const string SKILL = "skill";

		public Player(Dictionary<string, Animation> newSprites, Vector2 newPosition)
		{
			currentState = AIR;

			position = newPosition;

			hurtbox = new Hurtbox(this, position, hurtboxWidth, hurtboxHeight, newSprites["Hurtbox"], true);
			
			// Hitboxes just to test primitive attacks. Will modify later
			hitbox = new Hitbox(position + hitboxPosition, 32f, 20f, 3, new Vector2(1f, 0f), 2, 10, newSprites["Hitbox"], true);
			hitbox2 = new Hitbox(position + hitboxPosition, 32f, 30f, 3, new Vector2(1f, 0f), 5, 10, newSprites["Hitbox"], true);
			hitbox3= new Hitbox(position + hitboxPosition, 32f, 40f, 3, new Vector2(5f, -5f), 10, 20, newSprites["Hitbox"], true);
			
			hitbox.active = false;
			hitbox2.active = false;
			hitbox3.active = false;
			
			hitbox.addIgnore(hurtbox); // Prevent the player from hitting themselves
			hitbox2.addIgnore(hurtbox);
			hitbox3.addIgnore(hurtbox);

			activeCollisions.Add(hurtbox);

			stateFrame = 0;
			stateTimer = 0f;

            animDictionary = newSprites;
            animManager = new AnimationManager(newSprites["Base_Idle"], true);
            animManager.Position = position;
			currentAnimation = AIR;

            controls = new Controls();

			flip = SpriteEffects.None;
		}


		// ========================================== PHYSICS FUNCTIONS ==========================================

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
        private void move()
        {
            if (direction == 0) 
			{
				if (velocity.X > -velocityMax) velocity.X -= speed * acceleration;
				else velocity.X = -velocityMax;
			}
            else if (direction == 1) 
			{
				if (velocity.X < velocityMax) velocity.X += speed * acceleration;
				else velocity.X = velocityMax;
			}

        }
        
		private void jump()
        {
			velocity.Y = -8.0f;
			numJumps -= 1;
			
			if (numJumps < 0) numJumps = 0;
			grounded = false;
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
		
		private void dash()
		{
		// dash physics
			if (stateFrame >= 0 && stateFrame <= 3)
			{
				// Console.WriteLine("StateFrame: " + stateFrame);
				
				velocity.X = 0.1f;
			}
			else 
			{
				if (direction == 1) velocity.X = 6f;
				else velocity.X = -6f;
			}
			velocity.Y = 0f;

			acceleration = accelMin;

            numDashes = 0;
			grounded = false;
		}		
		
		private void knockback()
		{
			velocity.X = KB.X;
			velocity.Y = KB.Y;
			applyKB = false;
		} 
		// ------------------------------ Attacks
		private void groundAttack()
		{	
			if (stateFrame == 6)
			{
				hitbox.active = true;
				activeCollisions.Add(hitbox); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 8)
			{
				hitbox.active = false;
				activeCollisions.Remove(hitbox); // Attack has ended
			}
			else if (stateFrame == 10)
			{

				hitbox2.active = true;
				activeCollisions.Add(hitbox2); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 12)
			{
				hitbox2.active = false;
				activeCollisions.Remove(hitbox2); // Attack has ended
				
			}
			else if (stateFrame == 17)
			{

				hitbox3.active = true;
				activeCollisions.Add(hitbox3); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 20)
			{
				hitbox3.active = false;
				activeCollisions.Remove(hitbox3); // Attack has ended
			}
			else if (stateFrame == 25) 
			{
				attacking = false;
				set_state(AIR);
				air_state();
			}
			
		}
		private void airAttack()
		{
			if (stateFrame == 11)
			{
				hitbox3.active = true;
				activeCollisions.Add(hitbox3); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 16)
			{
				hitbox3.active = false;
				activeCollisions.Remove(hitbox3); // Attack has ended
			}	
			else if (stateFrame == 22) 
			{
				attacking = false;
				set_state(AIR);
				air_state();
			}
			
		}
		private void dashAttack()
		{
			if (stateFrame == 11)
			{
				hitbox.active = true;
				activeCollisions.Add(hitbox); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 16)
			{
				hitbox.active = false;
				activeCollisions.Remove(hitbox); // Attack has ended
			}	
			else if (stateFrame == 22) 
			{
				attacking = false;
				set_state(AIR);
				air_state();
			}
			
		}
		private void crouchAttack()
		{
			if (stateFrame == 11)
			{
				hitbox.active = true;
				activeCollisions.Add(hitbox); // Add collision to the list of collisions to check
			}
			else if (stateFrame == 16)
			{
				hitbox.active = false;
				activeCollisions.Remove(hitbox); // Attack has ended
			}	
			else if (stateFrame == 22) 
			{
				attacking = false;
				set_state(AIR);
				air_state();
			}
			
		}
		
		
		// ========================================== STATE FUNCTIONS ==========================================
		
		// All the state functions for the player depending on player input and player state
		// These functions contain what to do when in a certain state and when to do it
		public void air_state()
		{
			if (grounded)
			{
				set_state(IDLE);
				velocity.Y = 0;
				idle_state();
			}
			else if (jumping) 
			{
				set_state(JUMP);
				jump_state();
			}
			else if (dashing) 
			{
				set_state(DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(ATTACK);
				attack_state();
			}
			else
			{
				if (move_left) 
				{
					direction = 0;
					move();
				}
				else if (move_right)
				{
					direction = 1;
					move();
				}
			}
			
		}
		
		public void idle_state()
		{
			if (!grounded)
			{
				set_state(AIR);
				air_state();
			}
			else if (jumping) 
			{
				grounded = false;
				set_state(JUMP);
				jump_state();
			}
			else if (move_left) 
			{
				direction = 0;
				set_state(RUN);
				move();
			}
			else if (move_right)
			{
				direction = 1;
				set_state(RUN);
				move();
			}
			else if (dashing) 
			{
				set_state(DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(ATTACK);
				attack_state();
			}
		}
		
		public void jump_state()
		{	
			if (stateFrame == 9)
			{
				jumping = false;
				set_state(AIR);
				air_state();
				return;
			}
			else if (grounded)
			{
				jumping = false;
				velocity.Y = 0;
				set_state(IDLE);
				idle_state();
			}
			else if (dashing) 
			{
				jumping = false;
				set_state(DASH);
				dash_state();
			} 
			else if (attacking)
			{
				set_state(ATTACK);
				attack_state();
			}
			else 
			{
				if (move_left || move_right) move();
				if (stateFrame == 0) jump();
			}
		}
		
		public void run_state()
		{
			currentAnimation = RUN;
			if (!grounded)
			{
				set_state(AIR);
				air_state();
			}
			else if (jumping) 
			{
				grounded = false;
				set_state(JUMP);
				jump_state();
			}
			else if (dashing) 
			{
				set_state(DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(ATTACK);
				attack_state();
			} 
			else
			{
				if (!(move_left || move_right))
				{
					currentAnimation = RUN_END;
					if (animManager.isDone) 
					{
						currentAnimation = IDLE;
						set_state(IDLE);
						idle_state();
					}
					return;
				} else if (move_left) 
				{
					direction = 0;
					move();
				}
				else if (move_right)
				{
					direction = 1;
					move();
				}
			}
		}
		
		public void dash_state()
		{
			if (stateFrame == 10) // State ends
			{
				dashing = false;
				dashCooldown = 30;

				set_state(AIR);
				air_state();
				return;
			}
			else if (jumping)
			{
				//cancel the dash input with jump
				dashing = false;
				dashCooldown = 15;
				set_state(JUMP);
				jump_state();
			}
			else if (attacking)
			{
				set_state(ATTACK);
				attack_state();
			}
			else // now for the states
			{
				dash();
			}

		}
		
		public void hit_state()
		{
			if (hitstun == 0) 
			{
				hit = false;
				set_state(AIR);
				air_state();
				return;
			}
			else
			{
				move_left = false;
				move_right = false;
				jumping = false;
				crouching = false;
				dashing = false;

				if (applyKB) knockback();
			}
		}

		public void attack_state()
		{
			move_left = false;
			move_right = false;
			jumping = false;
			crouching = false;
			dashing = false;
			
			set_attack();
		}

		
		// ========================================== SETTING FUNCTIONS ==========================================
		
		// Sets the current and previous position AFTER player input but BEFORE collision handling.
		public void set_positions()
		{
			currPosition = position;

			float tempVelocityX = velocity.X + (speed * acceleration);
			float tempVelocityY = velocity.Y + (speed * gravity);

            nextPosition.X = position.X + tempVelocityX;
            nextPosition.Y = position.Y + tempVelocityY;
		}

		// For dealing with attacks. Adds any hitboxes to the lists of collisions and sets any necessary
		// physics values
		public void set_attack()
		{
			// need to make sure grounded is set to false AFTER this function is called
			switch (previousState)
			{
				case AIR:
					airAttack();
					break;
				case JUMP:
					airAttack();
					break;
				case IDLE:
					groundAttack();
					break;
				case RUN:
					groundAttack();
					break;
				case DASH:
					dashAttack();
					break;
				case CROUCH:
					crouchAttack();
					break;
			}
		}

		/// Changes the players current state, setting it to <param name="newState"> while recording the previous state.
		/// Also resets the stateFrame count.
        public void set_state(string newState)
        {
			if (currentState != newState)
			{
				previousState = currentState; 
				stateFrame = 0;
			}
            currentState = newState;
			currentAnimation = currentState; // Set the current animation to play
        }
		
		
		// ========================================== UPDATING ==========================================
		
		/// Keeps track of the elasped time of each state in 24FPS
        public void update_time(GameTime gameTime)
        {
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stateTimer >= FPS24)
            {
                stateTimer = FPS24 - stateTimer;
                stateFrame++;	
				update_cooldowns();
            }
			if (stateFrame >= 240) stateFrame = 0;
        }

		// Sets the corresponding boolean values depending on player input. Useful for setting the values
		// without calling the analogous state method
		public void update_input()
		{
			speed = airSpeed;
			acceleration = airAccel;
			deceleration = airDecel; 

			// Horizontal movement
			if (controls.isDown(controls.Right) && controls.isDown(controls.Left)) // dont move when both are pressed
			{
				move_right = false;
				move_left = false;
			}
			else if (controls.isDown(controls.Right))  // to prevent both from being true at the same time
			{
				move_right = true;
				move_left = false;
			}
			else if (controls.isDown(controls.Left) ) 
			{
				move_right = false;
				move_left = true;
			}
			else 
			{
				move_right = false;
				move_left = false;
			}

			// Jump
			if (controls.isDown(controls.Jump) && numJumps > 0) jumping = true;

			// Crouch
			if (controls.isDown(controls.Crouch)) crouching = true;
			else crouching = false;

			// Dash
			if (controls.isDown(controls.Dash) && numDashes > 0 && dashCooldown == 0) dashing = true;

			// Attack
			if (controls.isDown(controls.Attack)) attacking = true;

		}

		// Update the players state, and set the values for the physics to be applied
		public void update_state()
		{
			if (hit) 
			{
				set_state(HIT);
				hit_state();			
			}
			switch (currentState)
			{
				case AIR:
					air_state();
					break;
				case IDLE:
					idle_state();
					break;
				case JUMP:
					jump_state();
					break;
				case RUN:
					run_state();
					break;
				case DASH:
					dash_state();
					break;
				case ATTACK:
					attack_state();
					break;
			}
			grounded = false;
			decelerate();
			fall();
			set_positions(); // Update the "position to be drawn" of the player
		}
	
		// Applies physics to the position of the character. Reseting boolean values as necessary. 
		// This function should NOT set any physics related stuff like acclereation and velocity unless
		// those values are being reset
        public void update_physics()
        {
            // Flipping
            if (direction == 0) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

			if (grounded) 
			{
				numJumps = 1;
				velocity.Y = 0f;
			}
			if (acceleration < 0) acceleration = 0;

			position.Y += velocity.Y ;
            position.X += velocity.X ;
			
			if (position.Y >= 1000f || health == 0) 
			{
				position.Y = 0f;
				position.X = 400f; 
				velocity.Y = 0f; 
				acceleration = 0f;
				health = 100;
			}

			if (attacking) hitbox.update_Position(position + hitboxPosition);
			if (attacking) hitbox2.update_Position(position + hitboxPosition);
			if (attacking) hitbox3.update_Position(position + hitboxPosition);

			hurtbox.update_Position(position);
        }

		// Modify the players cooldowns depending on the time
		public void update_cooldowns()
		{
			// -------- Hitstun
			if (hit && hitstun > 0) hitstun -= 1;
			else 
			{
				hitstun = 0;
				hit = false;
			}
			// -------- Dash 
			if (dashCooldown > 0) 
			{
				dashCooldown -= 1;
				dashing = false;
			}
			else
			{
				dashCooldown = 0;
			 	numDashes = 1;
			}

			// -------- Health
			if (health < 0) health = 0;
		}
	
		// Updating the current animation to be played
		public void update_animations()
		{
            switch (currentAnimation)
            {
                case IDLE:
					animManager.Play(animDictionary["Base_Idle"]);
                    break;
                case AIR:
                    animManager.Play(animDictionary["Base_Falling"]);
                    break;
				case JUMP:
					animManager.Play(animDictionary["Base_Jump"]);
					break;
                case RUN:
                    animManager.Play(animDictionary["Base_Running"]);
                    break;
                case RUN_END:
                    animManager.Play(animDictionary["Base_RunStop"]);
                    break;
                case TURNAROUND:
                    animManager.Play(animDictionary["Base_Turnaround"]);
                    break;
                case CROUCH:
                    animManager.Play(animDictionary["ball"]); 
                    break;
				case DASH:
					animManager.Play(animDictionary["ball"]);
					break;
                case ATTACK:
                    animManager.Play(animDictionary["ball"]);
                    break;
				case HIT:
					animManager.Play(animDictionary["ball"]); 
					break;
                default:
                    break;
            }
        }

		

		// Old function, will modify later
        public virtual void Update(GameTime gameTime)
        {
			// update_test();
			update_physics(); // set the final position, velocity, and acceleration of the player
			hurtbox.Update(gameTime);
			update_time(gameTime); // update the frame count for the state
			update_animations(); // set the corresponding animation
			animManager.Update(gameTime); // update the frame count for the animation
        }


// ========================================== DRAWING ==========================================
		public void DrawPlayer(GameTime gameTime, SpriteBatch spritebatch)
		{
			// Draw player animation
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// Draw the players hurtbox, will change later
			hurtbox.Draw(gameTime, spritebatch, position, flip);
			
			// Attack hitboxes just for testing. Will remove/change later
			hitbox.Draw(gameTime, spritebatch, position + hitboxPosition, flip);
			hitbox2.Draw(gameTime, spritebatch, position + hitboxPosition, flip);
			hitbox3.Draw(gameTime, spritebatch, position + hitboxPosition, flip);
		}
        
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

			// Messages to display game / player information
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nFrame Time (24FPS): " + stateFrame + " Frames" 
			+ "\nGrounded: " + grounded.ToString()
			+ "\nCooldown: " + dashCooldown
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun;
			
			movementMSG = "Position: " + position.ToString() 
			+ "\nVelocity: " + velocity.ToString()
            + "\nAcceleration: " + acceleration.ToString() 
			+ "\nDirection: " + direction.ToString()
			+ "\nGravity: " + gravity.ToString();
			
			string inputMSG = 
			"Left = " + move_left.ToString() 
			+ "\nRight = " + move_right.ToString()
			+ "\nJumping = " + jumping
			+ "\nDashing = " + dashing
			+ "\nCrouching = " + crouching
			+ "\nGrounded = " + grounded
			+ "\nAttacking = " + attacking
			+ "\nHit = " + hit
			+ "\nIgnore Size: " + hitbox.ignoreObjects.Count;
;
			DrawPlayer(gameTime, spriteBatch);

            spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(800f, 400), Color.DarkRed);
            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(5f, 550), Color.Orange);

			
        }
	}
}

