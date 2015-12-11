using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfirmActionMenu : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject tileManagerGame;
    private GameObject tileWithPlayer;
    private int currentSpot = 0;
    private bool attackSelectionState = false;
    private int targetCycleSpot = 0;

	// Use this for initialization
	void Start () {
	}

    void OnEnable()
    {
        currentSpot = 0;
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.x, buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.y);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S) && !attackSelectionState)
        {
            currentSpot = (currentSpot + 1) % buttons.Length;
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.x, buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.y);
        }
        else if (Input.GetKeyDown(KeyCode.S) && attackSelectionState)
        {
            TileManager tm = tileManagerGame.GetComponent<TileManager>();
            TileManagerGame tmg = tileManagerGame.GetComponent<TileManagerGame>();
            targetCycleSpot = (targetCycleSpot + 1) % tmg.AttackTiles.Count;
            tm.SelectedTile = tmg.AttackTiles[targetCycleSpot];

        }

        if (Input.GetKeyDown(KeyCode.W) && !attackSelectionState)
        {
            currentSpot = (currentSpot - 1);
            if(currentSpot < 0)
            {
                currentSpot = buttons.Length - 1;
            }
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.x, buttons[currentSpot].GetComponent<RectTransform>().anchoredPosition.y);
        }
        else if (Input.GetKeyDown(KeyCode.W) && attackSelectionState)
        {
            TileManager tm = tileManagerGame.GetComponent<TileManager>();
            TileManagerGame tmg = tileManagerGame.GetComponent<TileManagerGame>();
            targetCycleSpot = (targetCycleSpot - 1);
            if (targetCycleSpot < 0)
            {
                targetCycleSpot = tmg.AttackTiles.Count - 1;
            }
            tm.SelectedTile = tmg.AttackTiles[targetCycleSpot];
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TileManagerGame tmg = tileManagerGame.GetComponent<TileManagerGame>();
            if (!attackSelectionState )
            {
                switch (currentSpot)
                {
                    case 0:
                        if (tmg.AttackTiles.Count > 0)
                        {
                            attackSelectionState = true;
                            tileWithPlayer = tileManagerGame.GetComponent<TileManager>().SelectedTile;
                            tileManagerGame.GetComponent<TileManager>().SelectedTile = tmg.AttackTiles[0];
                        }
                        break;
                    case 1:
                        tmg.ConfirmMovement();
                        break;
                    case 2:
                        tmg.RetractMovement();
                        break;
                }
            }
            else
            {
                tmg.ConfirmMovement(tileWithPlayer);
                StartCoroutine(tmg.AttackEnemy(tileWithPlayer.GetComponent<Tile>().InstantiatedOccupyingObject, tmg.AttackTiles[targetCycleSpot].GetComponent<Tile>().InstantiatedOccupyingObject));
                tmg.AttackTiles.Clear();
                attackSelectionState = false;
                tileManagerGame.GetComponent<TileManager>().SelectedTile = tileWithPlayer;
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (!attackSelectionState)
            {
                tileManagerGame.GetComponent<TileManagerGame>().RetractMovement();
            }
            else
            {
                attackSelectionState = false;
                tileManagerGame.GetComponent<TileManager>().SelectedTile = tileWithPlayer;
            }
        }

	}
}
