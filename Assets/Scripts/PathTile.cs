using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PathTile
{
    public Vector3Int cell;
    public Vector3 position;
    public List<Vector3Int> neighbors = new List<Vector3Int>();

    public PathTile(Vector3Int cell, Vector3 worldPos)
    {
        this.cell = cell;
        this.position = worldPos;
    }
}
