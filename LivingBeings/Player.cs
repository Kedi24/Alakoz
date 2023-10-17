using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
using Alakoz.Collision;
using Alakoz.GameInfo;


namespace Alakoz.LivingBeings
{
	public class Player : Species
	{
		// ------ OTHER ------ //
		public string stateMSG = "";
		public SpriteFont stateFONT { get; set; }

		public float stateTimer;
		public int stateFrame;
		public const float FPS24 = AnimationManager.FPS24;

		public string movementMSG = "";

		// // ------ MOVEMENT ------- //
        // public new Vector2 position;
        public new Vector2 velocity;

		public new Vector2 currPosition;
		public new Vector2 nextPosition;

		public new float gravity = 0.3f;
		public new float speed = 2f; // Default speed
		public new float airSpeed = 1f;
		public new float fallSpeed = 1f;
		public new float groundSpeed = 1f;
		public new float velocityMax = 4f;
		public new float terminalVelocity = 10f;
		public new float accelMax = 10f;
		public new float accelMin = 0.1f;
		public new float acceleration = 0.2f; // Default acceleration
		public new float deceleration = 0.2f; // Default deceleration
		public new float airAccel = 0.2f;
		public new float airDecel = 0.2f;
		public new float groundAccel = 0.3f;
		public new float groundDecel = 0.1f;

		// ---- Moving
		public new int direction = 1;
		public new bool move_left = false;
		public new bool move_right = false;
		public new bool grounded = false;
		
		// ---- Jumping
		public new bool jumping = false;
		public new int numJumps = 1;
		public bool softJumping = false;
		public bool wallCollide = false; // Indicates a wall collision
		public int wallBuffer = 0; // To prevent immediate wall interactions
		public int walljumpCooldown = 0;
		
		// ---- Dashing
		public new bool dashing = false;
		public new int numDashes = 1;
		public new int dashCooldown = 15;
		public float dashSpeed = 6f;

		// ---- Crouching
		public new bool crouching = false;

		// ---- Attacking
		public new bool attacking = false;

		// ---- Hit 
		public new bool hit = false;
		public new int hitstun = 0;
		public new int health = 100;
	
		// ---- Interacting
		public bool interacting = false;
		public int interactCooldown = 0;

		// ------ COLLISION ------ //
		
		// public new Hurtbox hurtbox {get; set;}
		public new Hitbox hitbox {get; set;}
		public new Hitbox hitbox2 {get; set;}
		public new Hitbox hitbox3 {get; set;}
		public new Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public new Vector2 KB; // The knockback from the hitbox that interescts the player
		public new bool applyKB = false;
		public float hurtboxWidth = 34;
		public float hurtboxHeight = 44;
		public new bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite


		// ------ ANIMATION ------ //
		public Dictionary<StateType, Animation> animations;
		public StateType currentAnimation;
		public StateType previousAnimation;
		public StateType tempAnimation;
		public ArrayList preAnimations;

		// ------ SKILLS ------ //


		// ------ SOUND ------ //


		// ------ EFFECTS ------ //
		SpriteEffects flip;


		// ------ INPUTS ------ //
		public Controls controls;


		// ------ STATES ----- // 
		public new StateType currentState;
		public new StateType previousState;

		public Player(Dictionary<StateType, Animation> newSprites, Vector2 newPosition)
		{
			currentState = StateType.AIR;

			position = newPosition;

			hurtbox = new Hurtbox(this, position, hurtboxWidth, hurtboxHeight, newSprites[StateType.HURTBOX], true);
			
			// Hitboxes just to test primitive attacks. Will modify later
			hitbox = new Hitbox(position + hitboxPosition, 32f, 20f, 3, new Vector2(1f, 0f), 2, 10, newSprites[StateType.HITBOX], true);
			hitbox2 = new Hitbox(position + hitboxPosition, 32f, 30f, 3, new Vector2(1f, 0f), 5, 10, newSprites[StateType.HITBOX], true);
			hitbox3= new Hitbox(position + hitboxPosition, 32f, 40f, 3, new Vector2(5f, -5f), 10, 20, newSprites[StateType.HITBOX], true);
			
			hitbox.active = false;
			hitbox2.active = false;
			hitbox3.active = false;
			
			hitbox.addIgnore(hurtbox); // Prevent the player from hitting themselves
			hitbox2.addIgnore(hurtbox);
			hitbox3.addIgnore(hurtbox);

			activeCollisions.Add(hurtbox);

			stateFrame = 0;
			stateTimer = 0f;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[StateType.FALL], true);
            animManager.Position = position;
			currentAnimation = StateType.FALL;
			preAnimations = new ArrayList();

            controls = new Controls();

			flip = SpriteEffects.None;
		}


		// ========================================== PHYSICS ==========================================

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
		// ------------------------------ Basic
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
			if (stateFrame == 1)
			{
				velocity.Y = -8.0f;
				numJumps -= 1;

				// Prevent dash -> jump velocity from being transferred when high.
				if (velocity.X > velocityMax) velocity.X = velocityMax;
				else if (velocity.X < -velocityMax) velocity.X = -velocityMax;
				
				if (numJumps < 0) numJumps = 0;
				grounded = false;
			}
		}
		
		private void fall()
		{
			if (velocity.Y < terminalVelocity) velocity.Y += fallSpeed * gravity;
			else velocity.Y = terminalVelocity;

			if (dashing) velocity.Y = 0;
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
			if (stateFrame == 4) 
			{
				if (direction == 1) velocity.X = 20;
				else velocity.X = -20;
			}
			else if (stateFrame == 5)
			{
				if (direction == 1) velocity.X = 6;
				else velocity.X = -6;
			}
			else 
			{
				if (direction == 1) velocity.X = dashSpeed;
			 	else velocity.X = -dashSpeed;
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
				set_state(StateType.AIR);
				
				tempAnimation = StateType.FALL;
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
				set_state(StateType.AIR);
				tempAnimation = StateType.FALL;
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
				set_state(StateType.AIR);
				tempAnimation = StateType.FALL;
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
				set_state(StateType.AIR);
				tempAnimation = StateType.FALL;
				air_state();
			}
			
		}
		
		//------------------------------ Unique
		private void walljump() 
		{
			// Physics
			velocity.Y = -7.0f;
			if (direction == 0) velocity.X = 7f;
			else velocity.X = -7f;

			// Booleans
			walljumpCooldown = 20;
			jumping = false;
		
			// Animation
			preAnimations = new ArrayList(){StateType.BALLEND, StateType.BALL, StateType.BALLSTART};
			set_preAnimations(); // state doesnt change, modify animations in isolation
		}
		
		
		// ========================================== STATE FUNCTIONS ==========================================
		
		// All the state functions for the player depending on player input and player state
		// These functions contain what to do when in a certain state and when to do it
		public void air_state()
		{
			if (grounded)
			{
				velocity.Y = 0;
				preAnimations = new ArrayList(){StateType.CROUCHEND};
				tempAnimation = StateType.IDLE;
				set_state(StateType.IDLE);
				idle_state();
			}
			else if (wallCollide && softJumping && wallBuffer == 0) { walljump(); }
			else if (jumping && walljumpCooldown == 0) 
			{
					preAnimations = new ArrayList(){StateType.JUMPSTART};
					set_state(StateType.JUMP);
					tempAnimation = StateType.JUMP;
					jump_state();
			}
			else if (dashing) 
			{
				preAnimations = new ArrayList(){StateType.DASHSTART};
				tempAnimation = StateType.DASH;
				set_state(StateType.DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(StateType.ATTACK);
				attack_state();
			}
			else
			{
				tempAnimation = StateType.FALL;
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
				tempAnimation = StateType.FALL;
				set_state(StateType.AIR);
				air_state();
			}
			else if (jumping) 
			{
				grounded = false;
				set_state(StateType.JUMP);
				preAnimations = new ArrayList(){StateType.JUMPSTART};
				tempAnimation = StateType.JUMP;
				jump_state();
			}
			else if (move_left) 
			{
				direction = 0;
				set_state(StateType.RUN);
				tempAnimation = StateType.RUN;
				move();
			}
			else if (move_right)
			{
				direction = 1;
				set_state(StateType.RUN);
				tempAnimation = StateType.RUN;
				move();
			}
			else if (crouching)
			{
				preAnimations = new ArrayList(){StateType.CROUCHSTART};
				tempAnimation = StateType.CROUCH;

				hurtbox.changeDimensions(new Vector2(0, hurtbox.height / 2), 34, 22);
				set_state(StateType.CROUCH);
				crouch_state();
			}
			else if (dashing) 
			{
				preAnimations = new ArrayList(){StateType.DASHSTART};
				tempAnimation = StateType.DASH;
				set_state(StateType.DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(StateType.ATTACK);
				attack_state();
			} 
			else 
			{
				tempAnimation = StateType.IDLE;
			}
		}
		
		public void jump_state()
		{	
			if (velocity.Y < 0.5 && velocity.Y > -0.5 && stateFrame > 1) // fix later
			{
				jumping = false;
				preAnimations = new ArrayList{ StateType.BALLEND, StateType.BALL, StateType.BALLSTART };
				tempAnimation = StateType.FALL;
				set_state(StateType.AIR);
				air_state();
				return;
			}
			else if (grounded && stateFrame > 1) // To prevent jump -> idle -> jump... loop 
			{
				jumping = false;
				velocity.Y = 0;
				preAnimations = new ArrayList{ StateType.CROUCHEND};
				tempAnimation = StateType.IDLE;
				set_state(StateType.IDLE);
				idle_state();
			}
			else if (dashing) 
			{
				jumping = false;
				preAnimations = new ArrayList(){StateType.DASHSTART};
				tempAnimation = StateType.DASH;
				set_state(StateType.DASH);
				dash_state();
			} 
			else if (attacking)
			{
				set_state(StateType.ATTACK);
				attack_state();
			}
			else if (wallCollide && softJumping && wallBuffer == 0) { walljump(); }
			else 
			{
				tempAnimation = StateType.JUMP;
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
				jump();
			}
		}
		
		public void run_state()
		{
			tempAnimation = StateType.RUN;
			
			if (!grounded)
			{
				preAnimations = new ArrayList(){StateType.BALLEND, StateType.BALL, StateType.BALLSTART};
				tempAnimation = StateType.FALL;
				set_state(StateType.AIR);
				air_state();
			}
			else if (jumping) 
			{
				grounded = false;
				preAnimations = new ArrayList(){StateType.JUMPSTART};
				tempAnimation = StateType.JUMP;
				set_state(StateType.JUMP);
				jump_state();
			}
			else if (dashing) 
			{
				preAnimations = new ArrayList(){StateType.DASHSTART};
				tempAnimation = StateType.DASH;
				set_state(StateType.DASH);
				dash_state();
			}
			else if (attacking)
			{
				set_state(StateType.ATTACK);
				attack_state();
			} 
			else
			{
				if (!(move_left || move_right))
				{
					if (velocity.X > 2.5 || velocity.X < -2.5) preAnimations = new ArrayList{ StateType.RUNEND };
					tempAnimation = StateType.IDLE;
					set_state(StateType.IDLE);
					idle_state();
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
		
		public void crouch_state()
		{
			if (!grounded)
			{
				tempAnimation = StateType.FALL;
				preAnimations = new ArrayList(){StateType.BALLEND, StateType.BALL, StateType.BALLSTART};
				// Update the hurtbox
				hurtbox.resetDimensions();
				set_state(StateType.AIR);
				air_state();
			} 
			else if (!crouching)
			{
				preAnimations = new ArrayList(){StateType.CROUCHEND};
				tempAnimation = StateType.IDLE;
				// Update the hurtbox
				hurtbox.resetDimensions();
				set_state(StateType.IDLE);
				idle_state();
			}
			else if (jumping)
			{
				tempAnimation = StateType.JUMP;
				preAnimations = new ArrayList(){StateType.JUMPSTART};
				// Update the hurtbox
				hurtbox.resetDimensions();
				set_state(StateType.JUMP);
				jump_state();	
			}
			else if (dashing)
			{
				preAnimations = new ArrayList(){StateType.DASHSTART};
				tempAnimation = StateType.DASH;
				// Update the hurtbox
				hurtbox.resetDimensions();
				set_state(StateType.DASH);
				dash_state();
			}
			else if (move_left || move_right)
			{
				tempAnimation = StateType.RUN;
				// Update the hurtbox
				hurtbox.resetDimensions();
				set_state(StateType.RUN);
				run_state();
			}
			else
			{
				// if (stateFrame == 0) hurtbox.changeDimensions(new Vector2(hurtbox.width / 2, hurtbox.height / 2), 34, 22);
			}
		}

		public void dash_state()
		{
			if (stateFrame == 12) // State ends
			{
				dashing = false;
				dashCooldown = 15;
				
				// Update the Animations
				preAnimations = new ArrayList(){StateType.DASHEND};
				tempAnimation = StateType.FALL;

				set_state(StateType.AIR);
				air_state();
				return;
			}
			else if (jumping)
			{
				// Jump Cancel
				dashing = false;
				dashCooldown = 15;
				tempAnimation = StateType.JUMP;
				set_state(StateType.JUMP);
				jump_state();
			}
			else if (attacking)
			{
				set_state(StateType.ATTACK);
				attack_state();
			}
			else
			{
				dash();	
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
				dashing = false;

				if (applyKB) 
				{
					if (tempAnimation == currentAnimation)  // To make sure the impact frames play when hit again DURING hitstun
					{
						preAnimations = new ArrayList(){StateType.HITSTART};
						set_preAnimations();
					}
					knockback();
				}
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

		
		
		// ========================================== SETTERS ==========================================
		
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
				case StateType.AIR:
					airAttack();
					break;
				case StateType.JUMP:
					airAttack();
					break;
				case StateType.IDLE:
					groundAttack();
					break;
				case StateType.RUN:
					groundAttack();
					break;
				case StateType.DASH:
					dashAttack();
					break;
				case StateType.CROUCH:
					crouchAttack();
					break;
			}
		}

		/// Changes the players current state, setting it to <param name="newState"> while recording the previous state.
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
		
		/// Keeps track of the elasped time of each state in 24FPS
        public override void update_time(GameTime gameTime)
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
		public override void update_input()
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
			// Just for walljumping 
			if (controls.isDown(controls.Jump)) softJumping = true;
			else softJumping = false;
			

			if (controls.isDown(controls.Jump) && numJumps > 0) jumping = true;

			// Crouch
			if (controls.isDown(controls.Crouch)) crouching = true;
			else crouching = false;

			// Dash
			if (controls.isDown(controls.Dash) && numDashes > 0 && dashCooldown == 0) dashing = true;

			// Attack
			if (controls.isDown(controls.Attack)) attacking = true;

			// // Map Interactions
			// if (controls.isDown(controls.Interact) && interactCooldown == 0) interacting = true;
			// else interacting = false;

		}

		// Update the players state, and set the values for the physics to be applied
		public override void update_state()
		{
			if (hit) 
			{	
				preAnimations = new ArrayList(){StateType.HITSTART};
				tempAnimation = StateType.HIT;
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
				case StateType.JUMP:
					jump_state();
					break;
				case StateType.CROUCH:
					crouch_state();
					break;
				case StateType.RUN:
					run_state();
					break;
				case StateType.DASH:
					dash_state();
					break;
				case StateType.ATTACK:
					attack_state();
					break;
				
			}
			grounded = false;
			wallCollide = false;
			decelerate();
			fall();
			set_positions(); // Update the "position to be drawn" of the player
		}
	
		// Applies physics to the position of the character. Reseting boolean values as necessary. 
		// This function should NOT set any physics related stuff like acclereation and velocity unless
		// those values are being reset
        public override void update_physics()
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
			
			if (position.Y >= 2000f || health <= 0) 
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
			// -------- Wall Jump
			if (!wallCollide || grounded) wallBuffer = 7; // to prevent instant wall jumps
			else 
			{
				if (wallBuffer <= 0) wallBuffer = 0;
				else wallBuffer -= 1;				
			}

			if (walljumpCooldown > 0) // to prevent burning your jump
			{
				walljumpCooldown -= 1;
				jumping = false;
			}
			else walljumpCooldown = 0;

			// -------- Health
			if (health < 0) health = 0;

			// // -------- Interact
			// if (interactCooldown > 0) interactCooldown -= 1;
			// else if (interactCooldown < 0) interactCooldown = 0;
		}
	
		// Updating the current animation to be played
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

		

		// Old function, will modify later
        // public virtual void Update(GameTime gameTime)
        // {
		// 	// update_test();
		// 	update_physics(); // set the final position, velocity, and acceleration of the player
		// 	hurtbox.Update(gameTime);
		// 	update_time(gameTime); // update the frame count for the state
		// 	update_animations(); // set the corresponding animation
		// 	animManager.Update(gameTime); // update the frame count for the animation
        // }


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
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

			// Messages to display game / player information
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nState Time (24FPS): " + stateFrame
			+ "\nAnimation: " + currentAnimation
			+ "\nDash Cooldown: " + dashCooldown
			+ "\nWalljump Cooldown: " + walljumpCooldown
			+ "\nWallBuffer: " + wallBuffer
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun;
			
			movementMSG = "Position: " + position.ToString() 
			+ "\nHurtbox: " + hurtbox.position.ToString()
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
			+ "\nWall = " + wallCollide
			+ "\nIgnore Size: " + hitbox.ignoreObjects.Count;
			DrawPlayer(gameTime, spriteBatch);

            // spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(800f, 400), Color.DarkRed);
            // spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			// spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(5f, 550), Color.Orange);

            // spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(hurtboxWidth+ 20, -100) + position, Color.GreenYellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            // spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(-39, -120) + position, Color.Gold, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			// spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(-39, hurtboxHeight) + position, Color.Blue, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

			
        }
	}
}

