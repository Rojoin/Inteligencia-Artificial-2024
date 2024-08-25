using System.Collections.Generic;
using System.Linq;

public class BreadthPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    protected override int Distance(NodeType A, NodeType B)
    {
        throw new System.NotImplementedException();
    }

    protected override TransitionToNode[] GetNeighbors(NodeType node)
    {
        return node.GetNeighbors();
    
    }

    protected override bool IsBloqued(NodeType node)
    {
        throw new System.NotImplementedException();
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