using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundTilemap : MonoBehaviour
{
    public static GroundTilemap Instance { get; private set; }

    static readonly Vector3Int[] directions = {
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0)
    };

    [field: Header("Refs")]
    [field: SerializeField] public Tilemap Tilemap { get; private set; }

    [field: Header("Tiles List")]
    [field: SerializeField] public List<GroundTile> GroundTiles { get; private set; } = new List<GroundTile>();


    [field: Header("Ground Tile Map")]
    [field: SerializeField] public Dictionary<Vector3Int, GroundTile> GroundTileMapping { get; private set; } = new Dictionary<Vector3Int, GroundTile>();

    public void BuildGraph()
    {
        GroundTiles.Clear();
        GroundTileMapping.Clear();

        // Build TileNode graph
        BoundsInt bounds = Tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (Tilemap.HasTile(pos))
            {
                var worldPos = Tilemap.CellToWorld(pos) + Tilemap.tileAnchor;
                var tile = new GroundTile(pos, worldPos);
                GroundTiles.Add(tile);
                GroundTileMapping[pos] = tile;
            }
        }

        // Connect neighbors
        foreach (var tile in GroundTiles)
        {
            foreach (var dir in directions)
            {
                Vector3Int neighborPos = tile.cell + dir;
                if (GroundTileMapping.TryGetValue(neighborPos, out var neighborNode))
                {
                    tile.neighbors.Add(neighborPos);
                }
            }
        }
    }

    public GroundTile GetGroundTile(Vector3Int cell) =>
    GroundTileMapping.TryGetValue(cell, out var tile) ? tile : null;

    void OccupyGroundTiles()
    {
        foreach (var groundTile in GroundTiles)
        {
            groundTile.isOccupied = false;
        }

        if (PathTilemap.Instance.PathTiles != null)
        {
            foreach (var pathTile in PathTilemap.Instance.PathTiles)
            {
                if (pathTile != null)
                {
                    if (GroundTileMapping.TryGetValue(pathTile.cell, out var groundTile))
                    {
                        groundTile.isOccupied = true;
                    }
                }
            }
        }
    }

    void ProcessMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = Tilemap.WorldToCell(mouseWorld);

            if (Tilemap.HasTile(cellPos))
            {
                var groundTile = GetGroundTile(cellPos);
                if (groundTile != null && !groundTile.isOccupied && GameManager.Instance.CanBuyTower)
                {
                    if (GameManager.Instance.BuyTower(groundTile.position)) groundTile.isOccupied = true;
                }
            }
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (Tilemap == null) Tilemap = GetComponent<Tilemap>();
        BuildGraph();
    }

    void Start()
    {
        OccupyGroundTiles();
    }

    void Update()
    {
        ProcessMouseInput();
    }
}
