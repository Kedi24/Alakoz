using System.Collections;
using Microsoft.Xna.Framework;

using Alakoz.GameInfo;
using Alakoz.Collision;

using MonoGame.Extended;

namespace Alakoz.GameObjects
{
	public abstract class PlayerAttacks
	{
        #region // ========== Helper ========== //
        public static void setFinisher(Player P, int frame)
        {
            if (P.move_up) 
            {
                P.attackCounter++;
                P.set_state(TState.UPFINISHER, TState.UPFINISHER, TState.UPFINISHER);
            }
            // Down Finisher
            else if (P.crouching)
            {
                P.attackCounter++;
                // P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHERSTART}, TState.DOWNFINISHERLOOP);
                P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER);
            }
            // Back Finisher
            else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
            {

            }
            // Front Finisher
            else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1) || P.attacking)
            {
                P.attackCounter++;
                P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
            }
        }
		public static void attackConnected(Player P)
		{
			P.velocity.Y = 0;
			// P.applyAttackBounce = false;
		}
        #endregion
        
        #region ========== Hitboxes ========== //
		
        #region // ----- Ground
		public static void bAttack1(Player P, bool isActive){
            // -------------------------------------- ATTACK 1
            // First hit of the attack 
            // Check if the hitbox is active and activate it. 
            if (isActive)
            {
            	float tempWidth = 30f;
            	float tempHeight = 20f;
            	float tempOriginX = P.origin.X - (tempWidth/2); // Offset from the origin on the X axis
            	float tempOffsetY = -22; // Offset from the origin on the Y axis

            	// Choose a hitbox to use from the hitbox list and set the parameters for it
            	P.allHitboxes[0].setParameters(
                    (15*P.direction) + tempOriginX, 
                    P.origin.Y + tempOffsetY, 
                    tempWidth, 
                    tempHeight, 
                    newActiveFrames:0, 
                    newKBX:1 * P.direction, 
                    newKBY: P.grounded? 0 : -2f, 
                    newDamage:5, 
                    newHitstun:30
                );
            	P.allHitboxes[0].active = true;

                // P.allHitboxes[1].setParameters(45*(P.direction) + tempOriginX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 1 * P.direction, 1, 25, 15);
            	// P.allHitboxes[1].active = true;

                // P.allHitboxes[2].setParameters(75*(P.direction) + tempOriginX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 1 * P.direction, 1, 5, 15);
            	// P.allHitboxes[2].active = true;

                // P.allHitboxes[0].addLink(P.allHitboxes[1]);
                // P.allHitboxes[0].addLink(P.allHitboxes[2]);
                // P.allHitboxes[1].addLink(P.allHitboxes[0]);

            }
            else
            { 
                // P.allHitboxes[0].removeLink();
                // P.allHitboxes[1].removeLink();
                // P.allHitboxes[2].removeLink();
                P.allHitboxes[0].active = false;
                // P.allHitboxes[1].active = false;
                // P.allHitboxes[2].active = false;
            }

            // For single attacks that have 2 parts, (ie hitbox frame 2, and hitbox on frame 5)
            // use the attackNum to parse which part should be activated

            // NOTE: For some better performance. The hitboxes (as well as hurtboxes) are always in the active collisions list
            // Since there is a fixed, maximum number of hitboxes on any given frame , its faster to loop a couple more times
            // rather than modify the activeCollisions list every single time a hitbox is no longer active. 

        }

        public static void bAttack2(Player P, bool isActive){
            // -------------------------------------- ATTACK 2
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 25f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(
                    P.origin.X + tempOffsetX, 
                    P.origin.Y + tempOffsetY, 
                    newWidth: tempWidth, 
                    newHeight: tempHeight, 
                    newActiveFrames: 0, 
                    newKBX: 2 * P.direction, 
                    newKBY: P.grounded? -4f : -2f, 
                    newDamage: 10, 
                    newHitstun: 40, 
                    newHitstop: THitstop.LIGHT
                );
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }

        public static void bAttack3(Player P, bool isActive){
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
                
                P.allHitboxes[0].setParameters(
                    P.origin.X + tempOffsetX, 
                    P.origin.Y + tempOffsetY, 
                    newWidth: tempWidth, 
                    newHeight: tempHeight, 
                    newActiveFrames: 0, 
                    newKBX: 2.5f * P.direction, 
                    newKBY:  P.grounded? 3 : -1f, 
                    newDamage: 15, 
                    newHitstun: 40, 
                    newHitstop: THitstop.MEDIUM
                );

                if (!P.grounded) P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 2f * P.direction, -2, 15, 20, THitstop.LIGHT2);

                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
		
        public static void bAttack4(Player P, bool isActive, int part = 1){
            // -------------------------------------- ATTACK 3
            if (isActive)
            {
                float tempWidth = 60f;
                float tempHeight = 50f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -40; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * P.direction;
				else tempOffsetX = tempOffsetX  * P.direction;

                // Choose a hitbox to use from the hitbox list and set the parameters for it

                P.allHitboxes[0].setParameters(
                    P.origin.X + tempOffsetX, 
                    P.origin.Y + tempOffsetY, 
                    newWidth: tempWidth, 
                    newHeight: tempHeight, 
                    newActiveFrames: 0, 
                    newKBX: P.velocity.X, 
                    newKBY:  P.grounded? 0 : (part == 1? -1f: -1f), 
                    // newKBY:  P.velocity.Y, 
                    newDamage: 5, 
                    newHitstun: 80, 
                    newHitstop: THitstop.LIGHT2
                    
                );
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
		#endregion

		#region // ----- Air
		public static void airAttack1(Player P, bool isActive){
            // -------------------------------------- ATTACK 1
            if (isActive)
            {
            	float tempWidth = 30f;
            	float tempHeight = 20f;
            	float tempOffsetX = 0; // Offset from the origin on the X axis
            	float tempOffsetY = -22; // Offset from the origin on the Y axis

            	if (P.direction == -1) tempOffsetX =  (tempOffsetX + tempWidth) * -1; 
            	P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 1 * P.direction, -3, 5, 15, THitstop.LIGHT);
            	P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }

        public static void airAttack2(Player P, bool isActive){
            // -------------------------------------- ATTACK 2
            if (isActive)
            {
                float tempWidth = 30f;
                float tempHeight = 25f;
                float tempOffsetX = 0; // Offset from the origin on the X axis
                float tempOffsetY = -22; // Offset from the origin on the Y axis

                if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                // Choose a hitbox to use from the hitbox list and set the parameters for it
                P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 2 * P.direction, -2, 10, 20, THitstop.LIGHT);
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

				P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 4f * P.direction, -3, 15, 30, THitstop.MEDIUM);
                
                P.allHitboxes[0].active = true;
            }
            else P.allHitboxes[0].active = false;
        }
        #endregion

		#region // ----- Finishers
		public static void upFinisher(Player P, bool isActive, int part)
		{
            if (part == 1)
            {
                if (isActive)
                {
                    float tempWidth = 82f;
                    float tempHeight = 44f;
                    float tempOffsetX = -32; // Offset from the origin on the X axis
                    float tempOffsetY = -22; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 2 * P.direction, -2, 5, 30, THitstop.LIGHT2);
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
            else if (part == 2 )
            {
                if (isActive)
                {
                    float tempWidth = 82f;
                    float tempHeight = 44f;
                    float tempOffsetX = -32; // Offset from the origin on the X axis
                    float tempOffsetY = -22; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, -4 * P.direction, -4, 5, 30, THitstop.LIGHT2);
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
            else if (part == 3 )
            {
                if (isActive)
                {
                    float tempWidth = 82f;
                    float tempHeight = 62f;
                    float tempOffsetX = -32; // Offset from the origin on the X axis
                    float tempOffsetY = -42; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(
                        P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, 
                        tempWidth, tempHeight, 
                        newActiveFrames:0, 
                        newKBX:0, newKBY: P.jumpHeight, 
                        newDamage:10, 
                        newHitstun:20, THitstop.MEDIUM);
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
            
		}
		public static void downFinisher(Player P, bool isActive, int part)
		{
            if (part == 1){
                if (isActive){
                    float tempWidth = 42f;
                    float tempHeight = 62f;
                    float tempOffsetX = 10; // Offset from the origin on the X axis
                    float tempOffsetY = -42; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[2].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 0, 4, 20, 20, THitstop.MEDIUM);
                    P.allHitboxes[2].active = true;
                }
                else P.allHitboxes[2].active = false;
            }
            else if (part == 2){// Landing Hitbox
                if (isActive){
                    float tempWidth = 82f;
                    float tempHeight = 44f;
                    float tempOffsetX = -32; // Offset from the origin on the X axis
                    float tempOffsetY = -22; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(P.origin.X + tempOffsetX, P.origin.Y + tempOffsetY, tempWidth, tempHeight, 0, 0, -4, 5, 30, THitstop.MEDIUM);
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
		}
		public static void frontFinisher(Player P, bool isActive, int part){
			if (part == 1){
                if (isActive){
                    float tempWidth = 90f;
                    float tempHeight = 44f;
                    float tempOffsetX = -22; // Offset from the origin on the X axis
                    float tempOffsetY = -22; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(
                        P.origin.X + tempOffsetX, 
                        P.origin.Y + tempOffsetY, 
                        tempWidth, 
                        tempHeight, 
                        newActiveFrames: 0, 
                        newKBX: -2 * P.direction, 
                        newKBY: -2, 
                        newDamage: 5, 
                        newHitstun: 30, 
                        newHitstop: THitstop.LIGHT
                        );
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
            else if (part == 2){
                if (isActive){
                    float tempWidth = 90f;
                    float tempHeight = 44f;
                    float tempOffsetX = -22; // Offset from the origin on the X axis
                    float tempOffsetY = -22; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(
                        P.origin.X + tempOffsetX, 
                        P.origin.Y + tempOffsetY, 
                        tempWidth, 
                        tempHeight, 
                        newActiveFrames: 0, 
                        newKBX: 2 * P.direction, 
                        newKBY: -2, 
                        newDamage: 5, 
                        newHitstun: 30, 
                        newHitstop: THitstop.LIGHT
                        );
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
            else if (part == 3){
                if (isActive){
                    float tempWidth = 90f;
                    float tempHeight = 50f;
                    float tempOffsetX = -22; // Offset from the origin on the X axis
                    float tempOffsetY = -32; // Offset from the origin on the Y axis

                    if (P.direction == -1) tempOffsetX = (tempOffsetX + tempWidth) * -1;

                    // Choose a hitbox to use from  the hitbox list and set the parameters for it
                    P.allHitboxes[0].setParameters(
                        P.origin.X + tempOffsetX, 
                        P.origin.Y + tempOffsetY, 
                        tempWidth, 
                        tempHeight, 
                        newActiveFrames: 0, 
                        newKBX: 7 * P.direction, 
                        newKBY: -2, 
                        newDamage: 10, 
                        newHitstun: 50, 
                        newHitstop: THitstop.MEDIUM);
                    P.allHitboxes[0].active = true;
                }
                else P.allHitboxes[0].active = false;
            }
		}
		public static void backFinisher(Player P, bool isActive)
		{
			
		}
		#endregion
        
        #endregion
        
        #region ========== States ========== //
		
        #region // ----- Ground
        public static void bAttack1_State(Player P, int frame){ 
            // -------------------------------  ATTACK 1
            if (frame == 5){
				P.velocity.X = 1f * P.direction;
                bAttack1(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) bAttack1(P, false); // Deactivate the corresponding hitboxes
            else if (frame < 8) {} // Endlag = 6
            else if (frame < 15){
				// ENDLAG
                P.attackCounter = 2;
				if (P.attacking) P.set_state(TState.BASICATTACK2,  TState.BASICATTACK2 , TState.IDLE);
                else if (P.dashing) {
                    P.attacking = false;
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
				}
				else if (P.jumping){
                    P.attacking = false;
					P.attackCounter = 1; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
            }
            else{
				P.attackCounter = 1; // Reset the counter
                P.attacking = false;
				if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);	
                else P.set_state(TState.AIR, TState.BALLEND, TState.FALL);
            }
        }

        public static void bAttack2_State(Player P, int frame){
            // ------------------------------- Hit 1
            if (frame == 5){
				P.velocity.X = 1.5f * P.direction;
                bAttack2(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) bAttack2(P, false); // Deactivate the corresponding hitboxes
            // ------------------------------- Endlag / Startup
            else if (frame < 8) {} 
            // ------------------------------- Cancel
            else if (frame < 20){
                P.attackCounter = 3;
                // Up Finisher
				if (P.move_up) P.set_state(TState.UPFINISHER, TState.UPFINISHER);
				// Down Finisher
				else if (P.crouching) P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER);
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1)){
					P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
				}
				// Back Finisher
				// else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1)){

				// }
                else if (P.attacking) P.set_state(TState.BASICATTACK3, TState.BASICATTACK3);
				else if (P.jumping){
					P.attackCounter = 1; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
				else if (P.dashing) P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
            }
            else{
				P.attackCounter = 1;
                if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);
                else P.set_state(TState.AIR,  TState.BALLEND, TState.FALL);
            }
        }

        public static void bAttack3_State(Player P, int frame){
            // ------------------------------- Hit 1
            if (frame == 6){
				P.velocity.X = 4f * P.direction;
                bAttack3(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) bAttack3(P, false); // Deactivate the corresponding hitboxes
            // ------------------------------- Startup / Endlag
            else if (frame < 10) {}
            // ------------------------------- Cancel
            else if (frame < 22){
                P.attackCounter = 4;
				// Up Finisher
				if (P.move_up) P.set_state(TState.UPFINISHER, TState.UPFINISHER, TState.UPFINISHER);
				// Down Finisher
				else if (P.crouching) P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER);
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1)){
					P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
				}
				// Back Finisher
				// else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
				// {

				// }
				else if (P.attacking) P.set_state(TState.BASICATTACK4, TState.BASICATTACK4);
                else if (P.dashing) P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
            }
            else
            {
				P.attackCounter = 1; // Reset so that the first attack will start

                if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);
                else P.set_state(TState.AIR,  TState.BALLEND, TState.FALL);
            }
        }
		
		public static void bAttack4_State(Player P, int frame){
			P.velocity.Y = 0.1f;
            if (frame >= 7 && frame < 18 ) P.velocity.X = 3f*P.direction;
            // ------------------------------- ATTACK 3
            if (frame < 4){ // Startup = 5
				P.velocity.X = -1f * P.direction;
                // May switch from air -> ground or ground -> air if the player is no longer P.grounded
            }
            // Hit 1
            else if (frame ==  7){
				P.velocity.X = 4f * P.direction;
                P.spriteCoordinate.X += 7 * P.direction;
                bAttack4(P, true); // Activate the corresponding hitboxes
            }
            else if (frame ==  8) bAttack4(P, false); 
			// Hit 2
			else if (frame == 11) bAttack4(P, true);
			else if (frame == 12) bAttack4(P, false);
			// Hit 3
			else if (frame == 15) bAttack4(P, true, 2);
			else if (frame == 16) bAttack4(P, false, 2);
			// Endlag / Startup Gaps (Frames 1-3, 8, 10, 12, 14, 16 - 21)
			else if (frame < 21){}
            else if (frame < 22){
                P.attackCounter = 5;
                P.spriteCoordinate.X = P.spriteCoordDefault.X;
				// Up Finisher
                if (P.move_up) P.set_state(TState.UPFINISHER, TState.UPFINISHER, TState.UPFINISHER);
				// Down Finisher
				else if (P.crouching) P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER, TState.DOWNFINISHEREND);
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1))
				{
					P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
				}
				// Back Finisher
				// else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
				// {

				// }
                else if (P.attacking) P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER);
                else if (P.dashing) P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
            }
            else
            {
				P.attackCounter = 1; // Reset so that the first attack will start

                if (P.grounded) P.set_state(TState.IDLE,  TState.TOGROUND, TState.IDLE);
                else P.set_state(TState.AIR,  TState.TOAIR, TState.FALL);
            }
        }
		#endregion
       
        #region // ----- Air
		public static void airAttack1_State(Player P, int frame){
			P.velocity.Y = .5f;
			if (P.applyAttackBounce) attackConnected(P);
            // -------------------------------  ATTACK 1
            if (frame == 5){
				P.velocity.X = 1f * P.direction;
                bAttack1(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) bAttack1(P, false); // Deactivate the corresponding hitboxes
            else if (frame < 8) {} // Endlag = 6
            else if (frame < 15){
				// ENDLAG
                P.attackCounter = 2;
				if (P.attacking) P.set_state(TState.AIRATTACK2,  TState.AIRATTACK2 , TState.IDLE);
                else if (P.dashing) {
                    P.attacking = false;
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
				}
				else if (P.jumping){
                    P.attacking = false;
					P.attackCounter = 1; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
            }
            else{
				P.attackCounter = 1; // Reset the counter
                P.attacking = false;
				if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);	
                else P.set_state(TState.AIR, TState.BALLEND, TState.FALL);
            }
        }

        public static void airAttack2_State(Player P, int frame){
			P.velocity.Y = .5f;
			if (P.applyAttackBounce) attackConnected(P);

            // ------------------------------- Hit 1
            if (frame == 5){
				P.velocity.X = 1.5f * P.direction;
                bAttack2(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) bAttack2(P, false); // Deactivate the corresponding hitboxes
            // ------------------------------- Endlag / Startup
            else if (frame < 8) {} 
            // ------------------------------- Cancel
            else if (frame < 20){
                P.attackCounter = 3;
                // Up Finisher
				if (P.move_up) P.set_state(TState.UPFINISHER, TState.UPFINISHER);
				// Down Finisher
				else if (P.crouching) P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER);
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1)){
					P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
				}
				// Back Finisher
				// else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1)){

				// }
                else if (P.attacking) P.set_state(TState.AIRATTACK3, TState.AIRATTACK3);
				else if (P.jumping){
					P.attackCounter = 1; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
				else if (P.dashing) P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
            }
            else{
				P.attackCounter = 1;
                if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);
                else P.set_state(TState.AIR,  TState.BALLEND, TState.FALL);
            }
        }

        public static void airAttack3_State(Player P, int frame){   
			P.velocity.Y = .5f;
			if (P.applyAttackBounce) attackConnected(P);
            // ------------------------------- Hit 1
            if (frame == 6){
				P.velocity.X = 4f * P.direction;
                airAttack3(P, true); // Activate the corresponding hitboxes
            }
            else if (frame == 7) airAttack3(P, false); // Deactivate the corresponding hitboxes
            // ------------------------------- Startup / Endlag
            else if (frame < 10) {}
            // ------------------------------- Cancel
            else if (frame < 22){
                P.attackCounter = 4;
				// Up Finisher
				if (P.move_up) P.set_state(TState.UPFINISHER, TState.UPFINISHER, TState.UPFINISHER);
				// Down Finisher
				else if (P.crouching) P.set_state(TState.DOWNFINISHER, TState.DOWNFINISHER);
				// Front Finisher
				else if ((P.move_right && P.direction == 1) || (P.move_left && P.direction == -1)){
					P.set_state(TState.FRONTFINISHER, TState.FRONTFINISHER, TState.FRONTFINISHER);
				}
				// Back Finisher
				// else if ( (P.attacking && P.move_right && P.direction == -1) || (P.attacking && P.move_left && P.direction == 1))
				// {

				// }
				else if (P.attacking) P.set_state(TState.BASICATTACK4, TState.BASICATTACK4);
                else if (P.dashing) P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
            }
            else{
				P.attackCounter = 1; // Reset so that the first attack will start

                if (P.grounded) P.set_state(TState.IDLE, TState.IDLE);
                else P.set_state(TState.AIR,  TState.BALLEND, TState.FALL);
            }
        }
        #endregion

		#region // ----- Finishers
		public static void upFinisher_State(Player P, int frame)
		{
            if (frame < 25) P.velocity.Y = 0;
            if (!P.grounded && P.applyAttackBounce) attackConnected(P);
            // ------------------------------- Hit 1
            if (frame == 5) upFinisher(P, true, 1); 
            else if (frame == 6) upFinisher(P, false, 1);
            else if (frame == 8 )P.spriteCoordinate.X = (P.spriteCoordDefault.X + (P.direction * 20));

            // ------------------------------- Hit 2
            else if (frame == 13) upFinisher(P, true, 2); 
            else if (frame == 14) upFinisher(P, false, 2);
            else if (frame == 16) {
                P.spriteCoordinate.X = P.spriteCoordDefault.X;
                P.velocity.X += -1.5f*P.direction;
            }
            // ------------------------------- Hit 3
            else if (frame == 25) {
                upFinisher(P, true, 3);
            
                // Allow the player to jump after the upfinisher
                if (P.move_up && P.grounded) 
                {
                    P.velocity.Y = P.jumpHeight;
                    P.grounded = false;
                    P.applyAttackBounce = false;
                }
            }
            else if (frame ==  26) upFinisher(P, false, 3);
            // ------------------------------- Startup / Endlag
            else if (frame < 32){}
            // ------------------------------- Cancel
            else if (frame <  39){   
                if (P.attackCounter == 5) P.attackCounter = 1;
                if (P.dashing) {
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
                    return;
				}
				else if (P.jumping && !P.attacking)
				{
					P.grounded = false;
                    P.attackCounter = 1;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
                else if (P.attacking)
                {
                    P.attackCounter = 1;
                    if (P.grounded) P.set_state(TState.BASICATTACK1,  TState.BASICATTACK1 , TState.BASICATTACK1);
                    else P.set_state(TState.AIRATTACK1,  TState.AIRATTACK1 , TState.AIRATTACK1);
                }
            }
            else
            {
                P.attackCounter = 1;
                P.set_state(TState.AIR, TState.FALL);
            }
		}
		public static void downFinisher_State(Player P, int frame){
            if (frame == 15) P.velocity.Y = 12;
            else if (frame == 16) P.velocity.Y = 8;
            
            if (frame == 2) {
                P.dashing = false;
                P.velocity.Y = -2f;
            }
            else if (frame == 7) {
                downFinisher(P, true, 1);
                P.velocity.Y = -0.5f;
                // P.applyFall = false;
            }
            else if (frame == 8) downFinisher(P, false, 1);
            else if (frame < 17) {}
            else if (frame < 42) {
                if (P.attackCounter == 5) P.attackCounter = 1;
                if (P.dashing) {
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
                    return;
				}
				else if (P.jumping && !P.attacking){
					P.grounded = false;
                    P.attackCounter = 1;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
                if (P.grounded){
                    P.set_state(TState.DOWNFINISHERGROUND, TState.TOGROUND, TState.IDLE);
                } 
            }
            else{
                if (!P.grounded) P.set_state(TState.AIR, TState.FALL);
                else P.set_state(TState.IDLE, TState.IDLE);
            }
		}
        public static void downFinisherLag_State(Player P, int frame){

            if (frame == 1) downFinisher(P, true, 2); // Landing Hitbox
            else if (frame == 2) downFinisher(P, false, 2);
            else if  (frame < 5){} // Landing lag
            else if (frame < 16){ // Cancellable actions
                if (P.dashing) {
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
                    return;
				}
                // if (P.attacking) {
                //     P.attackCounter = 1;
                //     if (!P.grounded) P.set_state(TState.AIRATTACK1,  TState.AIRATTACK1, TState.AIRATTACK1);
                //     else P.set_state(TState.BASICATTACK1,  TState.BASICATTACK1, TState.BASICATTACK1);
                // }
                else if (P.jumping){
					P.attackCounter = 1; // Reset the counter
					P.grounded = false;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
            }else{
                P.attackCounter = 1; // Reset the counter
                if (!P.grounded) P.set_state(TState.AIR, TState.FALL);
                else P.set_state(TState.IDLE, TState.IDLE);
            } 
        }
		public static void frontFinisher_State(Player P, int frame){
            P.velocity.Y = 1;
            // ------------------------------- Hit 1
            if (frame == 8) {
                frontFinisher(P, true, 1); 
                P.spriteCoordinate.X += 20 * P.direction;
            }
            else if (frame == 9) frontFinisher(P, false, 1);
            // ------------------------------- Hit 2
            else if (frame ==  14){
                frontFinisher(P, true, 2); 
                P.spriteCoordinate.X = P.spriteCoordDefault.X;
            }
            else if (frame ==  15) frontFinisher(P, false, 2);
            // ------------------------------- Hit 3
            else if (frame == 20) {
                P.velocity.X = 3 * P.direction;
                frontFinisher(P, true, 3);
            }
            else if (frame == 21) frontFinisher(P, false, 3);
            // ------------------------------- StartUp / Endlag
            else if (frame < 30){}
            // ------------------------------- Cancel
            else if (frame < 39){
                if (P.attackCounter == 5) P.attackCounter = 1;
				if (P.dashing) {
					P.set_state(TState.DASH, TState.DASHSTART, TState.NULL);
                    return;
				}
				else if (P.jumping){
					P.grounded = false;
                    P.attackCounter = 1;
					P.hurtbox.changeDimensions(new Vector2(0, -10), 34, 42);
					P.set_state(TState.JUMP, TState.JUMPSTART, TState.JUMP);
				}
                // else if (P.attacking){
                //     P.attackCounter = 1;
                //     if (!P.grounded) P.set_state(TState.AIRATTACK1,  TState.AIRATTACK1, TState.AIRATTACK1);
                //     else P.set_state(TState.BASICATTACK1,  TState.BASICATTACK1, TState.BASICATTACK1);
                // }
            }
            else{
                P.attackCounter = 1;
                P.set_state(TState.AIR, TState.FALL);
            }
		}
		public static void backFinisher_State(Player P, int frame)
		{
            
		}
        #endregion
		
        #endregion
	}
    
}