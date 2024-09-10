using System;
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

public class Agent : MonoBehaviour
{
    private FSM<MinerStates,MinerFlags> fsm;
    public Transform[] waypoints;
    public Transform chaseTarget;
    [SerializeField] private float speed { get; set; }
    [SerializeField] private float chaseDistance = 0.2f;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;


    void Start()
    {
        int stateCount = 0;
        fsm = new FSM<MinerStates,MinerFlags> ();
        Func<float> getFloat;
        Action<float> setFloat;
     //   fsm.AddBehaviour<PatrolState>(Behaviour.Patrol,onTickParametes: () => new object[]
     //   {
     //       this.transform,waypoints[0],waypoints[1],chaseTarget, speed,chaseDistance, Tuple.Create( getFloat = () => speed,setFloat = value => speed = value)
     //   });
     //   fsm.AddBehaviour<ChaseState>(Behaviour.Chase,onTickParametes: () => new object[] {this.transform,chaseTarget, speed,explodeDistance,lostDistance});
     //   fsm.AddBehaviour<ExplodeState>(Behaviour.Explode);
     //  
     //  fsm.SetTransition(Behaviour.Patrol,Flags.OnTargetNear,Behaviour.Chase);
     //  fsm.SetTransition(Behaviour.Chase,Flags.OnTargetReach,Behaviour.Explode);
     //  fsm.SetTransition(Behaviour.Chase,Flags.OnTargetLost,Behaviour.Patrol);
    //    fsm.ForceState(Behaviour.Patrol);
        
        
        fsm.AddBehaviour<IdleState>(Miner.MinerStates.Travel,onTickParametes: ()=> new object[]
        {
            
        });
        fsm.AddBehaviour<MiningState>(Miner.MinerStates.Travel,onTickParametes: ()=> new object[]
        {
            
        });
        fsm.AddBehaviour<TravelState>(Miner.MinerStates.Travel,onTickParametes: ()=> new object[]
        {
            
        });
    }

    private void Update()
    {
        fsm.Tick();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
           Gizmos.color = Color.green;
           Gizmos.DrawWireSphere(transform.position,chaseDistance);
           Gizmos.color = Color.blue;
           Gizmos.DrawWireSphere(transform.position,lostDistance);
           Gizmos.color = Color.red;
           Gizmos.DrawWireSphere(transform.position,explodeDistance);
        }
    }
}