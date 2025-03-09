using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumbs : MonoBehaviour
{
    Vector3 previousPosition, currentLocation;
    public GameObject BC;
    float distance;
    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DropBreadCrumb();
    }

    private void DropBreadCrumb()
    {
        currentLocation = transform.position;
        distance = Vector3.Distance(previousPosition, currentLocation);
        if (distance > 1.0f)
        {
            previousPosition = currentLocation;
            GameObject g = Instantiate(BC, currentLocation, Quaternion.identity);
            Destroy(g, 5);
        }
    }
}
