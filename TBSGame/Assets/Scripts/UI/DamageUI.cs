using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DamageUI : MonoBehaviour {

    private float counter = 0;
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        RectTransform pos = this.GetComponent<RectTransform>();
        pos.anchoredPosition = new Vector2(pos.anchoredPosition.x, pos.anchoredPosition.y + Time.deltaTime * 20.0f);
        counter += Time.deltaTime;

        if(counter > 1.5f)
        {
            Destroy(this.gameObject);
        }
	}

    public void setPosition(GameObject objectToSpawnOn, Canvas canvas)
    {
        Vector3 pos;
        float width = canvas.GetComponent<RectTransform>().sizeDelta.x;
        float height = canvas.GetComponent<RectTransform>().sizeDelta.y;
        float x = Camera.main.WorldToScreenPoint(objectToSpawnOn.transform.parent.position).x / Screen.width;
        float y = Camera.main.WorldToScreenPoint(objectToSpawnOn.transform.parent.position).y / Screen.height;
        pos = new Vector3(width * x - width / 2, y * height - height / 2);

        this.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void setDamageText(int damageAmount)
    {
        this.GetComponent<Text>().text = "" + damageAmount;
    }
}
