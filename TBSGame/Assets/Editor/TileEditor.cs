using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Tile)), CanEditMultipleObjects]
public class TileEditor : Editor
{
    private Vector3 heightToChange;
    private Vector3 stair;
    private Vector3 stairPrevious;
    private Vector3 tileConnectionLeft;
    private Vector3 tileConnectionUp;
    private Vector3 tileConnectionRight;
    private Vector3 tileConnectionDown;
    private Vector3 connectedTileConnectionLeft;
    private Vector3 connectedTileConnectionUp;
    private Vector3 connectedTileConnectionRight;
    private Vector3 connectedTileConnectionDown;
    private GameObject previousOccupiedGO;
    private bool recalculateConnectionHandles;
    private bool sameGameObject;
    Tile t;
    Tool LastTool = Tool.None;


    void OnEnable()
    {
        t = target as Tile;


        LastTool = Tools.current;
        Tools.current = Tool.None;

        // Set to one above because the handle needs to be at 1 to change and tile height starts at 0
        heightToChange.y = t.HeightLevel;
        SetConnectionHandlesPosition();
        SetStairHandlePosition();
        stairPrevious = stair;
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }

    void OnSceneGUI()
    {
        // Create handles for the connections to other tiles

        #region Tile Connections

        // Put them at the level of the tile
        if (t.Left.ConnectedTile != null)
        {
            Handles.color = Color.blue;
            tileConnectionLeft.y = t.transform.position.y + 0.5f;
            tileConnectionLeft = Handles.Slider(tileConnectionLeft, Vector3.left, 0.3f, Handles.CubeCap, 1.0f);
            tileConnectionLeft.x = Mathf.Clamp(tileConnectionLeft.x, t.transform.position.x - 0.8f, t.transform.position.x - 0.4f);
            Handles.color = Color.green;
            connectedTileConnectionLeft.y = t.transform.position.y + 0.5f;
            connectedTileConnectionLeft = Handles.Slider(connectedTileConnectionLeft, Vector3.left, 0.3f, Handles.CubeCap, 1.0f);
            connectedTileConnectionLeft.x = Mathf.Clamp(connectedTileConnectionLeft.x, t.transform.position.x - 1.7f, t.transform.position.x - 1.3f);
        }
        if (t.Up.ConnectedTile != null)
        {
            Handles.color = Color.blue;
            tileConnectionUp.y = t.transform.position.y + 0.5f;
            tileConnectionUp = Handles.Slider(tileConnectionUp, Vector3.forward, 0.3f, Handles.CubeCap, 1.0f);
            tileConnectionUp.z = Mathf.Clamp(tileConnectionUp.z, t.transform.position.z + 0.4f, t.transform.position.z + 0.8f);
            Handles.color = Color.green;
            connectedTileConnectionUp.y = t.transform.position.y + 0.5f;
            connectedTileConnectionUp = Handles.Slider(connectedTileConnectionUp, Vector3.forward, 0.3f, Handles.CubeCap, 1.0f);
            connectedTileConnectionUp.z = Mathf.Clamp(connectedTileConnectionUp.z, t.transform.position.z + 1.3f, t.transform.position.z + 1.7f);
        }
        if (t.Right.ConnectedTile != null)
        {
            Handles.color = Color.blue;
            tileConnectionRight.y = t.transform.position.y + 0.5f;
            tileConnectionRight = Handles.Slider(tileConnectionRight, Vector3.right, 0.3f, Handles.CubeCap, 1.0f);
            tileConnectionRight.x = Mathf.Clamp(tileConnectionRight.x, t.transform.position.x + 0.4f, t.transform.position.x + 0.8f);
            Handles.color = Color.green;
            connectedTileConnectionRight.y = t.transform.position.y + 0.5f;
            connectedTileConnectionRight = Handles.Slider(connectedTileConnectionRight, Vector3.right, 0.3f, Handles.CubeCap, 1.0f);
            connectedTileConnectionRight.x = Mathf.Clamp(connectedTileConnectionRight.x, t.transform.position.x + 1.3f, t.transform.position.x + 1.7f);
        }
        if (t.Down.ConnectedTile != null)
        {
            Handles.color = Color.blue;
            tileConnectionDown.y = t.transform.position.y + 0.5f;
            tileConnectionDown = Handles.Slider(tileConnectionDown, Vector3.back, 0.3f, Handles.CubeCap, 1.0f);
            tileConnectionDown.z = Mathf.Clamp(tileConnectionDown.z, t.transform.position.z - 0.8f, t.transform.position.z - 0.4f);
            Handles.color = Color.green;
            connectedTileConnectionDown.y = t.transform.position.y + 0.5f;
            connectedTileConnectionDown = Handles.Slider(connectedTileConnectionDown, Vector3.back, 0.3f, Handles.CubeCap, 1.0f);
            connectedTileConnectionDown.z = Mathf.Clamp(connectedTileConnectionDown.z, t.transform.position.z - 1.7f, t.transform.position.z - 1.3f);
        }
        #endregion


        Handles.color = Color.cyan;
        #region Tile Height
        // Create a handle for the tile height
        if (!t.Stairs)
        {
            heightToChange = Handles.Slider(
                new Vector3(t.transform.position.x, t.transform.position.y + 1.0f, t.transform.position.z),
                Vector3.up, 0.8f, Handles.ArrowCap, 1.0f
                );
        }
        else
        {
            heightToChange = Handles.Slider(
                new Vector3(t.transform.position.x, t.transform.position.y + 0.5f, t.transform.position.z),
                Vector3.up, 1.0f, Handles.ArrowCap, 1.0f
                );
        }
        if (heightToChange.y < 1)
        {
            heightToChange.y = 1;
        }

        t.HeightLevel = (int)(heightToChange.y / 2) + 1;
        #endregion

        // Create handle for staircase

        Handles.color = Color.red;
        #region Stairs
        stair = Handles.FreeMoveHandle(
            stair,
            Quaternion.Euler(new Vector3(0.0f, 90.0f)),
            0.3f,
            Vector3.zero,
            Handles.SphereCap
            );


        // Clamp the values of the stairs vector so that it does not become stuck
        // in one direction
        stair.x = Mathf.Clamp(stair.x, t.transform.position.x - 1.5f, t.transform.position.x + 1.5f);
        stair.z = Mathf.Clamp(stair.z, t.transform.position.z - 1.5f, t.transform.position.z + 1.5f);
        stair.y = t.transform.position.y + 1.0f;

        // Set the stairs
        if (stair.x > t.transform.position.x + 1 && stairPrevious.x < t.transform.position.x + 1)
        {
            t.Stairs = true;
            t.StairDirection = Tile.StairDirections.right;
            recalculateConnectionHandles = true;
        }
        else if (stair.x < t.transform.position.x - 1 && stairPrevious.x > t.transform.position.x - 1)
        {
            t.Stairs = true;
            t.StairDirection = Tile.StairDirections.left;
            recalculateConnectionHandles = true;
        }
        else if (stair.z > t.transform.position.z + 1 && stairPrevious.z < t.transform.position.z + 1)
        {
            t.Stairs = true;
            t.StairDirection = Tile.StairDirections.up;
            recalculateConnectionHandles = true;
        }
        else if (stair.z < t.transform.position.z - 1 && stairPrevious.z > t.transform.position.z - 1)
        {
            t.Stairs = true;
            t.StairDirection = Tile.StairDirections.down;
            recalculateConnectionHandles = true;
        }
        else if(stair.x < t.transform.position.x + 1 && stair.x > t.transform.position.x - 1 && stair.z < t.transform.position.z + 1 && stair.z > t.transform.position.z - 1)
        {
            t.Stairs = false;
        }
        #endregion

        stairPrevious = new Vector3(stair.x, stair.y, stair.z);

        if (recalculateConnectionHandles)
        {
            SetConnectionHandlesPosition();
        }
        

        if (GUI.changed)
        {
            #region Check connections
            // Connections to other tiles
            if (tileConnectionLeft.x < t.transform.position.x - 0.6f)
            {
                t.Left.ConnectedToTile = true;
            }
            else
            {
                t.Left.ConnectedToTile = false;
            }
            if (tileConnectionUp.z > t.transform.position.z + 0.6f)
            {
                t.Up.ConnectedToTile = true;
            }
            else
            {
                t.Up.ConnectedToTile = false;
            }
            if (tileConnectionRight.x > t.transform.position.x + 0.6f)
            {
                t.Right.ConnectedToTile = true;
            }
            else
            {
                t.Right.ConnectedToTile = false;
            }
            if (tileConnectionDown.z < t.transform.position.z - 0.6f)
            {
                t.Down.ConnectedToTile = true;
            }
            else
            {
                t.Down.ConnectedToTile = false;
            }

            // Connections from other tiles
            if (t.Left.ConnectedTile != null)
            {
                if (connectedTileConnectionLeft.x < t.transform.position.x - 1.5f)
                {
                    t.Left.ConnectedTile.GetComponent<Tile>().Right.ConnectedToTile = false;
                }
                else
                {
                    t.Left.ConnectedTile.GetComponent<Tile>().Right.ConnectedToTile = true;
                }
            }

            if (t.Up.ConnectedTile != null)
            {
                if (connectedTileConnectionUp.z > t.transform.position.z + 1.5f)
                {
                    t.Up.ConnectedTile.GetComponent<Tile>().Down.ConnectedToTile = false;
                }
                else
                {
                    t.Up.ConnectedTile.GetComponent<Tile>().Down.ConnectedToTile = true;
                }
            }

            if (t.Right.ConnectedTile != null)
            {
                if (connectedTileConnectionRight.x > t.transform.position.x + 1.5f)
                {
                    t.Right.ConnectedTile.GetComponent<Tile>().Left.ConnectedToTile = false;
                }
                else
                {
                    t.Right.ConnectedTile.GetComponent<Tile>().Left.ConnectedToTile = true;
                }
            }

            if (t.Down.ConnectedTile != null)
            {
                if (connectedTileConnectionDown.z < t.transform.position.z - 1.5f)
                {
                    t.Down.ConnectedTile.GetComponent<Tile>().Up.ConnectedToTile = false;
                }
                else
                {
                    t.Down.ConnectedTile.GetComponent<Tile>().Up.ConnectedToTile = true;
                }
            }
            #endregion
            t.Staircase();
        }


    }

    public override void OnInspectorGUI()
    {

        // Tile connections
        EditorGUILayout.LabelField("Tile Connections");
       
        #region Left
        GUILayout.BeginHorizontal();
        t.Left.ConnectedToTile = EditorGUILayout.Toggle("Left", t.Left.ConnectedToTile);
        if (t.Left.ConnectedTile != null)
        {
            EditorGUILayout.LabelField("Left tile: " + t.Left.ConnectedTile.name);
        }
        else
        {
            EditorGUILayout.LabelField("No Left Tile");
        }
        GUILayout.EndHorizontal();

        #endregion

        #region Up
        GUILayout.BeginHorizontal();
        t.Up.ConnectedToTile = EditorGUILayout.Toggle("Up", t.Up.ConnectedToTile);
        if (t.Up.ConnectedTile != null)
        {
            EditorGUILayout.LabelField("Up tile: " + t.Up.ConnectedTile.name);
        }
        else
        {
            EditorGUILayout.LabelField("No Up Tile");
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Right
        GUILayout.BeginHorizontal();
        t.Right.ConnectedToTile = EditorGUILayout.Toggle("Right", t.Right.ConnectedToTile);
        if (t.Right.ConnectedTile != null)
        {
            EditorGUILayout.LabelField("Right tile: " + t.Right.ConnectedTile.name);
        }
        else
        {
            EditorGUILayout.LabelField("No Right Tile");
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Down
        GUILayout.BeginHorizontal();
        t.Down.ConnectedToTile = EditorGUILayout.Toggle("Down", t.Down.ConnectedToTile);
        if (t.Down.ConnectedTile != null)
        {
            EditorGUILayout.LabelField("Down tile: " + t.Down.ConnectedTile.name);
        }
        else
        {
            EditorGUILayout.LabelField("No Down Tile");
        }
        GUILayout.EndHorizontal();
        #endregion
        
        t.HeightLevel = EditorGUILayout.IntField("Level", t.HeightLevel);
        t.Disabled = EditorGUILayout.Toggle("Disabled", t.Disabled);
        t.Stairs = EditorGUILayout.Toggle("Stairs", t.Stairs);
        if (t.Stairs)
        {
            t.StairDirection = (Tile.StairDirections)EditorGUILayout.EnumPopup("Stair Directions", t.StairDirection);
        }

        t.Occupied = EditorGUILayout.Toggle("Occupied", t.Occupied);
        if(t.Occupied)
        {
            previousOccupiedGO = t.OccupyingObject;
            t.TileOccupation = (Tile.Occupation)EditorGUILayout.EnumPopup("Occupied by", t.TileOccupation);
            t.OccupyingObject = (GameObject)EditorGUILayout.ObjectField("Occupying Object", t.OccupyingObject, typeof(GameObject), true);
            t.ObjectRotation = EditorGUILayout.Vector3Field("Object Rotation", t.ObjectRotation);
            if(previousOccupiedGO == t.OccupyingObject)
            {
                sameGameObject = true;
            }
            else
            {
                sameGameObject = false;
            }
        }


        t.Terrain = (Tile.TileTerrain)EditorGUILayout.EnumPopup("Terrain Options", t.Terrain);


        if (GUI.changed)
        {
            SetStairHandlePosition();
            heightToChange.y = t.HeightLevel;
            
            t.CreateObject(!sameGameObject);
            if(!t.Occupied)
            {
                t.OccupyingObject = null;
                t.DestroyObject();
            }
            t.Staircase();
            t.setTileType();
        }


    }

    private void SetStairHandlePosition()
    {
        if (t.Stairs)
        {
            switch (t.StairDirection)
            {
                case Tile.StairDirections.left:
                    stair = t.transform.position;
                    stair.y += 0.5f;
                    stair.x -= 1.1f;
                    break;
                case Tile.StairDirections.up:
                    stair = t.transform.position;
                    stair.y += 0.5f;
                    stair.z += 1.1f;
                    break;
                case Tile.StairDirections.right:
                    stair = t.transform.position;
                    stair.y += 0.5f;
                    stair.x += 1.1f;
                    break;
                case Tile.StairDirections.down:
                    stair = t.transform.position;
                    stair.y += 0.5f;
                    stair.z -= 1.1f;
                    break;
            }
        }
        else
        {
            stair = t.transform.position;
            stair.y += 0.5f;
        }

    }

    private float SetConnectionHandleStartPos(bool connected, float connectedValue, float disconnectedValue)
    {
        if(connected)
        {
            return connectedValue;
        }
        else
        {
            return disconnectedValue;
        }
    }

    private void SetConnectionHandlesPosition()
    {
        // Set up tile connection handles positions
        if (t.Left.ConnectedTile != null)
        {
            tileConnectionLeft = t.transform.position;
            tileConnectionLeft.x -= SetConnectionHandleStartPos(t.Left.ConnectedToTile, 0.8f, 0.4f);
            connectedTileConnectionLeft = t.transform.position;
            connectedTileConnectionLeft.x -= SetConnectionHandleStartPos(t.Left.ConnectedTile.GetComponent<Tile>().Right.ConnectedToTile, 1.3f, 1.7f);

        }
        if (t.Up.ConnectedTile != null)
        {
            tileConnectionUp = t.transform.position;
            tileConnectionUp.z += SetConnectionHandleStartPos(t.Up.ConnectedToTile, 0.8f, 0.4f);
            connectedTileConnectionUp = t.transform.position;
            connectedTileConnectionUp.z += SetConnectionHandleStartPos(t.Up.ConnectedTile.GetComponent<Tile>().Down.ConnectedToTile, 1.3f, 1.7f); ;
        }
        if (t.Right.ConnectedTile != null)
        {
            tileConnectionRight = t.transform.position;
            tileConnectionRight.x += SetConnectionHandleStartPos(t.Right.ConnectedToTile, 0.8f, 0.4f); ;
            connectedTileConnectionRight = t.transform.position;
            connectedTileConnectionRight.x += SetConnectionHandleStartPos(t.Right.ConnectedTile.GetComponent<Tile>().Left.ConnectedToTile, 1.3f, 1.7f); ;
        }
        if (t.Down.ConnectedTile != null)
        {
            tileConnectionDown = t.transform.position;
            tileConnectionDown.z -= SetConnectionHandleStartPos(t.Down.ConnectedToTile, 0.8f, 0.4f); ;
            connectedTileConnectionDown = t.transform.position;
            connectedTileConnectionDown.z -= SetConnectionHandleStartPos(t.Down.ConnectedTile.GetComponent<Tile>().Up.ConnectedToTile, 1.3f, 1.7f); ;
        }
        recalculateConnectionHandles = false;
    }

}
