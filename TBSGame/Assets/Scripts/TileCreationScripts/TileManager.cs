using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode()][System.Serializable]
public class TileManager : MonoBehaviour {
	[SerializeField] private int xTiles = 0;
	[SerializeField] private int zTiles = 0;
	[SerializeField] private  GameObject tiles;
    [SerializeField] private GameObject firstTile;
    [SerializeField] private GameObject cameraSelectedTile;
    private const float inc = 2.1f;
    [SerializeField] private List<List<GameObject>> tilesList = new List<List<GameObject>>();


    public int XTiles
    {
       get { return xTiles; }
       set { xTiles = value; }
    }

    public int ZTiles
    {
        get { return zTiles; }
        set { zTiles = value; }
    }

    public List<List<GameObject>> TilesList
    { get { return tilesList; } }

    public GameObject FirstTile
    { get { return firstTile; } }

    public GameObject SelectedTile
    { get { return cameraSelectedTile; } set { cameraSelectedTile = value; } }

	// Use this for initialization
	void Start () {
        if(firstTile != null)
        {
            PopulateTileList(firstTile, 0, 0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {


	}


    /* Parameters:
     * Axis: Is the change on the x or z axis. 0 Is the x axis. 1 is the y axis
     * OldValue: How many tiles were in the axis originally
     * NewValue: How many tiles will be in the axis after the changes
     * 
     * This method will add tiles when the inspector is changed
     * */
    public void CreateTileGridX(int oldValue)
    {
        // Set the starting x position next to the last tile
        float x = (inc * oldValue);
        float z = 0;
        // Start cycling through from the last list entry
        for (int i = oldValue; i < xTiles; i++)
        {
            List<GameObject> column = new List<GameObject>();
            // Create all the tiles in the new columns
            for (int j = 0; j < zTiles; j++)
            {
                GameObject generatedTile = (GameObject)GameObject.Instantiate(tiles);
                column.Add(generatedTile);
                generatedTile.GetComponent<Tile>().TileManager = gameObject;
                generatedTile.GetComponent<Tile>().setTileType();
                generatedTile.transform.position = new Vector3(x, 0, z);
                generatedTile.name = "Tile(" + i + ", " + j + ")";
                generatedTile.transform.parent = transform;
                z -= inc;
        
            }
            x += inc;
            z = 0;
            
            tilesList.Add(column);
        }
        LinkTiles();
        if(tilesList.Count > 0 && tilesList[0] != null)
        {
            if (tilesList[0].Count > 0 && tilesList[0][0] != null)
            {
                firstTile = tilesList[0][0];
                if(cameraSelectedTile == null)
                {
                    cameraSelectedTile = tilesList[0][0];
                }
            }
        }
        else
        {
            firstTile = null;
        }
    }

    /* Parameters:
     * Axis: Is the change on the x or z axis. 0 Is the x axis. 1 is the y axis
     * OldValue: How many tiles were in the axis originally
     * NewValue: How many tiles will be in the axis after the changes
     * 
     * This method will add tiles when the inspector is changed
     * */
    public void CreateTileGridZ(int oldValue)
    {
        float x = 0;
        // Set the z position to be next to the last tile
        float z = -(inc * (oldValue));
        for (int i = 0; i < xTiles; i++)
        {
            // Start at the end of each list and add the tiles in the proper positions
            for (int j = oldValue; j < zTiles; j++)
            {
                GameObject generatedTile = (GameObject)GameObject.Instantiate(tiles);
                tilesList[i].Add(generatedTile);
                generatedTile.GetComponent<Tile>().TileManager = gameObject;
                generatedTile.GetComponent<Tile>().setTileType();
                generatedTile.transform.position = new Vector3(x, 0, z);
                generatedTile.name = "Tile(" + i + ", " + j + ")";
                generatedTile.transform.parent = transform;
                z -= inc;

            }
            x += inc;
            z = -(inc * (oldValue));
        }
        LinkTiles();
        if (tilesList.Count > 0 && tilesList[0] != null)
        {
            if (tilesList[0].Count > 0 && tilesList[0][0] != null)
            {
                firstTile = tilesList[0][0];
                if (cameraSelectedTile == null)
                {
                    cameraSelectedTile = tilesList[0][0];
                }
            }
        }
        else
        {
            firstTile = null;
        }
    }


    /* Parameters:
     * OldValue: How many tiles were in the axis originally
     * 
     * This method will remove tiles when the inspector is changed
     * */
    public void RemoveTilesX(int oldValue)
    {
        for (int i = oldValue - 1; i > xTiles - 1; i--)
        {
            for (int j = zTiles - 1; j >= 0; j--)
            {
                if(tilesList[i][j] != null)
                {
                    Tile t = tilesList[i][j].GetComponent<Tile>();
                    t.RemoveTile();
                    tilesList[i].RemoveAt(j);
                } 
            }
            tilesList.RemoveAt(i);
        }
        LinkTiles();
        if (tilesList.Count > 0 && tilesList[0] != null)
        {
            if (tilesList[0].Count > 0 && tilesList[0][0] != null)
            {
                firstTile = tilesList[0][0];
                if (cameraSelectedTile == null)
                {
                    cameraSelectedTile = tilesList[0][0];
                }
            }
        }
        else
        {
            firstTile = null;
        }
    }

    /* Parameters:
     * OldValue: How many tiles were in the axis originally
     * 
     * This method will remove tiles when the inspector is changed
     * */
    public void RemoveTilesZ(int oldValue)
    {
        for (int i = 0; i < xTiles; i++)
        {
            for (int j = oldValue - 1; j > zTiles - 1; j--)
            {
                if(tilesList[i][j] != null)
                {
                    Tile t = tilesList[i][j].GetComponent<Tile>();
                    t.RemoveTile();
                    tilesList[i].RemoveAt(j);
                }
                
            }
        }
        LinkTiles();
        if (tilesList.Count > 0 && tilesList[0] != null)
        {
            if (tilesList[0].Count > 0 && tilesList[0][0] != null)
            {
                firstTile = tilesList[0][0];
                if (cameraSelectedTile == null)
                {
                    cameraSelectedTile = tilesList[0][0];
                }
            }
        }
        else
        {
            firstTile = null;
        }
    }


    
    // Connects tiles to the proper tiles around them
    private void LinkTiles()
    {
        // Cycle through all the tiles
        // Everytime the first for loop increments, we go down a set of tiles
        for(int i = 0; i < xTiles; i++)
        {
            // Everytime the second for loop increments, we go over one row
            for(int j = 0; j < zTiles; j++)
            {
                // Get the tile component
                Tile tileComponent = tilesList[i][j].GetComponent<Tile>();
                // Set each tiles adjacent tiles(whether they are connected or not)
                // Left tile
                if(i > 0)
                {
                    tileComponent.Left.ConnectedTile = tilesList[i - 1][j];
                }
                else
                {
                    tileComponent.Left.ConnectedTile = null;
                }
        
                // Up tile
                if(j > 0)
                {
                    tileComponent.Up.ConnectedTile = tilesList[i][j - 1];
                }
                else
                {
                    tileComponent.Up.ConnectedTile = null;
                }
        
                // Right tile
                if(i < xTiles - 1)
                {
                    tileComponent.Right.ConnectedTile = tilesList[i + 1][j];
                }
                else
                {
                    tileComponent.Right.ConnectedTile = null;
                }
        
                // Down tile
                if(j < zTiles - 1)
                {
                    tileComponent.Down.ConnectedTile = tilesList[i][j + 1];
                }
                else
                {
                    tileComponent.Down.ConnectedTile = null;
                }
        
            }
        }
    }

    private void PopulateTileList(GameObject tile, int x, int z)
    {
        if(tilesList.Count <= x)
        {
            List<GameObject> column = new List<GameObject>();
            tilesList.Insert(x, column);
        }

        tilesList[x].Insert(z, tile);
        if (tile.GetComponent<Tile>().Right.ConnectedTile != null)
        {
            PopulateTileList(tile.GetComponent<Tile>().Right.ConnectedTile, x + 1, z);
        }
        if (x == 0)
        {
            if (tile.GetComponent<Tile>().Down.ConnectedTile != null)
            {
                PopulateTileList(tile.GetComponent<Tile>().Down.ConnectedTile, x, z + 1);
            }
        }
    }
 
}
