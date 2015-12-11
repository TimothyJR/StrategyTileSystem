using UnityEngine;
using System.Collections;

[ExecuteInEditMode()][System.Serializable]
public class Tile : MonoBehaviour {

	private GameObject tileManager;

    public GameObject TileManager
    { get { return tileManager; } set { tileManager = value; } }

    #region Connections
    // Create tile connections
    [SerializeField] private TileConnection left = new TileConnection(true, true, null);
    [SerializeField] private TileConnection up = new TileConnection(true, true, null);
    [SerializeField] private TileConnection right = new TileConnection(true, true, null);
    [SerializeField] private TileConnection down = new TileConnection(true, true, null);


    public TileConnection Left
    { get { return left; } set { left = value; } }
    public TileConnection Up
    { get { return up; } set { up = value; } }
    public TileConnection Right
    { get { return right; } set { right = value; } }
    public TileConnection Down
    { get { return down; } set { down = value; } }

	#endregion

	#region Stairs
	// Toggles if the tile is a staircase or not
	[SerializeField] private bool stairs = false;
	public enum StairDirections
	{
		left,
		up,
		right,
		down
	};
	[SerializeField] private StairDirections stairDirection = StairDirections.left;

    // Properties for stairs
    public bool Stairs
    { get { return stairs; } set { stairs = value; } }
    public StairDirections StairDirection
    { get { return stairDirection; } set { stairDirection = value; } }
    #endregion

    #region Tile Occupation
    [SerializeField] private bool occupied = false;
    public enum Occupation
    {
        ally,
        enemy,
        objects
    };
    [SerializeField] private Occupation tileOccupation = Occupation.ally;
    [SerializeField] private GameObject occupyingObject;
    [SerializeField] private Vector3 objectRotation;
    [SerializeField] private GameObject instantiatedOccupuyingObject;

    // Properties for occupation
    public bool Occupied
    { get { return occupied; } set { occupied = value; } }
    public Occupation TileOccupation
    { get { return tileOccupation; } set { tileOccupation = value; } }
    public GameObject OccupyingObject
    { get { return occupyingObject; } set { occupyingObject = value; } }
    public Vector3 ObjectRotation
    { get { return objectRotation; } set { objectRotation = value; } }
    public GameObject InstantiatedOccupyingObject
    { get { return instantiatedOccupuyingObject; } set { instantiatedOccupuyingObject = value; } }
    #endregion
    [SerializeField] private bool disabled = false;
    [SerializeField] private int heightLevel = 1;

    // Property for disabled
    public bool Disabled
    { get { return disabled; } set { disabled = value; } }

    // Property for height level
    public int HeightLevel
    { get { return heightLevel; } set { heightLevel = value; } }

    // Tile type
    [SerializeField] private GameObject tileType;
    public enum TileTerrain
    {
        original,
        grass,
        dirt,
        city
    }

    [SerializeField] private TileTerrain tileTerrain = TileTerrain.original;

    public TileTerrain Terrain
    { get { return tileTerrain; } set { tileTerrain = value; } }

    // Use this for initialization
    void Start () {
        this.tileManager = this.transform.parent.gameObject;
    }
	
	// Update is called once per frame
	void Update () {

	}

    #region Set up tile map

    // Destroys the tile
    public void RemoveTile()
    {
        DestroyImmediate(gameObject);
    }

    // Move the tile up or down to the proper height
	public void ChangeTileHeight()
	{
        if (heightLevel < 1)
        {
            heightLevel = 1;
        }
        transform.position = new Vector3(transform.position.x, (heightLevel - 1) * 2, transform.position.z);
	}

    // Rotate tiles to be tilted up towards the direction of the stairs
	public void Staircase()
	{
        if(heightLevel < 1)
        {
            heightLevel = 1;
        }
		if(stairs)
		{
			transform.position = new Vector3(transform.position.x, (heightLevel - 0.5f) * 2, transform.position.z);
			switch(stairDirection)
			{
			case StairDirections.left:
				transform.rotation = Quaternion.Euler(new Vector3(0,0,-45));
                break;
			case StairDirections.up:
				transform.rotation = Quaternion.Euler(new Vector3(-45,0,0));
                break;
			case StairDirections.right:
				transform.rotation = Quaternion.Euler(new Vector3(0,0,45));
                break;
			case StairDirections.down:
				transform.rotation = Quaternion.Euler(new Vector3(45,0,0));
                break;
			}
		}
		else
		{
			ChangeTileHeight ();
			transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
		}

	}

    public void CreateObject(bool overWriteGameObject)
    {
        if (occupyingObject != null)
        {
            if (overWriteGameObject)
            {
                if (instantiatedOccupuyingObject != null)
                {
                    DestroyImmediate(instantiatedOccupuyingObject);
                }
                instantiatedOccupuyingObject = (GameObject)Instantiate(occupyingObject, new Vector3(this.transform.position.x, this.transform.position.y + 0.35f, this.transform.position.z), Quaternion.Euler(objectRotation));
                instantiatedOccupuyingObject.transform.parent = this.transform;
            }
            if (instantiatedOccupuyingObject != null)
            {
                instantiatedOccupuyingObject.transform.rotation = Quaternion.Euler(objectRotation);
            }
        }
    }

    public void DestroyObject()
    {
        if(instantiatedOccupuyingObject != null)
        {
            DestroyImmediate(instantiatedOccupuyingObject);
        }
    }

    public void setTileType()
    {
        Transform t = this.transform;
        if(tileType != null)
        {
            DestroyImmediate(tileType);
        }
        tileType = (GameObject)Instantiate(Resources.Load("TileTerrain/tile_" + tileTerrain.ToString()));
        tileType.transform.position = t.position;
        tileType.transform.rotation = t.rotation;
        tileType.transform.parent = this.transform;
        tileType.hideFlags = HideFlags.HideInHierarchy;
    }

    #endregion

}
