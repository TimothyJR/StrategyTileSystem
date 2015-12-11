using UnityEngine;
using System.Collections;

public class SetRotation : MonoBehaviour {
    private float rotationAmount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        rotationAmount += Time.deltaTime * 10;
        this.transform.rotation = Quaternion.Euler(0.0f, -rotationAmount, 0.0f);
	}
}
