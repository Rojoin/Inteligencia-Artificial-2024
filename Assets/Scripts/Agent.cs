using System;
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
    private FSM<Behaviour,Flags> fsm;
    public Transform[] waypoints;
    public Transform chaseTarget;
    [SerializeField] private float speed;
    [SerializeField] private float chaseDistance = 0.2f;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;

    void Start()
    {
        int stateCount = 0;
        fsm = new FSM<Behaviour,Flags>();
        fsm.AddBehaviour<PatrolState>(Behaviour.Patrol,onTickParametes: () => new object[] {this.transform,waypoints[0],waypoints[1],chaseTarget, speed,chaseDistance});
        fsm.AddBehaviour<ChaseState>(Behaviour.Chase,onTickParametes: () => new object[] {this.transform,chaseTarget, speed,explodeDistance,lostDistance});
        fsm.AddBehaviour<ExplodeState>(Behaviour.Explode);
        
        fsm.SetTransition(Behaviour.Patrol,Flags.OnTargetNear,Behaviour.Chase);
        fsm.SetTransition(Behaviour.Chase,Flags.OnTargetReach,Behaviour.Explode);
        fsm.SetTransition(Behaviour.Chase,Flags.OnTargetLost,Behaviour.Patrol);
        fsm.ForceState(Behaviour.Patrol);
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