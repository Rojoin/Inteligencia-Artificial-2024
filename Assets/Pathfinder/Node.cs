using System.Collections.Generic;

public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;
    private bool isBlocked;
    private List<INode> neighbours = new List<INode>();

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

    public INode[] GetNeighbors()
    {
        return neighbours.ToArray();
    }
}