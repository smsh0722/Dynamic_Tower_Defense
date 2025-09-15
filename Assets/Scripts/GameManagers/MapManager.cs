using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public class MapNode
    {
        public bool bWalkable;
        public bool bPlaceable;
        public bool bIsDefenseArea;
        public int cost;

        public MapNode(bool isWalkable = true, bool isPlaceable = true, bool isDefenseArea = false, int cost = 0)
        {
            this.bWalkable = isWalkable;
            this.bPlaceable = isPlaceable;
            this.bIsDefenseArea = isDefenseArea;
            this.cost = cost;
        }
    }

    // 맵의 크기
    public const int MapWidth = 40;
    public const int MapHeight = 40;

    // Properties
    public static MapManager Instance { get; private set; }

    // NOTE:
    // KEY: Grid coordinate
    // VALUE: map 정보
    private Dictionary<Vector3Int, MapNode> mapData = new Dictionary<Vector3Int, MapNode>();

    [Header("Grid / Tilemap refs")]
        // Hierarchy의 Grid 할당
    [SerializeField] private Grid mGrid;
        // 타일을 그린 Tilemap 할당
    [SerializeField] private Tilemap mGroundTileMap;  
        // 타일 위 물체 Tilemap
    [SerializeField] private Tilemap mOnGroundTileMap; 

    [Header("Optional fine-tune")]
        // 미세조정용
    [SerializeField] private Vector3 placementOffset = Vector3.zero;

    // Flow Field 용
    private List<Vector3Int> mSpawnTilePos = new List<Vector3Int>();
    private Vector3Int mDstTilePos = new Vector3Int();
        // Index: ([-x, x], [-y,y])
    private Grid2DArray<Vector3> mFlowField = new Grid2DArray<Vector3>( -MapWidth, MapWidth, -MapHeight, MapHeight, Vector3.zero);

    [Header("Cell HightLight")]
    [SerializeField] private Color mHighlightColor = new Color(1f, 1f, 1f, 1f);
    private Vector3Int mHighlightedCell;

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

        GenerateMapData();

        MakeFlowField(mSpawnTilePos, mDstTilePos);
    }

    private void GenerateMapData()
    {
        for (int x = -MapWidth; x <= MapWidth; x++)
        {
            for (int y = -MapHeight; y <= MapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                TileData groundTile = (TileData)mGroundTileMap.GetTile(position);
                TileData onGroundTile = (TileData)mOnGroundTileMap.GetTile(position);

                bool argWalkable = false;
                bool argPlacable = false;
                bool argDefenseArea = false;
                int cost = 0;
                if (groundTile != null)
                {
                    argWalkable = groundTile.bWalkable;
                    argPlacable = groundTile.bPlaceable;
                    argDefenseArea = groundTile.bIsDefenseArea;
                    if ( groundTile.bIsDefenseArea )
                    {
                        mDstTilePos = position;
                    }
                    if (groundTile.bIsSpawnArea)
                    {
                        mSpawnTilePos.Add( position );
                    }
                    cost += groundTile.Cost;
                }
                if (onGroundTile != null) 
                {
                    argWalkable &= onGroundTile.bWalkable;
                    argPlacable &= onGroundTile.bPlaceable;
                    argDefenseArea &= onGroundTile.bIsDefenseArea;
                    cost += onGroundTile.Cost;
                }

                mapData.Add(position, new MapNode (argWalkable, argPlacable, argDefenseArea, cost) );
            }
        }

        Debug.Log("맵 데이터 생성이 완료되었습니다.");
    }

    public Vector3 GridToWorld(Vector3Int cell)
    {
        return mGroundTileMap.GetCellCenterWorld(cell) + placementOffset;
    }

    public Vector3Int WorldToGrid(Vector3 world) => mGroundTileMap.WorldToCell(world);

    public MapNode GetNode(Vector3Int cell)
    {
        mapData.TryGetValue(cell, out var n); return n;
    }
    
    public bool UpdateFlowField()
    {
        return MakeFlowField(mSpawnTilePos, mDstTilePos);
    }

    struct Node : IComparable<Node>
    {
        public Vector2Int pos;
        public int cost;
        public Node(Vector2Int c, int k) { pos = c; cost = k; }

        public int CompareTo(Node other) => other.cost.CompareTo(this.cost);
    }

    private void GetAdjList(Vector2Int[] adjList, Vector2Int pos )
    {
        adjList[0] = new Vector2Int(pos.x + 1, pos.y);
        adjList[1] = new Vector2Int(pos.x - 1, pos.y);
        adjList[2] = new Vector2Int(pos.x, pos.y - 1);
        adjList[3] = new Vector2Int(pos.x, pos.y + 1);
    }

    private bool MakeFlowField(List<Vector3Int> srcCells, Vector3Int dstCell)
    {
        // Init PQ
        Priority_Queue<Node> pq = new Priority_Queue<Node>();

        // Init dijkstra
        Grid2DArray<int> dijkstra = new Grid2DArray<int>(-MapWidth, MapWidth, -MapHeight, MapHeight, int.MaxValue);

        Vector2Int[] adjList = new Vector2Int[4];

        // Start Dijkstra Algorithm
        dijkstra.SetNodeAt(dstCell.x, dstCell.y, 0);
        pq.Push(new Node(((Vector2Int)dstCell), 0));
        while (pq.IsEmpty() == false)
        {
            Node top = pq.Top();
            pq.Pop();

            if (top.cost > dijkstra.GetNodeAt(top.pos.x, top.pos.y))
                continue;

            
            GetAdjList(adjList, top.pos);

            // Check Adjacency List
            for (int i = 0; i < adjList.Length; i++)
            {
                Vector2Int adjCell = adjList[i];

                // Check out of Bounds
                if (adjCell.x < -MapWidth || adjCell.x > MapWidth
                    || adjCell.y < -MapHeight || adjCell.y > MapHeight)
                    continue;

                // 걸을 수 있는 Cell인지 확인
                MapNode m = null;
                mapData.TryGetValue(new Vector3Int(adjCell.x, adjCell.y, 0), out m);
                if (m == null || m.bWalkable == false )
                    continue;

                // Get New Cost
                int newCost = top.cost + m.cost;

                // 새로운 Optimal인지 확인
                if ( newCost < dijkstra.GetNodeAt(adjCell.x, adjCell.y) )
                {
                    dijkstra.SetNodeAt(adjCell.x, adjCell.y, newCost);
                    pq.Push(new Node(adjCell, newCost));
                }
            }
        }

        // Src to dst 막힌 경로 없는 지 확인
        foreach (var srcCell in srcCells)
        {
            if (dijkstra.GetNodeAt(srcCell.x, srcCell.y) == int.MaxValue)
                return false; // enemy 경로 막힘, 경로 생성 불가능
        }

        // FlowField 
        for (int cellX = -MapWidth; cellX <= MapWidth; cellX++)
        {
            for (int cellY = -MapHeight; cellY <= MapHeight; cellY++)
            {
                // Find Min Adj Cell
                Vector2Int minCell = new Vector2Int(cellX, cellY);

                GetAdjList(adjList, new Vector2Int(cellX, cellY));
                for (int i = 0; i < adjList.Length; i++)
                {
                    Vector2Int adjCell = adjList[i];

                    // Check out of bounds
                    if (adjCell.x < -MapWidth || adjCell.x > MapWidth
                        || adjCell.y < -MapHeight || adjCell.y > MapHeight)
                        continue;

                    if ( dijkstra.GetNodeAt(minCell.x, minCell.y) > dijkstra.GetNodeAt(adjCell.x, adjCell.y) )
                        minCell = adjList[i];
                }

                Vector3 from = GridToWorld(new Vector3Int(cellX, cellY, 0));
                Vector3 to = GridToWorld(new Vector3Int(minCell.x, minCell.y, 0));
                Vector3 worldFlowVec = (to - from).normalized;
                mFlowField.SetNodeAt(cellX, cellY, worldFlowVec);
            }
        }

        Debug.Log("Flow Field 완성!");
        return true;
    }

    public Vector3 GetFlowDirection(Vector3 worldPos)
    {
        Vector3Int cellPos = WorldToGrid(worldPos);
        if (cellPos.x < -MapWidth || cellPos.x > MapWidth ||
            cellPos.y < -MapHeight || cellPos.y > MapHeight)
            return Vector3.zero;

        Vector3 flow = mFlowField.GetNodeAt(cellPos.x, cellPos.y);

        // NOTE: 모서리 타기 방지. 중앙 쪽으로 보간
        Vector3 cellCenterWorldPos = GridToWorld(cellPos);
        Vector3 centerDir = (cellCenterWorldPos - worldPos); 
        float centerWeight = 1f;

        flow = ( flow + centerWeight * centerDir).normalized; // 보간 O

        return flow;
    }

    public bool IsAtDestination(Vector3 worldPos)
    {
        Vector3Int playerCell = WorldToGrid(worldPos);
        if (playerCell.Equals(mDstTilePos))
        {
            return true;
        }

        return false;
    }

    public void HightlightCell(Vector3 worldPos)
    {
        Vector3Int cell = mGroundTileMap.WorldToCell(worldPos);

        if (cell == mHighlightedCell)
            return;

        // 기존 highlight 제거
        if (mHighlightedCell != null)
        {
            mGroundTileMap.SetTileFlags(mHighlightedCell, TileFlags.None);
            mGroundTileMap.SetColor(mHighlightedCell, Color.white);
        }

        // 새 highlight 적용
        mHighlightedCell = cell;
        if (mGroundTileMap.HasTile(mHighlightedCell))
        {
            mGroundTileMap.SetTileFlags(mHighlightedCell, TileFlags.None);
            mGroundTileMap.SetColor(mHighlightedCell, mHighlightColor);
        }
    }
}