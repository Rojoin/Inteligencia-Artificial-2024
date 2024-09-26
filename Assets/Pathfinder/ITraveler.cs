public interface ITraveler
{
    public bool CanTravelNode(NodeTravelType type);
    public float GetNodeCostToTravel(NodeTravelType type);
}

public interface IPlace
{
    public void ActionOnPlace();
}
public interface IFlock
{
    public BoidAgent GetBoid();
}
public interface IAlarmable
{
    public void InvokeAlarmOn();
    public void InvokeAlarmOff();
}