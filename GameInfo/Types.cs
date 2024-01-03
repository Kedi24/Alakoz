




namespace Alakoz.GameInfo
{
    // ========================================== GAME OBJECT TYPES ==========================================
    public enum GameObjectType
    {
        NONE,
        PLAYER,
        ENEMY,
        DOOR,
        BASICDOOR,
    }
    // ========================================== STATE TYPES ==========================================
    // Enumerators for the different states / animations. Used to compare and reference different states.
    public enum StateType
    {
        // PLAYER STATES
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
        DOWNFINISHER,
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
    public enum CollisionType
    {
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

}