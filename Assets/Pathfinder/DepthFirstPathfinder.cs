using System;
using System.Collections.Generic;

public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    protected override int Distance(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override TransitionToNode[] GetNeighbors(NodeType node)
    {
        return node.GetNeighbors();
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
        return A.IsEqual(B);
    }
}
