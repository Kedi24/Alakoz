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

		public float acceleration;
		public float gravity;
		public float friction;
		public float accelMax = 10f;
		public float accelMin = 0.1f;

		public float gravityMax = 2f;
		public float gravityMin = 0;

		public int direction = 1;
		public int numJumps = 1;
		public bool jumping = false;
		public bool grounded = false;
		
		public bool dashing = false;
		public Vector2 dashPosition = new Vector2(0f, 0f);
		public Vector2 dashVelocity = new Vector2(0f, 0f);
		public float dashAcceleration = 1;

		// ------ COLLISION ------ //
		
		public Hurtbox hurtbox {get; set;}
		float hurtboxWidth = 34;
		float hurtboxHeight = 45;
		bool hurtboxVisual = true;
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite

		public Vector2 hurtboxOffset;

		// ------ ANIMATION ------ //
		public Dictionary<string, Animation> animDictionary;
		public AnimationManager animManager;

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
		public const string JUMPSQUAT = "jumpsquat";
		public const string LAND = "land";

		public const string RUN = "run";
		public const string RUN_START = "run_start";
		public const string RUN_END = "run_end";
		public const string TURNAROUND = "turnaround";
		
		public const string CROUCH = "crouch";
		public const string CROUCH_START = "crouch_start";
		public const string CROUCH_END = "crouch_end";

		public const string DASH = "dash";

		public const string ATTACK = "attack";
		public const string ATTACK_START = "attack_start";
		public const string ATTACK_END = "attack_end";

		public const string SKILL = "skill";

		public int STATELOCK { get; set; }

		// =========================================== CONSTRUCTORS ============================================
		public Player(Dictionary<string, Animation> newSprites, Vector2 newPosition)
		{
			// Default state
			currentState = AIR; 
			stateFrame = 0;
			stateTimer = 0f;

			// Default physics
			position = newPosition ;
			acceleration = 1f;
			friction = 1f;

			// Default Collision
			hurtbox = new Hurtbox(this, position, hurtboxWidth, hurtboxHeight, newSprites["Hurtbox"]);

			// Default Animations
            animDictionary = newSprites;
            animManager = new AnimationManager(newSprites["Base_Idle"], true);
            animManager.Position = position;
			flip = SpriteEffects.None;

			// Default Controls
            controls = new Controls();


		}
		// =========================================== MOVEMENT FUNCTIONS ============================================
       private void move()
        {
            if (direction == 0) velocity.X = -0.5f;
            else if (direction == 1) velocity.X = 0.5f;

            if (acceleration < accelMax) acceleration += 0.1f;
            else acceleration = accelMax;

			if (gravity < gravityMax) gravity += 0.1f;
			else gravity = gravityMax;
			return;
        }

        private void jump()
        {
            velocity.Y = -6.0f;
            numJumps = 0;
            jumping = true;
			grounded = false;
        }
		private void fall()
		{
			velocity.Y += 0.2f;
			if (gravity < gravityMax) gravity += 0.1f;
			else gravity = gravityMax;
			
		}

		private void dash()
		{
			dashing = true; 
			// dash physics
			if (direction == 1) velocity.X = 3f;
			else velocity.X = -3f;
			velocity.Y = 0f;
			acceleration = 3f;
		}

		// ============================================= WIP FUNCTIONS (they are usable but they will change) =============================================
		// Useful for getting the next position without setting the state.
		public void get_Input()
		{
			// Horiziontal movement
			if (controls.isDown(controls.Right)) 
			{
				direction = 1;
				move();
			}
			else if (controls.isDown(controls.Left) )
			{
				direction = 0;
				move();
			}

			// Jump
			if (controls.isDown(controls.Jump) && numJumps > 0) jump();

			// Dash
			if (controls.isDown(controls.Dash)) dash();
			
			// No inputs
			if (controls.isAllUp()) 
			{
				if (acceleration > accelMin) acceleration -= 0.1f;
            	else acceleration = accelMin;
			}

			// if (Keyboard.GetState().IsKeyDown(Keys.Q)) hurtboxVisual = true;
			// else hurtboxVisual = false;

			fall();
			setPositions();
		}

		// Sets the current and previous position AFTER player input but BEFORE collision handling.
		public void setPositions()
		{
			currPosition = position;
            nextPosition.X = position.X + (velocity.X * acceleration);
            nextPosition.Y = position.Y + (velocity.Y * gravity);

		}

		// ============================================= STATE FUNCTIONS =============================================
        // ---------------------------------------- Idle States
		public void idle_state()
		{
			
			if (!grounded) 
			{
				set_state(AIR);
				air_state();
				return;
			}

			if (controls.isDown(controls.Left) ) // Left & Right inputs 
			{
				direction = 0;
				set_state(RUN);
				run_state();
			}
			else if ( controls.isDown(controls.Right) )
			{
				direction = 1;
				set_state(RUN);
				run_state();
			}

			// else if (controls.isDown(controls.Crouch)) // Crouch Input
			// {
			// 	set_state(CROUCH);
			// 	crouch_state();
			// }
			else if (controls.isDown(controls.Jump) && numJumps > 0 && !jumping) // Jump Input
			{
				
				STATELOCK = 10;
				set_state(JUMP);
				jump_state();
				
			}
			else if (controls.isDown(controls.Dash)) // Dash Input
			{
				STATELOCK = 6;
				set_state(DASH);
				dash_state();
			}
			else if (controls.isDown(controls.Attack)) // Attack Input
			{
				set_state(ATTACK);
				attack_state();
			}
			else 
			{
				if (acceleration > accelMin) acceleration -= 0.5f;
				else
				{
					velocity.X = 0;
					acceleration = accelMin;
				}
				velocity.Y = 0;
			}
		}

		/* 
		General state while airborne with no inputs
		*/
		public void air_state()
		{
			if (grounded) 
			{
				set_state(IDLE);
				idle_state();
				return;
			}
			if (controls.isDown(controls.Dash))
			{
				STATELOCK = 6;
				set_state(DASH);
				dash_state();
				return;
			}
			if (jumping)
			{
				STATELOCK = animDictionary["Base_Jump"].totalFrames - 1;
				set_state(JUMP);
				jump_state();
				return;
			}

            if (controls.isDown(controls.Left)) // Horizontal Movement
			{
				direction = 0;
			}
			else if (controls.isDown(controls.Right))
			{
				direction = 1;
			}
        }

		// -------------------- Jump States
		/*
		State during a jump
		*/
		public void jump_state()
		{
			if (STATELOCK == stateFrame)
			{
				jumping = false;
				set_state(AIR);
				air_state();
				return;
			}
			if (controls.isDown(controls.Dash))
			{

			}
            else if (controls.isDown(controls.Left)) // Horizontal Movement
            {
                direction = 0;
            }
            else if (controls.isDown(controls.Right))
            {
                direction = 1;
            }
        }

        /*
		State when starting a jump 
		*/
		public void jumpsquat_state()
		{
           
        }   

		/* 
		State when landing
		*/
        public void land_state()
		{

		}

		// -------------------- Run States
		/* 
		State during a run
		*/
		public void run_state()
		{
    
			if (!grounded) 
			{
				set_state(AIR); 
				air_state(); 
				return;
			}

			if (controls.isDown(controls.Crouch)) // Crouch Input
			{
				set_state(CROUCH);
				crouch_state();
			}
			else if (controls.isDown(controls.Jump)) // Jump Input
			{
				if (jumping)
				{
					Console.WriteLine("Executing Jump...");
					STATELOCK = animDictionary["Base_Jump"].totalFrames - 1;
					set_state(JUMP);
					jump_state();
				}
			}
			else if (controls.isDown(controls.Dash)) // Dash Input
			{
				set_state(DASH);
				dash_state();
			}

			else if (direction == 1) // Travelling Right
			{
				if (controls.isDown(controls.Left) && controls.isDown(controls.Right))
				{
					set_state(TURNAROUND);
					turnaround_state();
				}
				else if (controls.isDown(controls.Right))
				{
				}
				else // No Inputs
				{
					set_state(IDLE);
					idle_state();
					// For the runEnd_state when its fixed
					// set_state(RUN_END);
					// STATELOCK = animDictionary["Base_RunStop"].totalFrames - 1;
					// runEnd_state();
				}
			}
			else if (direction == 0) // Travelling Left
			{
				if (controls.isDown(controls.Left) && controls.isDown(controls.Right))
				{
					set_state(TURNAROUND);
					turnaround_state();
				}
				else if (controls.isDown(controls.Left))
				{

				}
				else // No inputs
				{
					set_state(IDLE);
					idle_state();
					// For the runEnd_state when its fixed
					// set_state(RUN_END);
					// STATELOCK = animDictionary["Base_RunStop"].totalFrames - 1;
					// runEnd_state();
				}
			}
        }

		/*
		State when starting a run
		*/
		public void runStart_state()
		{

		}


		/* 
		State when a run ends
		*/
		public void runEnd_state()
		{
			
        }


		/*
		State when changing directions during a run
		*/
		public void turnaround_state()
		{
			
        }

		// -------------------- Crouch States

		/* 
		State during a crouch
		*/
		public void crouch_state()
		{
			
		}


		/* 
		State when starting a crouch
		*/
		public void crouchStart_state()
		{

		}

		/* 
		State when ending a crouch
		*/
		public void crouchEnd_state()
		{

		}


		// -------------------- Other

		/* 
		State during a dash
		*/
		public void dash_state()
		{
			if (stateFrame == STATELOCK)
			{
				velocity.Y = dashVelocity.Y;
				acceleration = dashAcceleration;

				set_state(previousState);
				STATELOCK = 0;
			}
			else {
				if (stateFrame <= 1) // storing the physics values prior to dash
				{
					dashPosition = position; // might not need
					
					dashVelocity = velocity;
					dashAcceleration = acceleration;
				}
			}
		}

		// -------------------- Attack States

		/*
		State during an attack
		*/
		public void attack_state()
		{

		}

		/*
		State during the startup of an attack
		*/
		public void attackStart_state()
		{

		}


		/*
		State during the endlag of an attack
		*/
		public void attackEnd_State()
		{

		}

		// -------------------- Skill States
		
		/*
		State during a sklll
		*/
		public void skill_state()
		{

		}

		// =========================================== GENERAL FUNCTIONS ============================================
		/*
		Finds the current state of the player based on the users input. 
		*/
		public void update_state()
        {
			switch (currentState)
			{
				case IDLE:
					idle_state();
					break;
				case AIR:
					air_state();
					break;
				case JUMP:
					jump_state();
					break;
				case JUMPSQUAT:
					jumpsquat_state();
					break;
				case LAND:
					land_state();
					break;
				case RUN:
					run_state();
					break;
				case RUN_START:
					runStart_state();
					break;
				case RUN_END:
					runEnd_state();
					break;
				case TURNAROUND:
					turnaround_state();
					break;
				case CROUCH:
					crouch_state();
					break;
				case CROUCH_START:
					crouchStart_state();
					break;
				case CROUCH_END:
					crouchEnd_state();
					break;
				case DASH:
					dash_state();
					break;
				case ATTACK:
					attack_state();
					break;
				case ATTACK_START:
					attackStart_state();
					break;
				case ATTACK_END:
					attackEnd_State();
					break;
				default:
					break;
			}
        }


		/* 
		Changes the state of the player and records the previous state
		*/
		
        public void set_state(string newState)
        {
			if (currentState != newState)
			{
				previousState = currentState;
				stateFrame = 0;
			}
            currentState = newState;
        }

		// =========================================== UPDATE ============================================
		/*
		Modifies the current animation of the player based on the current state
		*/
		public void update_animations()
		{
            switch (currentState)
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
                case JUMPSQUAT:
                    animManager.Play(animDictionary["ball"]);
                    break;
                case LAND:
                    animManager.Play(animDictionary["ball"]);
                    break;
                case RUN:
                    animManager.Play(animDictionary["Base_Running"]);
                    break;
                case RUN_START:
                    animManager.Play(animDictionary["ball"]);
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
                case CROUCH_START:
                    animManager.Play(animDictionary["ball"]);
                    break;
                case CROUCH_END:
                    animManager.Play(animDictionary["ball"]);
                    break;
				case DASH:
					animManager.Play(animDictionary["ball"]);
					break;
                case ATTACK:
                    animManager.Play(animDictionary["ball"]);
                    break;
                case ATTACK_START:
                    animManager.Play(animDictionary["ball"]);
                    break;
                case ATTACK_END:
                    animManager.Play(animDictionary["ball"]);
                    break;
                default:
                    break;
            }
        }

		/*
		Modifies the postion, velocity and acceleration of the player based on the current state
		*/
        private void update_physics()
        {
			// Flipping
            if (direction == 0) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

			if (grounded) 
			{
				numJumps = 1;
				gravity = 0;
				velocity.Y = 0;
			}

			position.Y += velocity.Y ;
            position.X += velocity.X * acceleration;
			
			if (position.Y >= 1000f) 
			{
				position.Y = 0f;
				position.X = 400f; 
				velocity.Y = 0f; 
				acceleration = 0f;
				gravity = 0f;
			}

			hurtbox.update_Position(position);		
        }


		/* 
		 * Keeps track of the elasped time of each state in 24FPS
		 * 
		 */
        private void update_time(GameTime gameTime)
        {
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stateTimer >= FPS24)
            {
                stateTimer = FPS24 - stateTimer;
                stateFrame++;
            }
			if (stateFrame >= 240) stateFrame = 0;
        }

		public virtual void Update(GameTime gameTime)
        {
			update_physics(); // set the final position, velocity, and acceleration of the player
			update_state(); // set the current and previous states
			update_time(gameTime); // update the frame count for the state
			update_animations(); // set the corresponding animation
			animManager.Update(gameTime); // update the frame count for the animation
			
			if (hurtboxVisual) hurtbox.Update(gameTime);
        }

		// =========================================== UPDATE ============================================
		public void DrawPlayer(GameTime gameTime, SpriteBatch spritebatch)
		{
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); // Draw player
			
			if (hurtboxVisual) hurtbox.Draw(gameTime, spritebatch, position, flip); // Draw Hurtbox visualization
		}
        

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Draw Player
			DrawPlayer(gameTime, spriteBatch);

			// Frame dispaly
			stateMSG = "Current State: " + currentState + "\nPrevious State: " + previousState
				+ "\nTime (24FPS): " + stateFrame + " Frames" + "\nGrounded: " + grounded.ToString();
			movementMSG = "Position: " + position.ToString() + "\nVelocity: " + velocity.ToString()
                + "\nAcceleration: " + acceleration.ToString() + "\nDirection: " + direction.ToString();

            spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(550f, 400), Color.DarkRed);
            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(5f, 415), Color.Gold);
			
        }
	}
}

