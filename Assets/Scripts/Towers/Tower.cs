using UnityEngine;
using UnityEngine.Timeline;

public class Tower : MonoBehaviour
{
    // Member variables
    private TargetingSystem mTargeting;
    private AttackSystem mAttack;

    [SerializeField] private float mRange = 5f;
    [SerializeField] private float mBaseAttackSpeed = 10f;    // 기본 타워 스펙
    [SerializeField] private float mBaseAttackDamage = 1f;

    [SerializeField] private MaterialData mMaterial;
    [SerializeField] private EffectData mEffect;

    [Header("TowerMenu")]
    [SerializeField] private TowerMenuController mMenuPrefab;

    //[Header("투사체 설정")]
    //[SerializeField] private GameObject mProjectilePrefab;

    // Properties
    public string TowerName { get; set; }
    public int Cost { get; set; }
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    public float AttackSpeed =>
        mBaseAttackSpeed
        * mMaterial.attackSpeedMultiplier
        + mEffect.AttackSpeedBonus;
    public float AttackRange =>
        mRange
        * mMaterial.rangeMultiplier;
    public float AttackDamage =>
        mBaseAttackDamage
        * mMaterial.damageMultiplier
        + mEffect.DamageBonus;
    public MaterialData Material { get => mMaterial; }
    public EffectData Effect { get => mEffect; }

    // Methods
    public void OnClick()
    {
        SelectionManager.Instance.ShowMenuForTower(this);
    }
    // 메뉴에서 콜백 받음
    public void ApplyMaterial(MaterialData data)
    {
        mMaterial = data;
    }

    public void ApplyEffect(EffectData effect)
    {
        mEffect = effect;
    }

    private void Update()
    {
        // 공격
        mAttack.Tick(Time.deltaTime);
        if (mAttack.CanAttack())
        {
            Enemy target = mTargeting.GetClosestTarget();
            if (target != null)
            {
                mAttack.TryAttack(target);
            }
        }
    }

    private void Awake()
    {
        mTargeting = new TargetingSystem(this);
        mAttack = new ProjectileAttack(this);
    }
}