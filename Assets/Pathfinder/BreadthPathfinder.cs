using System;
using System.Collections.Generic;

public class BreadthPathfinder<NodeType, Coordinate> : Pathfinder<NodeType, Coordinate>
    where NodeType : class, INode<Coordinate>
{
    protected override float Distance(NodeType A, NodeType B)
    {
        throw new NotImplementedException();
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        ICollection<INode<Coordinate>> neighbors = node.GetNeighbors();
        List<NodeType> neighborsList = new List<NodeType>();

        foreach (var neighbor in neighbors)
        {
            neighborsList.Add(neighbor as NodeType);
        }

        return neighborsList;
    }

    protected override bool IsBloqued(NodeType node)
    {
       return node.IsBlocked();
    }

    protected override bool IsImpassable(NodeType node, ITraveler traveler)
    {
        throw new NotImplementedException();
    }

    protected override float MoveToNeighborCost(NodeType A, NodeType b, ITraveler iTraveler)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.IsEqual(B);
    }
}