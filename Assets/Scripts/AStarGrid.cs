using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    [SerializeField] float nodeRadius;
    [SerializeField] LayerMask unwalkable;
    [SerializeField] MazeMaker mazeMaker;

    Node[,] nodes;

    int amountOfNodesX;
    int amountOfNodesY;

    float nodeDiameter;

    [SerializeField] Transform player;
    public void ResetGrid()
    {
        nodeDiameter = nodeRadius * 2;

        amountOfNodesX = Mathf.RoundToInt(mazeMaker.xSize / nodeDiameter);
        amountOfNodesY = Mathf.RoundToInt(mazeMaker.ySize /nodeDiameter);

        MakeGrid();
    }

    void MakeGrid()
    {
        nodes = new Node[amountOfNodesX, amountOfNodesY];

        Vector3 startPos = Vector3.zero - Vector3.right * mazeMaker.xSize / 2 - Vector3.forward * mazeMaker.ySize / 2;
        Vector3 currentWorldPos;

        for (int i = 0; i < amountOfNodesX; i++)
        {
            for (int z = 0; z  < amountOfNodesY; z ++)
            {
                currentWorldPos = new Vector3(startPos.x + i * nodeDiameter, 1f, startPos.z + z * nodeDiameter);

                bool unwalkableBool = Physics.CheckSphere(currentWorldPos, nodeRadius, unwalkable);

                nodes[i, z] = new Node(unwalkableBool, currentWorldPos, i, z);
            }
        }
    }

    public Node GetWorldPoint(Vector3 position)
    {
        float percentX = (position.x + mazeMaker.xSize / 2) / mazeMaker.xSize;
        float percentY = (position.z + mazeMaker.ySize / 2) / mazeMaker.ySize;

        percentX += 0.1f;
        percentY += 0.1f;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((amountOfNodesX - 1) * percentX);
        int y = Mathf.RoundToInt((amountOfNodesY - 1) * percentY);

        return nodes[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neigbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int xPos = node.x + x;
                int yPos = node.y + y;

                if (xPos >= 0 && xPos < amountOfNodesX && yPos >= 0 && yPos < amountOfNodesY)
                {
                    neigbours.Add(nodes[xPos, yPos]);
                }
            }
        }

        return neigbours;
    }


    private void OnDrawGizmos()
    {
        if(nodes != null)
        {
            Node playerNode = GetWorldPoint(player.transform.position);
            for (int i = 0; i < amountOfNodesX; i++)
            {
                for (int z = 0; z < amountOfNodesY; z++)
                {
                    if(nodes[i, z].unwalkable)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if(nodes[i, z] == playerNode)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
    
                    Gizmos.DrawCube(nodes[i, z].worldPosition, new Vector3(nodeDiameter - 0.05f, 1, nodeDiameter - 0.05f));
                }
            }
        }
    }
}
