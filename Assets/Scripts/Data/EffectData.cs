using UnityEngine;

[CreateAssetMenu]
public class EffectData : ScriptableObject
{
    public string EffectName;    // 기본, 독, 빙결 등
    public float AttackSpeedBonus = 0f;      // 속도 특성: +0.5
    public float DamageBonus = 0f;           // 파워 특성: +2.0
    public float Duration = 0f; // 지속시간, 독, 빙결 같은 경우.
    public float Rate = 0f;

    public int Cost = 100;
    public Sprite IconSprite;
}