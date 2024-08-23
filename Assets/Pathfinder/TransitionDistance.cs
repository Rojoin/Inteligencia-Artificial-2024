public class TransitionDistance<Node> : TransitionToNode<Node>where Node : INode
{
    protected float heuristics;

    public TransitionDistance(Node destination, float heuristics) : base(destination)
    {
        this.heuristics = heuristics;
    }

    public float GetDistance() => heuristics;
}