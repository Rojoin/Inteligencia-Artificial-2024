public class Mine : IPlace
{
    private int gold = 50;
    private int food = 15;

    public bool TryGetFood()
    {
        if (food > 0)
        {
            food--;
            return true;
        }

        return false;
    }

    public bool TryGetGold()
    {
        if (gold > 0)
        {
            gold--;
            return true;
        }

        return false;
    }

    public bool hasGold => gold > 0;

    public void SetFood(int food) => this.food = food;
    public void ActionOnPlace()
    {
        
    }
}