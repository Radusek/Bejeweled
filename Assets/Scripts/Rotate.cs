using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;


    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.forward, rotationSpeed);
    }
}
