public interface IDistance<NodeType> where NodeType : INode
{
    public int GetManhattanDistance(NodeType a, NodeType b);
    public int GetEuclideanDistance(NodeType a, NodeType b);
}