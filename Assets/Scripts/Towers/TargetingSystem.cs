using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TargetingSystem
{
    public TargetingSystem( Tower tower ) => mOwner = tower;

    public Enemy GetClosestTarget()
    {
        // Physics2D.OverlapCircle 등으로 적 탐색
        if (mOwner == null) return null;

        float range = mOwner.AttackRange;
        Vector2 towerPosition = mOwner.transform.position;
        Enemy target = null;
        {
            List<Enemy> enemies = new List<Enemy>();

            Collider2D[] colliders = Physics2D.OverlapCircleAll(towerPosition, range);
            foreach (var collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.Alive)
                {
                    enemies.Add(enemy);
                }
            }

            float minDist = float.MaxValue;
            foreach( var enemy in enemies )
            {
                float dist = Vector2.Distance( towerPosition, enemy.Position );
                if (dist < minDist)
                {
                    minDist = dist;
                    target = enemy;
                }
            }
        }

        return target;
    }

    protected Tower mOwner;
}