using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public enum eCondition 
{
    Default = 0,
    Iced = 1<<0,
    Poisoned = 1<<1,
};

public class Enemy : MonoBehaviour
{
    // Members
    [SerializeField] private float mMaxHP = 10.0f;
    private float mCurrentHP = 10.0f;

    [SerializeField] private float mMoveSpeed = 2.0f;
    [SerializeField] private float mMoveMultiplier = 1.0f;

    [SerializeField] private int mReward = 10;

    [SerializeField] private int mDamageToPlayer = 1;

    private eCondition mConditions = eCondition.Default;
    private Dictionary<eCondition, float> mDurationOfConditions = new();
    private Dictionary<eCondition, float> mCoolTimeConditions = new();

    [SerializeField] private SpriteRenderer enemyRenderer;

    [Header("UI References")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private UnityEngine.Canvas canvas;
    private HPBar hpBarInstance;

    // Properties
    private bool mbIsAlive = true;
    public bool Alive
    {
        get => mbIsAlive;
        set => mbIsAlive = value;
    }
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }



    // Methods
    public void ApplyDamage( float damage )
    {
        mCurrentHP -= damage;
        mCurrentHP = 0.0f < mCurrentHP ? mCurrentHP : 0.0f;
        
        // Update HP on UI
        hpBarInstance.UpdateHP(mCurrentHP, mMaxHP);
    }

    public void AddCondition( EffectData ef )
    {
        eCondition condition = eCondition.Default;

        if (ef.EffectName == "Poison") 
            condition = eCondition.Poisoned;
        else if (ef.EffectName == "Ice") 
            condition = eCondition.Iced;
        if (condition == eCondition.Default) return;

        // �÷��� �ø���
        mConditions |= condition;

        // ���ӽð�/��ٿ� ����
        mDurationOfConditions[condition] = ef.Duration;
        if (ef.Rate > 0f && !mCoolTimeConditions.ContainsKey(condition))
            mCoolTimeConditions[condition] = 1f / ef.Rate;
    }


    private void Awake()
    {
        mCurrentHP = mMaxHP;
    }

    private void Start()
    {
        if (canvas == null)
        {
            var go = GameObject.FindWithTag("MainCanvas");
            if (go != null) canvas = go.GetComponent<Canvas>();
        }

        GameObject hpBarObject = Instantiate(hpBarPrefab, canvas.transform);
        hpBarInstance = hpBarObject.GetComponent<HPBar>();
        hpBarInstance.Initialize(transform);
        hpBarInstance.UpdateHP(mCurrentHP, mMaxHP);
    }

    private void Update()
    {
        // Condition
        updateConditions(Time.deltaTime);
        applyConditions();

        
        // Should Die?
        if (mCurrentHP <= 0 || mbIsAlive == false)
        {
            die(true);
            return;
        }


        // Check Arrival at destination
        if ( MapManager.Instance.IsAtDestination(Position))
        {
            GameManager.Instance.ApplyDamage(mDamageToPlayer);
            
            die(false);

            return;
        }

        // �̵�
        move();
    }

    private void move()
    {
        if ( (mConditions & eCondition.Iced) > 0)
            return;

        Vector3 direction = MapManager.Instance.GetFlowDirection(Position);
        Position += direction * mMoveSpeed * mMoveMultiplier * Time.deltaTime;
    }

    private void die(bool bReward = false)
    {
        mbIsAlive = false;

        // TODO: ���� ����Ʈ

        if ( bReward==true)
        {
            GameManager.Instance.AddMoney(mReward);
        }

        if ( hpBarInstance != null) 
            Destroy(hpBarInstance.gameObject);
        Destroy(gameObject);
    }
    private void updateConditions(float dt)
    {
        var keys = new List<eCondition>(mDurationOfConditions.Keys);
        foreach (var cond in keys)
        {
            mDurationOfConditions[cond] -= dt;
            if (mDurationOfConditions[cond] <= 0f)
            {
                mDurationOfConditions.Remove(cond);
                mConditions &= ~cond; 
                mCoolTimeConditions.Remove(cond); 
                continue;
            }

            if (mCoolTimeConditions.ContainsKey(cond))
                mCoolTimeConditions[cond] -= dt;
        }

        // �׾����� �⺻������
        if (mConditions == eCondition.Default)
            enemyRenderer.color = Color.white;
    }
    private void applyConditions()
    {
        Color c = Color.white; 
        Color cIce = Color.white; 
        Color cPoison = Color.white;

        if ((mConditions & eCondition.Iced) != 0)
        {
            cIce = Color.skyBlue;
        }
        if ((mConditions & eCondition.Poisoned) != 0)
        {
            if (mCoolTimeConditions.TryGetValue(eCondition.Poisoned, out float t) && t <= 0f)
            {
                // TODO: ������, ��Ÿ�� ��������
                ApplyDamage(1f);
                mCoolTimeConditions[eCondition.Poisoned] = 1f / 2.5f;
            }

            cPoison = Color.green;
        }

        if (cIce != Color.white && cPoison != Color.white) 
            c = cIce + cPoison; 
        else if (cIce != Color.white) 
            c = cIce; 
        else if (cPoison != Color.white) 
            c = cPoison; 
        
        enemyRenderer.color = c;
    }
}