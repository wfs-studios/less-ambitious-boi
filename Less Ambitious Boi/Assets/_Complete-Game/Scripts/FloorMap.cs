using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMap {

    public float x, y, z;
    public GameObject marker;
    
    public FloorMap(float newX, float newY, float newZ, GameObject newMarker)
    {
        x = newX;
        y = newY;
        z = newZ;
        marker = newMarker;
    }
}
