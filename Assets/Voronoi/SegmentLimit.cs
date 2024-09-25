using UnityEngine;

[System.Serializable]
public class SegmentLimit
{
    [SerializeField] private Transform origin;
    [SerializeField] private Transform final;
    [SerializeField] private DirectionLimit directionLimit = DirectionLimit.None;
    private Vector2 opositePosition;

    public Vector2 Origin => origin.position;
    public Vector2 Final => final.position;

    public Vector2 GetOpositePosition (Vector2 pos)
    {
        Vector2 newPos = Vector2.zero;
        float distanceX = Mathf.Abs(Mathf.Abs(pos.x) - Mathf.Abs(origin.position.x)) * 2;
        float distanceZ = Mathf.Abs(Mathf.Abs(pos.y) - Mathf.Abs(origin.position.y)) * 2;

        switch (directionLimit)
        {
            case DirectionLimit.None:
                Debug.LogWarning("Está en None el Limite.");
                break;
            case DirectionLimit.Left:
                newPos.x = pos.x - distanceX;
                newPos.y = pos.y;
                break;
            case DirectionLimit.Up:
                newPos.x = pos.x;
                newPos.y = pos.y+ distanceZ;
                break;
            case DirectionLimit.Right:
                newPos.x = pos.x + distanceX;
                newPos.y = pos.y;
                break;
            case DirectionLimit.Down:
                newPos.x = pos.x;
                newPos.y = pos.y- distanceZ;
                break;
            default:
                Debug.LogWarning("Default el Limite.");
                break;
        }

        opositePosition = newPos;
        return newPos;
    }
}

enum DirectionLimit
{
    None,
    Left,
    Up,
    Right,
    Down
}