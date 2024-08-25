using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node<Coordinate> : INode, INode<Coordinate>, IEquatable<Node<Coordinate>>
    where Coordinate : IEquatable<Coordinate>
{
    private Coordinate coordinate;
    public bool isBlocked = false;
    public int id;
    [SerializeField] private List<TransitionToNode> neighbours = new();

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool IsBloqued()
    {
        return isBlocked;
    }

    public bool IsEqual(INode other)
    {
        return coordinate.Equals(((Node<Coordinate>)other).GetCoordinate());
    }

    public int GetID()
    {
        return id;
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public void SetNeighbor(TransitionToNode tNode)
    {
        if (!neighbours.Contains(tNode))
        {
            neighbours.Add(tNode);
        }
    }

    public TransitionToNode[] GetNeighbors()
    {
        return neighbours.ToArray();
    }

    public bool Equals(Node<Coordinate> other)
    {
        return isBlocked == other.isBlocked && coordinate.Equals(other.coordinate) && neighbours == other.neighbours;
    }
}