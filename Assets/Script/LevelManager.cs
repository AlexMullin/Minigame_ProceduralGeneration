using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform bounds;
    public GameObject levelObject;

    private GameObject currentLevel;


    // Start is called before the first frame update
    void Start()
    {
        makeNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void makeNextLevel()
    {
        //isntantiate new Level Object
        GameObject tempLevel = Instantiate(
            levelObject,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            gameObject.transform);

        //Locate its generate script
        Level tempScript = tempLevel.GetComponent<Level>();

        //provide necessary Bounds information
        tempScript.bounds = bounds;
        tempScript.manager = gameObject;

        //get rid of the current level
        //TODO: sink level through floor before destroying
        Destroy(currentLevel);

        //Have the new level generate the obstacles
        tempScript.generateLevel();

        //bring the new level into play
        currentLevel = tempLevel;

        tempLevel.transform.position = new Vector3(0, 0, 0);
        //TODO: raise level through floor
    }


}
