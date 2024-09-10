public interface ITraveler
{
    public bool CanTravelNode(NodeTravelType type);
    public float GetNodeCostToTravel(NodeTravelType type);
}

public interface IPlace
{
    public void ActionOnPlace();
}