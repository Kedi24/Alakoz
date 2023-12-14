using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.GameInfo;
using Alakoz.Animate;
using Alakoz.Collision;


namespace Alakoz.LivingBeings
{
    // A representation of a Door

    public class BasicDoor : Door
    {
        // --------------------------------------------------- CONSTRUCTOR ---------------------------------------------------
        public BasicDoor( Vector2 newPosition, float newWidth, float newHeight, int newID, int newTargetID, Vector2 movePosition, Dictionary<StateType, Animation> newAnimations)
        {
            // Dimensions\  
            position = newPosition;
            width = newWidth;
            height = newHeight;
            ID = newID;
            targetID = newTargetID;
            endPosition = movePosition;

            // Collision
            bounds = new Doorbox(this, position, width, height);
            activeCollisions.Add(bounds);

            // States
            previousState = StateType.INACTIVE;
			currentState = StateType.INACTIVE;
            set_state(StateType.INACTIVE); // Default state 

            // Animations
            animations = newAnimations;
            animManager = new AnimationManager(animations[StateType.INACTIVE]);
            spriteCoordinate = new Vector2(0, 0);

            currentAnimation = StateType.INACTIVE;
            tempAnimation = StateType.CLOSE;
            preAnimations = new ArrayList();

            if (!allDoors.ContainsKey(ID)) allDoors.Add(ID, this);
        }

        // ========================================== STATE FUNCTIONS ==========================================

        /// When the target is not colliding with the door
        public void inactiveState()
        {
            if (hovering) 
            {
                tempAnimation = StateType.CLOSE;
				preAnimations = new ArrayList(){StateType.FADEOUT, StateType.UNLOCK};
                set_preAnimations(); // Need to call it here since currentAnimation == StateType.Close == tempAnimation
                
                set_state(StateType.ACTIVE);
                activeState();
            }
            else if (open)
            {
                tempAnimation = StateType.OPEN;
                preAnimations = new ArrayList(){StateType.OPENSTART};
                set_state(StateType.OPEN);
                openState();
            }
            else
            {
                if (stateFrame % 60 == 0 && stateFrame > 0 || stateFrame == 5) 
                {
                    preAnimations = new ArrayList(){StateType.IDLE};
                    set_preAnimations();
                }
                tempAnimation = StateType.CLOSE;
            }
        }
       
        /// When the target is colliding with the door. The door becomes active and can be used by the target. 
        public void activeState()
        {
            if (!hovering) 
            {
                tempAnimation = StateType.INACTIVE;
                set_state(StateType.INACTIVE);
                inactiveState();
            }
            else if (open)
            {
                tempAnimation = StateType.OPEN;
                preAnimations = new ArrayList(){StateType.OPENSTART};
                set_state(StateType.OPEN);
                openState();
            }
            else 
            {
                tempAnimation = StateType.CLOSE;
				if (stateFrame % 40 == 0 && stateFrame > 0) 
                {
                    preAnimations = new ArrayList(){StateType.FADEOUT, StateType.FADEIN};
                    set_preAnimations();
                }
            }
        }
        
        /// When the target opens the door.
        public void openState()
        {
            if (!open) 
            {
                // To make the door close when changing from open to inactive/active
                if (!hovering) 
                {
                    tempAnimation = StateType.CLOSE;
				    preAnimations = new ArrayList(){StateType.CLOSESTART};
                    set_state(StateType.INACTIVE);
                    inactiveState();
                }
                else
                {
                    tempAnimation = StateType.ACTIVE;
				    preAnimations = new ArrayList(){StateType.CLOSESTART};
                    set_state(StateType.ACTIVE);
                    activeState();
                }
            }
            else
            {
                tempAnimation = StateType.OPEN;
            }
        }


        // ------------------ Activation States ( invoked by another GameObject)

        /// State for when the door is "sending" a GameObject to another Door
        public override void sendState(object sender, EventArgs args)
        {
            // Some player variables that will need to be modified
            Player currPlayer = (Player) sender;
            StateType currTempAnimation = currPlayer.tempAnimation;
		    ArrayList currPreAnimations = currPlayer.preAnimations;
            int playerFrame = currPlayer.stateFrame;

            currPlayer.velocity.X = 0;

            // Need to set the state of the player to "INTERACT STATE"
            if (playerFrame >= 40)
            {
                Send(currPlayer, targetID); // Id taken from Tiled
            }
        }

        /// State for when the door is "recieving" a GameObject that was sent from another Door
        public override void receiveState(object sender, EventArgs args)
        {
            // Some player variables that will need to be modified
            Player currPlayer = (Player) sender;
            StateType currTempAnimation = currPlayer.tempAnimation;
		    ArrayList currPreAnimations = currPlayer.preAnimations;
            int playerFrame = currPlayer.stateFrame;

            if (playerFrame >= 40)
            {
                Receive(currPlayer); // Return control to the player
            }
        }
        
        // ========================================== DOOR SPECIFIC FUNCTIONS ==========================================
        
        /// Take Control from the player and delegate it to the door with <doorID>
        public override void Send(Player player, int doorID)
        {
            // Remove this Doors references to the Player
            player.interactFunction -= sendState;
            player.stateFrame = 0;

            // Add the nextDoor's reference to the Player
            Door nextDoor = allDoors[doorID];
            player.interactFunction += nextDoor.receiveState;
            nextDoor.open = true;

            // Move the player to the new door
            player.position.X = nextDoor.position.X + (nextDoor.width/2) - (player.hurtboxWidth/2); // Centers the player to the middle of the door
            player.position.Y = nextDoor.position.Y + nextDoor.height - player.hurtboxHeight;
            
            // Return state control back to this class.
            hovering = false;
            open = false;
        }
        
        /// Return control back to the Player
        public override void Receive(Player player)
        { 
            // Return control to the player
            player.beginInteract = false;
            player.interacting = false;
            player.interactFunction -= receiveState;

            // Close this door. 
            open = false;
        }


        // ========================================== SETTERS ==========================================

        /// Changes the players current state, setting it to <param name="newState"> while recording the previous state.
        /// Also resets the stateFrame count.
        public void set_state(StateType newState)
        {
			
            if (currentState != newState)
			{
				previousState = currentState; 
				stateFrame = 0;
                currentState = newState;
            }
            
        }
		
        // Play a set of non looping animations. This functions keeps the base looping animations the same
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
        // --------------------------------------------------- UPDATING ---------------------------------------------------
        /// <summary>
        /// Sets the values that cause the Door to react
        /// </summary>
        public override void update_input()
        {
            if (hovering) active = true;
        }

        /// <summary>
        /// Modifies the current state of the door based on the inputs
        /// </summary>
        public override void update_state()
        {
            switch (currentState)
            {
                case StateType.ACTIVE:
                    set_state(StateType.ACTIVE);
                    activeState();
                    break;
                case StateType.INACTIVE:
                    set_state(StateType.INACTIVE);
                    inactiveState();
                    break;
                case StateType.OPEN:
                    set_state(StateType.OPEN);
                    openState();
                    break;
            }
        }

        /// <summary>
        /// Update the animations to be played depending on the state
        /// </summary>
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

        /// <summary>
        /// Update the doors physics 
        /// </summary>
        public override void update_physics()
        {

        }

        /// <summary>
        /// Update the timer associated with the door
        /// </summary>s
        public override void update_time(GameTime gameTime)
        {
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stateTimer >= FPS24)
            {
                stateTimer = FPS24 - stateTimer;
                stateFrame++;
            }
			if (stateFrame >= 999) stateFrame = 0;
        }
        public void Update(GameTime gameTime)
        {

        }

        // --------------------------------------------------- DRAWING ---------------------------------------------------
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            animManager.Draw(gameTime, spriteBatch, position + spriteCoordinate, Vector2.One, flip); // Draw the Door
            
            // bounds.Draw(spriteBatch, flip); // Draw the collision bounds
            
            // drawDebug(spriteBatch); 
        }

        public void drawDebug(SpriteBatch spriteBatch)
        {
            string physicsMSG = "Position: " + position 
			+ "\nWidth: " + width
            + "\nHeight: " + height;

            string inputMSG = "Hovering: " + hovering
            + "\nOpen: " + open;

            // Messages to display game / player information
			string stateMSG = "Current State: " + currentState 
			+ "\nPrevious State: " + previousState
            + "\nFrame: " + stateFrame
            + "\nCurrent Animation: " + currentAnimation;

            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(width, -50) + position, Color.DeepPink, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, physicsMSG, new Vector2(-80, -50) + position, Color.GreenYellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(-80, 0) + position, Color.Orange, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
    }
}