using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float bounceForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 directionBounce = other.transform.position - transform.position;
            Debug.Log(other.transform.name);
            other.transform.GetComponent<Rigidbody>().velocity = directionBounce.normalized * bounceForce;
        }
    }
}
