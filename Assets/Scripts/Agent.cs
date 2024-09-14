using System;
using System.Collections;
using System.Collections.Generic;
using Miner;
using Unity.VisualScripting;
using UnityEngine;

public enum Behaviour
{
    Patrol,
    Chase,
    Explode
}

public enum Flags
{
    OnTargetReach,
    OnTargetNear,
    OnTargetLost
}

public class Agent : MonoBehaviour, ITraveler
{
    private FSM<MinerStates, MinerFlags> fsm;
    
    [SerializeField] private float speed = 10;
    [SerializeField] private float chaseDistance = 0.2f;

    private int gold = 0;
    private int energy = 3;
    private int maxGold = 15;
    private float timeBetweenGold = 1.0f;
    public BoidAgent boid;

    public List<Node<Vector2>> path;
    private IPlace currentPlace;

//Todo: Make Boid Connection
    private Node<Vector2> startNode;
    private Node<Vector2> destinationNode;
    private Coroutine startPathFinding;
 
    public GrapfView grafp;
    private AStarPathfinder<Node<Vector2>, Vector2> Pathfinder =
        new AStarPathfinder<Node<Vector2>, Vector2>();

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
        startNode = grafp.graph.nodes[0];
        destinationNode = grafp.graph.nodes[^1];

        //path = Pathfinder.FindPath(startNode, destinationNode, grafp.graph, this);
        currentPlace = startNode.GetPlace();
        fsm = new FSM<MinerStates, MinerFlags>();

        Action<int> setGold;
        Action<int> setEnergy;
        Action<List<Node<Vector2>>> setPath;


        fsm.AddBehaviour<IdleState>(MinerStates.Idle, onTickParametes: () => new object[]
        {
            setEnergy = a => energy = a, currentPlace,path,this,setPath = list => path = list,
        });
        fsm.AddBehaviour<MiningState>(MinerStates.Mining, onTickParametes: () => new object[]
        {
            gold, maxGold, energy, Time.deltaTime, timeBetweenGold,
            setGold = value => gold = value,
            setEnergy = a => energy = a,
        }, onEnterParametes: () => new object[]
        {
            currentPlace
        });
        fsm.AddBehaviour<TravelState>(MinerStates.Travel, onTickParametes: () => new object[]
        {
            this.transform, path, transform.up * 2, speed, chaseDistance
        });

        fsm.SetTransition(MinerStates.Travel, MinerFlags.OnStartMining, MinerStates.Mining);
        fsm.SetTransition(MinerStates.Travel, MinerFlags.OnWaitingOnCenter, MinerStates.Idle);
        fsm.SetTransition(MinerStates.Idle, MinerFlags.OnStartMining, MinerStates.Mining,
            ()=>
            {
                currentPlace = path[^1].GetPlace();
            });
        fsm.SetTransition(MinerStates.Mining, MinerFlags.OnGoingToCenter, MinerStates.Travel,
            () => { path.Reverse(); });
        fsm.SetTransition(MinerStates.Mining, MinerFlags.OnEmptyEnergy, MinerStates.Idle);
        fsm.SetTransition(MinerStates.Idle, MinerFlags.OnGoingToMine, MinerStates.Travel,
            ()=>
            {
                currentPlace = path[^1].GetPlace();
            });

        fsm.ForceState(MinerStates.Idle);
    }

    void Start()
    {
        int stateCount = 0;
    }

    private void Update()
    {
        fsm?.Tick();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            Gizmos.color = Color.blue;
        }
    }

    public virtual bool CanTravelNode(NodeTravelType type)
    {
        return true;
    }

    public float GetNodeCostToTravel(NodeTravelType type)
    {
        return type switch
        {
            NodeTravelType.Mine => 0,
            NodeTravelType.HumanCenter => 0,
            NodeTravelType.Grass => 1,
            NodeTravelType.Rocks => 2,
            NodeTravelType.Water => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
}