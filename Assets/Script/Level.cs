using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject keyObject;
    public GameObject goalObject;
    public GameObject[] obstacles;

    //public GameObject[] obstacles;

    //set by LevelManager.cs
    public GameObject manager;
    public Transform bounds;

    //
    //Keeps track of how much space is used in the level
    //Takes space information from bounds (provided by NE corner in level)
    const int GRIDSIZE_X = 16;
    const int GRIDSIZE_Y = 9;
    private float gridScaleX;
    private float gridScaleY;

    public bool[,] grid = new bool[GRIDSIZE_X, GRIDSIZE_Y];


    //shift speed and phase control how the level phases in and out of the boundaries
    public float shiftSpeed = 10;

    public enum phase {Start, Current, End}
    public phase currentPhase;

    //called by level manager to properly set up grid before level generation
    public void setupGrid()
    {
        gridScaleX = (bounds.position.x * 2) / GRIDSIZE_X;
        gridScaleY = (bounds.position.y * 2) / GRIDSIZE_Y;

        Debug.Log("GridScale X: " + gridScaleX + "\n GridScale Y:" + gridScaleY);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initialize Grid
        resetGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //moving obstacles in and out of level
        switch (currentPhase)
        {
            case phase.Current:
                break;

            case phase.Start:
                transform.position = new Vector3(0, Mathf.MoveTowards(transform.position.y, 0, shiftSpeed * Time.deltaTime), 0);
                
                if (transform.position.y == 0)
                    currentPhase = phase.Current;

                break;

            case phase.End:
                transform.position = new Vector3(0, Mathf.MoveTowards(transform.position.y, -10, shiftSpeed * Time.deltaTime), 0);
                
                if (transform.position.y == -10)
                    Destroy(gameObject);

                break;
        }
    }

    //Randomly generate obstacles within the level's bounds
    //Obstacles are placed into an array of [9][16]. Each object has a size and takes up space accordingly.
    public void generateLevel()
    {
        //places key in the level and ensures it gets there
        GameObject tempKey;
        do
        {
            Debug.Log("Placing: Key");
            tempKey = placeObject(keyObject);
        } while (tempKey == null);

        //places a goal in the level and ensures it gets there
        GameObject tempGoal;
        do
        {
            Debug.Log("Placing Goal");
            tempGoal = placeObject(goalObject);
        } while (tempGoal == null);

        //Assign goal to key
        tempKey.GetComponent<Key>().goalTarget = tempGoal;

        //Assign goal to level manager
        tempGoal.GetComponent<Goal>().manager = manager;

        //places a bumper if possible
        placeObject(obstacles[0]);

        //place 5 walls throughout the level
        for (int i = 0; i < 6; i++)
        {
            placeObject(obstacles[1]);
        }
    }

    private void resetGrid()
    {
        for (int i = 0; i < GRIDSIZE_Y; i++)
            for (int j = 0; j < GRIDSIZE_Y; j++)
                grid[i, j] = false;
    }

    //Sets the space in grid an object takes up to true
    private void placeGrid(Vector2Int position, Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                //offset - width / 2 + i
                grid[position.x - (size.x / 2) + i, position.y - (size.y / 2) + j] = true;
            }
        }
    }


    //used by PlaceObject. returns a initial location to place an object on the grid based on dimensions.
    Vector2Int getGridRandom(GameObject obj)
    {
        ObstacleProperties objProp;
        try
        {
            objProp = obj.gameObject.GetComponent<ObstacleProperties>();
        }
        catch
        {
            Debug.Log("getGrid Random: could not cast to object properties");
            return Vector2Int.zero;
        }

        int placementX = Random.Range(0 + objProp.Size.x / 2, GRIDSIZE_X - objProp.Size.x / 2);
        int placementY = Random.Range(0 + objProp.Size.y / 2, GRIDSIZE_Y - objProp.Size.y / 2);

        return new Vector2Int(placementX, placementY);
    }
    
    //used by placeObject. returns true if an object can fit where getGridRandom generates
    bool canObjectFit(ObstacleProperties objProp, Vector2Int gridPos)
    {
        for (int x = 0; x < objProp.Size.x; x++)
        {
            for (int y = 0; y < objProp.Size.y; y++)
            {

                //bug: Sometimes grid tries to test out of bounds index
                int placeX = gridPos.x - objProp.Size.x / 2 + x;
                int placeY = gridPos.y - objProp.Size.y / 2 + y;


                //if any squares within the target space are occupied, return false
                try
                {
                    if (grid[placeX, placeY])
                    {
                        return false;
                    }
                }
                catch
                {
                    //I will fix this LLLLLLLLLLLLLLLLLLATER if at all possible. 
                    //For now, putting an exception down fixes the problem, so I'm leaving it as is.
                    Debug.LogError("Index bug!" + "\nPlacementX: " + placeX + "\nPlacementY: " + placeY);
                    return false;
                }
            }
        }

        //No squares are occupied, return true
        return true;
    }

    GameObject placeObject(GameObject obj)
    {
        //Script attached to obstacles and collectables
        ObstacleProperties objProp;


        //see if the object has the right script
        try
        {
            objProp = obj.gameObject.GetComponent<ObstacleProperties>();
        }
        catch
        {
            Debug.Log("placeObject: could not cast to object properties");
            return new GameObject();
        }

        Vector2Int gridPos = getGridRandom(obj);

        //if object can fit in that spot, place it there,
        if (canObjectFit(objProp, gridPos))
        {
            //fill out the grid accordingly
            placeGrid(gridPos, objProp.Size);

            //return the new object as it's placed in the level
            return Instantiate(
            obj,
            new Vector3(gridPos.x * gridScaleX + 0.5f - bounds.position.x, -5, gridPos.y * gridScaleY + 0.5f - bounds.position.y),
            Quaternion.identity,
            gameObject.transform
            );
        }
        else //True placing the obstacle in a 5x5 square of the target.
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Vector2Int tempPos = gridPos;
                    tempPos.x += i;
                    tempPos.y += j;

                    //if the obstacle can fit in the new position, place it there and reutrn true.
                    if (canObjectFit(objProp, tempPos)){
                        // Fill out the grid accordingly to the new testing position
                        placeGrid(tempPos, objProp.Size);

                        return Instantiate(
                            obj,
                            new Vector3(tempPos.x * gridScaleX + 0.5f - bounds.position.x, -5, tempPos.y * gridScaleY + 0.5f - bounds.position.y),
                            Quaternion.identity,
                            gameObject.transform
                        );
                    }
                }
            }
        }


        //if all else fails, return false and log that the object could not be placed.
        Debug.Log("Object " + obj.name + " could not be placed at " + gridPos);
        return null;
    }
}
