using UnityEngine;

[CreateAssetMenu]
public class MaterialData : ScriptableObject
{
    public string materialName;
    public float attackSpeedMultiplier = 1f;  // Ã¶: 1.0, Àº: 1.5, ¸¶¹ý¼®: 0.5
    public float damageMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public int cost = 100;
    public Sprite iconSprite;
    public GameObject mProjectilePrefab;
}