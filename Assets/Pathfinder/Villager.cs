using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour, ITraveler
{
    public GrapfView grafp;
    private AStarPathfinder<Node<Vector2Int>, Vector2Int> Pathfinder =
        new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;
    private Coroutine startPathFinding;

    private void OnEnable()
    {
        if (startPathFinding != null)
        {
            StopCoroutine(startPathFinding);
        }

        startPathFinding = StartCoroutine(StartVillager());
    }

    public IEnumerator StartVillager()
    {
        yield return null;
        startNode = grafp.grapf.nodes[0];
        destinationNode = grafp.grapf.nodes[^1];

        List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode, grafp.grapf, this);
        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path)
    {
        transform.position = new Vector3(path[0].GetCoordinate().x, path[0].GetCoordinate().y);
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public virtual bool CanTravelNode(NodeTravelType type)
    {
        return true;
    }
}