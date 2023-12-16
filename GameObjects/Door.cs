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

    public abstract class Door : GameObject
    {
        // ------ DOOR ------ //
        public static Dictionary<int, Door> allDoors; // all the Doors in the curret level
        public int targetID; // the ID of the door that the GameObject is being sent to

        // ------ MOVEMENT ------- //
        public Vector2 endPosition;
        public float width;
        public float height;
        public int ID;

        // ------ COLLISION ------- //
        public bool hovering = false;
        public bool active = false;
        public bool open = false;
        public bool locked = false;
        public Doorbox bounds;

        // ------ ANIMATION ------ //
		public Dictionary<StateType, Animation> animations;
		public StateType currentAnimation;
		public StateType previousAnimation;
		public StateType tempAnimation;
		public ArrayList preAnimations;
        public Vector2 spriteCoordinate; // Placement of sprite in relation to the hurtbox. Calculated with aesprite


        // ------ EFFECTS ------ //
		public SpriteEffects flip;

        // ------ STATES ------ //
        public StateType currentState;
		public StateType previousState;
        public int stateFrame; 		
        public float stateTimer;

        // ----- OTHER ----- //
        public SpriteFont stateFONT { get; set; }

        // ========================================== LOADING ==========================================
        public static void LoadDoors(){allDoors = new Dictionary<int, Door>();}
        
        // ========================================== STATE FUNCTIONS ==========================================
        
        /// Manages the state of the player when the Door is in the send state. 
        public abstract void sendState(object sender, EventArgs args);

        /// Manages the state of the player when the Door is in the receive state. 
        public abstract void receiveState(object sender, EventArgs args);
        
        // ========================================== DOOR FUNCTIONS ==========================================
        
        /// Sends the player to another door
        public abstract void Send(Player player, int newDoor);

        /// Returns control back to the player
        public abstract void Receive(Player player);
    }
}