using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] AStarGrid grid;

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
		Node startNode = grid.GetWorldPoint(startPos);
		Node targetNode = grid.GetWorldPoint(endPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				return GetPath(startNode, targetNode);
			}

			foreach (Node neighbour in grid.GetNeighbours(node))
			{
				if (neighbour.unwalkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDifference(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDifference(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

		return null;
	}

    int GetDifference(Node nodeOne, Node nodeTwo)
    {
        int x = Mathf.Abs(nodeOne.x - nodeTwo.x);
        int y = Mathf.Abs(nodeOne.y - nodeTwo.y);

        if (x > y)
            return 14 * y + 10 * (x - y);
        return 14 * x + 10 * (y - x);
    }

    List<Vector3> GetPath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();

        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }
}
