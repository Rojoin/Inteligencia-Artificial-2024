public interface INode
{
    public bool IsBloqued();
    public bool IsEqual(INode other);
    //Todo: Change to Transitions
    public INode[] GetNeighbors();
}

public interface INode<Coorninate> 
{
    public void SetCoordinate(Coorninate coordinateType);
    public Coorninate GetCoordinate();
}
