using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Ray ray;
    RaycastHit2D hit;

    bool mouseOver = false;

    public bool walkable = true;
    public bool target = false;
    public bool selectable = false;
    public bool nextMove = false;

    public List<Tile> adjacencyList = new List<Tile>();

    //Needed BFS (breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    //For A*
    public float f = 0;
    public float g = 0;
    public float h = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (nextMove)
        {
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.white;
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset()
    {
        nextMove = false;
    }
}
