using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool unwalkable;
    public Vector3 worldPosition;

    public bool closed;

   public int hCost;
   public int gCost;
    public int x;
    public int y;
    public Node parent;

    public Node(bool unwalkable, Vector3 position, int x, int y)
    {
        this.unwalkable = unwalkable;
        worldPosition = position;

        this.x = x;
        this.y = y;
    }

    public int fCost
    {
        get
        {
            return hCost + gCost;
        }
    }
}
