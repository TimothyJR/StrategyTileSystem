using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileSystem {

	[MenuItem("GameObject/Create Other/Tile System")]
	static void Create()
	{

        //GameObject tSystem = (GameObject)GameObject.Instantiate((GameObject)Resources.Load ("TileManager"));
        //GameObject cam = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Camera"));
        //tSystem.transform.position = new Vector3(0,0,0);
        //tSystem.name = "TileSystem";
        //cam.GetComponent<CameraController>().TileManager = tSystem;
        //cam.name = "Camera";

        GameObject tSystem = (GameObject)GameObject.Instantiate((GameObject)Resources.Load ("System"));
        tSystem.transform.position = new Vector3(0,0,0);
        tSystem.name = "TileSystem";
    }
}
