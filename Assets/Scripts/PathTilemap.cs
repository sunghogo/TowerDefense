using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class PathTilemap : MonoBehaviour
{
    public static PathTilemap Instance { get; private set; }

    static readonly Vector3Int[] directions = {
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0)
    };

    [field: Header("Refs")]
    [field: SerializeField] public Tilemap Tilemap { get; private set; }
    [field: SerializeField] public Transform endMarker { get; private set; }

    [field: Header("Tiles List")]
    [field: SerializeField] public List<PathTile> PathTiles { get; private set; } = new List<PathTile>();
    [field: SerializeField] public PathTile EndTile { get; private set; }

    [field: Header("Path Tile Map")]
    [field: SerializeField] public Dictionary<Vector3Int, PathTile> PathTileMapping { get; private set; } = new Dictionary<Vector3Int, PathTile>();

    public List<PathTile> FindShortestPath(PathTile start, PathTile end)
    {
        // Open set = frontier we still need to explore
        var openSet = new List<PathTile> { start };

        // Keeps track of where we came from
        var cameFrom = new Dictionary<PathTile, PathTile>();

        // gScore = cost from start to this node
        var gScore = new Dictionary<PathTile, float> { [start] = 0f };

        // fScore = gScore + heuristic
        var fScore = new Dictionary<PathTile, float> { [start] = Heuristic(start, end) };

        while (openSet.Count > 0)
        {
            // Find node with smallest fScore
            PathTile current = openSet[0];
            foreach (var node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < fScore[current])
                    current = node;
            }

            // Goal reached
            if (current == end)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighborCell in current.neighbors)
            {
                if (!PathTileMapping.TryGetValue(neighborCell, out var neighbor)) continue;

                float tentativeG = gScore[current] + Vector3.Distance(current.position, neighbor.position);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return new List<PathTile>();
    }

    float Heuristic(PathTile a, PathTile b)
    {
        // Manhattan distance (good for 4-directional grids)
        return Mathf.Abs(a.cell.x - b.cell.x) + Mathf.Abs(a.cell.y - b.cell.y);
    }

    List<PathTile> ReconstructPath(Dictionary<PathTile, PathTile> cameFrom, PathTile current)
    {
        var totalPath = new List<PathTile> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    public void BuildGraph()
    {
        PathTiles.Clear();
        PathTileMapping.Clear();

        // Build TileNode graph
        BoundsInt bounds = Tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (Tilemap.HasTile(pos))
            {
                var worldPos = Tilemap.CellToWorld(pos) + Tilemap.tileAnchor;
                var tile = new PathTile(pos, worldPos);
                PathTiles.Add(tile);
                PathTileMapping[pos] = tile;

                if (endMarker != null && Tilemap.WorldToCell(endMarker.position) == pos)
                {
                    EndTile = tile;
                }
            }
        }

        // Connect neighbors
        foreach (var tile in PathTiles)
        {
            foreach (var dir in directions)
            {
                Vector3Int neighborPos = tile.cell + dir;
                if (PathTileMapping.TryGetValue(neighborPos, out var neighborNode))
                {
                    tile.neighbors.Add(neighborPos);
                }
            }
        }
    }

    public PathTile GetPathTile(Vector3Int cell) =>
    PathTileMapping.TryGetValue(cell, out var tile) ? tile : null;

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
}
