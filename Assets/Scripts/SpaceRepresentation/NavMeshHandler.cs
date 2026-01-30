using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NavMeshHandler : NodeHandler
{

    //Holds all of the nodes
    private Dictionary<string, GraphNode> nodeDictionary;
    private List<Polygon> meshes;

    public override void CreateNodes()
    {
        meshes = new List<Polygon>();
        nodeDictionary = new Dictionary<string, GraphNode>();

        //ASSIGNMENT 1 EXTRA CREDIT EDIT BELOW THIS LINE
        Vector2[] allPoints = GetPoints();

        foreach (Vector2 a in allPoints)
        {
            foreach (Vector2 b in allPoints)
            {
                foreach (Vector2 c in allPoints)
                {
                    if (!a.Equals(b) && !a.Equals(c) && !b.Equals(c))
                    {
                        //Change
                        meshes.Add(new Polygon(new Vector2[] { a, b, c }));
                    }

                }
            }
        }

        //ASSIGNMENT 1 EXTRA CREDIT EDIT ABOVE THIS LINE

        //Create graph from mesh line midpoints
        foreach (Polygon mesh in meshes)
        {
            Vector2[] midPoints = mesh.GetMidpoints();
            foreach (Vector2 midPoint in midPoints)
            {
                Vector3 loc = new Vector3(midPoint.x, midPoint.y);
                bool attemptToAdd = true;
                foreach(Polygon obstacle in ObstacleHandler.Instance.Obstacles) {
                    if (obstacle.AnyPointOnAnyLine(midPoint) || obstacle.ContainsPoint(midPoint))
                    {
                        attemptToAdd = false;
                        break;
                    }
                }

                if (attemptToAdd)
                {
                    if (!nodeDictionary.ContainsKey(loc.ToString()))
                    {
                        nodeDictionary.Add(loc.ToString(), new GraphNode(loc));
                    }
                }
            }

            foreach (Vector2 midPoint in midPoints)
            {
                Vector3 loc = new Vector3(midPoint.x, midPoint.y);
                foreach (Vector2 midPoint2 in midPoints)
                {
                    Vector3 loc2 = new Vector3(midPoint2.x, midPoint2.y);
                    if (!midPoint.Equals(midPoint2))
                    {
                        if (nodeDictionary.ContainsKey(loc.ToString()) && nodeDictionary.ContainsKey(loc2.ToString()))
                        {
                            nodeDictionary[loc.ToString()].AddNeighbor(nodeDictionary[loc2.ToString()]);
                        }
                    }
                }
            }
        }
    }

    //Visualize the mesh
    public override void VisualizeNodes()
    {
        foreach(Polygon mesh in meshes)
        {
            foreach(Vector2[] line in mesh.GetLines())
            {
                Debug.DrawLine(new Vector3(line[0].x, line[0].y), new Vector3(line[1].x, line[1].y));
            }
        }
    }

    //Get the set of all possible points for this specific scene
    private Vector2[] GetPoints()
    {
        Vector2[] mapCorners = ObstacleHandler.Instance.GetMapCorners();
        Vector2[] obstaclePnts = ObstacleHandler.Instance.GetObstaclePoints();

        Vector2[] pointsList = new Vector2[mapCorners.Length + obstaclePnts.Length];
        mapCorners.CopyTo(pointsList, 0);
        obstaclePnts.CopyTo(pointsList, mapCorners.Length);

        return pointsList;
    }

    public override GraphNode ClosestNode(Vector3 position)
    {
        float minDist = 1000;
        GraphNode closest = null;
        foreach (KeyValuePair<string, GraphNode> kvp in nodeDictionary)
        {
            float dist = (kvp.Value.Location - position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = kvp.Value;
            }
        }
        return closest;
    }
}
