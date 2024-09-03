using System.Collections.Generic;

public class DijkstraPathfinder<NodeType, Coordinate> : Pathfinder<NodeType, Coordinate>
    where NodeType : class, INode<Coordinate>
{
    protected override int Distance(NodeType A, NodeType B)
    {
        return useManhattan
            ? graph.GetManhattanDistance(A, B)
            : graph.GetEuclideanDistance(A, B);
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
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.Equals(B);
    }
}