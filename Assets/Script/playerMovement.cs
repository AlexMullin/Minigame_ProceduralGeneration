using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 0;
    public float slowSpeed = 0;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 inputXYZ = new Vector3(inputX, 0, inputZ);

        if (Input.GetButton("Jump"))
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, slowSpeed * Time.deltaTime);
        }
        else if (inputXYZ != Vector3.zero)
        {
            rb.AddForce(inputXYZ * Time.deltaTime * moveSpeed);
        }

    }
}
