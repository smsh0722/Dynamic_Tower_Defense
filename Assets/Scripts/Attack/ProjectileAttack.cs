using System;
using UnityEngine;

public class ProjectileAttack : AttackSystem
{
    
    // Methods
    public ProjectileAttack(Tower tower)
    : base(tower)
    {
    }

    public override void Tick(float dt)
    {
        mCooltime -= dt;
    }

    public override bool TryAttack(Enemy target)
    {
        if (mCooltime > 0) return false;

        // ����ü �߻�
        if (mOwner.Material != null)
        {
            Debug.Log("����ü �߻�!\n");

            GameObject projectile = GameObject.Instantiate(mOwner.Material.mProjectilePrefab, mOwner.Position, Quaternion.identity);

            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.Init(target, mOwner.AttackDamage, mOwner.Material, mOwner.Effect);
            }
        }

        // ��Ÿ�� �ʱ�ȭ
        mCooltime = 1f / mOwner.AttackSpeed;

        return true;
    }

    public override bool CanAttack()
    {
        return (mCooltime <= 0);
    }



}