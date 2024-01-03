using System.Collections;
using Microsoft.Xna.Framework;

using Alakoz.GameInfo;
using Alakoz.Collision;

using MonoGame.Extended;

namespace Alakoz.GameObjects
{
	public abstract class PlayerAttacks
	{
		// ----------------------------- HITBOXES
		public static void attackConnected(Player P)
		{
			P.velocity.Y = 0;
			// applyAttackBounce = false;
		}
		// ------------------------------ Attacks
		//  ********** Ground
		public static void bAttack1(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 1
            // First hit of the attack 
            // Check if the hitbox is active and activate it. 
            if (isActive)
            {
            	float tempWidth = 30f;
            	float tempHeight = 20f;
            	float tempOffsetX = 0; // Offset from the origin on the X axis
            	float tempOffsetY = -22; // Offset from the origin on the Y axis

            	if (P.direction == -1) tempOffsetX =  (tempOffsetX + tempWidth) * -1; 

            	// Choose a hitbox to use from the hitbox list and set the parameters for it
            	P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 1 * P.direction, 1, 5, 15);
            	P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;

            // For single attacks that have 2 parts, (ie hitbox frame 2, and hitbox on frame 5)
            // use the attackNum to parse which part should be activated

            // NOTE: For some better performance. The hitboxes (as well as hurtboxes) are always in the active collisions list
            // Since there is a fixed, maximum number of hitboxes on any given frame, its faster to loop a couple more times
            // rather than modify the activeCollisions list every single time a hitbox is no longer active. 

        }

        public static void bAttack2(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 2
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 25f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 2 * P.direction, -4, 10, 20, Hitbox.LIGHT);
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[1].active = false;
        }

        public static void bAttack3(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 3
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 35f;
                float tempOffsetX = 10; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * P.direction;
				else tempOffsetX = tempOffsetX  * P.direction;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 4.5f * P.direction, 5, 15, 20, Hitbox.MEDIUM);
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
		public static void bAttack4(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 3
            if (isActive)
            {
                float tempWidth = 70f;
                float tempHeight = 50f;
                float tempOffsetX = -20; // Offset from the origin on the X axis
                float tempOffsetY = -40; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * P.direction;
				else tempOffsetX = tempOffsetX  * P.direction;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
				if (!P.grounded)
				{
                	P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, P.velocity.X, -2, 5, 20, Hitbox.LIGHT);
				}
				else 
				{
					P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, P.velocity.X, 0, 5, 20, Hitbox.LIGHT);
				}
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
		
		// ********** Air
		public static void airAttack1(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 1
            // First hit of the attack 
            // Check if the hitbox is active and activate it. 
            if (isActive)
            {
            	float tempWidth = 30f;
            	float tempHeight = 20f;
            	float tempOffsetX = 0; // Offset from the origin on the X axis
            	float tempOffsetY = -22; // Offset from the origin on the Y axis

            	if (P.direction == -1) tempOffsetX =  (tempOffsetX + tempWidth) * -1; 

            	// Choose a hitbox to use from the hitbox list and set the parameters for it
            	P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 1 * P.direction, -3, 5, 15);
            	P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;

            // For single attacks that have 2 parts, (ie hitbox frame 2, and hitbox on frame 5)
            // use the attackNum to parse which part should be activated

            // NOTE: For some better performance. The hitboxes (as well as hurtboxes) are always in the active collisions list
            // Since there is a fixed, maximum number of hitboxes on any given frame, its faster to loop a couple more times
            // rather than modify the activeCollisions list every single time a hitbox is no longer active. 

        }

        public static void airAttack2(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 2
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 25f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 2 * P.direction, -2, 10, 20);
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }

        public static void airAttack3(Player P, bool isActive)
        {
            // -------------------------------------- ATTACK 3
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 35f;
                float tempOffsetX = 10; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * P.direction;
				else tempOffsetX = tempOffsetX  * P.direction;

				// ---------- Two types of hitboxes depending on the attack counter
				// Hitbox A
				if (P.attackCounter < 4)
				{
					P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 4f * P.direction, -3, 15, 20, Hitbox.MEDIUM);
				}
				else
				{
					P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 4.5f * P.direction, 7, 15, 20, Hitbox.MEDIUM);
				}
                // Choose a hitbox to use from the hitbox list and set the parameters for it
                
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
        
		// ********** Finishers
		public static void upFinisher(Player P, bool isActive)
		{
			// -------------------------------------- ATTACK 2
            if (isActive)
            {
                float tempWidth = 50f;
                float tempHeight = 25f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, -2 * P.direction, -7.5f, 10, 20, Hitbox.MEDIUM);
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[1].active = false;
		}
		public static void downFinisher(Player P, bool isActive)
		{

		}
		public static void frontFinisher(Player P, bool isActive)
		{
			// -------------------------------------- ATTACK 3
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 35f;
                float tempOffsetX = 10; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * P.direction;
				else tempOffsetX = tempOffsetX  * P.direction;

				// ---------- Two types of hitboxes depending on the attack counter
				// Hitbox A
				if (P.attackCounter < 4)
				{
					P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 4f * P.direction, -3, 15, 20, Hitbox.MEDIUM);
				}
				else
				{
					P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 8f * P.direction, 5, 15, 20, Hitbox.HEAVY);
				}
                // Choose a hitbox to use from the hitbox list and set the parameters for it
                
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
		}
		public static void backFinisher(Player P, bool isActive)
		{
			
		}
		// ----------------------------- BASIC ATTACK STATES
		// ********** Ground
        public static void bAttack1_State(Player P)
        {
            // -------------------------------  ATTACK 1
            if (P.stateFrame <  4) // Startup = 5
            {	
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 1f * P.direction;
                bAttack1(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  6) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  6)
            {
                bAttack1(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame < 19)
            {
				// ENDLAG
				if (P.attacking) 
				{
					P.attackCounter++;
					P.set_state(StateType.BASICATTACK2, new ArrayList(){ StateType.BASICATTACK2 }, StateType.BASICATTACK2);
				}
				else if (P.jumping)
				{
					P.attackCounter = 0; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
				}
				else if (P.dashing) 
				{
					P.attackCounter = 0; // Reset the counter
					P.set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset the counter

				if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);	
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }

        public static void bAttack2_State(Player P)
        {
            // ------------------------------- ATTACK 2
            if (P.stateFrame <  4) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 1.5f * P.direction;
                bAttack2(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  5) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  5)
            {
                bAttack2(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame <  20)
            {
                if (P.attacking) 
				{
					P.attackCounter++;
					P.set_state(StateType.BASICATTACK3, new ArrayList(){StateType.BASICATTACK3}, StateType.BASICATTACK3);
				}
				else if (P.jumping)
				{
					P.attackCounter = 0; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
				}
				else if (P.dashing) 
				{
					P.attackCounter = 0; // Reset the counter
					P.set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }

        public static void bAttack3_State(Player P)
        {
            // ------------------------------- ATTACK 3
            if (P.stateFrame < 5) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  5)
            {
				P.velocity.X = 4f * P.direction;
                bAttack3(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  7) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  7)
            {
                bAttack3(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame < 22)
            {
				
				// Up Finisher
				if (P.move_up) 
				{
					P.attackCounter++;
					P.set_state(StateType.UPFINISHER, new ArrayList(){StateType.BASICATTACK2}, StateType.BASICATTACK2);
				}
				// Down Finisher
				else if (P.attacking && P.crouching)
				{
					
				}
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1))
				{
					P.attackCounter++;
					P.set_state(StateType.FRONTFINISHER, new ArrayList(){StateType.BASICATTACK1}, StateType.BASICATTACK1);
				}
				// Back Finisher
				else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
				{

				}
				else if (P.attacking)
				{
					P.attackCounter++;
					P.set_state(StateType.BASICATTACK4, new ArrayList(){StateType.BASICATTACK4}, StateType.BASICATTACK4);
				}
				
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }
		
		public static void bAttack4_State(Player P)
        {
			P.velocity.Y = 0;
            // ------------------------------- ATTACK 3
            if (P.stateFrame < 4) // Startup = 5
            {
				P.velocity.X = -1f * P.direction;
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  5)
            {
				P.velocity.X = 6f * P.direction;
                bAttack4(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame ==  7) bAttack4(P, false); 
			// Hit 2
			else if (P.stateFrame == 9) bAttack4(P, true);
			else if (P.stateFrame == 11) bAttack4(P, false);
			// Hit 3
			else if (P.stateFrame == 13) bAttack4(P, true);
			else if (P.stateFrame == 15) bAttack4(P, false);
			// Endlag / Startup Gaps (Frames 1-3, 8, 10, 12, 14, 16 - 21)
			else if (P.stateFrame < 21){}
            else if (P.stateFrame < 22)
            {
				// Up Finisher
                if (P.move_up) 
				{
					P.attackCounter++;
					P.set_state(StateType.UPFINISHER, new ArrayList(){StateType.BASICATTACK2}, StateType.BASICATTACK2);
				}
				// Down Finisher
				else if (P.attacking && P.crouching)
				{
					
				}
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1))
				{
					P.attackCounter++;
					P.set_state(StateType.FRONTFINISHER, new ArrayList(){StateType.BASICATTACK1}, StateType.BASICATTACK1);
				}
				// Back Finisher
				else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
				{

				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { StateType.TOGROUND}, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.TOAIR}, StateType.FALL);
            }
        }
		// ********** Air
		public static void airAttack1_State(Player P)
        {
			// P.move_left = false;
            // P.move_right = false;
            P.jumping = false;
            P.crouching = false;
			P.velocity.Y = 2f;
			if (P.applyAttackBounce) attackConnected(P);
			

            // -------------------------------  ATTACK 1
            if (P.stateFrame <  4) // Startup = 5
            {	
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 1f * P.direction;
                airAttack1(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  6) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  6)
            {
                airAttack1(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame <  19)
            {
                // ENDLAG
				if (P.attacking) 
				{
					P.attackCounter++;
					P.set_state(StateType.AIRATTACK2, new ArrayList(){ StateType.AIRATTACK2 }, StateType.AIRATTACK2);
				}
				else if (P.jumping)
				{
					P.attackCounter = 0; // Reset the counter
					P.applyAttackBounce = false;

					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
				}
				else if (P.dashing) 
				{
					P.attackCounter = 0; // Reset the counter
					P.applyAttackBounce = false;

					P.set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
				}
            }
            else
            {
				P.applyAttackBounce = false;
				P.attackCounter = 0; // Reset the counter

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }

        public static void airAttack2_State(Player P)
        {
			P.move_left = false;
            P.move_right = false;
            P.jumping = false;
            P.crouching = false;
			if (P.applyAttackBounce) attackConnected(P);
			else P.velocity.Y = 2f;

            // ------------------------------- ATTACK 2
            if (P.stateFrame <  4) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 1.5f * P.direction;
                airAttack2(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  5) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  5)
            {
                airAttack2(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame < 20)
            {
                if (P.attacking) 
				{
					P.attackCounter++;
					P.set_state(StateType.AIRATTACK3, new ArrayList(){StateType.AIRATTACK3}, StateType.AIRATTACK3);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start
				P.applyAttackBounce = false;

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }

        public static void airAttack3_State(Player P)
        {
			P.move_left = false;
            P.move_right = false;
            P.jumping = false;
            P.crouching = false;
            P.dashing = false;
			if (P.applyAttackBounce) attackConnected(P);
			else P.velocity.Y = 2f;

            // ------------------------------- ATTACK 3
            if (P.stateFrame <  5) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  5)
            {
				P.velocity.X = 4f * P.direction;
                airAttack3(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  7) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  7)
            {
                airAttack3(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame < 22)
            {
				if (P.attacking)
				{
					P.attackCounter++;
					P.set_state(StateType.BASICATTACK4, new ArrayList(){StateType.BASICATTACK4}, StateType.BASICATTACK4);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start
				P.applyAttackBounce = false;

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
        }
		
		// *********** Finishers
		public static void upFinisher_State(Player P)
		{
			 // ------------------------------- ATTACK 2
            if (P.stateFrame <  4) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 1.5f * P.direction;
                upFinisher(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  5) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  5)
            {
                upFinisher(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame <  20)
            {
				if (P.jumping && !P.attacking)
				{
					P.attackCounter = 0; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
				}
				if (P.dashing) 
				{
					P.attackCounter = 0; // Reset the counter
					P.set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
		}
		public static void downFinisher_State(Player P)
		{

		}
		public static void frontFinisher_State(Player P)
		{
            P.jumping = false;
            P.crouching = false;
            P.dashing = false;
            // ------------------------------- ATTACK 3
            if (P.stateFrame <  4) // Startup = 5
            {
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            else if (P.stateFrame ==  4)
            {
				P.velocity.X = 4f * P.direction;
                frontFinisher(P, true); // Activate the corresponding hitboxes
            }
            else if (P.stateFrame <  7) // Endlag = 6
            {
                // ACTIVE
            }
            else if (P.stateFrame ==  7)
            {
                frontFinisher(P, false); // Deactivate the corresponding hitboxes
            }
            else if (P.stateFrame < 22)
            {
				if (P.jumping)
				{
					P.attackCounter = 0; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(StateType.JUMP, new ArrayList(){StateType.JUMPSTART}, StateType.JUMP);
				}
				else if (P.dashing) 
				{
					P.attackCounter = 0; // Reset the counter
					P.set_state(StateType.DASH, new ArrayList(){StateType.DASHSTART}, StateType.DASH);
				}
            }
            else
            {
				P.attackCounter = 0; // Reset so that the first attack will start

                if (P.grounded) P.set_state(StateType.IDLE, new ArrayList() { }, StateType.IDLE);
                else P.set_state(StateType.AIR, new ArrayList() { StateType.BALLEND, StateType.BALL, StateType.BALLSTART }, StateType.FALL);
            }
		}
		public static void backFinisher_State(Player P)
		{

		}
		
	}
    
}