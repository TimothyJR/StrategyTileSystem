using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(TileManager))]
public class TileManagerGame : MonoBehaviour {
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject canvas;
    private List<GameObject> enemies;
    private List<GameObject> playerAllies;
    private List<GameObject> movementTiles;
    private List<GameObject> tilesInPath;
    private List<GameObject> linesForPath;
    private List<GameObject> attackTiles;
    private TileManager tm;

    private GameObject controllerSelectedTile = null;

    private Color enteredTileColor;
    private Color originalTileColor;
    private Color selectedColor;
    private Color movementColor;
    private Color attackColor;

    private bool pathRecalculation = false;
    private bool movingCharacter = false;
    private bool movementConfirmation = false;

    private GameObject previouslyOnTile;
    private GameObject selectedCharacter;
    private Quaternion previousRotation;

    public List<GameObject> AttackTiles
    { get { return attackTiles; } }

    public GameObject ControllerSelectedTile
    { get { return controllerSelectedTile; } }

    public bool MovingCharacter
    { get { return movingCharacter; } set { movingCharacter = value; } }

    public bool MovementConfirmation
    { get { return movementConfirmation; } set { movementConfirmation = value; } }

    public enum GameState
    {
        preGame,
        playerPhase,
        enemyPhase,
        winGame,
        loseGame
    }

    private GameState currentGameState = GameState.preGame;

	// Use this for initialization
	void Awake () {
        enteredTileColor = Color.green;
        selectedColor = Color.red;
        movementColor = Color.blue;
        attackColor = Color.cyan;

        tm = this.GetComponent<TileManager>();
        tm.SelectedTile.GetComponent<TileGame>().CameraSelectedTile = true;
        originalTileColor = tm.SelectedTile.GetComponent<TileGame>().getMaterialColor();

        playerAllies = new List<GameObject>();
        enemies = new List<GameObject>();
        movementTiles = new List<GameObject>();
        tilesInPath = new List<GameObject>();
        linesForPath = new List<GameObject>();
        attackTiles = new List<GameObject>();

        GetAllCharacters(tm.FirstTile, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        // Control the stages of the game
        switch (currentGameState)
        {
            case GameState.preGame:
                currentGameState = GameState.playerPhase;
                break;


            case GameState.playerPhase:
                PlayerTurn();                
                break;


            case GameState.enemyPhase:
                // Are all of the enemies dead
                if (enemies.Count == 0)
                {
                    currentGameState = GameState.winGame;
                    break;
                }

                // Check to see if all enemies moved
                // Switch back to players phase
                bool switchToPlayerPhase = true;

                //foreach(GameObject go in enemies)
                //{
                //    switchToPlayerPhase = false;
                //}

                if(switchToPlayerPhase)
                {
                    for(int i = 0; i < playerAllies.Count; i++)
                    {
                        playerAllies[i].GetComponent<MovableCharacter>().HasMoved = false;
                    }
                    currentGameState = GameState.playerPhase;
                }

                break;


            case GameState.winGame:
                break;


            case GameState.loseGame:
                break;

        }
    }

    public void EnemyDied(GameObject deadEnemy)
    {
        enemies.Remove(deadEnemy);
    }

    public void AllyDied(GameObject deadAlly)
    {
        playerAllies.Remove(deadAlly);
    }

    // Fills the lists with the proper characters
    private void GetAllCharacters(GameObject tile, int x, int z)
    {
        Tile t = tile.GetComponent<Tile>();
        if(t.Occupied)
        {
            switch(t.TileOccupation)
            {
                case Tile.Occupation.ally:
                    playerAllies.Add(t.InstantiatedOccupyingObject);
                    break;
                case Tile.Occupation.enemy:
                    enemies.Add(t.InstantiatedOccupyingObject);
                    break;
            }
        }
        if (t.Right.ConnectedTile != null)
        {
            GetAllCharacters(tile.GetComponent<Tile>().Right.ConnectedTile, x + 1, z);
        }
        if (x == 0 && tile.GetComponent<Tile>().Down.ConnectedTile != null)
        {
            GetAllCharacters(tile.GetComponent<Tile>().Down.ConnectedTile, x, z + 1);
        }
    }

    private void PlayerTurn()
    {
        // Check to see if all players are still alive
        if (playerAllies.Count == 0)
        {
            currentGameState = GameState.loseGame;
        }
        
        bool switchToEnemyPhase = true;
        // Check to see if we should automatically go to the enemy phase
        // (All players have been moved)
        foreach (GameObject go in playerAllies)
        {
            if (!go.GetComponent<MovableCharacter>().HasMoved)
            {
                switchToEnemyPhase = false;
            }
        }
        
        if (switchToEnemyPhase == true)
        {
            tilesInPath.Clear();
            currentGameState = GameState.enemyPhase;
        }
        
        // For selecting characters and movement
        // Make sure a character is not moving
        if (!movingCharacter)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TileSelection();
            }
        
            // For deselecting characters
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TileDeselect();
            }
        }
    }

    // ------------------------------------------------------- Movement and Attack Tile Display/Hide

    private void TileSelection()
    {
        Tile currentTile = tm.SelectedTile.GetComponent<Tile>();

        // Select a tile with an ally on it
        if (currentTile.Occupied == true && currentTile.TileOccupation == Tile.Occupation.ally && controllerSelectedTile == null)
        {
            if (currentTile.InstantiatedOccupyingObject.GetComponent<MovableCharacter>().HasMoved == false)
            {
                controllerSelectedTile = tm.SelectedTile;
                controllerSelectedTile.GetComponent<TileGame>().ControllerSelectedTile = true;
                SetTileColor(controllerSelectedTile);

                // Get movement tiles
                PlayerStats ps = controllerSelectedTile.GetComponent<Tile>().InstantiatedOccupyingObject.GetComponent<PlayerStats>();
                GetMovementTiles(controllerSelectedTile, ps.Movement, ps.Movement, ps.AttackRange);

                // Set tile for path
                tilesInPath.Add(controllerSelectedTile);

            }
        }
        // Already have a player selected
        else if (controllerSelectedTile != null && (currentTile.Occupied == false || controllerSelectedTile == tm.SelectedTile)  && movementTiles.Contains(tm.SelectedTile) && movementConfirmation == false)
        {
            // Pre-set up in case movement is reverted
            previouslyOnTile = controllerSelectedTile;
            movementConfirmation = true;
            selectedCharacter = controllerSelectedTile.GetComponent<Tile>().InstantiatedOccupyingObject;
            previousRotation = selectedCharacter.transform.rotation;
            if (!(tilesInPath.Count == 1))
            {
                StartCoroutine(selectedCharacter.GetComponent<MovableCharacter>().MoveToTile(tilesInPath, this, panel));
                movingCharacter = true;
            }
            
            controllerSelectedTile.GetComponent<TileGame>().ControllerSelectedTile = false;
            SetTileColor(controllerSelectedTile);
            for (int i = 0; i < movementTiles.Count; i++)
            {
                TileGame tg = movementTiles[i].GetComponent<TileGame>();
                tg.MovementTile = false;
                tg.CharacterSpaceAway = 100000;
                SetTileColor(movementTiles[i]);
            }
            for(int i = 0; i < attackTiles.Count; i++)
            {
                TileGame tg = attackTiles[i].GetComponent<TileGame>();
                tg.AttackTile = false;
                SetTileColor(attackTiles[i]);
            }
            for (int i = 0; i < linesForPath.Count; i++)
            {
                linesForPath[i].GetComponent<LineRenderer>().enabled = false;
            }
            attackTiles.Clear();
            movementTiles.Clear();
            attackTiles = GetEnemiesInAttackRange(tm.SelectedTile, selectedCharacter.GetComponent<PlayerStats>().AttackRange, 0);
            for (int i = 0; i < attackTiles.Count; i++)
            {
                TileGame tg = attackTiles[i].GetComponent<TileGame>();
                tg.AttackTile = true;
                SetTileColor(attackTiles[i]);
            }
        }
    }

    private void TileDeselect()
    {
        if (controllerSelectedTile != null && !movementConfirmation)
        {
            controllerSelectedTile.GetComponent<TileGame>().ControllerSelectedTile = false;
            SetTileColor(controllerSelectedTile);
            for (int i = 0; i < movementTiles.Count; i++)
            {
                TileGame tg = movementTiles[i].GetComponent<TileGame>();
                tg.MovementTile = false;
                tg.CharacterSpaceAway = 100000;
                SetTileColor(movementTiles[i]);
            }
            for (int i = 0; i < attackTiles.Count; i++)
            {
                TileGame tg = attackTiles[i].GetComponent<TileGame>();
                tg.AttackTile = false;
                SetTileColor(attackTiles[i]);
            }
            // Clear all lines and movement tiles
            for (int i = 0; i < linesForPath.Count; i++)
            {
                linesForPath[i].GetComponent<LineRenderer>().enabled = false;
            }
            attackTiles.Clear();
            movementTiles.Clear();
        }

        controllerSelectedTile = null;
    }

    public void ConfirmMovement()
    {
        selectedCharacter.GetComponent<MovableCharacter>().ConfirmTileMove(previouslyOnTile, tm.SelectedTile, Tile.Occupation.ally);
        controllerSelectedTile = null;
        movementConfirmation = false;
        attackTiles.Clear();
        tilesInPath.Clear();
        linesForPath.Clear();
        panel.SetActive(false);
    }

    public void ConfirmMovement(GameObject tile)
    {
        selectedCharacter.GetComponent<MovableCharacter>().ConfirmTileMove(previouslyOnTile, tile, Tile.Occupation.ally);
        controllerSelectedTile = null;
        movementConfirmation = false;
        tilesInPath.Clear();
        linesForPath.Clear();
    }

    public void RetractMovement()
    {
        movementConfirmation = false;
        selectedCharacter.transform.position = previouslyOnTile.transform.position;
        selectedCharacter.transform.rotation = previousRotation;
        controllerSelectedTile = null;
        attackTiles.Clear();
        tilesInPath.Clear();
        linesForPath.Clear();
        panel.SetActive(false);
    }

    private List<GameObject> GetEnemiesInAttackRange(GameObject tile, int attackRange, int amountTraversed)
    {
        Tile t = tile.GetComponent<Tile>();
        List<GameObject> go = new List<GameObject>();
        
        if (attackRange > amountTraversed)
        {
            if (t.Left.ConnectedTile != null && t.Left.ConnectedToTile && !t.Left.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                go.AddRange(GetEnemiesInAttackRange(t.Left.ConnectedTile, attackRange, amountTraversed + 1));
            }
            if (t.Up.ConnectedTile != null && t.Up.ConnectedToTile && !t.Up.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                go.AddRange(GetEnemiesInAttackRange(t.Up.ConnectedTile, attackRange, amountTraversed + 1));
            }
            if (t.Right.ConnectedTile != null && t.Right.ConnectedToTile && !t.Right.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                go.AddRange(GetEnemiesInAttackRange(t.Right.ConnectedTile, attackRange, amountTraversed + 1));
            }
            if (t.Down.ConnectedTile != null && t.Down.ConnectedToTile && !t.Down.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                go.AddRange(GetEnemiesInAttackRange(t.Down.ConnectedTile, attackRange, amountTraversed + 1));
            }
        }

        if(t.Occupied == true && t.TileOccupation == Tile.Occupation.enemy && !go.Contains(tile))
        {
            go.Add(tile);
        }
        
        return Enumerable.ToList<GameObject>(Enumerable.Distinct<GameObject>(go)); ;
    }


    private void GetMovementTiles(GameObject tile, int moveAmount, int maxMovement, int attackRange)
    {
        Tile t = tile.GetComponent<Tile>();
        // Add tiles that are used for movement
        if ((maxMovement - moveAmount) < maxMovement)
        {
            if (t.Left.ConnectedTile != null && t.Left.ConnectedToTile && !t.Left.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                CheckConnectedTileForMovement(tile, t.Left.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Up.ConnectedTile != null && t.Up.ConnectedToTile && !t.Up.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                CheckConnectedTileForMovement(tile, t.Up.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Right.ConnectedTile != null && t.Right.ConnectedToTile && !t.Right.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                CheckConnectedTileForMovement(tile, t.Right.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Down.ConnectedTile != null && t.Down.ConnectedToTile && !t.Down.ConnectedTile.GetComponent<Tile>().Disabled)
            {
                CheckConnectedTileForMovement(tile, t.Down.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
        }

        // Add tiles that show attack range
        else if(!(maxMovement - moveAmount == maxMovement + attackRange))
        {
            if (t.Left.ConnectedTile != null && t.Left.ConnectedToTile && !t.Left.ConnectedTile.GetComponent<Tile>().Disabled && !movementTiles.Contains(t.Left.ConnectedTile))
            {
                MarkAsAttackTile(t.Left.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Up.ConnectedTile != null && t.Up.ConnectedToTile && !t.Up.ConnectedTile.GetComponent<Tile>().Disabled && !movementTiles.Contains(t.Up.ConnectedTile))
            {
                MarkAsAttackTile(t.Up.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Right.ConnectedTile != null && t.Right.ConnectedToTile && !t.Right.ConnectedTile.GetComponent<Tile>().Disabled && !movementTiles.Contains(t.Right.ConnectedTile))
            {
                MarkAsAttackTile(t.Right.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
            if (t.Down.ConnectedTile != null && t.Down.ConnectedToTile && !t.Down.ConnectedTile.GetComponent<Tile>().Disabled && !movementTiles.Contains(t.Down.ConnectedTile))
            {
                MarkAsAttackTile(t.Down.ConnectedTile, moveAmount, maxMovement, attackRange);
            }
        }
    }

    private void CheckConnectedTileForMovement(GameObject originalTile, GameObject tile, int moveAmount, int maxMovement, int attackRange)
    {

        TileGame tg = tile.GetComponent<TileGame>();
        Tile t = tile.GetComponent<Tile>();
        if (tg.CharacterSpaceAway > maxMovement - moveAmount && ((t.Occupied == true && t.TileOccupation != Tile.Occupation.enemy && t.TileOccupation != Tile.Occupation.objects) || t.Occupied == false))
        {
            
            if(maxMovement - (maxMovement - moveAmount) <= 1 && (t.Occupied == true && t.TileOccupation == Tile.Occupation.ally))
            {
                return;
            }
            tg.CharacterSpaceAway = maxMovement - moveAmount;
            tg.ClosestTileToPlayer = originalTile;
            tg.MovementTile = true;
            movementTiles.Remove(tile);
            movementTiles.Add(tile);
            GetMovementTiles(tile, moveAmount - 1, maxMovement, attackRange);
            SetTileColor(tile);
        }
        else if(t.TileOccupation == Tile.Occupation.enemy)
        {
            tg.AttackTile = true;
            attackTiles.Add(tile);
            SetTileColor(tile);
        }
    }

    private void MarkAsAttackTile(GameObject tile, int moveAmount, int maxMovement, int attackRange)
    {
        TileGame tg = tile.GetComponent<TileGame>();

        if(!tg.MovementTile && !tg.AttackTile && tile.GetComponent<Tile>().TileOccupation != Tile.Occupation.objects)
        {
            tg.AttackTile = true;
            attackTiles.Add(tile);
            GetMovementTiles(tile, moveAmount - 1, maxMovement, attackRange);
            SetTileColor(tile);
            
        }
    }

    public IEnumerator AttackEnemy(GameObject controlledCharacter, GameObject enemyCharacter)
    {

        MovableCharacter playerMC = controlledCharacter.GetComponent<MovableCharacter>();
        MovableCharacter enemyMC = enemyCharacter.GetComponent<MovableCharacter>();
        Canvas c = canvas.GetComponent<Canvas>();
        yield return StartCoroutine(playerMC.AttackCharacter(enemyCharacter, c));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(enemyMC.AttackCharacter(controlledCharacter, c));
        panel.SetActive(false);
    }

    // ------------------------------------------------------- Pathing

    public void PathingLines(KeyCode key)
    {
        // Show the path that the play should be taking
        if (controllerSelectedTile != null)
        {
            // Check to make sure you can actually move to the tile
            if (movementTiles.Contains(tm.SelectedTile))
            {
                // Now check to see if you already have the tile in your path
                if (!tilesInPath.Contains(tm.SelectedTile))
                {
                    // Add tile to your path if you still have movement left
                    if (tilesInPath.Count < controllerSelectedTile.GetComponent<Tile>().InstantiatedOccupyingObject.GetComponent<PlayerStats>().Movement + 1)
                    {
                        // Is the last tile in path connected to this one
                        switch(key)
                        {
                            // Check to see if the path is connected to the new tile
                            // If it is not, recalculate the path
                            case KeyCode.A:
                                if(!tm.SelectedTile.GetComponent<Tile>().Right.ConnectedToTile)
                                {
                                    pathRecalculation = true;
                                }
                                break;
                            case KeyCode.W:
                                if (!tm.SelectedTile.GetComponent<Tile>().Down.ConnectedToTile)
                                {
                                    pathRecalculation = true;
                                }
                                break;
                            case KeyCode.D:
                                if (!tm.SelectedTile.GetComponent<Tile>().Left.ConnectedToTile)
                                {
                                    pathRecalculation = true;
                                }
                                break;
                            case KeyCode.S:
                                if (!tm.SelectedTile.GetComponent<Tile>().Up.ConnectedToTile)
                                {
                                    pathRecalculation = true;
                                }
                                break;
                        }
                                
                        if (pathRecalculation)
                        {
                            RecalculatePath(tm.SelectedTile.GetComponent<TileGame>());
                        }
                        else
                        {
                            tilesInPath.Add(tm.SelectedTile);
                            CreatePath();
                        }
                    }
                    else
                    {
                        // This tile cannot be gotten to on the current path
                        // Recalculate the path using the fastest possible path to the character
                        RecalculatePath(tm.SelectedTile.GetComponent<TileGame>());
                    }
                }
                else
                {
                    // This tile is already in the path
                    int index = tilesInPath.IndexOf(tm.SelectedTile);
                    
                    tilesInPath.RemoveRange(index + 1, tilesInPath.Count - (index + 1));
                    if (pathRecalculation)
                    {
                        RecalculatePath(tm.SelectedTile.GetComponent<TileGame>());
                    }
                    else
                    {
                        for (int i = index; i < linesForPath.Count; i++)
                        {
                            linesForPath[i].GetComponent<LineRenderer>().enabled = false;
                        }
                        CreatePath();
                    }

                }
            }
            else
            {
                pathRecalculation = true;
            }
        }
        
    }

    private void CreatePath()
    {
        for (int i = 0; i < tilesInPath.Count - 1; i++)
        {
            if (linesForPath.Count < i + 1)
            {
                linesForPath.Add(GameObject.Instantiate(line));
            }
            LineRenderer lr = linesForPath[i].GetComponent<LineRenderer>();
            lr.enabled = true;
            lr.SetPosition(0, new Vector3(tilesInPath[i].transform.position.x, tilesInPath[i].transform.position.y + 0.2f, tilesInPath[i].transform.position.z));
            lr.SetPosition(1, new Vector3(tilesInPath[i + 1].transform.position.x, tilesInPath[i + 1].transform.position.y + 0.2f, tilesInPath[i + 1].transform.position.z));
        }

    }

    private void RecalculatePath(TileGame t)
    {
        tilesInPath.Clear();
        tilesInPath.Add(t.gameObject);
        if (t.gameObject != controllerSelectedTile)
        {
            GameObject tile = t.ClosestTileToPlayer;
            while (tile != controllerSelectedTile)
            {
                tilesInPath.Add(tile);
                tile = tile.GetComponent<TileGame>().ClosestTileToPlayer;
            }
            tilesInPath.Add(tile);
            tilesInPath.Reverse();
        }
        pathRecalculation = false;
        for (int i = 0; i < linesForPath.Count; i++)
        {
            LineRenderer lr = linesForPath[i].GetComponent<LineRenderer>();
            lr.enabled = false;
        }
        CreatePath();
    }

    // ------------------------------------------------------- Enemies

    
    // ------------------------------------------------------- Other

    public void SetTileColor(GameObject go)
    {
        TileGame tg = go.GetComponent<TileGame>();

        if(tg.ControllerSelectedTile)
        {
            tg.setMaterialColor(enteredTileColor);
            return;
        }
        if(tg.CameraSelectedTile)
        {
            tg.setMaterialColor(selectedColor);
            return;
        }
        if(tg.MovementTile)
        {
            tg.setMaterialColor(movementColor);
            return;
        }
        if(tg.AttackTile)
        {
            tg.setMaterialColor(attackColor);
            return;
        }
        tg.setMaterialColor(originalTileColor);
    }
}
