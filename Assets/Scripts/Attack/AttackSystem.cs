using UnityEngine;

public abstract class AttackSystem
{
    protected Tower mOwner;
    protected float mCooltime; // sec

    // Methods
    public AttackSystem(Tower tower )
    {
        mOwner = tower;
        mCooltime = 0.0f;
    }

    public abstract void Tick(float dt);
    public abstract bool TryAttack(Enemy target);
    public abstract bool CanAttack();


}