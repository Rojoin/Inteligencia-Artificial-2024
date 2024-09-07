using System;
using UnityEngine;

public class GrapfView : MonoBehaviour
{
    public Vector2IntGrapf<Node<Vector2Int>> grapf;
    [SerializeField] private Sprite grass;
    [SerializeField] private Sprite rock;
    [SerializeField] private Sprite mine;
    [SerializeField] private Sprite water;
    [SerializeField] private Sprite humanCenter;
    public GameObject tile;
    public int nodesX = 3;
    public int nodesY = 3;

    void OnEnable()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(nodesX, nodesY);

        DrawMap(grapf);
    }

    private void DrawMap(Vector2IntGrapf<Node<Vector2Int>> vector2IntGrapf)
    {
        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            var position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            var newTile = Instantiate(tile, position, Quaternion.identity);
            newTile.GetComponent<SpriteRenderer>().sprite = GetSpriteType(node.GetNodeType());
        }
    }

    private Sprite GetSpriteType(NodeTravelType getNodeType)
    {
        return getNodeType switch
        {
            NodeTravelType.Mine => mine,
            NodeTravelType.HumanCenter => humanCenter,
            NodeTravelType.Grass => grass,
            NodeTravelType.Rocks => rock,
            NodeTravelType.Water => water,
            _ => throw new ArgumentOutOfRangeException(nameof(getNodeType), getNodeType, null)
        };
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node.IsBlocked())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Vector3 currentNodeCoordinate = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            Gizmos.DrawWireSphere(currentNodeCoordinate, 0.1f);
            foreach (INode<Vector2Int> neighborConnections in node.GetNeighbors())
            {
                Vector2Int vector2Int = neighborConnections.GetCoordinate();


                Vector3 nodePos = new Vector3(vector2Int.x, vector2Int.y);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(currentNodeCoordinate, nodePos);
            }
        }
    }
}