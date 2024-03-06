using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    //variables for speed and magnitude of floating
    public float floatingSpeed = 1f; 
    public float floatingMagnitude = 0.15f; 
    private Vector3 initialPosition;

    void Start()
    {
        //getting intial position of pickup
        initialPosition = transform.position;
    }

    void Update()
    {
        //moving the heart up and down
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatingSpeed) * floatingMagnitude;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
