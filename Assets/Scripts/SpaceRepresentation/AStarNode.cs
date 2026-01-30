using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AStarNode : IComparable<AStarNode>
{
    private AStarNode parent;
    public AStarNode Parent { get { return parent; } }
    private GraphNode graphNode;
    public GraphNode GraphNode { get { return graphNode; } }
    public Vector3 Location { get { return graphNode.Location; } }
    private float gScore;
    public float GScore { get { return gScore; } }
    private float hScore;
    public float HScore { get { return hScore; } }
    public float FScore { get { return gScore + hScore; } }

    public AStarNode(AStarNode _parent, GraphNode _graphNode, float _hScore)
    {
        AStarPathFinder.nodesOpened += 1;
        this.parent = _parent;
        this.graphNode = _graphNode;
        this.gScore = 0f;
        if (parent != null)
        {
            this.gScore = parent.gScore + 1f;
        }
        this.hScore = _hScore;
    }

    public float GetFScore()
    {
        return gScore + hScore;
    }

    public GraphNode GetGraphNode()
    {
        return graphNode;
    }

    public float GetGScore()
    {
        return gScore;
    }

    public int CompareTo(AStarNode other)
    {
        if (other == null) return 1;

        AStarNode otherNode = other as AStarNode;
        if (otherNode != null)
            return this.FScore.CompareTo(otherNode.FScore);
        else
            throw new ArgumentException("Object is not an AStarNode");
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        AStarNode otherNode = obj as AStarNode;
        if (otherNode.Location == Location && otherNode.gScore.Equals(gScore) && otherNode.hScore.Equals(hScore))
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return Location.GetHashCode() + GScore.GetHashCode() + HScore.GetHashCode();
    }
}
