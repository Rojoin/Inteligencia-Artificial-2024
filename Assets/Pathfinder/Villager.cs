using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour , ITraveler
{
    public GrapfView grafp;
    private AStarPathfinder<Node<Vector2Int>,Vector2Int> Pathfinder = new AStarPathfinder<Node<Vector2Int>,Vector2Int>();
    private Node<Vector2Int> startNode; 
    private Node<Vector2Int> destinationNode;

    private void OnEnable()
    {
        // int firstRandomCard = Random.Range(0, grafp.grapf.nodes.Count);
        // startNode = grafp.grapf.nodes[firstRandomCard];
        //
        // int secondRandom = Random.Range(0, grafp.grapf.nodes.Count);
        // while (firstRandomCard == secondRandom)
        // {
        //     secondRandom = Random.Range(0, grafp.grapf.nodes.Count);
        // }
        //
        // destinationNode = grafp.grapf.nodes[secondRandom];

        startNode = grafp.grapf.nodes[0];
        destinationNode = grafp.grapf.nodes[^1];
        
        List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode, grafp.grapf, this);
        StartCoroutine(Move(path));
    }
    public IEnumerator Move(List<Node<Vector2Int>> path) 
    {
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