using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Vector2 dimensions;
    [SerializeField] float nodeRadius;
    [SerializeField] LayerMask unwalkableLayer;

    [SerializeField] GridFloor floor;
    [SerializeField] GameObject wall;
    [SerializeField] Player player;

    Vector2 aStarDimensions;

    float nodeDiameter;

    Node[,] nodes;

    int amountofNodesX;
    int amountofNodesY;

    GridFloor[,] gridFloors;
    [SerializeField] float floorSize;

    // Start is called before the first frame update
    void Start()
    {
        aStarDimensions = new Vector2(dimensions.x * 10, dimensions.y * 10);

        nodeDiameter = nodeRadius * 2;

        amountofNodesX = Mathf.RoundToInt(aStarDimensions.x / nodeDiameter);
        amountofNodesY = Mathf.RoundToInt(aStarDimensions.y / nodeDiameter);

        CreateFloor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.StopAllCoroutines();

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int z = 0; z < dimensions.y; z++)
                {
                    Destroy(gridFloors[i, z].leftWall);
                    Destroy(gridFloors[i, z].rightWall);
                    Destroy(gridFloors[i, z].upWall);
                    Destroy(gridFloors[i, z].downWall);
                    Destroy(gridFloors[i, z].gameObject);
                }
            }

            CreateFloor();
        }
    }

    void CreateFloor()
    {
        gridFloors = new GridFloor[(int)dimensions.x, (int)dimensions.y];

        float offSetX = (aStarDimensions.x / 2) - 5;
        float offSetY = (aStarDimensions.y / 2) - 5;

        Vector3 startPos = transform.position - Vector3.right * offSetX - Vector3.forward * offSetY;

        for (int i = 0; i < dimensions.x; i++)
        {
            for (int z = 0; z < dimensions.y; z++)
            {
                gridFloors[i, z] = Instantiate(floor, new Vector3(startPos.x + i * floorSize, 0, startPos.z + z * floorSize), Quaternion.identity);

                if (z != dimensions.y - 1)
                {
                    gridFloors[i, z].downWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x, 5.5f, gridFloors[i, z].transform.position.z - 4.75f), Quaternion.identity);
                    gridFloors[i, z].downWall.transform.parent = gridFloors[i, z].transform;
                }
                else
                {
                    gridFloors[i, z].upWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x, 5.5f, gridFloors[i, z].transform.position.z + 4.75f), Quaternion.identity);
                    gridFloors[i, z].upWall.transform.parent = gridFloors[i, z].transform;
                }

                if (i != dimensions.x - 1)
                {
                    gridFloors[i, z].leftWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x - 4.75f, 5.5f, gridFloors[i, z].transform.position.z), Quaternion.Euler(0, 90, 0));
                    gridFloors[i, z].leftWall.transform.parent = gridFloors[i, z].transform;
                }
                else
                {
                    gridFloors[i, z].rightWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x + 4.75f, 5.5f, gridFloors[i, z].transform.position.z), Quaternion.Euler(0, 90, 0));
                    gridFloors[i, z].rightWall.transform.parent = gridFloors[i, z].transform;
                }
            }
        }

        StartMazeGeneration();
    }

    bool CheckIfAllNeighbouringFloorsVisited(int x, int y)
    {
        if (x > 0 && !gridFloors[x - 1, y].visited)
        {
            return true;
        }

        if (x < dimensions.x - 1 && !gridFloors[x + 1, y].visited)
        {
            return true;
        }

        if (y > 0 && !gridFloors[x, y - 1].visited)
        {
            return true;
        }

        if (y < dimensions.y - 1 && !gridFloors[x, y + 1].visited)
        {
            return true;
        }

        return false;
    }

    void StartMazeGeneration()
    {
        int xPos = 0;
        int yPos = Random.Range(0, (int)dimensions.y);

        player.transform.position = new Vector3(gridFloors[xPos, yPos].transform.position.x, 3.39f, gridFloors[xPos, yPos].transform.position.z);

        Destroy(gridFloors[xPos, yPos].leftWall);

        gridFloors[xPos, yPos].visited = true;

        StartHunt(xPos, yPos);
    }

    void StartHunt(int xPos, int yPos)
    {
        gridFloors[xPos, yPos].visited = true;

        while (CheckIfAllNeighbouringFloorsVisited(xPos, yPos))
        {
            int direction = Random.Range(0, 4);

            switch (direction)
            {
                case 0:
                    if (xPos != dimensions.x - 1 && !gridFloors[xPos + 1, yPos].visited)//Right
                    {
                        Destroy(gridFloors[xPos, yPos].rightWall);
                        xPos += 1;
                        Destroy(gridFloors[xPos, yPos].leftWall);
                        gridFloors[xPos, yPos].visited = true;
                    }
                    break;
                case 1:
                    if (xPos != 0 && !gridFloors[xPos - 1, yPos].visited)//Left
                    {
                        Destroy(gridFloors[xPos, yPos].leftWall);
                        xPos -= 1;
                        Destroy(gridFloors[xPos, yPos].rightWall);
                        gridFloors[xPos, yPos].visited = true;
                    }
                    break;
                case 2:
                    if (yPos != dimensions.y - 1 && !gridFloors[xPos, yPos + 1].visited)//Up
                    {
                        Destroy(gridFloors[xPos, yPos].upWall);
                        yPos += 1;
                        Destroy(gridFloors[xPos, yPos].downWall);
                        gridFloors[xPos, yPos].visited = true;
                    }
                    break;
                case 3:
                    if (yPos != 0 && !gridFloors[xPos, yPos - 1].visited)//Down
                    {
                        Destroy(gridFloors[xPos, yPos].downWall);
                        yPos -= 1;
                        Destroy(gridFloors[xPos, yPos].upWall);
                        gridFloors[xPos, yPos].visited = true;
                    }
                    break;
                default:
                    break;
            }
        }

        FindUnvisitedFloor();
    }

    void FindUnvisitedFloor()
    {
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int z = 0; z < dimensions.y; z++)
            {
                if (!gridFloors[i, z].visited && ConnectToVisitedNeighbour(i, z))
                {
                    return;
                }
            }
        }

        StartCoroutine(SetGrid());
        CreateEndPoint();
    }

    bool ConnectToVisitedNeighbour(int x, int y)
    {
        if (x > 0 && gridFloors[x - 1, y].visited)
        {
            Destroy(gridFloors[x, y].leftWall);
            Destroy(gridFloors[x - 1, y].rightWall);

            StartHunt(x, y);
            return true;
        }

        if (x < dimensions.x - 1 && gridFloors[x + 1, y].visited)
        {
            Destroy(gridFloors[x, y].rightWall);
            Destroy(gridFloors[x + 1, y].leftWall);

            StartHunt(x, y);

            return true;
        }

        if (y > 0 && gridFloors[x, y - 1].visited)
        {
            Destroy(gridFloors[x, y].downWall);
            Destroy(gridFloors[x, y - 1].upWall);

            StartHunt(x, y);

            return true;
        }

        if (y < dimensions.y - 1 && gridFloors[x, y + 1].visited)
        {
            Destroy(gridFloors[x, y].upWall);
            Destroy(gridFloors[x, y + 1].downWall);

            StartHunt(x, y);

            return true;
        }

        return false;
    }

    void CreateEndPoint()
    {
        Destroy(gridFloors[ (int)dimensions.x - 1, Random.Range(0, (int)dimensions.y)].rightWall);
    }

    IEnumerator SetGrid()
    {
        yield return new WaitForSeconds(0.1f);

        CreateNodes();
    }

    void CreateNodes()
    {
        nodes = new Node[amountofNodesX, amountofNodesY];

        Vector3 bottomLeft = transform.position - Vector3.right * aStarDimensions.x / 2 - Vector3.forward * aStarDimensions.y / 2;
        Vector3 currentPos;
        bool unwalkable;

        for (int i = 0; i < amountofNodesX; i++)
        {
            for (int z = 0; z < amountofNodesY; z++)
            {
                currentPos = bottomLeft + new Vector3(i * nodeDiameter + nodeRadius, 0, z * nodeDiameter + nodeRadius);
                unwalkable = Physics.CheckSphere(currentPos, nodeRadius, unwalkableLayer);

                nodes[i, z] = new Node(unwalkable, currentPos, i, z);
            }
        }
    }

    public Node GetWorldPoint(Vector3 position)
    {
        float percentX = (position.x + aStarDimensions.x / 2) / aStarDimensions.x;
        float percentY = (position.z + aStarDimensions.y / 2) / aStarDimensions.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((amountofNodesX - 1) * percentX);
        int y = Mathf.RoundToInt((amountofNodesY - 1) * percentY);

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

                if (xPos >= 0 && xPos < amountofNodesX && yPos >= 0 && yPos < amountofNodesY)
                {
                    neigbours.Add(nodes[xPos, yPos]);
                }
            }
        }

        return neigbours;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(aStarDimensions.x, 0, aStarDimensions.y));

        if(nodes != null)
        {
            Node playerNode = GetWorldPoint(player.transform.position);
            foreach (Node node in nodes)
            {
                if (node.unwalkable)
                {
                    Gizmos.color = Color.red;
                }
                else if(node == playerNode)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                Gizmos.DrawCube(node.worldPosition, new Vector3(nodeDiameter - 0.1f, 8, nodeDiameter - 0.1f));
            }
        }
    }
}
