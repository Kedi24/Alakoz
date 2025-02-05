using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.Collision;
using Alakoz.GameInfo;
using MonoGame.Extended;

namespace Alakoz.GameObjects
{
	public class Player : GameObject
	{	
		#region // ------ OTHER ------ //

		#endregion

		#region // ----- Physics Variables ----- //
		public Vector2 spawnPoint;

		public float gravity = 0.25f;
		public float speed = 2f; // Default speed
		public float fallSpeed = 1f;
		public float velocityMax = 4f;
		public float terminalVelocity = 8f;
		public float acceleration = 0.175f; // Default acceleration
		public float deceleration = 0.1f; // Default deceleration

		// ---- Moving
		public bool move_left = false;
		public bool move_right = false;
		public bool move_up = false;
		public bool move_down = false;
		public bool grounded = false;
		
		// ---- Jumping
		public bool jumping = false;
		public int numJumps = 1;
		public int jumpDelay = 1;
		public float jumpHeight = -7f;
		public bool softJumping = false;
		public bool wallJumping = false;
		public bool wallCollide = false; // Indicates a wall collision
		public int wallBuffer = 0; // To prevent immediate wall interactions
		public int wallCooldown = 80;
		public float wallFallSpeed = 0.5f;
		
		// ---- Dashing
		public bool dashing = false;
		public int numDashes = 1;
		public int dashCooldown = 15;
		public float dashSpeed = 4f;
		public float dashMaxSpeed = 12f;

		// ---- Crouching
		public bool crouching = false;

		// ---- Attacking
		public bool attacking = false;
		public int attackCounter = 1;

		// ---- Hit 
		public bool hit = false;
		public int hitstun = 0;
		public int health = 100;
		public float tempHealthWidth = 100;
		public int maxHealth = 100;
	
		// ---- Interacting
		public bool interacting = false; // When the player presses the interact button
		public bool beginInteract = false; // When there is something for the player to interact with
		
		// The function to call when the player interacts with another GameObject
		// This delegates the states of the player to the function defined in the corresonding GameObject instead.
		public event EventHandler interactFunction; 
		public int interactCooldown = 0;
		#endregion
		
		#region // ----- Collision Variables ------ //

		// ---- Hurtbox		
		public Hurtbox hurtbox {get; set;}		
		
		// ---- Attack hitboxes
		public Hitbox[] allHitboxes = new Hitbox[5]; // Array to store a preset number of hitboxes 

		#endregion
		
		#region // ----- Animation Variables ----- //
		public Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite
		public Vector2 spriteCoordDefault = new Vector2(-39, -36);
		#endregion

		#region // ----- Skill Variables ----- //
		#endregion
		
		#region // ----- Sound Variables ----- //
		#endregion

		#region // ----- Effect Variables ----- //
		SpriteEffects flip;
		#endregion

		#region // ----- Input Variables ----- //
		public Controls controls;
		#endregion

		#region // ----- State Variables ----- //
		#endregion

		#region // ----- Debug Variables ----- //
		public string movementMSG = "";
		public string stateMSG = "";
		float tempSteps = 1;
		#endregion;
		public Player(Dictionary<TState, Animation> newSprites, Vector2 newPosition, int ID = -1){
			id = ID;
			type = TObject.PLAYER;
			previousState = TState.AIR;
			currentState = TState.AIR;
			
			hurtboxWidth = 34;
			hurtboxHeight = 44;
			hurtbox = new PlayerHurtbox(this, position, hurtboxWidth, hurtboxHeight);

			position = newPosition;
			spawnPoint = newPosition;
			originOffset = new Vector2(hurtboxWidth / 2, hurtboxHeight / 2);
			origin = position + originOffset;
			
			// Add hitboxes and hurtboxes to collision objects
			// There is space for, at most, 5 hitboxes on a single frame
			for (int i = 0; i < 5; i++) 
			{
				allHitboxes[i] = new Hitbox(Vector2.Zero, 0f, 0f, 0, Vector2.Zero, 0, 0);
				allHitboxes[i].owner = this;
				allHitboxes[i].addIgnore(hurtbox); // To prevent dealing damage to yourself
				activeCollisions.Add(allHitboxes[i]);
			}
			activeCollisions.Add(hurtbox);

			stateFrame = 0;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[TState.FALL]);

			currAnimation = TState.FALL;
            controls = new Controls();

			flip = SpriteEffects.None;
		}

		public int getHealth() {return health;}
		public int getMaxHealth() {return maxHealth;}
		public int getStateFrame() {return stateFrame;}
		public int getFrame() {return (int)Math.Ceiling( (double)stateFrame / 2);}

		#region // ========== PHYSICS FUNCTIONS ========== //

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
		// ------------------------------ Basic
        private void move(){
			velocity.X = approach(velocity.X, direction*velocityMax, speed * acceleration);
        }

		private void jump()
        {
			velocity.Y = jumpHeight;
			numJumps -= 1;

			// Prevent dash -> jump velocity from being transferred when high.
			if (velocity.X > velocityMax) velocity.X = velocityMax;
			else if (velocity.X < -velocityMax) velocity.X = -velocityMax;
			
			if (numJumps < 0) numJumps = 0;
		}
		private void dash()
    	{
			int frame = getFrame();
    		// dash physics
			velocity.Y = 0.01f;
            numDashes = 0;
			if (frame == 3) set_animations(TState.NONE);
    		
			if (frame < 4){
				if (move_left) direction = -1;
				else if (move_right) direction = 1;
				velocity.X *= 0.8f;
    		}
			if (frame == 4)velocity.X = direction*dashMaxSpeed;
    		else if (frame < 9) velocity.X = approach(velocity.X, direction*speed, .5f);
            // else velocity.X = direction*dashMaxSpeed;
    		// grounded = false;
    	}		

		private void fall()
		{
			if (velocity.Y < terminalVelocity) velocity.Y = approach(velocity.Y, terminalVelocity, gravity);
			else velocity.Y = terminalVelocity;
		}
		
		private void decelerate() {
			velocity.X = approach(velocity.X, 0, speed * deceleration);
		}
		
		private void knockback()
		{
			velocity = KB;
			applyKB = false;
		} 

		//------------------------------ Unique
        private void walljump() 
		{
			fallSpeed = 1f;
			wallBuffer = 7;
			wallCollide = false;

			// Physics
			velocity.Y = -7f;
			// if (direction == -1) velocity.X
			velocity.X = direction*-7f;

			// Booleans
			jumping = false;
		}
		#endregion
		
		#region // ========== STATE FUNCTIONS ========== //
		
		// All the state functions for the player depending on player input and player state
		// These functions contain what to do when in a certain state and when to do it
		public void air_state()
		{
			set_coordinate(0, 15);

			if (grounded)
			{
				set_state(TState.IDLE, TState.CROUCHEND, TState.IDLE);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				// hurtbox.resetDimensions();
				set_state(TState.WALLCLING, TState.WALLCLING);
			}
			else if (jumping) 
			{
				// hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
			}
			else if (dashing) 
			{
				// hurtbox.resetDimensions();
				set_state(TState.DASH, TState.DASHSTART);
			}
			else if (attacking)
			{
				// attackCounter++;
				set_state(TState.AIRATTACK1, TState.AIRATTACK1, TState.NULL);
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
				set_state(TState.AIR, TState.FALL);
			}
			else if (jumping) {
				grounded = false;
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
			}
			else if (move_left || move_right) {
				hurtbox.resetDimensions();
				set_state(TState.RUN, TState.RUNSTART, TState.RUN);
			}
			else if (crouching)
			{
				hurtbox.changeDimensions(new Vector2(0, hurtbox.height / 2), hurtboxWidth, hurtboxHeight/2);
				set_state(TState.CROUCH, TState.CROUCHSTART, TState.CROUCH);
			}
			else if (dashing) set_state(TState.DASH, TState.DASHSTART);
			else if (attacking){
				attackCounter++;
				set_state(TState.BASICATTACK1, TState.BASICATTACK1, TState.IDLE);
			} 
			else if (beginInteract) set_state(TState.INTERACT, nextAnimation);
			else{
				if (stateFrame % 800 == 0) set_animations(TState.CROUCHSTART, TState.CROUCH);
				if (stateFrame % 1600 == 0) set_animations(TState.CROUCHEND, TState.IDLE);
			}
		}
		
		public void jump_state()
		{
			set_coordinate(0, 8);
			if (stateFrame < jumpDelay) velocity.Y = 0; // Jump delay
			if (velocity.Y < 0.5 && velocity.Y > -0.5 && stateFrame > jumpDelay) // fix later
			{
				jumping = false;
				hurtbox.resetDimensions();
				set_state(TState.AIR, TState.BALLSTART, TState.BALL, TState.BALLEND, TState.FALL);
				return;
			}	
			else if (grounded) // To prevent jump -> idle -> jump... loop 
			{
				jumping = false;
				velocity.Y = 0;
				hurtbox.resetDimensions();
				set_state(TState.IDLE, TState.CROUCHEND, TState.IDLE);
				
			}
			else if (dashing) 
			{
				jumping = false;
				
				hurtbox.resetDimensions();
				set_state(TState.DASH, TState.DASHSTART);
			} 
			else if (attacking)
			{
				jumping = false;
				hurtbox.resetDimensions();

				attackCounter++;
				set_coordinate();
				set_state(TState.AIRATTACK1, TState.AIRATTACK1, TState.NULL);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				jumping = false;
				hurtbox.resetDimensions();
				set_coordinate();
				set_state(TState.WALLCLING, TState.WALLCLING);
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
				set_state(TState.AIR, TState.BALLSTART, TState.BALL, TState.BALLEND, TState.FALL);
			}
			else if (jumping) 
			{
				grounded = false;
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);		
				set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);	
			}
			else if (dashing) 
			{
				set_state(TState.DASH, TState.DASHSTART);
			}
			else if (attacking)
			{
				// attackCounter++;
				set_state(TState.BASICATTACK1, TState.BASICATTACK1);
			} 
			else if (beginInteract) /// FIX LATER
			{
				if (velocity.X > 2.5 || velocity.X < -2.5) set_animations(post:TState.RUNEND);
				
				hurtbox.resetDimensions();
				set_state(TState.INTERACT, TState.CROUCHEND, TState.IDLE);
			}
			else
			{
				if (!(move_left || move_right))
				{
					
					if (velocity.X > 2.5 || velocity.X < -2.5) set_state(TState.IDLE, TState.RUNEND, TState.IDLE);
					else set_state(TState.IDLE, TState.IDLE, TState.IDLE);
				} else if (move_left) 
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
				set_state(TState.AIR, TState.BALLEND, TState.FALL);
			} 
			else if (!crouching)
			{
				hurtbox.resetDimensions();
				set_state(TState.IDLE, TState.CROUCHEND, TState.IDLE);
			}
			else if (jumping)
			{
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);		
				set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);	
			}
			else if (dashing)
			{
				hurtbox.resetDimensions();
				set_state(TState.DASH, TState.DASHSTART, TState.DASH);
			}
			else if (move_left || move_right)
			{
				hurtbox.resetDimensions();
				set_state(TState.RUN, TState.RUNSTART, TState.RUN);
			}
			else if (beginInteract)
			{
				hurtbox.resetDimensions();
				set_state(TState.INTERACT, TState.CROUCHEND, TState.IDLE);
			}
		}

		public void dash_state(){
			int frame = getFrame();
			if (frame == 12){ // State ends
				dashing = false;
				
				dashCooldown = 15;
				attackCounter = 1;
				
				// hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				if (grounded) set_state(TState.IDLE, TState.TOGROUND, TState.IDLE);
				else set_state(TState.AIR, TState.TOAIR, TState.FALL);
				return;
			}
			else if (jumping){
				// Jump Cancel
				dashing = false;
				
				dashCooldown = 30;
				grounded = false;
				attackCounter = 1;
				
				hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
				set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
			}
			else if (wallCollide && wallBuffer == 0) 
			{ 
				dashing = false;
				dashCooldown = 15;
				attackCounter = 1;
				set_state(TState.WALLCLING, TState.WALLCLING);
			}
			else if (attacking)
			{
				if (velocity.X > velocityMax || velocity.X < -velocity.X) velocity.X = velocityMax * direction;
				dashing = false;
				
				if (attackCounter > 5 ) attackCounter = 1;
				set_attack();
			}
			else dash();	
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

				set_state(TState.IDLE, TState.CROUCHEND, TState.IDLE);
			}
			else if (dashing)
			{
				fallSpeed = 1f;
				wallBuffer = 7; // Prevent scaling walls infinitely
				wallCollide = false;
				wallJumping = false;
				wallCooldown -= 5;
				
				set_state(TState.DASH, TState.DASHSTART, TState.DASH);
			}
			else if (!wallCollide || wallCooldown <= 0 || (direction == -1 && move_right) || (direction == 1 && move_left))
			{
				fallSpeed = 1f;
				wallBuffer = 7;
				wallJumping = false;
				wallCollide = false;

				// hurtbox.changeDimensions(nwwew Vector2(0, -10), 34, 42);
				set_state(TState.AIR, TState.FALL);
			} 
			else 
			{

				set_coordinate(9, 8);
				// fallSpeed = wallFallSpeed;
				if (softJumping)
				{
					wallCollide = false;
					set_state(TState.WALLJUMP, TState.WALLJUMPSTART, TState.WALLJUMP, TState.FALL);
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
			int frame = getFrame();

			if (stateFrame < 7) set_coordinate(9, 8);
			else set_coordinate(5, 15);
			
			if (grounded)
			{
				velocity.Y = 0;
				fallSpeed = 1f;
				wallBuffer = 7;
				wallCollide = false;
				wallJumping = false;
				
				set_state(TState.IDLE, TState.CROUCHEND, TState.IDLE);
			}
			// State Restrictions
			else if (stateFrame == 7)
			{
				wallCooldown -= 5; // Prevent scaling walls infinitely
				wallJumping = false;
				wallBuffer = 7;
				
				walljump(); // Perform the jump		
			} 
			else if (frame < 4) velocity.Y = 0;
			else if (frame < 15)
			{
				if (wallCollide)  
				{ 
					if (direction == -1) direction = 1;
					else direction = -1;

					wallJumping = false;
					wallBuffer = 0;

					hurtbox.resetDimensions();
					set_state(TState.WALLCLING, TState.WALLCLING);
				}
			}
			// else if (stateFrame == 15)
			// {
			// 	if (direction == -1 ) direction = 1;
			// 	else direction = -1;
			// }
			else 
			{
				set_coordinate();
				set_state(TState.AIR);

			}
		}
		
		public void hit_state()
		{
			grounded = false;
			if (stateFrame == hitstun) {
				hit = false;
				hitstun = 0;
				set_state(TState.AIR, TState.FALL);
				return;
			}
			else{
				move_left = false;
				move_right = false;
				jumping = false;
				crouching = false;
				dashing = false;

				nextPosition = position; // To prevent map based collisions that depend on the next position from being invoked (grounded, etc)

				if (applyKB) {
					stateFrame = 1; // So that the frames dont accumulate when hit multiple times during hitstun
					// To make sure the impact frames play when hit again DURING hitstun
					if (currAnimation == TState.HIT) set_animations(TState.HITSTART, TState.HIT);
					knockback();
				}
			}
		}
		
		// ----------------------------- MAP INTERACTIONS
        public void interact_state()
		{
			if (!beginInteract) 
			{
				nextAnimation = TState.IDLE;
				set_state(TState.IDLE);
				idle_state();
			}
			else interactFunction.Invoke(this, new EventArgs());			
		}
		#endregion
		
		#region // ========== SETTING FUNCTIONS ========== //
		public void set_coordinate(float x = 0, float y = 0){ 
			spriteCoordinate.X = spriteCoordDefault.X + (direction*x); 
			spriteCoordinate.Y = spriteCoordDefault.Y + y;
		}
		// Sets the current and previous position AFTER player input but BEFORE collision handling.
		public void set_positions(){
			float tempVelocityX;
			float tempVelocityY;

			if (direction == -1) tempVelocityX = velocity.X - (speed * acceleration);
			else tempVelocityX = velocity.X + (speed * acceleration);

			tempVelocityY = velocity.Y + (fallSpeed * gravity);
			
			nextPosition.X = position.X + tempVelocityX;
            nextPosition.Y = position.Y + tempVelocityY;

			// When the object travels at high speed (> 16px), break up the motion
            float velocityTotal = Math.Abs(tempVelocityX) + Math.Abs(tempVelocityY);
            hurtbox.velocitySteps = (float)Math.Ceiling( velocityTotal / 16);
            tempSteps = (float)Math.Ceiling( velocityTotal / 16);
		}
		public override void set_state(TState newState, TState newAnim = TState.NULL, params TState[] newPostAnim){
			if (currentState == newState) return;
			base.set_state(newState, newAnim, newPostAnim);
			set_coordinate();
            find_state(newState); // Call the corresponding state function
        }

		public void set_attack()
		{
			switch (attackCounter){
				case 2:
					set_state(TState.BASICATTACK2, TState.BASICATTACK2, TState.IDLE);
					break;
				case 3:
					set_state(TState.BASICATTACK3, TState.BASICATTACK3, TState.IDLE);
					break;
				case 4: 
					set_state(TState.BASICATTACK4, TState.BASICATTACK4, TState.IDLE);
					break;
				case 5:
					PlayerAttacks.setFinisher(this, getFrame());
					break;
				default:
					set_state(grounded? 
						TState.BASICATTACK1 : TState.AIRATTACK1, 
						TState.BASICATTACK1, 
						TState.IDLE);
					break;
			}
		}
		
		private void find_state(TState newState){
            switch (newState)
            {
                case TState.AIR:
                    air_state();
                    break;
                case TState.IDLE:
                    idle_state();
                    break;
                case TState.JUMP:
                    jump_state();
                    break;
                case TState.RUN:
                    run_state();
                    break;
                case TState.CROUCH:
                    crouch_state();
                    break;
				case TState.DASH:
                    dash_state();
                    break;
                case TState.WALLCLING:
                    wallCling_state();
                    break;
                case TState.WALLJUMP:
                    wallJump_state();
                    break;
				case TState.BASICATTACK1:
                    PlayerAttacks.bAttack1_State(this, getFrame());
                    break;
				case TState.BASICATTACK2:
                    PlayerAttacks.bAttack2_State(this, getFrame());
                    break;
				case TState.BASICATTACK3:
                    PlayerAttacks.bAttack3_State(this, getFrame());
                    break;
				case TState.BASICATTACK4:
                    PlayerAttacks.bAttack4_State(this, getFrame());
                    break;
				case TState.AIRATTACK1:
                    PlayerAttacks.airAttack1_State(this, getFrame());
                    break;
				case TState.AIRATTACK2:
                    PlayerAttacks.airAttack2_State(this, getFrame());
                    break;
				case TState.AIRATTACK3:
                    PlayerAttacks.airAttack3_State(this, getFrame());
                    break;
				case TState.UPFINISHER:
                    PlayerAttacks.upFinisher_State(this, getFrame());
                    break;
				case TState.DOWNFINISHER:
                    PlayerAttacks.downFinisher_State(this, getFrame());
                    break;
				case TState.DOWNFINISHERGROUND:
					PlayerAttacks.downFinisherLag_State(this, getFrame());
					break;
				case TState.FRONTFINISHER:
                    PlayerAttacks.frontFinisher_State(this, getFrame());
                    break;
				case TState.BACKFINISHER:
                    PlayerAttacks.backFinisher_State(this, getFrame());
                    break;
				case TState.HIT:
					hit_state();
					break;
                case TState.INTERACT:
                    interact_state();
                    break;
            }
        }
		
		#endregion
		
		#region // ========== UPDATING ========== //
        public override void update_time(){
			animManager.Update();
			base.update_time();
			update_cooldowns();
        }
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

			// Interactions
			if (controls.isDown(controls.Interact) && interactCooldown == 0) interacting = true;
			else interacting = false;

		}

		// Update the players state, and set the values for the physics to be applied
		public override void update_state(){
			prevPosition = position;	
			if (hit) set_state(TState.HIT, TState.HITSTART, TState.HIT);
			if (hitStop > 0 ) return;

			fall();
			decelerate();
			find_state(currentState);

			grounded = false;
			wallCollide = false;
			
			set_positions(); // Update the "position to be drawn" of the player
		}
	
		// Applies physics to the position of the character. Reseting boolean values as necessary. 
		// This function should NOT set any physics related stuff like acclereation and velocity unless
		// those values are being reset
        public override void update_physics()
        {
			if (hitStop > 0) return;
            
			// Flipping
            if (direction == -1) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

			if (grounded) numJumps = 1;

			position += velocity;
			
			if (position.Y >= 2000f || health == 0) {
				position = spawnPoint;
				velocity.Y = 0f; 
				health = 100;
			}

			origin = position + originOffset;
			hurtbox.update_Position(position);
			hurtbox.velocitySteps = 1; // reset steps
            hurtbox.currentStep = 1;
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
			if (wallCollide && !grounded){
				if (wallBuffer <= 0) 
				{
					wallBuffer = 0;

					if (wallCooldown <= 0) wallCooldown = 0; // to prevent scaling the same wall 
					else wallCooldown -= 1;
				}
				else wallBuffer -= 1;
			}
			else wallBuffer = 7;

			if (grounded){
				numJumps = 1;
				wallCooldown = 80;
			}
			if (attackCounter > 5) attackCounter = 5;

			// -------- Health
			if (health < 0) health = 0;

			// -------- Interact
			if (interactCooldown > 0) interactCooldown -= 1;
			else if (interactCooldown < 0) interactCooldown = 0;
		}
	
		#endregion
		
		#region // ========== DRAWING ========== // 
		public void drawStateMSG(SpriteBatch spriteBatch, float x, float y, Vector2 newScale){
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nState Frame (24FPS): " + stateFrame
			+ "\nAnimation: " + currAnimation
			+ "\nAnimation: " + getFrame()
			+ "\nDash Cooldown: " + dashCooldown
			+ "\nWalljump Cooldown: " + wallCooldown
			+ "\nWallBuffer: " + wallBuffer
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun
			+ "\nHitstop: " + hitStop
			+ "\nCurrent Attack: " + attackCounter;

			spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(x, y), Color.Gold, 0f, Vector2.Zero, scale:newScale, SpriteEffects.None, 1f);

			// spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(-39, -120) + position, Color.Gold, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
		}
		
		public void drawMovementMSG(SpriteBatch spriteBatch, float x, float y, Vector2 newScale){

			movementMSG = "Position: " + position.ToString()
			+  "\nOrigin: " + origin.ToString()
			+ "\nHurtbox: " + hurtbox.position.ToString()
			+ "\nVelocity: " + velocity.ToString()
            + "\nAcceleration: " + acceleration.ToString() 
			+ "\nDirection: " + direction.ToString()
			+ "\nGravity: " + gravity.ToString()
			+ "\nSprite Coord: " + spriteCoordinate.ToString();

			spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(x, y), Color.GreenYellow, 0f, Vector2.Zero, scale:newScale, SpriteEffects.None, 0f);
		}
		
		public void drawInputMSG(SpriteBatch spriteBatch, float x, float y, Vector2 newScale){
			string inputMSG = 
			" >>>>> CONTROLS"
			+ "\n" + controls.Left.ToString() + " | " + "(A) Left = " + move_left.ToString() 
			+ "\n" + controls.Right.ToString() + " | " + "Right = " + move_right.ToString()
			+ "\n" + controls.Crouch.ToString() + " | " + "Crouching = " + crouching
			+ "\n" + controls.Jump.ToString() + " | " + "Jumping = " + jumping
			+ "\n" + controls.Dash.ToString() + " | " + "Dashing = " + dashing
			+ "\n" + controls.Attack.ToString() + " | " + "Attacking = " + attacking
			+ "\n" + controls.Interact.ToString() + " | " + "Interating = " + beginInteract
			+ "\n" + "Grounded = " + grounded
			+ "\n" + "Hit = " + hit
			+ "\n" + "Wall = " + wallCollide
			+ "\n" + "WallJumping = " + wallJumping;

			spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(x, y), Color.GhostWhite, 0f, Vector2.Zero, scale:newScale, SpriteEffects.None, 0f);

		}
		
		public void drawCollisionMSG(SpriteBatch spriteBatch, float x, float y, Vector2 newScale){
			string collisionMSG =
			"\nHitbox 1 Links = " + allHitboxes[0].linkedObjects.Count
			+ "\nHitbox 2 Links = " + allHitboxes[1].linkedObjects.Count
			+ "\nHitbox 1 Collisions = " + allHitboxes[0].collidingObjects.Count
			+ "\nHitbox 2 Collisions = " + allHitboxes[1].collidingObjects.Count;

			spriteBatch.DrawString(stateFONT, collisionMSG, new Vector2(x, y), Color.OrangeRed, 0f, Vector2.Zero, scale:newScale, SpriteEffects.None, 0f);		
		}
        
		public void drawUI(SpriteBatch spriteBatch, Camera camera){
			float healthX = camera.Right() - (camera.Width()*0.35f);
			float healthY = camera.Bottom() - (camera.Height()*0.10f); 
			float healthHeight = camera.Height()*0.05f;
			float healthWidth = camera.Width()*0.30f;

			float tempWidth = healthWidth*((float)health/ (float)maxHealth);
			Vector2 healthLerp = Vector2.Lerp(new Vector2((float)tempHealthWidth, 0f), new Vector2((float)health, 0f), 0.05f);
			tempHealthWidth = healthLerp.X;

			// Draw health bar
			// spriteBatch.DrawRectangle(healthX, healthY, healthWidth, healthHeight, Color.DarkRed, camera.Height()*0.005f);
			// spriteBatch.DrawRectangle(healthX, healthY, healthWidth * (healthLerp.X/(float)maxHealth), healthHeight, Color.DarkRed, camera.Height()*0.05f);
			// spriteBatch.DrawRectangle(healthX, healthY, tempWidth, healthHeight, Color.GhostWhite, camera.Height()*0.05f);
			
			drawStateMSG(spriteBatch, camera.Left()+(camera.Width()*0.01f), camera.Top()+(camera.Height()*0.01f), camera.Scale());
			drawMovementMSG(spriteBatch, camera.Left()+(camera.Width()*0.01f), camera.Top()+(camera.Height()*0.8f), camera.Scale());
			drawInputMSG(spriteBatch, camera.Left()+(camera.Width()*0.82f), camera.Top()+(camera.Height()*0.01f), camera.Scale());
			// drawCollisionMSG(spriteBatch, camera.Left()+(camera.Width()*0.85f), camera.Top()+(camera.Height()*0.8f), camera.Scale());
		}
        
		public void drawCollision(SpriteBatch spriteBatch){
			// ----- Hitbox
			foreach (Hitbox hitbox in allHitboxes) {
				if (hitbox.active) hitbox.Draw(spriteBatch, SpriteEffects.None, Color.White, hitbox.position, hitbox.width, hitbox.height);
			}
			// ----- Hurtbox
            // spriteBatch.DrawRectangle(position.X, position.Y, hurtboxWidth, hurtboxHeight, Color.Gold, 2);
            spriteBatch.DrawRectangle(hurtbox.getPosition().X, hurtbox.getPosition().Y, hurtbox.width, hurtbox.height, Color.Gold, 2);
			spriteBatch.DrawRectangle(origin.X- 1, origin.Y-1, 5, 5, Color.Purple, thickness:2);

			// Interpolated Hurtbox
			if (tempSteps > 1){
                for (int i = 0; i <= tempSteps; i++){
                    float drawX = position.X + (i*(velocity.X / tempSteps));
                    float drawY = position.Y + (i*(velocity.Y / tempSteps));
                    // spriteBatch.DrawRectangle(drawX, drawY, hurtboxWidth, hurtboxHeight, Color.AntiqueWhite, 2);
                } 
                float drawX2 = position.X + (tempSteps*(velocity.X / tempSteps));
                float drawY2 = position.Y + (tempSteps*(velocity.Y / tempSteps));
                // spriteBatch.DrawRectangle(drawX2, drawY2, hurtboxWidth, hurtboxHeight, Color.Red, 2);
            } 
		}
		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Pause current frame if hit
			if (hitStop > 0) animManager.Pause();
			else animManager.Resume();
			
			// Draw player animation
			Vector2 drawPos = position; 
			// Vector2 drawPos = Vector2.Lerp(prevPosition, position, Game1.thisGame.frameProgress); 
			animManager.Draw(gameTime, spriteBatch, drawPos + spriteCoordinate, Vector2.One, flip);
			// drawCollision(spriteBatch);
			

			// hurtbox.Draw(spriteBatch,flip);
        }
		
		#endregion
	}
}

