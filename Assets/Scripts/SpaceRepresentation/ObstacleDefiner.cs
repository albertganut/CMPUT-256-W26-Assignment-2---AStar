using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDefiner : MonoBehaviour
{
	protected float xBound;
	protected float yBound;

	public float XBound { get { return xBound; } }
	public float YBound { get { return yBound; } }
	public virtual Vector2[][] GetObstaclePoints()
	{
		return null;
	}


    
}
