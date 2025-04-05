using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(seeker.position, target.position);
        }
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                return;
            }
            foreach (Node neighbour in grid.GetNeigbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int NewMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                if (NewMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = NewMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }
    int getDistance(Node nodeA, Node nodeB)
    {
        // change this to Sebastian lagues orginal code
        int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
