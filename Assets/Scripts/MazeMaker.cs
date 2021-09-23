using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{
    public int xSize;
    public int ySize;

    [SerializeField] GameObject wall;
    [SerializeField] GridFloor floor;
    [SerializeField] AStarGrid grid;

    public float floorSize;

    [SerializeField] GameObject player;

    GridFloor[,] gridFloors;
    // Start is called before the first frame update
    void Start()
    {
        gridFloors = new GridFloor[xSize, ySize];
        SpawnFullMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < xSize; i++)
            {
                for (int z = 0; z < ySize; z++)
                {
                    Destroy(gridFloors[i, z].leftWall);
                    Destroy(gridFloors[i, z].rightWall);
                    Destroy(gridFloors[i, z].upWall);
                    Destroy(gridFloors[i, z].downWall);
                    Destroy(gridFloors[i, z].gameObject);
                }
            }

            gridFloors = new GridFloor[xSize, ySize];
            SpawnFullMaze();

            Debug.Log("SpawnedNewMaze");
        }
    }

    void SpawnFullMaze()
    {
        Vector3 startPos = Vector3.zero - Vector3.right * xSize / 2 - Vector3.forward * ySize / 2;

        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {
                gridFloors[i, z] = Instantiate(floor, new Vector3(startPos.x + i * floorSize, 0, startPos.z + z * floorSize), Quaternion.identity);

                if(z != ySize - 1)
                {
                    gridFloors[i, z].downWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x, 1, gridFloors[i, z].transform.position.z - 0.4680834f), Quaternion.identity);
                    gridFloors[i, z].downWall.transform.parent = gridFloors[i, z].transform;
                }
                else
                {
                    gridFloors[i, z].upWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x, 1, gridFloors[i, z].transform.position.z + 0.4680834f), Quaternion.identity);
                    gridFloors[i, z].upWall.transform.parent = gridFloors[i, z].transform;
                }

                if (i != xSize - 1)
                {
                    gridFloors[i, z].leftWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x - 0.4680834f, 1, gridFloors[i, z].transform.position.z), Quaternion.Euler(0, 90, 0));
                    gridFloors[i, z].leftWall.transform.parent = gridFloors[i, z].transform;
                }
                else
                {
                    gridFloors[i, z].rightWall = Instantiate(wall, new Vector3(gridFloors[i, z].transform.position.x + 0.4680834f, 1, gridFloors[i, z].transform.position.z), Quaternion.Euler(0, 90, 0));
                    gridFloors[i, z].rightWall.transform.parent = gridFloors[i, z].transform;
                }
            }
        }

        StartMazeGeneration();
    }

    bool CheckIfAllNeighbouringFloorsVisited(int x, int y)
    {
        if(x > 0 && !gridFloors[x - 1, y].visited)
        {
            return true;
        }

        if(x < xSize - 1 && !gridFloors[x + 1, y].visited)
        {
            return true;
        }

        if (y > 0 && !gridFloors[x, y - 1].visited)
        {
            return true;
        }

        if (y < ySize - 1 && !gridFloors[x, y + 1].visited)
        {
            return true;
        }

        return false;
    }

    void StartMazeGeneration()
    {
        int xPos = 0;
        int yPos = Random.Range(0, ySize);

        player.transform.position = new Vector3(gridFloors[xPos, yPos].transform.position.x, 0.8f, gridFloors[xPos, yPos].transform.position.z);

        Destroy(gridFloors[xPos, yPos].leftWall);

        gridFloors[xPos, yPos].visited = true;

        StartHunt(xPos, yPos);
    }

    void StartHunt(int xPos,int yPos)
    {
        gridFloors[xPos, yPos].visited = true;

        while (CheckIfAllNeighbouringFloorsVisited(xPos, yPos))
        {
            int direction = Random.Range(0, 4);

            switch (direction)
            {
                case 0:
                    if (xPos != xSize - 1 && !gridFloors[xPos + 1, yPos].visited)//Right
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
                    if (yPos != ySize - 1 && !gridFloors[xPos, yPos + 1].visited)//Up
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
        for (int i = 0; i < xSize; i++)
        {
            for (int z = 0; z < ySize; z++)
            {
                if(!gridFloors[i, z].visited && ConnectToVisitedNeighbour(i, z))
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

        if (x < xSize - 1 && gridFloors[x + 1, y].visited)
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

        if (y < ySize - 1 && gridFloors[x, y + 1].visited)
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
        Destroy(gridFloors[xSize - 1, Random.Range(0, ySize)].rightWall);
    }

    IEnumerator SetGrid()
    {
        yield return new WaitForSeconds(0.1f);

        grid.ResetGrid();
    }
}