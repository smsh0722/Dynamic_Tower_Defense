using UnityEngine;
using UnityEngine.Timeline;

public class Tower : MonoBehaviour
{
    // Member variables
    private TargetingSystem mTargeting;
    private AttackSystem mAttack;

    [SerializeField] private float mRange = 5f;
    [SerializeField] private float mBaseAttackSpeed = 10f;    // �⺻ Ÿ�� ����
    [SerializeField] private float mBaseAttackDamage = 1f;

    [SerializeField] private MaterialData mMaterial;
    [SerializeField] private EffectData mEffect;

    [Header("TowerMenu")]
    [SerializeField] private TowerMenuController mMenuPrefab;

    //[Header("����ü ����")]
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
    // �޴����� �ݹ� ����
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
        // ����
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