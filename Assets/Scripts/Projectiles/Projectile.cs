using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Members
    private Enemy mTarget;
    private float mDamage;
    private MaterialData mMaterial;
    private EffectData mEffect;
    private Vector3 mPrevTargetPos;
    [SerializeField] private float mSpeed = 10f;


    // Methods
    public void Init(Enemy target, float damage, MaterialData material, EffectData effect)
    {
        mTarget = target;
        mDamage = damage;
        mMaterial = material;
        mEffect = effect;
    }

    private void Update()
    {
        if (mTarget != null)
            mPrevTargetPos = mTarget.transform.position;

        // Check hit
        if (Vector3.Distance(transform.position, mPrevTargetPos) < 0.1f)
        {
            if (mTarget != null)
            {
                mTarget.ApplyDamage(mDamage);
                ApplyEffects();
            }

            // TODO: hit 이미지 보여주기

            Destroy(gameObject);
            return;
        }

        // Move to target
        MoveTo(mPrevTargetPos);
    }

    private void ApplyEffects()
    {
        if (mEffect == null) return;

        mTarget.AddCondition(mEffect);
    }

    private void MoveTo( Vector3 targetPosition)
    {
        Vector3 direction = Vector3.Normalize(targetPosition - transform.position);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-45f, Vector3.forward);

        transform.position += direction * mSpeed * Time.deltaTime;
    }
}