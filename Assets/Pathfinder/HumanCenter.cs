using System.Collections.Generic;
using UnityEngine;


public abstract class HumanCenterBase
{
}

public class HumanCenter<NodeType, Coordinate> : HumanCenterBase, IPlace where NodeType : class, INode<Coordinate>
{
    public NodeType currentNode;
    private List<Villager> _villagers = new List<Villager>();
    private IGraph<NodeType,Coordinate> graph;
    private AStarPathfinder<NodeType, Coordinate> a = new AStarPathfinder<NodeType, Coordinate>();
    private List<NodeType> goldMines = new List<NodeType>();

    public void SetGraph(IGraph<NodeType,Coordinate> graph)
    {
        this.graph = graph;
    }

    public void AddGoldNode(NodeType node)
    {
        goldMines.Add(node);
    }

    public void SetNode(NodeType node)
    {
        currentNode = node;
    }

    public void SpawnVillager()
    {
    }

    public void ActionOnPlace()
    {
    }

    public List<NodeType> GetNewDestination(ITraveler traveler)
    {
        //Todo:Change to grab vornoid GoldMine
        return PathFinderManager<NodeType, Coordinate>.GetPath(currentNode, goldMines[0], traveler);
    }
}