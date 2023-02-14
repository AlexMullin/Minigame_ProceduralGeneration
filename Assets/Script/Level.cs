using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject keyObject;
    public GameObject goalObject;

    //public GameObject[] obstacles;

    //set by LevelManager.cs
    public GameObject manager;
    public Transform bounds;


    //Keeps track of how much space is used in the level
    //Takes space information from bounds (provided by NE corner in level)
    const int GRIDSIZE_X = 16;
    const int GRIDSIZE_Y = 9;
    private float gridScaleX;
    private float gridScaleY;

    public bool[,] grid = new bool[GRIDSIZE_X, GRIDSIZE_Y];

    // Start is called before the first frame update
    void Start()
    {
        gridScaleX = (bounds.position.x * 2) / GRIDSIZE_X;
        gridScaleY = (bounds.position.y * 2) / GRIDSIZE_Y;
        //Initialize Grid
        resetGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Randomly generate obstacles within the level's bounds
    //Obstacles are placed into an array of [9][16]. Each object has a size and takes up space accordingly.
    public void generateLevel()
    {

        //MAKE KEY AND GOAL WORK WITH PLACEOBJECT
        /*
        //place Key
        GameObject tempKey = Instantiate(
            keyObject,
            new Vector3(Random.Range(-bounds.position.x, bounds.position.x), 0, Random.Range(-bounds.position.z, bounds.position.z)),
            Quaternion.identity, 
            gameObject.transform
            );
        */
        GameObject tempKey = placeObject(keyObject);

        //place Goal
        /*
        GameObject tempGoal = Instantiate(
            goalObject,
            new Vector3(Random.Range(-bounds.position.x, bounds.position.x), 0, Random.Range(-bounds.position.z, bounds.position.z)),
            Quaternion.identity,
            gameObject.transform
            );
        */
        GameObject tempGoal = placeObject(goalObject);

        //Assign goal to key
        tempKey.GetComponent<Key>().goalTarget = tempGoal;

        //Assign goal to level manager
        tempGoal.GetComponent<Goal>().manager = manager;

        //place Objects
            //Pick out X object from list
            //pick a spot on the board accounting for its width and place it
            //where it can fit.

            //Fill out the grid on 
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

        Debug.Log(obj.name + ": " + placementX + " " + placementY);
        return new Vector2Int(placementX, placementY);
    }
    
    //used by placeObject. returns true if an object can fit where getGridRandom generates
    bool canObjectFit(ObstacleProperties objProp, Vector2Int gridPos)
    {
        for (int x = 0; x < objProp.Size.x; x++)
        {
            for (int y = 0; y < objProp.Size.y; y++)
            {
                //if any squares within the target space are occupied, return false
                if (!grid[gridPos.x - objProp.Size.x / 2 + x, gridPos.y - objProp.Size.y / 2 + y])
                {
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
            new Vector3(gridPos.x * gridScaleX + 0.5f, 0, gridPos.y * gridScaleY + 0.5f),
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
                            new Vector3(tempPos.x * gridScaleX + 0.5f, 0, tempPos.y * gridScaleY + 0.5f),
                            Quaternion.identity,
                            gameObject.transform
                        );
                    }
                }
            }
        }


        //if all else fails, return false and log that the object could not be placed.
        Debug.Log("Object " + obj.name + " could not be placed at " + gridPos);
        return new GameObject();
    }
}
