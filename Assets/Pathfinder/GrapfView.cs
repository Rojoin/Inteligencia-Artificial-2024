using UnityEngine;

public class GrapfView : MonoBehaviour
{
    public Vector2IntGrapf<Node<Vector2Int>> grapf;
    public int nodesX = 3;
    public int nodesY = 3;

    void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(nodesX, nodesY);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node.IsBloqued())
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