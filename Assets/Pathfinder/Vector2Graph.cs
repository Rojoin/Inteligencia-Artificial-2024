using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

[Serializable]
public class Vector2Graph<NodeType> : IGraph<NodeType, UnityEngine.Vector2>
    where NodeType : class, INode<UnityEngine.Vector2>, INode, new()
{
    public List<NodeType> nodes = new List<NodeType>();
    public NodeType[,] nodesMatrix;
    private System.Random random = new System.Random();
    private CaravanFazade _caravanFazade = new();
    public List<NodeType> mines = new List<NodeType>();

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

        PathFinderManager<NodeType, Vector2>.graph = this;
        SetCardinalConnections(x, y);
        SetRandomType();
        SetRandomHumanCenter();
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
                case NodeTravelType.Grass:
                    break;
                case NodeTravelType.Rocks:
                    break;
                case NodeTravelType.Water:
                    break;
                default:
                    nodeTravelType = NodeTravelType.Grass;
                    node.SetNodeType(nodeTravelType);
                    node.SetWeight(1);
                    break;
            }

            if (nodeTravelType == NodeTravelType.Water)
            {
                node.SetBlocked();
            }
        }
    }

    private void SetRandomHumanCenter()
    {
        nodes[0].SetNodeType(NodeTravelType.HumanCenter);
        nodes[0].SetWeight(0);
        nodes[0].SetPlace(new HumanCenter<NodeType, UnityEngine.Vector2>());
        nodes[0].SetBlocked(false);
        HumanCenter<NodeType, Vector2> humanCenter = (HumanCenter<NodeType, UnityEngine.Vector2>)nodes[0].GetPlace();
        humanCenter.SetGraph(this);
        humanCenter.SetNode(nodes[0]);
        SetRandomMine(humanCenter);
        SetRandomMine(humanCenter);
        //SetRandomMine(humanCenter);
    }

    private void SetRandomMine(HumanCenter<NodeType, Vector2> humanCenter)
    {
        int randomNode = Random.Range(0, nodes.Count);
        if (nodes[randomNode].GetPlace() is HumanCenter<NodeType, Vector2> || nodes[randomNode].GetPlace() is Mine)
        {
            SetRandomMine(humanCenter);
        }
        else
        {
            if (CalculatePathToMine(humanCenter, nodes[randomNode], out var nodeToMine))
            {
                nodeToMine.SetNodeType(NodeTravelType.Mine);
                nodeToMine.SetWeight(0);
                nodeToMine.SetBlocked(false);
                nodeToMine.SetPlace(new Mine());
                mines.Add(nodeToMine);
                humanCenter.AddGoldNode(nodeToMine);
            }
            else
            {
                SetRandomMine(humanCenter);
            }
        }
    }

    private bool CalculatePathToMine(HumanCenter<NodeType, Vector2> humanCenter, NodeType node, out NodeType nodeToAdd)
    {
        List<NodeType> nodeTypes =
            PathFinderManager<NodeType, Vector2>.GetPath(humanCenter.currentNode, node, _caravanFazade);

        if (nodeTypes != null && nodeTypes.Count > 0)
        {
            nodeToAdd = node;
            return true;
        }
        else
        {
            List<NodeType> nodesOut = new List<NodeType>();
            var previousNode = node;
            nodesOut.Add(node);

            int maxIterations = 1000; // Limit the number of iterations to prevent infinite loop
            int iterations = 0;
            bool foundPath = false;

            while ((nodeTypes == null || nodeTypes.Count <= 0) && iterations < maxIterations)
            {
                bool foundNeighbor = false;

                foreach (INode<Vector2> neighbor in previousNode.GetNeighbors())
                {
                    NodeType nodeToChange = nodes[neighbor.GetID()];

                    if (!nodesOut.Contains(nodeToChange))
                    {
                        nodesOut.Add(nodeToChange);
                        nodeToChange.SetNodeType(NodeTravelType.Grass);
                        nodeToChange.SetWeight(1);
                        nodeToChange.SetBlocked(false);

                        // Attempt to find a new path after modifying the neighbor
                        nodeTypes = PathFinderManager<NodeType, Vector2>.GetPath(humanCenter.currentNode, nodeToChange,
                            _caravanFazade);

                        if (nodeTypes != null && nodeTypes.Count > 0)
                        {
                            foundPath = true;
                            break;
                        }

                        previousNode = nodeToChange;
                        foundNeighbor = true;
                        break;
                    }
                }

                if (!foundNeighbor)
                {
                    // If no valid neighbor is found, break out of the loop
                    break;
                }

                iterations++;
            }

            if (foundPath)
            {
                nodeToAdd = previousNode; // Add the valid node with a path
                return true;
            }

            // If no valid path is found
            nodeToAdd = null;
            return false;
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

    public Vector2 GetMediatrix(NodeType a, NodeType b)
    {
        //TODO: Make Mediatrix
        Vector2 perp = b.GetCoordinate() - a.GetCoordinate();
        Vector2 perpendicular = new Vector2(-perp.y, perp.x);
        //return GetEuclideanDistance(a, b) + perpendicular *0.5f;
        return default;
    }

    public ICollection<NodeType> GetNodes()
    {
        return nodes;
    }
}