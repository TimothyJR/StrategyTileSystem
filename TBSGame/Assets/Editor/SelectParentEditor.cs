using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(SelectParent))]
public class SelectParentEditor : Editor {
    private Transform objectTransform;

    void OnEnable()
    {
        SelectParent sp = (SelectParent)target;
        objectTransform = sp.transform;
    }

	void OnSceneGUI()
    {
        Selection.activeTransform = objectTransform.parent;
    }

}
