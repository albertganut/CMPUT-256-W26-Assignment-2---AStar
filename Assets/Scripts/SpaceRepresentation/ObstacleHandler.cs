using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObstacleHandler : MonoBehaviour
{
	public Material obstacleLineMaterial;

	public static ObstacleHandler Instance;
	private List<Polygon> obstacles = new List<Polygon>();
	public Polygon[] Obstacles{ get { return obstacles.ToArray(); } }
	private ObstacleDefiner obstacleDefiner;
    
	public float XBound { get { return obstacleDefiner.XBound; } }
	public float YBound { get { return obstacleDefiner.YBound; } }

	//Size of grid points
	private float gridSize = 0.2f;
	public float GridSize { get { return gridSize; } }

	//Initialize this singleton
	public void Init(ObstacleDefiner _obstacleDefiner)
	{
		ObstacleHandler.Instance = this;
		obstacleDefiner = _obstacleDefiner;
	}

	//Creates and renders an individual obstacle
	private void CreateAndRenderObstacle(Vector2[] points, string obstacleName = "")
	{
		GameObject newObstacleObj = new GameObject();
		if (obstacleName.Length > 0)
		{
			newObstacleObj.name = obstacleName;
		}
		Polygon obstacle = new Polygon(points, obstacleLineMaterial);//newObstacleObj.GetComponent<Polygon>();

		obstacles.Add(obstacle);

		obstacle.RenderPolygon(newObstacleObj);
	}

	//Creates and renders all obstacles
	public void CreateAndRenderObstacles(Vector2[][] arrayOfPoints)
	{
		for(int o=0; o<arrayOfPoints.Length; o++)
		{
			Vector2[] points = arrayOfPoints[o];
			CreateAndRenderObstacle(points, "Obstacle " + o);
		}
	}

    //Do any polygons intersect between these two  points?
	public bool AnyIntersect(Vector2 pt1, Vector2 pt2) {
		foreach (Polygon o in obstacles)
		{
			if (o.AnyIntersect(pt1, pt2)) {
				return true;
			}
		}
		return false;
	}

    //Purely so one doesn't have to convert vector3 to vector2
    public bool AnyIntersect(Vector3 pt1, Vector3 pt2)
    {
        return AnyIntersect(new Vector2(pt1.x, pt1.y), new Vector2(pt2.x, pt2.y));
    }

    //Returns true if this point is in any obstacle
    public bool PointInObstacles(Vector2 pnt)
	{
		foreach (Polygon obst in obstacles)
		{
			if (obst.ContainsPoint(pnt))
			{
				return true;
			}
		}
		return false;
	}

    //Returns the corners of the map
    public Vector2[] GetMapCorners()
    {
        return new Vector2[] { new Vector2(ObstacleHandler.Instance.XBound * -1, ObstacleHandler.Instance.YBound * -1),
            new Vector2(ObstacleHandler.Instance.XBound * -1, ObstacleHandler.Instance.YBound),
        new Vector2(ObstacleHandler.Instance.XBound, ObstacleHandler.Instance.YBound),
        new Vector2(ObstacleHandler.Instance.XBound, ObstacleHandler.Instance.YBound * -1)};
    }

    //Returns an array of obstacle points
    public Vector2[] GetObstaclePoints()
    {
        List<Vector2> pointsList = new List<Vector2>();

        foreach(Polygon o in obstacles)
        {
            foreach(Vector2 p in o.Points)
            {
                pointsList.Add(p);
            }
        }

        return pointsList.ToArray();
    }

}
