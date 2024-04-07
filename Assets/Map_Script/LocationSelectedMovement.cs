using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSelectedMovement : MonoBehaviour
{
    private float amplitud = 2.0f;  
    private float velocidad = 0.5f; 
    private float newAltura = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (transform.position.x,(Mathf.Sin(Time.fixedTime * Mathf.PI * velocidad) * amplitud)+ newAltura,transform.position.z);
    }
}
