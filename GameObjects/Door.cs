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

        // ------ COLLISION ------- //
        public bool hovering = false;
        public bool active = false;
        public bool open = false;
        public bool locked = false;
        public Doorbox bounds;

        // ------ ANIMATION ------ //
        public Vector2 spriteCoordinate; // Placement of sprite in relation to the hurtbox. Calculated with aesprite


        // ------ EFFECTS ------ //
		public SpriteEffects flip;

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