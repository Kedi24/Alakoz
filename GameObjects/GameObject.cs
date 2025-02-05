using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.GameInfo;
using Alakoz.Collision;
using Microsoft.Xna.Framework.Content;

namespace Alakoz.GameObjects
{
    public abstract class GameObject
    {
        #region // ----- Physics Variables ----- //
        public int id = -1;
        public TObject type = TObject.NULL;
        public Vector2 position;
        public Vector2 prevPosition;
		public Vector2 nextPosition;

		public int direction = 1;
        public Vector2 velocity = Vector2.Zero;
        public Vector2 origin;
        public Vector2 originOffset;
        #endregion

        #region // ----- Animation Variables ----- //
        public Dictionary<TState, Animation> animations;
		public TState currAnimation = TState.NULL;
		public TState prevAnimation = TState.NULL;
		public TState nextAnimation = TState.NULL;
        public TState[] postAnimation = Array.Empty<TState>();
        public AnimationManager animManager;
        #endregion

        #region // ----- Collision Variables ----- //
		public List<CollisionObject> activeCollisions = new List<CollisionObject>();
        public int hitStop = 0; // Number of frames to "pause" the state timer.
        public bool applyKB = false; // To make the object fall
        public bool applyAttackBounce = false; // For air attacks. When a hit lands the object bounce/not fall
        public Vector2 KB; // The knockback from the hitbox that interescts the player

		public float hurtboxWidth = 1; // Default values. DONT CHANGE
		public float hurtboxHeight = 1; // Default values, DONT CHANGE
        #endregion

        #region // ----- State Variables ----- //
		public SpriteFont stateFONT { get; set; }
		public int stateFrame = 0; // Frame for the current state
		public TState currentState;
		public TState previousState;
        #endregion
        
        #region // ==================== SETTERS ==================== //
		public void set_animations(TState next = TState.NULL, params TState[] post){
			if (next != TState.NULL) nextAnimation = next;
			if (post.Length != 0 ) postAnimation = post;
		}

		public virtual void set_state(TState newState, TState newAnim = TState.NULL, params TState[] newPostAnim){
            if (currentState == newState) return;

            previousState = currentState;
            currentState = newState;
            stateFrame = 1;
			set_animations(
				newAnim != TState.NULL? newAnim : nextAnimation,
				newPostAnim.Length != 0? newPostAnim : Array.Empty<TState>()
			);
        }
        #endregion

        #region // ==================== UPDATING ==================== //
        /// <summary>
        /// Update the time associated with the GameObject
        /// </summary>
        public virtual void update_time(){
            if (hitStop <= 0 )stateFrame++;
			else hitStop--;
				
			if (stateFrame > 9999) stateFrame = 1;
        }
        
        
        public abstract void update_input();

        /// <summary>
        /// Modifies the current state of the door based on the inputs
        /// </summary>
        public abstract void update_state();

        /// <summary>
        /// Update the physics to be applied depending on the state
        /// </summary>
        public abstract void update_physics();

        /// <summary>
        /// Update the animations to be played depending on the state
        /// </summary>
        // Updating the current animation to be played
		public virtual void update_animations(){
            if (currAnimation != nextAnimation && nextAnimation != TState.NULL){
            	prevAnimation = currAnimation;

				// Create a copy and map each animation name to the animation
				Animation[] postAnimationList = ((TState[])postAnimation.Clone()).ToList().ConvertAll(name => animations[name]).ToArray();
				Array.Reverse(postAnimationList);
				
                if (postAnimation.Length == 0) animManager.Play(animations[nextAnimation]);
                else  animManager.Play(animations[nextAnimation], postAnimationList);
                postAnimation = Array.Empty<TState>(); // reset postAnimation  
            }
            currAnimation = animManager.getName();
            nextAnimation = TState.NULL; // reset nextAnimation  
        }
        
        // public abstract void Update();
        #endregion

        #region // ==================== DRAWING ==================== //
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        #endregion
        
        #region // ========== COMPUTATIONS ========== //
        /// <summary>
        /// Approaches <end> starting from <start> by an amount specified by <shift>.
        /// Returns the amount to move by. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public float approach(float start, float end, float shift)
        {
            if (end > start)
            {
                var amount = start + shift;
                if (amount < end) return amount;
                else return end;
            } 
            else 
            {
                var amount = start - shift;
                if (amount > end) return amount;
                else return end;
            }
        }

        
        /// <summary>
        /// Recursive function using the Bezier Curve Formula with multiple points.
        /// Returns the point given a sequence of points [p0, p1, ..., pn] and the "percentage" <t>
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector2 bezierCurve(Vector2[] points, float t)
        {
            // Base Case
            if (points.Length == 2) return computeBezierCurve(points[0], points[1], t);
            // Recursive Call
            else
            {   
                Vector2[] firstList = new Vector2[points.Length - 1];
                Vector2[] secondList = new Vector2[points.Length - 1];
                
                // Split the list into two. One that contains the first n-1, and the second tha contains the last n-1
                for (int i = 0; i < points.Length; i++)
                {
                    if (i == 0) firstList[0] = points[i]; // First element goes in the fist list
                    else if (i == points.Length - 1) secondList[i - 1] = (points[i]); // Last element goes in the second list
                    else // The remaining elements are shared
                    {
                        firstList[i] = points[i]; // firstList = [0, 1, 2,....,n - 1] 
                        secondList[i - 1] = points[i]; // secondList = [1, 2, 3....,n] shift over to the left (i - 1) since index starts at 0
                    }
                }
                Vector2 L1 = bezierCurve(firstList, t);
                Vector2 L2 = bezierCurve(secondList, t);

                return computeBezierCurve(L1, L2, t);
            } 
           

        }
        

        /// <summary>
        /// Bezier Curve Formula.
        /// Returns the point on a line segement given a start <p0> and end <p1> points. 
        /// The point will depend on t <percent>. 0 will give the start and 1 will give the end.
        /// </summary>
        /// 
        public Vector2 computeBezierCurve(Vector2 p0, Vector2 p1, float percent)
        {
            return ((1 - percent) * p0) + (percent * p1); // L(t) = (1 - t)p0 + (t)p1
        }
        #endregion
    }
    

    // ############################################################################################################################################
    // // #################################################   GAME OBJECT ASSETS   ################################################################
    // ############################################################################################################################################
    public interface GameAsset
    {
        // ========================================== References ==========================================
        public static ContentManager Content = Game1.thisGame.Content;
        public static Dictionary<TState, Animation> PlayerAnimations {get; set;}
        public static Dictionary<TState, Animation> EnemyAnimations {get; set;}
        public static Dictionary<TState, Animation> DoorAnimations {get; set;}
        public static Dictionary<TCollision, Animation> CollisionAnimations {get; set;}

        // ========================================== LOADING ==========================================
        // ---------- Species ---------- //
        public static void LoadPlayerAssets()
        {
            string playerDirectory = "Alakoz Content/Species/Player/Rebel_Animations/"; 
            string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";

            Animation[] animations = new Animation[]{
                new Animation( TState.SYMBOL, Content.Load<Texture2D>( enemyDirectory + "ball"), 1),
                new Animation( TState.NULL, Content.Load<Texture2D>( playerDirectory + "Rebel_None"), 1),
                new Animation( TState.NONE, Content.Load<Texture2D>( playerDirectory + "Rebel_None"), 1),
                // ----- Idle
                new Animation( TState.IDLE, Content.Load<Texture2D>(playerDirectory + "Rebel_Idle"), frames:40),
                // ----- Run
                new Animation( TState.RUNSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_RunStart"), 10, false),
                new Animation( TState.RUN, Content.Load<Texture2D>(playerDirectory + "Rebel_Run"), 20),
                new Animation( TState.RUNEND, Content.Load<Texture2D>(playerDirectory + "Rebel_Runstop"), 21, false),
                // ----- Jump
                new Animation( TState.JUMPSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_JumpStart"), 8, false),
                new Animation( TState.JUMP, Content.Load<Texture2D>(playerDirectory + "Rebel_Jump"), 12),
                new Animation( TState.FALL, Content.Load<Texture2D>(playerDirectory + "Rebel_Fall"), 12),
                // ----- Crouch
                new Animation( TState.CROUCHSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchStart"), 4, false),
                new Animation( TState.CROUCH, Content.Load<Texture2D>(playerDirectory + "Rebel_Crouch"), 24),
                new Animation( TState.CROUCHEND, Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchEnd"), 6),       
                // ----- Dash
                new Animation( TState.DASHSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_DashStart"), 6, false),
                new Animation( TState.DASH, Content.Load<Texture2D>(playerDirectory + "Rebel_Dash"), 12),
                new Animation( TState.DASHEND, Content.Load<Texture2D>(playerDirectory + "Rebel_DashEnd"), 13, false), 
                // ----- Walljump
                new Animation( TState.WALLCLING, Content.Load<Texture2D>(playerDirectory + "Rebel_WallCling"), 20),
                new Animation( TState.WALLJUMPSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_WallJumpStart"), 1), 
                new Animation( TState.WALLJUMP, Content.Load<Texture2D>(playerDirectory + "Rebel_WallJump"), 20, false),      
                // ----- Ball
                new Animation( TState.BALLSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_BallStart"), 4, false),
                new Animation( TState.BALL, Content.Load<Texture2D>(playerDirectory + "Rebel_Ball"), 12),
                new Animation( TState.BALLEND, Content.Load<Texture2D>(playerDirectory + "Rebel_BallEnd"), 2, false),
                // ----- Attacks
                new Animation( TState.BASICATTACK1, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack1"), 20, false),
                new Animation( TState.BASICATTACK2, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack2"), 22, false),
                new Animation( TState.BASICATTACK3, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack3"), 23, false),
                new Animation( TState.BASICATTACK4, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack4"), 23 , false),
                // ----- Air Attacks
                new Animation( TState.AIRATTACK1, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack1"), 20, false),
                new Animation( TState.AIRATTACK2, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack2"), 22, false),
                new Animation( TState.AIRATTACK3, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack3"), 23, false),
                // ----- Finishers
                new Animation( TState.FRONTFINISHER, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherFront"), 39, false),
                new Animation( TState.BACKFINISHER, Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack1"), 20, false),
                new Animation( TState.UPFINISHER, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherUp"), 39, false),

                new Animation( TState.DOWNFINISHER, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherDown"), 42, false, rows:7, columns:6),
                new Animation( TState.DOWNFINISHERSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherDownStart"), 6, false),
                new Animation( TState.DOWNFINISHEREND, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherDownEnd"), 10, false),
                new Animation( TState.DOWNFINISHERGROUND, Content.Load<Texture2D>(playerDirectory + "Rebel_FinisherDownGround"), 14, false),
                // ----- Other
                new Animation( TState.TOGROUND, Content.Load<Texture2D>(playerDirectory + "Rebel_TeleportToGround"), 14, false),
                new Animation( TState.TOAIR, Content.Load<Texture2D>(playerDirectory + "Rebel_TeleportToAir"), 4, false),
                // ----- Hit
                new Animation( TState.HITSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_HitStart"), 8, false),
                new Animation( TState.HIT, Content.Load<Texture2D>(playerDirectory + "Rebel_Hit"), 16),
                // ----- Interaction
                new Animation( TState.DOORENTER, Content.Load<Texture2D>(playerDirectory + "Rebel_DoorEnter"), 34, false)
            };

            PlayerAnimations = new Dictionary<TState, Animation>{};
            foreach(Animation animation in animations){ PlayerAnimations.Add(animation.Name, animation );};
        }

        public static void LoadEnemyAssets()
        {
            string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";
            string effectDirectory = "Alakoz Content/Effects/General/";
            string playerDirectory = "Alakoz Content/Species/Player/Rebel_Animations/";

            Animation[] animations = new Animation[]{
                new Animation(TState.NULL, Content.Load<Texture2D>( enemyDirectory + "ball"), 1),
                new Animation(TState.HURTBOX, Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1),
                new Animation(TState.HITBOX, Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1),
                new Animation(TState.IDLE, Content.Load<Texture2D>( enemyDirectory + "Base_Idle"), 32),
                // new Animation(TState.NONE, Content.Load<Texture2D>( enemyDirectory + "Base_Idle2"), 8),
                new Animation(TState.RUN, Content.Load<Texture2D>( enemyDirectory + "Base_Running"), 22),
                new Animation(TState.RUNEND, Content.Load<Texture2D>( enemyDirectory + "Base_RunStop"), 24, false),
                new Animation(TState.NONE, Content.Load<Texture2D>( enemyDirectory + "Base_Turnaround"), 12),
                new Animation(TState.JUMP, Content.Load<Texture2D>( enemyDirectory + "Base_Jump"), 10, false),
                new Animation(TState.FALL, Content.Load<Texture2D>( enemyDirectory + "Base_Falling"), 12),
                new Animation(TState.HITSTART, Content.Load<Texture2D>(playerDirectory + "Rebel_HitStart"), 8, false),
                new Animation(TState.HIT, Content.Load<Texture2D>(playerDirectory + "Rebel_Hit"), 16)
            };
            EnemyAnimations = new Dictionary<TState, Animation>{};
            foreach(Animation animation in animations){ EnemyAnimations.Add(animation.Name, animation );}; 
        }

        // ---------- Map Objects---------- //
        public static void LoadDoorAssets()
        {
            // Load the animations for the Door

            string doorDirectory = "Alakoz Content/Maps/MapObjects/Doors/Door - Small/";
            string effectDirectory = "Alakoz Content/Effects/General/";
            Animation Hurtbox = new Animation(TState.NULL, Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation Hitbox = new Animation(TState.NULL, Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            Animation open = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_Open"), 1);                
            Animation openStart = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_OpenStart"), 9, false);
            
            Animation close = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_Close"), 1);
            Animation closeStart = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_CloseStart"), 10, false);
            
            Animation idle = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_Idle"), 38, false);
            Animation unlock = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_Unlock"), 30, false);
            Animation fadeIn = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_FadeIn"), 11, false);
            Animation fadeOut = new Animation(TState.NULL, Content.Load<Texture2D>(doorDirectory + "SmallDoor_FadeOut"), 10, false);

            // Animations for the Door
            DoorAnimations = new Dictionary<TState, Animation>
            {
                {TState.ACTIVE, Hitbox},
                {TState.ACTIVESTART, Hitbox},
                {TState.INACTIVE, Hurtbox}, 
                {TState.INACTIVESTART, Hurtbox},

                {TState.OPEN, open},
                {TState.OPENSTART, openStart},
                {TState.CLOSE, close},
                {TState.CLOSESTART, closeStart},
                {TState.IDLE, idle},
                {TState.UNLOCK, unlock},
                {TState.FADEIN, fadeIn},
                {TState.FADEOUT, fadeOut},
            };
        }

        // ---------- Backgrounds ---------- //
        // ---------- Effects ---------- //
        // ---------- Sounds ---------- //
        // ---------- Debug ---------- //
        public static void LoadDebug()
        {
            string effectDirectory = "Alakoz Content/Effects/General/";
            
            Animation hurtboxSprite = new Animation(TState.NULL, Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation hitboxSprite = new Animation(TState.NULL, Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            CollisionAnimations = new Dictionary<TCollision, Animation>
            {
                { TCollision.HURTBOX, hurtboxSprite },
                { TCollision.ENEMYHURTBOX, hurtboxSprite },
                { TCollision.HITBOX, hitboxSprite }
            };
        }
    }

}