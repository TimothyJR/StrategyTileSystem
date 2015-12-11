using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor {
    
    private int oldXValue = 0;
    private int oldZValue = 0;

	public override void OnInspectorGUI()
	{
		TileManager tm = (TileManager)target;
        oldXValue = tm.XTiles;
        oldZValue = tm.ZTiles;
		EditorGUILayout.LabelField("Tile System Dimensions");
		tm.XTiles = EditorGUILayout.IntField("X",tm.XTiles);
		tm.ZTiles = EditorGUILayout.IntField("Z",tm.ZTiles);
        tm.SelectedTile = (GameObject)EditorGUILayout.ObjectField("Camera Start Tile", tm.SelectedTile, typeof(GameObject), true);



        if (GUI.changed)
		{
            if (oldXValue < tm.XTiles)
            {
                tm.CreateTileGridX(oldXValue);
                oldXValue = tm.XTiles;
            }
            else if (oldXValue > tm.XTiles)
            {
                tm.RemoveTilesX(oldXValue);
                oldXValue = tm.XTiles;
            }

            if (oldZValue < tm.ZTiles)
            {
                tm.CreateTileGridZ(oldZValue);
                oldZValue = tm.ZTiles;
            }
            else if (oldZValue > tm.ZTiles)
            {
                tm.RemoveTilesZ(oldZValue);
                oldZValue = tm.ZTiles;
            }
        }

        
	}


}
