using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : GreedyPathFinder
{
    public static int nodesOpened = 0;

    //ASSIGNMENT 2: EDIT BELOW THIS LINE, IMPLEMENT A* 
    // goal: fix the path-finding by implementing A*
    public override Vector3[] CalculatePath(GraphNode startNode, GraphNode goalNode)
    {
        nodesOpened = 0;

        AStarNode start = new AStarNode(null, startNode, Heuristic(startNode, goalNode));
        float gScore = 0;

        PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>();
        openSet.Enqueue(start);
        Dictionary<string, float> gScores = new Dictionary<string, float>();

        int attempts = 0;
        while (openSet.Count() > 0 && attempts < 10000)
        {
            attempts += 1;
            AStarNode currNode = openSet.Dequeue();

            //Did we find the goal?
            if (currNode.Location == goalNode.Location)
            {
                Debug.Log("CHECKED " + nodesOpened + " NODES");
                //Reconstruct the path?
                return ReconstructPath(start, currNode);
            }

            //Check each neighbor
            foreach (GraphNode neighbour in currNode.GraphNode.Neighbors)
            {
                gScore = currNode.GetGScore() + ObstacleHandler.Instance.GridSize;

                AStarNode aStarNeighbour = new AStarNode(currNode, neighbour, Heuristic(neighbour, goalNode));
                openSet.Enqueue(aStarNeighbour);

            }
        }
        Debug.Log("CHECKED " + nodesOpened + " NODES");
        return null;
    }
    //ASSIGNMENT 2: EDIT ABOVE THIS LINE, IMPLEMENT A*

    //EXTRA CREDIT ASSIGNMENT 2 EDIT BELOW THIS LINE
    public float Heuristic(GraphNode currNode, GraphNode goalNode)
    {
        return (Mathf.Abs(currNode.Location.x - goalNode.Location.x) + Mathf.Abs(currNode.Location.y - goalNode.Location.y));
    }
    //EXTRA CREDIT ASSIGNMENT 2 EDIT ABOVE THIS LINE

    //Code for reconstructing the path, don't edit this.
    private Vector3[] ReconstructPath(AStarNode startNode, AStarNode currNode)
    {
        List<Vector3> backwardsPath = new List<Vector3>();

        while (currNode != startNode)
        {
            backwardsPath.Add(currNode.Location);
            currNode = currNode.Parent;
        }
        backwardsPath.Reverse();

        return backwardsPath.ToArray();
    }
}



