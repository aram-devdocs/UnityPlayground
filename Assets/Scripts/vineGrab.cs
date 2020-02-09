using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vineGrab : MonoBehaviour
{

    public Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter()
    {
        Debug.Log("Trigggggered");
        Debug.Log(rb.velocity);
    }
}
