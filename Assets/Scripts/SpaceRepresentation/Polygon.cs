using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Polygon
{
	private Vector2[] points;
    public Vector2[] Points { get { return points; } }
	private float lineWidth = 0.02f;
	private Color lineColor = new Color(0.129f, 0.129f, 1.0f);
	private Material lineMaterial;

	public Polygon() { }

	public Polygon(Vector2[] _points)
    {
		points = _points;
    }

	public Polygon(Vector2[] _points, Material _lineMaterial)
	{
		points = _points;
		lineMaterial = _lineMaterial;
	}

	public void SetLineMaterial(Material _lineMaterial)
    {
		lineMaterial = _lineMaterial;
    }

    public void SetPoints(Vector2[] _points)
    {
        this.points = _points;
    }

	// Given three colinear points p, q, r, the function checks if 
	// point q lies on line segment 'pr' 
	bool onSegment(Vector2 p, Vector2 q, Vector2 r)
	{
		if (q[0] <= Mathf.Max(p[0], r[0]) && q[0] >= Mathf.Min(p[0], r[0]) &&
			q.y <= Mathf.Max(p[1], r[1]) && q[1] >= Mathf.Min(p[1], r[1])) {
			return true;
		}

		return false;
	}

	// To find orientation of ordered triplet (p, q, r). 
	// The function returns following values 
	// 0 --> p, q and r are colinear 
	// 1 --> Clockwise 
	// 2 --> Counterclockwise 
	int orientation(Vector2 p, Vector2 q, Vector2 r)
	{
		// See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
		// for details of below formula. 
		float val = (q[1] - p[1]) * (r[0] - q[0]) -
				(q[0] - p[0]) * (r[1] - q[1]);

		if (val == 0) return 0; // colinear 

		return (val > 0) ? 1 : 2; // clock or counterclock wise 
	}

	// The main function that returns true if line segment 'p1q1' 
	// and 'p2q2' intersect. 
	private bool doIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
	{
		// Find the four orientations needed for general and 
		// special cases 
		int o1 = orientation(p1, q1, p2);
		int o2 = orientation(p1, q1, q2);
		int o3 = orientation(p2, q2, p1);
		int o4 = orientation(p2, q2, q1);

		// General case 
		if (o1 != o2 && o3 != o4)
			return true;

		// Special Cases 
		// p1, q1 and p2 are colinear and p2 lies on segment p1q1 
		if (o1 == 0 && onSegment(p1, p2, q1)) return true;

		// p1, q1 and q2 are colinear and q2 lies on segment p1q1 
		if (o2 == 0 && onSegment(p1, q2, q1)) return true;

		// p2, q2 and p1 are colinear and p1 lies on segment p2q2 
		if (o3 == 0 && onSegment(p2, p1, q2)) return true;

		// p2, q2 and q1 are colinear and q1 lies on segment p2q2 
		if (o4 == 0 && onSegment(p2, q1, q2)) return true;

		return false; // Doesn't fall in any of the above cases 
	}

	//Return the lines of this polygon
	public Vector2[][] GetLines()
	{
		Vector2[][] lines = new Vector2[points.Length][];
		for (int i = 0; i < points.Length; i++)
		{
			Vector2[] line = new Vector2[2];
			if (i < points.Length - 1)
			{
				line[0] = points[i];
				line[1] = points[i + 1];
			}
			else
			{
				line[0] = points[i];
				line[1] = points[0];
			}
			lines[i] = line;
		}
		return lines;
	}

	//Returns true if the passed in pnt is one of polygon's points
	public bool HasPoint(Vector2 pnt)
    {
		return points.Contains(pnt);
    }

	//Returns true if the passed in line is one of polygon's lines
	public bool HasLine(Vector2[] line)
	{
		foreach(Vector2[] polygonLine in GetLines())
        {
			if( (line[0].Equals(polygonLine[0]) && line[1].Equals(polygonLine[1]))
				|| (line[1].Equals(polygonLine[0]) && line[0].Equals(polygonLine[1])))
            {
				return true;
            }
        }

		return false;
	}

	//Is there any intersect between the polygon lines and the line segment passed in
	//Note: this includes whether either point equals a point on the line
	public bool AnyIntersect(Vector2 pt1, Vector2 pt2)
	{
		Vector2[][] lines = GetLines();
		foreach (Vector2[] line in lines)
		{
			if (doIntersect(pt1, pt2, line[0], line[1]))
			{
				return true;
			}
		}
		return false;

	}

	//Is there any intersect between the polygon lines and the line segment passed in
	//Note: this does not include whether either point equals a point on the line
	public bool AnyIntersectWithoutSharedPoint(Vector2 pt1, Vector2 pt2)
	{
		Vector2[][] lines = GetLines();
		foreach (Vector2[] line in lines)
		{
			if (doIntersect(pt1, pt2, line[0], line[1]) && line[0]!=pt1 && line[0]!=pt2 && line[1]!=pt1 && line[1]!=pt2)
			{
				return true;
			}
		}
		return false;

	}

	//Does this obstacle contain the Vector2 point passed in.
	public bool ContainsPoint(Vector2 pnt)
	{
		Vector2[][] lines = GetLines();
		int numIntersections = 0;
		foreach (Vector2[] line in lines)
		{
			if (doIntersect(new Vector2(-4.2f, -20.1f), pnt, line[0], line[1]))
			{
				numIntersections += 1;
			}
		}

		return numIntersections % 2 == 1;
	}

	//Checks whether the given pnt is on any of the polygon's lines
	public bool AnyPointOnAnyLine(Vector2 pnt)
    {
		foreach(Vector2[] line in GetLines())
        {
			if(onSegment(line[0], pnt, line[1]))
            {
				return true;
            }
		}

		return false;
    }

	//Calculate and return the midpoints of each polygon line
	public Vector2[] GetMidpoints()
    {
		Vector2[][] lines = GetLines();
		Vector2[] midPoints = new Vector2[lines.GetLength(0)];

		for(int i = 0; i<lines.GetLength(0); i++)
        {
			Vector2 midPoint = lines[i][0];
			Vector2 difference = lines[i][1] - lines[i][0];
			midPoint += difference / 2;
			midPoints[i] = midPoint;
        }
		return midPoints;
	}

	// Return the cross product AB x BC.
	// The cross product is a vector perpendicular to AB
	// and BC having length |AB| * |BC| * Sin(theta) and
	// with direction given by the right-hand rule.
	// For two vectors in the X-Y plane, the result is a
	// vector with X and Y components 0 so the Z component
	// gives the vector's length and direction.
	public static float CrossProductLength(float Ax, float Ay,
		float Bx, float By, float Cx, float Cy)
	{
		// Get the vectors' coordinates.
		float BAx = Ax - Bx;
		float BAy = Ay - By;
		float BCx = Cx - Bx;
		float BCy = Cy - By;

		// Calculate the Z coordinate of the cross product.
		return (BAx * BCy - BAy * BCx);
	}

	//Check whether polygon is convex
	// Return True if the polygon is convex. as taken from http://csharphelper.com/blog/2014/07/determine-whether-a-polygon-is-convex-in-c/
	public bool IsConvex()
	{
		// For each set of three adjacent points A, B, C,
		// find the cross product AB · BC. If the sign of
		// all the cross products is the same, the angles
		// are all positive or negative (depending on the
		// order in which we visit them) so the polygon
		// is convex.
		bool got_negative = false;
		bool got_positive = false;
		int num_points = Points.Length;
		int B, C;
		for (int A = 0; A < num_points; A++)
		{
			B = (A + 1) % num_points;
			C = (B + 1) % num_points;

			float cross_product =
				CrossProductLength(
					Points[A].x, Points[A].y,
					Points[B].x, Points[B].y,
					Points[C].x, Points[C].y);
			if (cross_product < 0)
			{
				got_negative = true;
			}
			else if (cross_product > 0)
			{
				got_positive = true;
			}
			if (got_negative && got_positive) return false;
		}

		// If we got this far, the polygon is convex.
		return true;
	}

	//Render this polygon into the game
	public void RenderPolygon(GameObject polygonObject){		
		for (int i = 0; i < points.Length; i++){
			GameObject obj = new GameObject();
			LineRenderer line = obj.AddComponent<LineRenderer>();
			Vector2 point1, point2;
			if (i < points.Length - 1){
				point1 = points[i];
				point2 = points[i + 1];
			}
			else{
				point1 = points[i];
				point2 = points[0];
			}

			line.SetPosition(0, point1);
			line.SetPosition(1, point2);
			line.startWidth = lineWidth;
			line.endWidth = lineWidth;
			line.material = lineMaterial;
			obj.name = "" + i;
			line.startColor = lineColor;
			line.endColor = lineColor;
			obj.transform.parent = polygonObject.transform;

			PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();

			Vector2 perpindicularSlope1 = new Vector2(point2[0] - point1[0], point1[1]-point2[1]);
			Vector2 perpindicularSlope2 = new Vector2(point1[0] - point2[0], point2[1] - point1[1]);
			perpindicularSlope1.Normalize();
			perpindicularSlope2.Normalize();

			collider.points = new Vector2[4];
			collider.points[0] = new Vector2(point1[0] + perpindicularSlope1[0] * lineWidth / 2f, point1[1] + perpindicularSlope1[1] * lineWidth / 2f);
			collider.points[1] = new Vector2(point1[0] + perpindicularSlope2[0] * lineWidth / 2f, point1[1] + perpindicularSlope2[1] * lineWidth / 2f);
			collider.points[2] = new Vector2(point2[0] + perpindicularSlope2[0] * lineWidth / 2f, point2[1] + perpindicularSlope2[1] * lineWidth / 2f);
			collider.points[3] = new Vector2(point2[0] + perpindicularSlope1[0] * lineWidth / 2f, point2[1] + perpindicularSlope1[1] * lineWidth / 2f);

		}

    }
    public override bool Equals(object obj)
    {
		Polygon other = (Polygon)obj;

        if (other.Points.Length == Points.Length)
        {
			int matches = 0;
			foreach(Vector2 pnt in Points)
            {
				foreach(Vector2 pnt2 in other.Points)
                {
                    if (pnt.Equals(pnt2))
                    {
						matches++;
                    }
                }
            }

			return matches == Points.Length;
        }

		return false;
    }

	public override int GetHashCode()
    {
        return points.GetHashCode();
    }
	
}
