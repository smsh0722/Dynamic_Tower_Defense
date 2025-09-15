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
    // 타워의 프리팹
    public GameObject prefab;
    // 건설 비용
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
            Debug.Log(mSelectedTowerBlueprint.prefab.name + " 타워 선택됨.");
        }
    }

    public void TryPlaceTower(Vector3Int gridPosition)
    {
        // 타워 선택 확인
        if (mSelectedTowerBlueprint == null)
        {
            Debug.Log("건설할 타워가 선택되지 않았습니다.");
            mSelectedTowerBlueprint = towerBlueprints[0];
            //return;
        }

        // 건설 비용 확인
        if (GameManager.Instance.CurrentMoney < mSelectedTowerBlueprint.cost)
        {
            Debug.Log("돈이 부족합니다!");
            UIManager.Instance.NotifyMessage("Not enough Gold!");
            return;
        }

        // 건설이 가능한지 MapManager에 확인
        MapNode node = MapManager.Instance.GetNode(gridPosition);
        if (node == null) 
        {
            Debug.Log("노드가 존재하지 않습니다.");
            return;
        } 
        if ( !node.bPlaceable )
        {
            Debug.Log("해당 위치에는 타워를 건설할 수 없습니다.");
            UIManager.Instance.NotifyMessage("Can not place at here!");
            return;
        }

        // 그리드 좌표를 실제 월드 좌표로 변환
        // Vector3 worldPosition = MapManager.Instance.ConvertGridToWorld(gridPosition);
        Vector3 worldPosition = MapManager.Instance.GridToWorld(gridPosition);

        // 맵 데이터 업데이트
        bool bPlaceableOriginal = node.bPlaceable;
        bool bWalkableOriginal = node.bWalkable;
        node.bPlaceable = false;
        node.bWalkable = false;

        // Flow Filed 갱신
        bool bIsAllowed = MapManager.Instance.UpdateFlowField();
        if ( bIsAllowed == false)
        {
            node.bPlaceable = bPlaceableOriginal;
            node.bWalkable = bWalkableOriginal;
            UIManager.Instance.NotifyMessage("Cannot block enemy path!");
            return;
        }

        // 비용 차감
        GameManager.Instance.DeductMoney(mSelectedTowerBlueprint.cost);

        // 프리팹으로부터 타워 인스턴스 생성
        GameObject towerObject = Instantiate(mSelectedTowerBlueprint.prefab, worldPosition + mTowerOffset, Quaternion.identity);
        Tower newTower = towerObject.GetComponent<Tower>();
        mActiveTowers.Add(gridPosition, newTower); // 관리 목록에 추가

        Debug.Log(gridPosition + " 위치에 타워 건설 완료!");

        // 선택된 타워 초기화
        mSelectedTowerBlueprint = null;
    }

    public bool TryGetTower(Vector3Int cell, out Tower tower)
        => mActiveTowers.TryGetValue(cell, out tower);
}