public class TransitionToNode<Node> where Node : INode
{
    protected Node destination;

    public TransitionToNode(Node destination)
    {
        this.destination = destination;
    }
    public Node GetDestination() => destination;
}