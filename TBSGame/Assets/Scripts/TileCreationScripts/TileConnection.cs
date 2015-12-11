using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileConnection {
    // Determines if the tiles are connected
    [SerializeField] private bool connectedToTile;

    // Tile that this connection refers to
   [SerializeField] private GameObject connectedTile;

    // Properties
    public bool ConnectedToTile
    { get { return connectedToTile; } set { connectedToTile = value; } }

    public GameObject ConnectedTile
    { get { return connectedTile; } set { connectedTile = value; } }

    public TileConnection(bool connectedToTile = true, bool twoWayConnection = true, GameObject connectedTile = null)
    {
        this.connectedToTile = connectedToTile;
        this.connectedTile = connectedTile;
    }
}
