using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Transform bounds;
    public GameObject levelObject;

    private GameObject currentLevel;

    public Text timerText;
    public float timeRemaining;
    public float timeGained;

    private float startTime;

    public Text levelText;
    int levelCount = 0;

    bool gameEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        startTime = timeRemaining;
        //timerText.text = "Time Remaining: " + timeRemaining;
        timerText.text = "Blargh";

        levelText.text = "Current Level: " + levelCount;

        makeNextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        timerText.text = "Time Remaining: " + timeRemaining;

        if (timeRemaining <= 0)
        {
            //Game Over!
            timerText.text = "Time Remaining: 0";
            Debug.Log("Still Running");
            gameEnabled = false;
            enabled = false;

            restartButton.gameObject.SetActive(true);
        }
        
    }

    
    public void makeNextLevel()
    {
        if (gameEnabled)
        {
            levelCount++;
            levelText.text = "Current Level: " + levelCount;

            timeRemaining += timeGained;

            //isntantiate new Level Object
            GameObject tempLevel = Instantiate(
                levelObject,
                new Vector3(0, -5, 0),
                Quaternion.identity,
                gameObject.transform);

            //Locate its generate script
            Level tempScript = tempLevel.GetComponent<Level>();

            //provide necessary Bounds information
            tempScript.bounds = bounds;
            tempScript.manager = gameObject;
            tempScript.setupGrid();

            //get rid of the current level
            //TODO: sink level through floor before destroying
            //Destroy(currentLevel);
            if (currentLevel != null)
                currentLevel.GetComponent<Level>().currentPhase = Level.phase.End;

            //set phase, give reference to this script. When it reaches end phase,
            //it will unload itself and 

            //Have the new level generate the obstacles
            tempScript.generateLevel();

            //bring the new level into play
            currentLevel = tempLevel;
        }
    }

    public Button restartButton;
    public void onGameRestart()
    {
        levelCount = 0;
        timeRemaining = startTime;

        gameEnabled = true;
        enabled = true;

        makeNextLevel();

        restartButton.gameObject.SetActive(false);
    }
}
