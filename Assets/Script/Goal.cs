using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //set by Level.cs
    public GameObject manager;

    public Collider cd;
    public GameObject shieldObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void disableShield()
    {
        Destroy(shieldObject);
        cd.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            manager.GetComponent<LevelManager>().makeNextLevel();
        }
    }
}
