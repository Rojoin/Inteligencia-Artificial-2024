using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector2IntGrapf<NodeType>
    where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
{
    public List<NodeType> nodes = new List<NodeType>();
    private System.Random random = new System.Random();
    public Vector2IntGrapf(int x, int y)
    {
        int counter = 0;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetID(counter++);
                node.SetCoordinate(new UnityEngine.Vector2Int(i, j));
                nodes.Add(node);
            }
        }

        for (int index = 0; index < nodes.Count; index++)
        {
            NodeType current = nodes[index];
            MakeRandomConnection(index, current);
            MakeRandomConnection(index, current);
        }
    }

    private void MakeRandomConnection(int index, NodeType current)
    {
        int randomIndex = random.Next(nodes.Count);
        while (randomIndex == index)
        {
            randomIndex = random.Next(nodes.Count);
        }

        current.SetNeighbor(new TransitionToNode(randomIndex));
        nodes[randomIndex].SetNeighbor(new TransitionToNode(index));
    }
}