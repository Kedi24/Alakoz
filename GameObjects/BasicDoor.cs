using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alakoz.GameInfo;
using Alakoz.Animate;
using Alakoz.Collision;


namespace Alakoz.GameObjects
{
    // A representation of a Door
    public class BasicDoor : Door
    {
        #region // ========== CONSTRUCTOR ========== // 
        public BasicDoor( Vector2 newPosition, float newWidth, float newHeight, int newid, int newTargetid, Dictionary<TState, Animation> newAnimations)
        {
            id = newid;
            type = TObject.BASICDOOR;
            targetID = newTargetid;

            // Dimensions
            position = newPosition;
            width = newWidth;
            height = newHeight;
            originOffset = new Vector2(newWidth/2, newHeight/2);
			origin = position + originOffset;

            // Collision
            bounds = new Doorbox(this, new Vector2(position.X + 20, position.Y + 32), width - 40, height - 32); // Center the hurtbox
            activeCollisions.Add(bounds);

            // States
            previousState = TState.INACTIVE;
			currentState = TState.OPEN;

            // Animations
            animations = newAnimations;
            animManager = new AnimationManager(animations[TState.INACTIVE]);
            spriteCoordinate = new Vector2(0, 0);

            currAnimation = TState.INACTIVE;
            nextAnimation = TState.CLOSE;

            if (!allDoors.ContainsKey(id)) allDoors.Add(id, this);
        }
        #endregion

        #region // ========== SETTERS ========== //
		public override void set_state(TState newState, TState newAnim = TState.NULL, params TState[] newPostAnim){
			if (currentState == newState) return;
			base.set_state(newState, newAnim, newPostAnim);
            find_state(newState); // Call the corresponding state function
        }

		private void find_state(TState newState){
            switch (newState)
            {
                case TState.ACTIVE:
                    activeState();
                    break;
                case TState.INACTIVE:
                    inactiveState();
                    break;
                case TState.OPEN:
                    openState();
                    break;
                default:
                    inactiveState();
                    break;
            }
        }
        
		#endregion
        
        #region // ========== STATE FUNCTIONS ========== // 

        #region // ----- Basic States
        /// When the target is not colliding with the door
        public void inactiveState()
        {
            if (hovering) set_state(TState.ACTIVE,  TState.UNLOCK, TState.FADEOUT, TState.CLOSE);
            else if (open) set_state(TState.OPEN, TState.OPENSTART, TState.OPEN);
            else {
                if (stateFrame % 120 == 0 && stateFrame > 0) set_animations(TState.IDLE, TState.CLOSE);
            }
        }
       
        /// When the target is colliding with the door. The door becomes active and can be used by the target. 
        public void activeState()
        {
            if (!hovering) set_state(TState.INACTIVE, TState.NULL, TState.INACTIVE);
            else if (open) set_state(TState.OPEN, TState.OPENSTART, TState.OPEN);
            else {
				if (stateFrame % 80 == 0 && stateFrame > 0) set_animations(TState.FADEIN, TState.FADEOUT, TState.CLOSE);
            }
        }
        
        /// When the target opens the door.
        public void openState(){
            if (!open) {
                if (!hovering) set_state(TState.INACTIVE, TState.FADEOUT, TState.CLOSESTART, TState.CLOSE);
                else set_state(TState.ACTIVE, TState.CLOSESTART, TState.FADEOUT, TState.CLOSE);
            }
        }
        #endregion

        #region // ----- Interacting States

        /// State for when the door is "sending" a GameObject to another Door
        public override void sendState(object sender, EventArgs args){
            
            // Some player variables that will need to be modified
            Player currPlayer = (Player) sender;
            currPlayer.velocity.X = 0;
            
            if (currPlayer.stateFrame == 1) currPlayer.set_animations(TState.DOORENTER, TState.NONE);

            // Need to set the state of the player to "INTERACT STATE"
            if (currPlayer.stateFrame >= 100) Send(currPlayer, targetID); // Id taken from Tiled
        }

        /// State for when the door is "recieving" a GameObject that was sent from another Door
        public override void receiveState(object sender, EventArgs args){
            // Some player variables that will need to be modified
            Player currPlayer = (Player) sender;
            if (currPlayer.stateFrame == 1) currPlayer.set_animations(TState.NONE);

            if (stateFrame == 12) currPlayer.set_animations(TState.DOORENTER, TState.IDLE);
            if (currPlayer.stateFrame >= 100) Receive(currPlayer); // Return control to the player 
        }
        #endregion
        
        #endregion
        
        #region // ========== DOOR FUNCTIONS ========== // 
        
        /// Take Control from the player and delegate it to the door with <doorid>
        public override void Send(Player player, int doorid){
            // Remove this Doors references to the Player
            player.interactFunction -= sendState;
            player.stateFrame = 1;

            // Add the nextDoor's reference to the Player
            Door nextDoor = allDoors[doorid];
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
        public override void Receive(Player player){ 
            // Return control to the player
            player.beginInteract = false;
            player.interacting = false;
            player.interactFunction -= receiveState;

            // Close this door. 
            open = false;
        }

        #endregion
        
        
        #region // ========== UPDATING ========== // 
        public override void update_time(){
			animManager.Update();
			base.update_time();
        }

        /// <summary>
        /// Sets the values that cause the Door to react
        /// </summary>
        public override void update_input(){
            if (hovering) active = true;
            else active = false;
        }

        /// <summary>
        /// Modifies the current state of the door based on the inputs
        /// </summary>
        public override void update_state()
        {
            find_state(currentState);
            hovering = false;
        }
        public override void update_physics()
        {

        }
        #endregion

        #region // ========== DRAWING ========== // 
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Pause current frame if hit
			if (hitStop > 0) animManager.Pause();
			else animManager.Resume();
			
			// Draw player animation
			Vector2 drawPos = position; 
			// Vector2 drawPos = Vector2.Lerp(prevPosition, position, Game1.thisGame.frameProgress); 
			animManager.Draw(gameTime, spriteBatch, drawPos + spriteCoordinate, Vector2.One, flip);

            // bounds.Draw(spriteBatch, flip); // Draw the collision bounds
            drawDebug(spriteBatch);
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
            + "\nAnimation: " + currAnimation;

            spriteBatch.DrawString(stateFONT, stateMSG, new Vector2(width, -50) + position, Color.DeepPink, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, physicsMSG, new Vector2(-80, -50) + position, Color.GreenYellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stateFONT, inputMSG, new Vector2(-80, 0) + position, Color.Orange, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
        #endregion
    }

   }