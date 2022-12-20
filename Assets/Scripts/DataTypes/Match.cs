using System.Collections.Generic;
using UnityEngine;

public class Match
{
    public readonly List<GameObject> tiles = new List<GameObject>();

    public void AddTile(GameObject tile)
    {
        tiles.Add(tile);
    }
}
