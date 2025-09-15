using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData")]
public class TileData : Tile
{
    public int  Cost = 1; // Default 1
    public bool bWalkable = true;
    public bool bPlaceable = true;
    public bool bIsDefenseArea = false;
    public bool bIsSpawnArea = false;
}