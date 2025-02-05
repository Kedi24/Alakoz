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
using MonoGame.Extended;

namespace Alakoz.GameObjects
{
	public class Enemy : GameObject
	{
        #region // ----- Physics Variables ----- //
        public Vector2 startPosition;
		public float gravity = 0.275f;
		public float speed = 1f; // So that they move slower than the player
		public float fallSpeed = 1f;

		public float velocityMax = 3f;
		public float terminalVelocity = 8f;
		public float acceleration = 0.175f; // Default acceleration
		public float deceleration = 0.1f; // Default deceleration
		
		// ------ Moving
		public bool move_left = false;
		public bool move_right = false;
		public bool grounded = false;
		public bool switchero = false;

		// ---- Attacking
		public bool attacking = false;
		public int attackCounter = 0;

		// ---- Hit 
		public bool hit = false;
		public int hitstun = 0;
		public int health = 100;
		#endregion
		
		#region // ----- Collision Variables ------ //
		public EnemyHurtbox hurtbox {get; set;}
		Vector2 spriteCoordinate = new Vector2(-39, -36); // Placement of sprite in relation to the hurtbox. Calculated with aesprite
		public Hitbox[] allHitboxes = new Hitbox[5]; // Array to store a preset number of hitboxes 
		#endregion
		
		#region // ----- Effect Variables ----- //
		SpriteEffects flip;
		#endregion
		
		#region // ----- Debug Variables ----- //
		public string stateMSG = "";
		public string movementMSG = "";
		float tempSteps = 1;
		#endregion

		public Enemy(Dictionary<TState, Animation> newSprites ,Vector2 newPosition, int ID = -1)
		{
			id = ID;
			type = TObject.ENEMY;
			currentState = TState.AIR;
			previousState = TState.IDLE;
			stateFrame = 0;

			hurtboxWidth = 36;
			hurtboxHeight = 44;

            position = newPosition;
            startPosition = newPosition;
			originOffset = new Vector2(hurtboxWidth/2, hurtboxHeight/2);
			origin = position + originOffset;
			// distance = 0f;
            // distanceTraveled = 100f;

            animations = newSprites;
            animManager = new AnimationManager(newSprites[TState.FALL]);
			currAnimation = TState.FALL;

			hurtbox = new EnemyHurtbox(this, position, hurtboxWidth, hurtboxHeight);
			// Allocating space for the hitboxes
			for (int i = 0; i < 5; i++) 
			{
				allHitboxes[i] = new Hitbox(Vector2.Zero, 0f, 0f, 0, Vector2.Zero, 0, 0);
				allHitboxes[i].owner = this;
				allHitboxes[i].addIgnore(hurtbox); // To prevent dealing damage to yourself
				allHitboxes[i].active = false;
				activeCollisions.Add(allHitboxes[i]);
			}
			allHitboxes[0].setParameters(
				newX: origin.X - 10, 
				newY: origin.Y - 10, 
				newWidth: hurtboxWidth-10, 
				newHeight: hurtboxHeight-10, 
				newActiveFrames: 0, 
				newKBX: direction*15, 
				newKBY: -4, 
				newDamage: 10, 
				newHitstun: 15,
				newHitstop: THitstop.LIGHT
			); 
			allHitboxes[0].active = true;
			activeCollisions.Add(hurtbox);

            flip = SpriteEffects.None;
        }
		
		#region // ========== PHYSICS ========== //

		// Physics functions for each possible movement. These function simply set the velocity values depending
		// on player input and state
        private void move(){
			velocity.X = approach(velocity.X, direction*velocityMax, speed * acceleration);
        }
		private void fall(){
			if (velocity.Y < terminalVelocity) velocity.Y = approach(velocity.Y, terminalVelocity, gravity);
			else velocity.Y = terminalVelocity;
		}
		private void decelerate() {
			velocity.X = approach(velocity.X, 0, speed * deceleration);
		}
		private void knockback()
		{
            velocity.X = KB.X;
			velocity.Y = KB.Y;
			applyKB = false;
		} 
		#endregion
		
		#region // ========== STATE ========== //
		public void air_state(){
			if (grounded) set_state(TState.IDLE, TState.IDLE);
			else velocity.X = 0;
		}

		public void idle_state(){
			if (!grounded) set_state(TState.AIR, TState.FALL);
			else {
				if (stateFrame % 200 == 0) {
					set_hitboxes(true); // Reactivate hitboxes
					set_state(TState.RUN, TState.RUN);
				};
			}
		}
		
		public void run_state()
		{
            if (!grounded) set_state(TState.AIR, TState.FALL);
			else{
				if (stateFrame % 150 == 0) {
					direction *= -1;
					stateFrame = 1;
				}
				move();
			}
		}
		
		public void hit_state()
		{
			if (stateFrame == hitstun) {
				hit = false;
				hitstun = 0;
				set_state(TState.AIR, TState.FALL);
				return;
			}
			else{
				move_left = false;
				move_right = false;
				nextPosition = position; // To prevent map based collisions that depend on the next position from being invoked (grounded, etc)

				if (applyKB){
					stateFrame = 1; // So that the frames dont accumulate when hit multiple times during hitstun
					set_hitboxes(false); // Deactivate hitboxes
					// To make sure the impact frames play when hit again DURING hitstun
					if (currAnimation == TState.HIT) set_animations(TState.HITSTART, TState.HIT);
					knockback();
				}
			}
		}
		#endregion
		
		#region // ========== SETTERS ========== //
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
            find_state(newState); // Call the corresponding state function
        }
		
		public void set_hitboxes(bool active = false){
			foreach (Hitbox hitbox in allHitboxes)	hitbox.active=active; // Deactive all hitboxes
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
                case TState.RUN:
                    run_state();
                    break;
				case TState.HIT:
					hit_state();
					break;
				default:
					air_state();
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
			if (direction == -1){	
				move_left = true; 
				move_right = false;
			} 
			else {
				move_left = false; 
				move_right = true;
			}
        }

        public override void update_state()
        {   
			prevPosition = position;	
			if (hit) set_state(TState.HIT, TState.HITSTART, TState.HIT);
			if (hitStop > 0 ) return;

			fall();
			decelerate();
			find_state(currentState);

			grounded = false;
			set_positions(); // Update the "position to be drawn" of the player
        }

        public override void update_physics()
        {
			// Dont apply physics during hitstop
			if (hitStop > 0) return;

			// Flipping
            if (direction == -1) flip = SpriteEffects.FlipHorizontally;
            else if (direction == 1) flip = SpriteEffects.None;

            position += velocity;
			
			// To make the hitbox follow the Enemy
			origin = position + originOffset;
			allHitboxes[0].setParameters(
				newX: origin.X - 9, 
				newY: origin.Y - 11, 
				newWidth: hurtboxWidth/2, 
				newHeight: hurtboxHeight/2, 
				newActiveFrames: 0, 
				newKBX: direction*velocity.X, 
				newKBY: -7, 
				newDamage: 10, 
				newHitstun: 15,
				newHitstop: THitstop.LIGHT
			); 
			
			hurtbox.update_Position(position);
			hurtbox.velocitySteps = 1; // reset steps
            hurtbox.currentStep = 1;

            if (position.Y >= 2000f || health <= 0) {
				position = startPosition;
				velocity.Y = 0f; 
				health = 100;
			}
        }

		public void update_cooldowns()
		{
			// -------- Health
			if (health < 0) health = 0;
		}
		#endregion
		
		#region // ========== DRAWING ========== //
		public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
		{
			// Pause current frame during hitstop
			if (hitStop > 0) animManager.Pause();
			else animManager.Resume();

			// Draw enemy animation
			animManager.Draw(gameTime, spritebatch, position + spriteCoordinate, Vector2.One, flip); 
			
			// Draw debug information
			// drawDebug(spritebatch);
			// drawCollision(spritebatch);
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
        public void drawDebug(SpriteBatch spriteBatch)
		{
			// Messages to display game / player information
			stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
			+ "\nState Time (24FPS): " + stateFrame
			+ "\nAnimation: " + currAnimation
			+ "\nHealth: " + health
			+ "\nHitstun: " + hitstun;
			
			movementMSG = "Position: " + position.ToString() 
			+ "\nHurtbox: " + hurtbox.position.ToString()
			+ "\nVelocity: " + velocity.ToString()
            + "\nAcceleration: " + acceleration.ToString() 
			+ "\nDirection: " + direction.ToString()
			+ "\nGravity: " + gravity.ToString();
			
			string inputMSG = 
			"Left = " + move_left
			+ "\nRight = " + move_right
			+ "\nGrounded = " + grounded
			+ "\nAttacking = " + attacking
			+ "\nHit = " + hit;

            spriteBatch.DrawString(stateFONT, movementMSG, new Vector2(hurtboxWidth+ 20, -100) + position, Color.GreenYellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(-39, -120) + position, Color.Gold, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(-39, hurtboxHeight) + position, Color.Blue, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
		#endregion
		
	}
}

