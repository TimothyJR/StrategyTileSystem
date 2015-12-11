using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovableCharacter : MonoBehaviour {
    private bool hasMoved = false;
    [SerializeField] private float turnSpeed = 3.0f;
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private GameObject damageIndicator;
    private bool attacking = true;

    public bool HasMoved
    { get { return hasMoved; } set { hasMoved = value; } }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator MoveToTile(List<GameObject> tilesList, TileManagerGame tmg, GameObject panel)
    {
        for (int i = 0; i < tilesList.Count - 1; i++)
        {
            yield return StartCoroutine(TurnToTile(tilesList[i], tilesList[i + 1]));
        }
        tmg.MovingCharacter = false;
        panel.SetActive(true);
    }

    private IEnumerator TurnToTile(GameObject tileOn, GameObject tileTo)
    {
        Quaternion lookRotation = Quaternion.LookRotation((tileTo.transform.position - tileOn.transform.position).normalized);
        Quaternion startRotation = this.transform.rotation;
        float timePassed = 0;
        while (this.transform.rotation.eulerAngles != lookRotation.eulerAngles)
        {
            timePassed += Time.deltaTime;
            this.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, timePassed * turnSpeed);
            yield return null;
        }

        yield return StartCoroutine(MoveGameObjectToTile(tileOn, tileTo));
    }

    private IEnumerator MoveGameObjectToTile(GameObject tileOn, GameObject tileTo)
    {
        Vector3 targetPosition = tileTo.transform.position;
        Vector3 startPosition = tileOn.transform.position;
        float timePassed = 0;
        while (this.transform.position != targetPosition)
        {
            timePassed += Time.deltaTime;
            this.transform.position = Vector3.Lerp(startPosition, targetPosition, timePassed * moveSpeed);
            yield return null;
        }
    }

    public void ConfirmTileMove(GameObject tileOld, GameObject tileNew, Tile.Occupation occupationType)
    {
        Tile t = tileOld.GetComponent<Tile>();
        t.Occupied = false;
        t.OccupyingObject = null;
        t.InstantiatedOccupyingObject = null;
        t = tileNew.GetComponent<Tile>();
        t.Occupied = true;
        t.InstantiatedOccupyingObject = this.gameObject;
        t.TileOccupation = occupationType;
        this.transform.parent = tileNew.transform;
        hasMoved = true;
    }

    public IEnumerator AttackCharacter(GameObject charToAttack, Canvas canvas)
    {
        Quaternion lookRotation = Quaternion.LookRotation((charToAttack.transform.parent.position - this.transform.parent.position).normalized);
        Quaternion startRotation = this.transform.rotation;
        float timePassed = 0;
        
        while(this.transform.rotation.eulerAngles != lookRotation.eulerAngles)
        {
            timePassed += Time.deltaTime;
            this.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, timePassed * turnSpeed);
            yield return null;
        }
        while (attacking)
        {
            gameObject.GetComponent<Animator>().SetTrigger("attack");
            yield return null;
        }
        PlayerStats attackedCharStats = charToAttack.GetComponent<PlayerStats>();
        attackedCharStats.TakeDamage(this.GetComponent<PlayerStats>().Attack, damageIndicator, canvas);
        gameObject.GetComponent<Animator>().ResetTrigger("attack");

        attacking = false;
    }

    public void SetAttack()
    {
        attacking = false;
    }
}
