using UnityEngine;

[CreateAssetMenu]
public class EffectData : ScriptableObject
{
    public string EffectName;    // �⺻, ��, ���� ��
    public float AttackSpeedBonus = 0f;      // �ӵ� Ư��: +0.5
    public float DamageBonus = 0f;           // �Ŀ� Ư��: +2.0
    public float Duration = 0f; // ���ӽð�, ��, ���� ���� ���.
    public float Rate = 0f;

    public int Cost = 100;
    public Sprite IconSprite;
}