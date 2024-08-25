public interface INode
{
    public bool IsBloqued();
    public bool IsEqual(INode other);
    public int GetID();
    public void SetID(int id);
    public void SetNeighbor(TransitionToNode tNode);

    //Todo: Change to Transitions
    public TransitionToNode[] GetNeighbors();
}

public interface INode<Coorninate>
{
    public void SetCoordinate(Coorninate coordinateType);
    public Coorninate GetCoordinate();
}