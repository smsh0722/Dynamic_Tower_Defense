// TowerManager.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static MapManager;
using static UIManager;
using static Unity.Collections.AllocatorManager;

[System.Serializable]
public class TowerBlueprint
{
    // Ÿ���� ������
    public GameObject prefab;
    // �Ǽ� ���
    public int cost;          
}

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    [Header("Tower Blueprints")]
    public List<TowerBlueprint> towerBlueprints;

    private TowerBlueprint mSelectedTowerBlueprint;
    
    private Dictionary<Vector3Int, Tower> mActiveTowers = new Dictionary<Vector3Int, Tower>();

    [SerializeField] private Vector3 mTowerOffset = new Vector3(0f, -0.06f, 0f);


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SelectTowerToBuild(int index)
    {
        if (index >= 0 && index < towerBlueprints.Count)
        {
            mSelectedTowerBlueprint = towerBlueprints[index];
            Debug.Log(mSelectedTowerBlueprint.prefab.name + " Ÿ�� ���õ�.");
        }
    }

    public void TryPlaceTower(Vector3Int gridPosition)
    {
        // Ÿ�� ���� Ȯ��
        if (mSelectedTowerBlueprint == null)
        {
            Debug.Log("�Ǽ��� Ÿ���� ���õ��� �ʾҽ��ϴ�.");
            mSelectedTowerBlueprint = towerBlueprints[0];
            //return;
        }

        // �Ǽ� ��� Ȯ��
        if (GameManager.Instance.CurrentMoney < mSelectedTowerBlueprint.cost)
        {
            Debug.Log("���� �����մϴ�!");
            UIManager.Instance.NotifyMessage("Not enough Gold!");
            return;
        }

        // �Ǽ��� �������� MapManager�� Ȯ��
        MapNode node = MapManager.Instance.GetNode(gridPosition);
        if (node == null) 
        {
            Debug.Log("��尡 �������� �ʽ��ϴ�.");
            return;
        } 
        if ( !node.bPlaceable )
        {
            Debug.Log("�ش� ��ġ���� Ÿ���� �Ǽ��� �� �����ϴ�.");
            UIManager.Instance.NotifyMessage("Can not place at here!");
            return;
        }

        // �׸��� ��ǥ�� ���� ���� ��ǥ�� ��ȯ
        // Vector3 worldPosition = MapManager.Instance.ConvertGridToWorld(gridPosition);
        Vector3 worldPosition = MapManager.Instance.GridToWorld(gridPosition);

        // �� ������ ������Ʈ
        bool bPlaceableOriginal = node.bPlaceable;
        bool bWalkableOriginal = node.bWalkable;
        node.bPlaceable = false;
        node.bWalkable = false;

        // Flow Filed ����
        bool bIsAllowed = MapManager.Instance.UpdateFlowField();
        if ( bIsAllowed == false)
        {
            node.bPlaceable = bPlaceableOriginal;
            node.bWalkable = bWalkableOriginal;
            UIManager.Instance.NotifyMessage("Cannot block enemy path!");
            return;
        }

        // ��� ����
        GameManager.Instance.DeductMoney(mSelectedTowerBlueprint.cost);

        // ���������κ��� Ÿ�� �ν��Ͻ� ����
        GameObject towerObject = Instantiate(mSelectedTowerBlueprint.prefab, worldPosition + mTowerOffset, Quaternion.identity);
        Tower newTower = towerObject.GetComponent<Tower>();
        mActiveTowers.Add(gridPosition, newTower); // ���� ��Ͽ� �߰�

        Debug.Log(gridPosition + " ��ġ�� Ÿ�� �Ǽ� �Ϸ�!");

        // ���õ� Ÿ�� �ʱ�ȭ
        mSelectedTowerBlueprint = null;
    }

    public bool TryGetTower(Vector3Int cell, out Tower tower)
        => mActiveTowers.TryGetValue(cell, out tower);
}