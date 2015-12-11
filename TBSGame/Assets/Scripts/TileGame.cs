using UnityEngine;
using System.Collections;

public class TileGame : MonoBehaviour {

    private bool cameraSelectedTile = false;
    private bool controllerSelectedTile = false;
    private bool movementTile = false;
    private bool attackTile = false;
    private int characterSpaceAway = 100000;
    private GameObject closestTileToPlayer;


    public bool CameraSelectedTile
    { get { return cameraSelectedTile; } set { cameraSelectedTile = value; } }

    public bool ControllerSelectedTile
    { get { return controllerSelectedTile; } set { controllerSelectedTile = value; } }

    public bool MovementTile
    { get { return movementTile; } set { movementTile = value; } }

    public bool AttackTile
    { get { return attackTile; } set { attackTile = value; } }

    public int CharacterSpaceAway
    { get { return characterSpaceAway; } set { characterSpaceAway = value; } }

    public GameObject ClosestTileToPlayer
    { get { return closestTileToPlayer; } set { closestTileToPlayer = value; } }
    // Use this for initialization
    void Start () {
	    if(this.GetComponent<Tile>().Disabled)
        {
            this.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setMaterialColor(Color col)
    {
        foreach (Transform t in transform)
        {
            if (t.tag == "Tile")
            {
                Renderer tRend = t.GetChild(0).GetComponent<Renderer>();
                foreach (Material m in tRend.materials)
                {
                    if (m.name == "TileDefault (Instance)")
                    {
                        m.color = col;
                    }
                }
            }
        }
    }

    public Color getMaterialColor()
    {
        foreach (Transform t in transform)
        {
            if (t.tag == "Tile")
            {
                Renderer tRend = t.GetChild(0).GetComponent<Renderer>();
                foreach (Material m in tRend.materials)
                {
                    if (m.name == "TileDefault (Instance)")
                    {
                        return m.color;
                    }
                }
            }
        }

        return Color.gray;
    }
}
