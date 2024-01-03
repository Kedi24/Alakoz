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
        public GameObjectType type = GameObjectType.NONE;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin;
        public Vector2 originOffset;
        
        // ------ COLLISION ------ //
		public List<CollisionObject> activeCollisions = new List<CollisionObject>();
        public int hitStop = 0; // Number of frames to "pause" the state timer.
        public bool applyKB = false;

        public bool applyAttackBounce = false; // For air attacks. When a hit lands the object bounce/not fall

        // ------ ANIMATION ----- //
        public AnimationManager animManager;
        public float FPS24 = AnimationManager.FPS24;

        // ========================================== ABSTRACT METHODS ==========================================
        /// <summary>
        /// Update the time associated with the GameObject
        /// </summary>
        public abstract void update_time(GameTime gameTime);
        
        
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
        public abstract void update_animations();

        // public abstract void Update();

        // ========================================== DRAWING  ==========================================
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        // ========================================== COMPUTATIONS ==========================================
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
    }

    // ############################################################################################################################################
    // // #################################################   GAME OBJECT ASSETS   ################################################################
    // ############################################################################################################################################
    public interface GameObjectAsset
    {
        // ========================================== References ==========================================
        public static ContentManager Content = Game1.thisGame.Content;
        public static Dictionary<StateType, Animation> PlayerAnimations {get; set;}
        public static Dictionary<StateType, Animation> EnemyAnimations {get; set;}
        public static Dictionary<StateType, Animation> DoorAnimations {get; set;}
        public static Dictionary<CollisionType, Animation> CollisionAnimations {get; set;}

        // ========================================== LOADING ==========================================
        // ---------- Species ---------- //
        public static void LoadPlayerAssets()
        {
            string playerDirectory = "Alakoz Content/Species/Player/Rebel_Animations/"; 
            string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";

            Animation Symbol = new Animation(Content.Load<Texture2D>( enemyDirectory + "ball"), 1);
            Animation None = new Animation(Content.Load<Texture2D>( playerDirectory + "Rebel_None"), 1);

            Animation idle = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Idle"), 40);
            
            Animation runStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_RunStart"), 10, false);
            Animation run = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Run"), 20);
            Animation runEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Runstop"), 21, false);
            
            Animation jumpStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_JumpStart"), 8, false);
            Animation jump = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Jump"), 12);
            Animation falling = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Fall"), 12);
            
            Animation crouchStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchStart"), 4, false);
            Animation crouch = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Crouch"), 24);
            Animation crouchEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_CrouchEnd"), 6);       
            
            Animation dashStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_DashStart"), 6, false);
            Animation dash = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Dash"), 12);
            Animation dashEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_DashEnd"), 14, false, 0.012f); 

            Animation wallCling = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_WallCling"), 20);
            Animation walljumpStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_WallJumpStart"), 1); 
            Animation walljump = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_WallJump"), 20, false);      

            Animation ballStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BallStart"), 4, false);
            Animation ball = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Ball"), 12, true, 0.012f);
            Animation ballEnd = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BallEnd"), 2, false);

            Animation basicAttack1 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack1"), 20, false);
            Animation basicAttack2 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack2"), 22, false);
            Animation basicAttack3 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack3"), 23, false);
            Animation basicAttack4 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack4"), 23, false);

            Animation airAttack1 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack1"), 20, false);
            Animation airAttack2 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack2"), 22, false);
            Animation airAttack3 = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_BasicAttack3"), 23, false);

            Animation teleportToGround = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_TeleportToGround"), 14, false);
            Animation teleportToAir = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_TeleportToAir"), 4, false);


            Animation hitStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_HitStart"), 8, false);
            Animation hit = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Hit"), 16);

            Animation doorEnter = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_DoorEnter"), 34, false);

            PlayerAnimations = new Dictionary<StateType, Animation>
            {
                { StateType.SYMBOL, Symbol },
                { StateType.NONE, None },
                { StateType.IDLE, idle },
                // Run
                { StateType.RUN, run },
                { StateType.RUNSTART, runStart },
                { StateType.RUNEND, runEnd },
                // Jump and Fall
                { StateType.JUMPSTART, jumpStart },
                { StateType.JUMP, jump },
                { StateType.FALL, falling },
                // Crouch
                { StateType.CROUCH, crouch },
                { StateType.CROUCHSTART, crouchStart },
                { StateType.CROUCHEND, crouchEnd },
                // Walljump
                { StateType.WALLCLING, wallCling },
                { StateType.WALLJUMPSTART, walljumpStart },
                { StateType.WALLJUMP, walljump },
                // Dash
                { StateType.DASH, dash },
                { StateType.DASHSTART, dashStart },
                { StateType.DASHEND, dashEnd },
                

                // Attacking
                { StateType.BASICATTACK1, basicAttack1 },
                { StateType.BASICATTACK2, basicAttack2 },
                { StateType.BASICATTACK3, basicAttack3 },
                { StateType.BASICATTACK4, basicAttack4 },

                { StateType.AIRATTACK1, airAttack1 },
                { StateType.AIRATTACK2, airAttack2 },
                { StateType.AIRATTACK3, airAttack3 },

                // Damaged
                { StateType.HIT, hit },
                { StateType.HITSTART, hitStart },
                { StateType.DOORENTER, doorEnter },

                // Misc
                { StateType.TOGROUND, teleportToGround },
                { StateType.TOAIR, teleportToAir},
                { StateType.BALL, ball },
                { StateType.BALLSTART, ballStart },
                { StateType.BALLEND, ballEnd },
            };
        }

        public static void LoadEnemyAssets()
        {
            string enemyDirectory = "Alakoz Content/Species/Player/Base_Animations/";
            string effectDirectory = "Alakoz Content/Effects/General/";
            string playerDirectory = "Alakoz Content/Species/Player/Rebel_Animations/";

            
            Animation Symbol = new Animation(Content.Load<Texture2D>( enemyDirectory + "ball"), 1);
            Animation SymbolNonLooping = new Animation(Content.Load<Texture2D>( enemyDirectory + "ball"), 1, false);
            Animation playerHurtbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation playerHitbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            Animation idle = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Idle"), 32);
            Animation idle2 = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Idle2"), 8);
            Animation run = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Running"), 22);
            Animation runEnd = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_RunStop"), 24, false);
            Animation turnaround = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Turnaround"), 12);
            Animation jump = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Jump"), 10, false);
            Animation falling = new Animation(Content.Load<Texture2D>( enemyDirectory + "Base_Falling"), 12);

            Animation hitStart = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_HitStart"), 8, false);
            Animation hit = new Animation(Content.Load<Texture2D>(playerDirectory + "Rebel_Hit"), 16);

            EnemyAnimations = new Dictionary<StateType, Animation>
            {
                { StateType.SYMBOL, Symbol },
                { StateType.NONE, Symbol },
                { StateType.HURTBOX, playerHurtbox },
                { StateType.HITBOX, playerHitbox },
                { StateType.IDLE, idle }, // Idle
                // Running
                { StateType.RUN, run }, 
                { StateType.RUNEND, runEnd },
                // Jumping 
                { StateType.JUMPSTART, Symbol },
                { StateType.JUMP, jump }, 
                { StateType.FALL, falling },

                { StateType.CROUCH, Symbol }, // Crouching
                { StateType.CROUCHSTART, SymbolNonLooping },
                { StateType.CROUCHEND, SymbolNonLooping },

                { StateType.DASH, Symbol }, // Dashing
                { StateType.DASHSTART, SymbolNonLooping },
                { StateType.DASHEND, SymbolNonLooping },

                { StateType.BALL, Symbol }, // Ball Spin
                { StateType.BALLSTART, SymbolNonLooping },
                { StateType.BALLEND, SymbolNonLooping },
                
                { StateType.HIT, hit }, // Hit
                { StateType.HITSTART, hitStart }
            };
        }

        // ---------- Map Objects---------- //
        public static void LoadDoorAssets()
        {
            // Load the animations for the Door

            string doorDirectory = "Alakoz Content/Maps/MapObjects/Doors/Door - Small/";
            string effectDirectory = "Alakoz Content/Effects/General/";
            Animation Hurtbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation Hitbox = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            Animation open = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_Open"), 1);                
            Animation openStart = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_OpenStart"), 9, false);
            
            Animation close = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_Close"), 1);
            Animation closeStart = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_CloseStart"), 10, false);
            
            Animation idle = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_Idle"), 38, false);
            Animation unlock = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_Unlock"), 30, false);
            Animation fadeIn = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_FadeIn"), 11, false);
            Animation fadeOut = new Animation(Content.Load<Texture2D>(doorDirectory + "SmallDoor_FadeOut"), 10, false);

            // Animations for the Door
            DoorAnimations = new Dictionary<StateType, Animation>
            {
                {StateType.ACTIVE, Hitbox},
                {StateType.ACTIVESTART, Hitbox},
                {StateType.INACTIVE, Hurtbox}, 
                {StateType.INACTIVESTART, Hurtbox},

                {StateType.OPEN, open},
                {StateType.OPENSTART, openStart},
                {StateType.CLOSE, close},
                {StateType.CLOSESTART, closeStart},
                {StateType.IDLE, idle},
                {StateType.UNLOCK, unlock},
                {StateType.FADEIN, fadeIn},
                {StateType.FADEOUT, fadeOut},
            };
        }

        // ---------- Backgrounds ---------- //
        // ---------- Effects ---------- //
        // ---------- Sounds ---------- //
        // ---------- Debug ---------- //
        public static void LoadDebug()
        {
            string effectDirectory = "Alakoz Content/Effects/General/";
            
            Animation hurtboxSprite = new Animation(Content.Load<Texture2D>( effectDirectory + "Hurtbox"), 1);
            Animation hitboxSprite = new Animation(Content.Load<Texture2D>( effectDirectory + "Hitbox"), 1);

            CollisionAnimations = new Dictionary<CollisionType, Animation>
            {
                { CollisionType.HURTBOX, hurtboxSprite },
                { CollisionType.ENEMYHURTBOX, hurtboxSprite },
                { CollisionType.HITBOX, hitboxSprite }
            };
        }
    }
    
    
    // Functions to load different GameObjects and retrieve their Assests



}