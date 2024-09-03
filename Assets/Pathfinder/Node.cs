using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node<Coordinate> : INode<Coordinate>, IEquatable<Node<Coordinate>>
    where Coordinate : IEquatable<Coordinate>
{
    private Coordinate coordinate;
    private int weight;
    public bool isBlocked = false;
    public int id;
    [SerializeField] private List<INode<Coordinate>>neighbours = new();

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

    public int GetWeight() => weight;

    public void SetID(int id) => this.id = id;

    public void SetNeighbor(INode<Coordinate> tNode)
    {
        if (!neighbours.Contains(tNode))
        {
            neighbours.Add(tNode);
        }
    }

    public ICollection<INode<Coordinate>> GetNeighbors()
    {
        return neighbours;
    }

    public bool Equals(Node<Coordinate> other)
    {
        return isBlocked == other.isBlocked && coordinate.Equals(other.coordinate) && neighbours == other.neighbours;
    }
}