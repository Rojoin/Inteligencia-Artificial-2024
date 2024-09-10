using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Vector2Graph<NodeType> : IGraph<NodeType>, IDistance<NodeType>
    where NodeType : INode<UnityEngine.Vector2>, INode, new()
{
    public List<NodeType> nodes = new List<NodeType>();
    public NodeType[,] nodesMatrix;
    private System.Random random = new System.Random();

    public Vector2Graph(int x, int y, float offSet)
    {
        int counter = 0;
        nodesMatrix = new NodeType[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetID(counter++);
                node.SetCoordinate(new UnityEngine.Vector2(i * offSet, j * offSet));
                nodes.Add(node);
                nodesMatrix[i, j] = node;
            }
        }

        SetCardinalConnections(x, y);
        SetRandomType();
    }

    private void SetRandomType()
    {
        foreach (NodeType node in nodes)
        {
            int range = Random.Range(0, Enum.GetValues(typeof(NodeTravelType)).Length);
            NodeTravelType nodeTravelType = (NodeTravelType)range;
            node.SetNodeType(nodeTravelType);
            node.SetWeight(range);
            switch (nodeTravelType)
            {
                case NodeTravelType.Mine:
                  node.SetPlace(new Mine());
                    break;
                case NodeTravelType.HumanCenter:
                  node.SetPlace(new HumanCenter());
                    break;
                case NodeTravelType.Grass:
                    break;
                case NodeTravelType.Rocks:
                    break;
                case NodeTravelType.Water:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (nodeTravelType == NodeTravelType.Water)
            {
                node.SetBlocked();
            }
        }
    }
    

    private void SetCardinalConnections(int x, int y)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (j + 1 < y)
                {
                    nodesMatrix[i, j].SetNeighbor(nodesMatrix[i, j + 1]);
                }

                if (j - 1 > 0)
                {
                    nodesMatrix[i, j].SetNeighbor(nodesMatrix[i, j - 1]);
                }

                if (i + 1 < x)
                {
                    nodesMatrix[i, j].SetNeighbor(nodesMatrix[i + 1, j]);
                }

                if (i - 1 > 0)
                {
                    nodesMatrix[i, j].SetNeighbor(nodesMatrix[i - 1, j]);
                }
            }
        }
    }

    private void MakeRandomConnection(int index, NodeType current)
    {
        int randomIndex = random.Next(nodes.Count);
        while (randomIndex == index)
        {
            randomIndex = random.Next(nodes.Count);
        }
    }

    public float GetManhattanDistance(NodeType a, NodeType b)
    {
        return Mathf.Abs(a.GetCoordinate().x - b.GetCoordinate().x) +
               Mathf.Abs(a.GetCoordinate().y - b.GetCoordinate().y);
    }

    public float GetEuclideanDistance(NodeType a, NodeType b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.GetCoordinate().x - b.GetCoordinate().x, 2) +
                          Mathf.Pow(Mathf.Abs(a.GetCoordinate().y - b.GetCoordinate().y), 2));
    }

    public ICollection<NodeType> GetNodes()
    {
        return nodes;
    }
}