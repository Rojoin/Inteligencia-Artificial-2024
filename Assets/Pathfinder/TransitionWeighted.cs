public class TransitionWeighted<Node> : TransitionDistance<Node>where Node : INode
{
    protected float accumulativeWeight;

    public TransitionWeighted(Node destination, float heuristics, float accumulativeWeight) : base(destination, heuristics)
    {
        this.accumulativeWeight = accumulativeWeight;
    }

    public float GetAccumulativeWeight() => accumulativeWeight;
}