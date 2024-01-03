using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Collision;
using Alakoz.GameObjects;
using Alakoz.GameInfo;
using MonoGame.Extended;

namespace Alakoz.GameObjects
{
	public class Player : GameObject
	{
		// ------ OTHER ------ //
		public string stateMSG = "";
		public string movementMSG = "";
		public SpriteFont stateFONT { get; set; }

		public float stateTimer;
		public int stateFrame;

		// // ------ MOVEMENT ------- //
		public Vector2 currPosition;
		public Vector2 nextPosition;
		public Vector2 spawnPoint;

		public bool applyFall = true;
		public float gravity = 0.3f;
		public float speed = 2f; // Default speed
		public float fallSpeed = 1f;

		public float velocityMax = 4f;
		public float terminalVelocity = 8f;
		public float acceleration = 0.2f; // Default acceleration
		public float deceleration = 0.1f; // Default deceleration


		// ---- Moving
		public int direction = 1;
		public bool move_left = false;
		public bool move_right = false;
		public bool move_up = false;
		public bool move_down = false;
		public bool grounded = false;
		
		// ---- Jumping
		public bool jumping = false;
		public int numJumps = 1;
		public int jumpDelay = 1;
		public bool softJumping = false;
		public bool wallJumping = false;
		public bool wallCollide = false; // Indicates a wall collision
		public int wallBuffer = 0; // To prevent immediate wall interactions
		public int wallCooldown = 80;
		public float wallFallSpeed = 0.5f;

		// ---- Crouching
		public bool crouching = false;
		
		// ---- Dashing
		public bool dashing = false;
		public int numDashes = 1;
		public int dashCooldown = 15;
		public float dashSpeed = 6f;

		// ---- Attacking
		public bool attacking = false;
		public int attackCounter = 0;

		// ---- Hit 
		public bool hit = false;
		public int hitstun = 0;
		public int health = 100;
	
		// ---- Interacting
		public bool interacting = false;
		public bool beginInteract = false; // When there is something for the player to interact with
		
		// The function to call when the player interacts with another GameObject
		// This delegates the states of the player to the function defined in the corresonding GameObject instead.
		public event EventHandler interactFunction; 
		public int interactCooldown = 0;

		// ------ COLLISION ------ //
		
		public Hurtbox hurtbox {get; set;}
		public Hitbox hitbox {get; set;}
		public Hitbox hitbox2 {get; set;}
		public Hitbox hitbox3 {get; set;}
		public Vector2 hitboxPosition = new Vector2(32f, 15f); // For the players attack, will change later
		public Vector2 KB; // The knockback from the hitbox that interescts the player
		public float hurtboxWidth = 34;
		public float hurtboxHeight = 44;
		public bool hurtboxVisual = true;

		// ---- Attack hitboxes
		public Hitbox[] allHitboxes = new Hitbox[5]; // Array to store a preset number of hitboxes 
		


		// ------ ANIMATION ------ //
		public Dictionary<StateType, Animation> animations;
		public StateType currentAnimation;
		public StateType previousAnimation;
		public StateType tempAnimation;
		public ArrayList preAnimations;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite
		
		// ------ SKILLS ------ //


		// ------ SOUND ------ //


		// ------ EFFECTS ------ //
		SpriteEffects flip;


		// ------ INPUTS ------ //
		public Controls controls;


		// ------ STATES ----- // 
		public StateType currentState;
		public StateType previousState;
		public Player(Dictionary<StateType, Animation> newSprites, Vector2 newPosition)
		{
			type = GameObjectType.PLAYER;
			previousState = StateType.AIR;
			currentState = StateType.AIR;

			position = newPosition;
			spawnPoint = newPosition;
			originOffset = new Vector2((hurtboxWidth / 2), (hurtboxHeight / 2));
			origin = position + originOffset;

			hurtbox = new Hurtbox(this, position, hurtboxWidth, hurtboxHeight, true);

			// Add hitboxes and hurtboxes to collision objects
			// There is space for, at most, 5 hitboxes on a single frame
			for (int i = 0; i < 5; i++) 
			{
				allHitboxes[i] = Hitbox.Zero;
				allHitboxes[i].owner = this;
				allHitboxes[i].addIgnore(hurtbox); // To prevent dealing damage to yourself
				activeCollisions.Add(allHitboxes[i]);
			}
					
			activeCollisions.Add(hurtbox);

			stateFrame = 0;
			stateTimer = 0f;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[StateType.FALL], true);
            animManager.Position = position;

			currentAnimation = StateType.FALL;
			tempAnimation = StateType.FALL;

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
            if (direction == -1) 
			{
				velocity.X = approach(velocity.X, -velocityMax, speed * acceleration);
			}
            else if (direction == 1) 
			{
				velocity.X = approach(velocity.X, velocityMax, speed * acceleration);
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
			if (!applyFall)
			{
				velocity.Y = 0; 
				return;
			}

			if (velocity.Y < terminalVelocity) velocity.Y = approach(velocity.Y, terminalVelocity, gravity);
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
		
		private void dash()
		{
		// dash physics
			if (stateFrame < 4)
			{
				velocity.X = 0;
			}
			else if (stateFrame == 4) 
			{
				if (direction == 1) velocity.X = 15;
				else velocity.X = -15;
			}
			else if (stateFrame < 7)
			{
				if (direction == 1) 
				{
					velocity.X -= 3;
					if (velocity.X < 0) velocity.X = 0;
				}
				else 
				{
					velocity.X += 3;
					if (velocity.X > 0) velocity.X = 0;
				}
			} 
			else
			{
				if (direction == 1) velocity.X = dashSpeed;
			 	else velocity.X = -dashSpeed;
			}
			velocity.Y = 0f;

            numDashes = 0;
			grounded = false;
		}		
		
		private void knockback()
		{
			velocity.X = KB.X;
			velocity.Y = KB.Y;
			applyKB = false;
		} 

		//------------------------------ Unique
		private void walljump() 
		{
			fallSpeed = 1f;
			wallBuffer = 7;
			wallCollide = false;
			spriteCoordinate = new Vector2(-39, -36);

			// Physics
			velocity.Y = -7.0f;
			if (direction == -1) velocity.X = 7f;
			else velocity.X = -7f;

			// Booleans
			jumping = false;
		}
		
		
		// ========================================== STATE FUNCTIONS ==========================================
		
		// All the state functions for the player depending on player input and player state
		// These functions contain what to do when in a certain state and when to do it
		public void air_state()
		{
			if (grounded)
			{
				velocity.Y = 0;
				hurtbox.resetDimensions();
				set_state(StateType.IDLE, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				hurtbox.resetDimensions();
				set_state(StateType.WALLCLING, new ArrayList(){}, StateType.WALLCLING);
			}
			else if (jumping) 
			{
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
			}
			else if (dashing) 
			{
				hurtbox.resetDimensions();
				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			}
			else if (attacking)
			{
				attackCounter++;
				set_state(StateType.AIRATTACK1, new ArrayList(){StateType.AIRATTACK1}, StateType.NONE);
			}
			else
			{
				if (move_left) 
				{
					direction = -1;
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
				// hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				hurtbox.resetDimensions();
				set_state(StateType.AIR, new ArrayList(){}, StateType.FALL);
			}
			else if (jumping) 
			{
				grounded = false;
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
			}
			else if (move_left) 
			{
				hurtbox.resetDimensions();
				set_state(StateType.RUN, new ArrayList(){StateType.RUNSTART}, StateType.RUN);
			}
			else if (move_right)
			{
				hurtbox.resetDimensions();
				set_state(StateType.RUN, new ArrayList(){StateType.RUNSTART}, StateType.RUN);
			}
			else if (crouching)
			{
				hurtbox.changeDimensions(new Vector2(0, hurtbox.height / 2), 34, 22);
				set_state(StateType.CROUCH, new ArrayList(){StateType.CROUCHSTART}, StateType.CROUCH);
			}
			else if (dashing) 
			{
				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			}
			else if (attacking)
			{
				attackCounter++;
				set_state(StateType.BASICATTACK1, new ArrayList(){StateType.BASICATTACK1}, StateType.NONE);
			} 
			else if (beginInteract)
			{
				set_state(StateType.INTERACT, new ArrayList(){}, tempAnimation);
				interact_state();
			}
		}
		
		public void jump_state()
		{
			if (stateFrame < jumpDelay) velocity.Y = 0; // Jump delay
			if (velocity.Y < 0.5 && velocity.Y > -0.5 && stateFrame > jumpDelay) // fix later
			{
				jumping = false;
				hurtbox.resetDimensions();
				set_state(StateType.AIR, new ArrayList{ StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
				return;
			}	
			else if (grounded) // To prevent jump -> idle -> jump... loop 
			{
				jumping = false;
				velocity.Y = 0;
				hurtbox.resetDimensions();
				set_state(StateType.IDLE, new ArrayList{ StateType.CROUCHEND}, StateType.IDLE);
				
			}
			else if (dashing) 
			{
				jumping = false;
				hurtbox.resetDimensions();

				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			} 
			else if (attacking)
			{
				jumping = false;
				hurtbox.resetDimensions();

				attackCounter++;
				set_state(StateType.AIRATTACK1, new ArrayList(){StateType.AIRATTACK1}, StateType.NONE);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				jumping = false;
				hurtbox.resetDimensions();
				set_state(StateType.WALLCLING, new ArrayList(){}, StateType.WALLCLING);
			}
			else
			{
				if (move_left) 
				{
					direction = -1;
					move();
					
				}
				else if (move_right)
				{
					direction = 1;
					move();
				}
				if (stateFrame == jumpDelay )jump();
			}
			
		}
		
		public void run_state()
		{	
			if (!grounded)
			{
				// hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				hurtbox.resetDimensions();
				set_state(StateType.AIR, new ArrayList(){StateType.BALLEND, StateType.BALL, StateType.BALLSTART}, StateType.FALL);
			}
			else if (jumping) 
			{
				grounded = false;
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);		
				set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);	
			}
			else if (dashing) 
			{
				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			}
			else if (attacking)
			{
				attackCounter++;
				set_state(StateType.BASICATTACK1, new ArrayList(){StateType.BASICATTACK1}, StateType.NONE);
			} 
			else if (beginInteract) /// FIX LATER
			{
				if (velocity.X > 2.5 || velocity.X < -2.5) preAnimations = new ArrayList{ StateType.RUNEND };
				
				hurtbox.resetDimensions();
				set_state(StateType.INTERACT, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
			else
			{
				if (!(move_left || move_right))
				{
					if (velocity.X > 2.5 || velocity.X < -2.5) set_state(StateType.IDLE, new ArrayList(){ StateType.RUNEND }, StateType.IDLE);
					else set_state(StateType.IDLE, new ArrayList(){}, StateType.IDLE);
					
					return;
				} 
				else if (move_left) 
				{
					direction = -1;
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
				hurtbox.resetDimensions();
				set_state(StateType.AIR, new ArrayList(){StateType.BALLEND, StateType.BALL, StateType.BALLSTART}, StateType.FALL);
			} 
			else if (!crouching)
			{
				hurtbox.resetDimensions();
				set_state(StateType.IDLE, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
			else if (jumping)
			{
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);		
				set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);	
			}
			else if (dashing)
			{
				hurtbox.resetDimensions();
				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			}
			else if (move_left || move_right)
			{
				hurtbox.resetDimensions();
				set_state(StateType.RUN, new ArrayList(){StateType.RUNSTART}, StateType.RUN);
			}
			else if (beginInteract)
			{
				hurtbox.resetDimensions();
				set_state(StateType.INTERACT, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
		}

		public void dash_state()
		{
			applyFall = false;
			if (stateFrame == 12) // State ends
			{
				dashing = false;
				applyFall = true;
				dashCooldown = 15;
				
				// hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(StateType.AIR, new ArrayList(){StateType.DASHEND}, StateType.FALL);
				return;
			}
			else if (jumping)
			{
				// Jump Cancel
				dashing = false;
				applyFall = true;
				dashCooldown = 15;
				grounded = false;
				
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				dashing = false;
				applyFall = true;
				dashCooldown = 15;

				set_state(StateType.WALLCLING, new ArrayList(){}, StateType.WALLCLING);
			}
			else if (attacking)
			{
				dashing = false;
				applyFall = true;
				attackCounter++;
				
				set_state(StateType.BASICATTACK4, new ArrayList(){StateType.BASICATTACK4}, StateType.BASICATTACK4);
			}
			else
			{
				dash();	
			}
		}
		
		public void wallCling_state()
		{
			if (grounded)
			{
				velocity.Y = 0;
				fallSpeed = 1f;
				wallBuffer = 7;
				wallCollide = false;
				wallJumping = false;
				
				spriteCoordinate = new Vector2(-39, -36);

				set_state(StateType.IDLE, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
			else if (dashing)
			{
				fallSpeed = 1f;
				wallBuffer = 7; // Prevent scaling walls infinitely
				wallCollide = false;
				wallJumping = false;
				wallCooldown -= 5;
				spriteCoordinate = new Vector2(-39, -36);
				
				set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
			}
			else if (!wallCollide || wallCooldown <= 0 || (direction == -1 && move_right) || (direction == 1 && move_left))
			{
				fallSpeed = 1f;
				wallBuffer = 7;
				spriteCoordinate = new Vector2(-39, -36);
				wallJumping = false;
				wallCollide = false;

				// hurtbox.changeDimensions(nwwew Vector2(0, -10), 34, 42);
				set_state(StateType.AIR, new ArrayList(){}, StateType.FALL);
			} 
			else 
			{

				if (direction == -1) spriteCoordinate = new Vector2(-48, -36);
				else spriteCoordinate = new Vector2(-30, -36);
				// fallSpeed = wallFallSpeed;
				if (softJumping)
				{
					wallCollide = false;
					set_state(StateType.WALLJUMP, new ArrayList(){}, StateType.WALLCLING);
					wallJump_state();
				}
				else 
				{
					velocity.Y = wallFallSpeed;
					move();
				}	
			}
		}
		
		public void wallJump_state()
		{
			move_left = false;
			move_right = false;
			jumping = false;
			
			if (grounded)
			{
				velocity.Y = 0;
				fallSpeed = 1f;
				wallBuffer = 7;
				wallCollide = false;
				wallJumping = false;
				
				spriteCoordinate = new Vector2(-39, -36);

				set_state(StateType.IDLE, new ArrayList(){StateType.CROUCHEND}, StateType.IDLE);
			}
			// State Restrictions
			else if (stateFrame == 0)
			{
				tempAnimation = StateType.WALLJUMPSTART;
			}
			else if (stateFrame < 3)
			{
				velocity.Y = wallFallSpeed;
			}
			else if (stateFrame == 3)
			{
				wallCooldown -= 5; // Prevent scaling walls infinitely
				wallJumping = false;
				wallBuffer = 7;
				preAnimations = new ArrayList(){StateType.WALLJUMP};
				tempAnimation = StateType.FALL;

				walljump(); // Perform the jump		
			} 
			else if (stateFrame < 15)
			{
				if (wallCollide) 
				{ 
					if (direction == -1) direction = 1;
					else direction = -1;

					wallJumping = false;
					wallBuffer = 0;

					hurtbox.resetDimensions();
					set_state(StateType.WALLCLING, new ArrayList(){}, StateType.WALLCLING);
				}
			}
			else 
			{
				spriteCoordinate = new Vector2(-39, -36);
				set_state(StateType.AIR, new ArrayList(){}, StateType.FALL);

			}
		}
		
		public void hit_state()
		{
			grounded = false;
			if (stateFrame == hitstun) 
			{
				hit = false;
				hitstun = 0;
				set_state(StateType.AIR, new ArrayList(){}, StateType.FALL);
				return;
			}
			else
			{
				
				move_left = false;
				move_right = false;
				jumping = false;
				crouching = false;
				dashing = false;

				nextPosition = currPosition; // To prevent map based collisions that depend on the next position from being invoked (grounded, etc)

				if (applyKB) 
				{
					stateFrame = 0; // So that the frames dont accumulate when hit multiple times during hitstun
					
					foreach (Hitbox hitbox in allHitboxes)	hitbox.active=false; // Deactive all hitboxes

					if (currentAnimation == StateType.HIT)  // To make sure the impact frames play when hit again DURING hitstun
					{
						preAnimations = new ArrayList(){StateType.HITSTART};
						set_preAnimations();
					}
					knockback();
				}
			}
		}

		public void interact_state()
		{
			if (!beginInteract) set_state(StateType.IDLE, new ArrayList(){}, StateType.IDLE);
			else interactFunction.Invoke(this, new EventArgs());			
		}
		
		// ========================================== SETTERS ==========================================
		
		// Sets the current and previous position AFTER player input but BEFORE collision handling.
		public void set_positions()
		{
			currPosition = position;
			float tempVelocityX;
			float tempVelocityY;

			if (direction == -1) tempVelocityX = velocity.X - (speed * acceleration);
			else tempVelocityX = velocity.X + (speed * acceleration);

			tempVelocityY = velocity.Y + (fallSpeed * gravity);
			
			nextPosition.X = position.X + tempVelocityX;
            nextPosition.Y = position.Y + tempVelocityY;
		}

		/// Changes the players current state, setting it to <param name="newState"> while recording the previous state.
		/// Also resets the stateFrame count.
        public void set_state(StateType newState, ArrayList newPreanimations, StateType newTempanimation)
        {
			if (currentState == newState) return;

            previousState = currentState;
            currentState = newState;
            stateFrame = 0;

            preAnimations = newPreanimations;
            tempAnimation = newTempanimation;

            find_state(newState); // Call the corresponding state function
        }

		// Finds the state <newState> and calls the function that corresponds to it
		private void find_state(StateType newState)
        {
            switch (newState)
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
                case StateType.RUN:
                    run_state();
                    break;
                case StateType.CROUCH:
                    crouch_state();
                    break;
				case StateType.DASH:
                    dash_state();
                    break;
                case StateType.WALLCLING:
                    wallCling_state();
                    break;
                case StateType.WALLJUMP:
                    wallJump_state();
                    break;
				case StateType.BASICATTACK1:
                    PlayerAttacks.bAttack1_State(this);
                    break;
				case StateType.BASICATTACK2:
                    PlayerAttacks.bAttack2_State(this);
                    break;
				case StateType.BASICATTACK3:
                    PlayerAttacks.bAttack3_State(this);
                    break;
				case StateType.BASICATTACK4:
                    PlayerAttacks.bAttack4_State(this);
                    break;
				case StateType.AIRATTACK1:
                    PlayerAttacks.airAttack1_State(this);
                    break;
				case StateType.AIRATTACK2:
                    PlayerAttacks.airAttack2_State(this);
                    break;
				case StateType.AIRATTACK3:
                    PlayerAttacks.airAttack3_State(this);
                    break;
				case StateType.UPFINISHER:
                    PlayerAttacks.upFinisher_State(this);
                    break;
				case StateType.DOWNFINISHER:
                    PlayerAttacks.downFinisher_State(this);
                    break;
				case StateType.FRONTFINISHER:
                    PlayerAttacks.frontFinisher_State(this);
                    break;
				case StateType.BACKFINISHER:
                    PlayerAttacks.backFinisher_State(this);
                    break;
				case StateType.HIT:
					hit_state();
					break;
                case StateType.INTERACT:
                    interact_state();
                    break;
            }
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
                
				if (hitStop <= 0 )stateFrame++; // Dont update the frame during hitstop
				else hitStop--;

				update_cooldowns();
            }
			if (stateFrame >= 240) stateFrame = 0;
        }

		// Sets the corresponding boolean values depending on player input. Useful for setting the values
		// without calling the analogous state method
		public override void update_input()
		{
			// Vertical Movement
			if (controls.isDown(controls.Up)) move_up = true;
			else move_up = false; 

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
			// Walljumping 
			if (controls.isDown(controls.Jump)) softJumping = true;
			else softJumping = false;
			
			// Jumping
			if (controls.isDown(controls.Jump) && numJumps > 0) jumping = true;

			// Crouch
			if (controls.isDown(controls.Crouch)) crouching = true;
			else crouching = false;

			// Dash
			if (controls.isDown(controls.Dash) && numDashes > 0 && dashCooldown == 0) dashing = true;

			// Attack
			if (controls.isDown(controls.Attack)) attacking = true;
			else attacking = false;

			// Map Interactions
			if (controls.isDown(controls.Interact) && interactCooldown == 0) interacting = true;
			else interacting = false;

		}

		// Update the players state, and set tqhe values for the physics to be applied
		public override void update_state()
		{
			if (hit) 
			{
				set_state(StateType.HIT, preAnimations = new ArrayList(){StateType.HITSTART}, tempAnimation = StateType.HIT);
			}
			if (hitStop > 0 ) return;

			find_state(currentState);
			
			grounded = false;
			wallCollide = false;
			fall();
			decelerate();
			set_positions(); // Update the "position to be drawn" of the player
		}
	
		// Applies physics to the position of the character. Reseting boolean values as necessary. 
		// This function should NOT set any physics related stuff like acclereation and velocity unless
		// those values are being reset
        public override void update_physics()
        {
			// Dont apply any physics during hitstun
			if (hitStop > 0) return;

            // Flipping
            if (direction == -1) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

			if (grounded) 
			{
				numJumps = 1;
				velocity.Y = 0f;
			}

			position.Y += velocity.Y ;
            position.X += velocity.X ;
			
			if (position.Y >= 2000f || health <= 0) 
			{
				position = spawnPoint;
				velocity.Y = 0f; 
				health = 100;
			}
			origin = position + originOffset;
			hurtbox.update_Position(position);
        }

		// Modify the players cooldowns depending on the time
		public void update_cooldowns()
		{
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
			// --------- Wall Collision
			if (wallCollide && !grounded)
			{
				if (wallBuffer <= 0) 
				{
					wallBuffer = 0;

					if (wallCooldown <= 0) wallCooldown = 0; // to prevent scaling the same wall 
					else wallCooldown -= 1;
				}
				else wallBuffer -= 1;
			}
			else wallBuffer = 7;

			if (grounded)
			{
				numJumps = 1;
				wallCooldown = 80;
			}

			// -------- Health
			if (health < 0) health = 0;

			// -------- Interact
			if (interactCooldown > 0) interactCooldown -= 1;
			else if (interactCooldown < 0) interactCooldown = 0;
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

		// ========================================== DRAWING ==========================================
		public void drawDebug(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Messages to display game / player information
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nState Time (24FPS): " + stateFrame
			+ "\nAnimation: " + currentAnimation
			+ "\nDash Cooldown: " + dashCooldown
			+ "\nWalljump Cooldown: " + wallCooldown
			+ "\nWallBuffer: " + wallBuffer
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun
			+ "\nHitstop: " + hitStop;
			
			movementMSG = "Position: " + position.ToString() 
			+ "\nHurtbox: " + hurtbox.position.ToString()
			+ "\nVelocity: " + velocity.ToString()
            + "\nAcceleration: " + acceleration.ToString() 
			+ "\nDirection: " + direction.ToString()
			+ "\nGravity: " + gravity.ToString();

			Matrix positionMatrix = Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0 ));
			// string animateMSG = "Animations: " 
			// + "\n" + animManager;
			
			string inputMSG = 
			"Left = " + move_left.ToString() 
			+ "\nRight = " + move_right.ToString()
			+ "\nJumping = " + jumping
			+ "\nDashing = " + dashing
			+ "\nCrouching = " + crouching
			+ "\nGrounded = " + grounded
			+ "\nAttacking = " + attacking
			+ "\nInterating = " + beginInteract
			+ "\nHit = " + hit
			+ "\nWall = " + wallCollide
			+ "\nIgnore Size: " + hitbox.ignoreObjects.Count;

			spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(hurtboxWidth+ 20, -100) + position, Color.GreenYellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(-39, -120) + position, Color.Gold, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(-39, hurtboxHeight) + position, Color.Blue, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			// spriteBatch.DrawString(stateFONT,animateMSG, new Vector2(hurtboxWidth + 20, hurtboxHeight) + position, Color.OrangeRed, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);		

		}
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Pause on the current frame when hit
			if (hitStop > 0) animManager.Pause();
			else animManager.Resume();

			// Draw player animation
			animManager.Draw(gameTime, spriteBatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// hurtbox.Draw(spriteBatch,flip);

			// foreach (Hitbox hitbox in allHitboxes) 
			// {
			// 	if (hitbox.active) hitbox.Draw(spriteBatch, SpriteEffects.None);
			// }
			// spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, 5f, 5f), Color.DarkSeaGreen, 2);
			
			// Draw debug messages
			// drawDebug(gameTime, spriteBatch);	
        }
	}
}

