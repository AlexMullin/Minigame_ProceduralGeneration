using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject keyObject;
    public GameObject goalObject;

    public GameObject obstacles;

    //set by LevelManager.cs
    public GameObject manager;
    public Transform bounds;

    public int gridX = 0;
    public int gridZ = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Randomly generate obstacles within the level's bounds
    //Obstacles are placed into an array of [9][16]. Each object has a size and takes up space accordingly.
    public void generateLevel()
    {
        //place Key
        GameObject tempKey =Instantiate(
            keyObject,
            new Vector3(Random.Range(-bounds.position.x, bounds.position.x), 0, Random.Range(-bounds.position.z, bounds.position.z)),
            Quaternion.identity, 
            gameObject.transform
            );

        //place Goal
        GameObject tempGoal = Instantiate(
            goalObject,
            new Vector3(Random.Range(-bounds.position.x, bounds.position.x), 0, Random.Range(-bounds.position.z, bounds.position.z)),
            Quaternion.identity,
            gameObject.transform
            );

        //Assign goal to key
        tempKey.GetComponent<Key>().goalTarget = tempGoal;

        //Assign goal to level manager
        tempGoal.GetComponent<Goal>().manager = manager;
        //place Objects
    }
}
