using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private float offsetZ = -25;
    private float offsetY = 35;
    private float lerpSpeed = 1.5f;
    private float lerpPercent;

    private Vector3 startPoint;
    private Vector3 cameraPosWithoutTranslate;


    private TileManager tm;
    private TileManagerGame tmg;
    [SerializeField] private GameObject tileManager;

    

    public GameObject TileManager
    { set { tileManager = value; } }


	// Use this for initialization
	void Start () {
        
        tm = tileManager.GetComponent<TileManager>();
        tmg = tileManager.GetComponent<TileManagerGame>();
        startPoint = tm.SelectedTile.transform.position;
        tm.gameObject.GetComponent<TileManagerGame>().SetTileColor(tm.SelectedTile);
            
        
    }
	
	// Update is called once per frame
	void Update () {

        SetCamera();
        if (!tmg.MovingCharacter && !tmg.MovementConfirmation)
        {
            if (Input.GetKeyUp("w"))
            {
                if (tm.SelectedTile.GetComponent<Tile>().Up.ConnectedTile != null && tm.SelectedTile.GetComponent<Tile>().Up.ConnectedTile.GetComponent<Tile>().Disabled == false)
                {
                    // Take care of the color of the old tile
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = false;
                    tmg.SetTileColor(tm.SelectedTile);

                    // Switch to the next tile
                    tm.SelectedTile = tm.SelectedTile.GetComponent<Tile>().Up.ConnectedTile;
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = true;
                    tmg.SetTileColor(tm.SelectedTile);
                    startPoint = cameraPosWithoutTranslate;
                    lerpPercent = 0;
                    tmg.PathingLines(KeyCode.W);
                }
            }

            if (Input.GetKeyUp("a"))
            {
                if (tm.SelectedTile.GetComponent<Tile>().Left.ConnectedTile != null && tm.SelectedTile.GetComponent<Tile>().Left.ConnectedTile.GetComponent<Tile>().Disabled == false)
                {
                    // Take care of the color of the old tile
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = false;
                    tmg.SetTileColor(tm.SelectedTile);

                    // Switch to the next tile
                    tm.SelectedTile = tm.SelectedTile.GetComponent<Tile>().Left.ConnectedTile;
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = true;
                    tmg.SetTileColor(tm.SelectedTile);
                    startPoint = cameraPosWithoutTranslate;
                    lerpPercent = 0;
                    tmg.PathingLines(KeyCode.A);
                }
            }

            if (Input.GetKeyUp("s"))
            {
                if (tm.SelectedTile.GetComponent<Tile>().Down.ConnectedTile != null && tm.SelectedTile.GetComponent<Tile>().Down.ConnectedTile.GetComponent<Tile>().Disabled == false)
                {
                    // Take care of the color of the old tile
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = false;
                    tmg.SetTileColor(tm.SelectedTile);

                    // Switch to the next tile
                    tm.SelectedTile = tm.SelectedTile.GetComponent<Tile>().Down.ConnectedTile;
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = true;
                    tmg.SetTileColor(tm.SelectedTile);
                    startPoint = cameraPosWithoutTranslate;
                    lerpPercent = 0;
                    tmg.PathingLines(KeyCode.S);
                }
            }

            if (Input.GetKeyUp("d"))
            {
                if (tm.SelectedTile.GetComponent<Tile>().Right.ConnectedTile != null && tm.SelectedTile.GetComponent<Tile>().Right.ConnectedTile.GetComponent<Tile>().Disabled == false)
                {
                    // Take care of the color of the old tile
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = false;
                    tmg.SetTileColor(tm.SelectedTile);

                    // Switch to the next tile
                    tm.SelectedTile = tm.SelectedTile.GetComponent<Tile>().Right.ConnectedTile;
                    tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = true;
                    tmg.SetTileColor(tm.SelectedTile);
                    startPoint = cameraPosWithoutTranslate;
                    lerpPercent = 0;
                    tmg.PathingLines(KeyCode.D);
                }
            }
        }
        
	}

    private void SetCamera()
    {
        lerpPercent += lerpSpeed * Time.deltaTime;
        if(lerpPercent > 1.0f)
        {
            lerpPercent = 1.0f;
        }       
        transform.rotation = Quaternion.identity;
        cameraPosWithoutTranslate = Vector3.Lerp(startPoint, tm.SelectedTile.transform.position, lerpPercent);
        transform.position = new Vector3(cameraPosWithoutTranslate.x, cameraPosWithoutTranslate.y + offsetY, cameraPosWithoutTranslate.z + offsetZ);
        transform.LookAt(cameraPosWithoutTranslate);
        
    }


}
