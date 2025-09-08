using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GroundTile
{
    public Vector3Int cell;
    public Vector3 position;
    public bool isOccupied;
    public List<Vector3Int> neighbors = new List<Vector3Int>();

    public GroundTile(Vector3Int cell, Vector3 worldPos, bool occupied = false)
    {
        this.cell = cell;
        this.position = worldPos;
        this.isOccupied = occupied;
    }
}
