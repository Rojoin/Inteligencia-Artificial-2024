using System.Collections.Generic;

public interface IDistance<NodeType> where NodeType : INode
{
    public float GetManhattanDistance(NodeType a, NodeType b);
    public float GetEuclideanDistance(NodeType a, NodeType b);
}
public interface IGraph<NodeType>: IDistance<NodeType> where NodeType : INode
{
    public ICollection<NodeType> GetNodes();

}