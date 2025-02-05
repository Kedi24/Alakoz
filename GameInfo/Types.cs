namespace Alakoz.GameInfo
{
    // ========================================== GAME OBJECT TYPES ==========================================
    public enum TObject{
        NULL,
        NONE,
        PLAYER,
        ENEMY,
        DOOR,
        BASICDOOR,
    }
   
    // ========================================== STATE TYPES ==========================================
    // Enumerators for the different states / animations. Used to compare and reference different states.
    public enum TState{
        NULL,
        NONE,
        IDLE,
        AIR,
        FALL,
        RUN,
        RUNSTART,
        RUNEND,
        CROUCH,
        CROUCHSTART,
        CROUCHEND,
        JUMP,
        JUMPSTART,
        JUMPEND,
        WALLJUMP,
        WALLJUMPSTART,
        WALLCLING,
        BALL,
        BALLSTART,
        BALLEND,
        DASH,
        DASHSTART,
        DASHEND,
        // ----- Attacks
        ATTACK,
        BASICATTACK1,
        BASICATTACK2,
        BASICATTACK3,
        BASICATTACK4,
        AIRATTACK1,
        AIRATTACK2,
        AIRATTACK3,
        UPFINISHER,
        DOWNFINISHERSTART,
        DOWNFINISHER,
        DOWNFINISHERGROUND,
        DOWNFINISHEREND,
        FRONTFINISHER,
        BACKFINISHER,
        DOORENTER,
        
        // Misc
        TOGROUND,
        TOAIR,
        SKILL,
        HIT,
        HITSTART,

        SYMBOL, // Placeholder for unimplemented animations
        HURTBOX, // Just for convenience, will delete later
        HITBOX, // Just for convenience, will delete later
        
        // ----- DOORS
        INTERACT,
        ACTIVE,
        ACTIVESTART,
        INACTIVE,
        INACTIVESTART,
        OPEN,
        OPENSTART,
        CLOSE,
        CLOSESTART,
        FADEIN,
        FADEOUT,
        UNLOCK,
    }

    // ========================================== COLLISION TYPES ==========================================
    // Enumerators for static and dynamic collision objects
    public enum TCollision{
        // ------------ STATIC
        HURTBOX,
        HITBOX,
        GROUND,
        PLATFORM,
        ENEMYHURTBOX,

        // ------------ DYNAMIC
        PLAYERSPAWN,
        ENEMYSPAWN,
        DOOR,
    }
    public enum THitstop{
        // -------------- CONSTANTS
        LIGHT = 1,
        LIGHT2 = 3,
        MEDIUM = 10,
        HEAVY = 18,
        SUPERHEAVY = 24,
        KILL = 16,
    }
    public enum TShape{
        RECTANGLE, 
        CIRCLE,
        TRIANGLE,
    }
}