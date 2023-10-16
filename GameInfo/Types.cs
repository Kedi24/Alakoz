




namespace Alakoz.GameInfo
{
    // ========================================== STATE ==========================================
    // Enumerators for the different states / animations. Used to compare and reference different states.
    public enum StateType
    {
        // PLAYER STATES
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
        BALL,
        BALLSTART,
        BALLEND,
        DASH,
        DASHSTART,
        DASHEND,
        ATTACK,
        SKILL,
        HIT,
        HITSTART,
        SYMBOL, // Placeholder for unimplemented animations
        HURTBOX, // Just for convenience, will delete later
        HITBOX, // Just for convenience, will delete later
    }

    // ========================================== COLLISION ==========================================
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