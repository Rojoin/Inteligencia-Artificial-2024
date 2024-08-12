using System;
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
    private FSM fsm;
    Transform[] waypoints;
    Transform chaseTarget;
    [SerializeField] private float speed;
    [SerializeField] private float chaseDistance = 0.2f;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;

    void Start()
    {
        int stateCount = 0;
        fsm = new FSM(Enum.GetValues(typeof(Behaviour)).Length, Enum.GetValues(typeof(Flags)).Length);
        fsm.AddBehaviour<PatrolState>((int)Behaviour.Patrol,onTickParametes: () => new object[] {this.transform,waypoints[0],waypoints[1],chaseTarget, speed,chaseDistance});
        fsm.AddBehaviour<ChaseState>((int)Behaviour.Chase,onTickParametes: () => new object[] {this.transform,chaseTarget, speed,explodeDistance,lostDistance});
        fsm.AddBehaviour<ExplodeState>((int)Behaviour.Explode);
        
        fsm.SetTransition((int)Behaviour.Patrol,(int)Flags.OnTargetNear,(int)Behaviour.Chase);
        fsm.SetTransition((int)Behaviour.Chase,(int)Flags.OnTargetReach,(int)Behaviour.Explode);
        fsm.SetTransition((int)Behaviour.Chase,(int)Flags.OnTargetLost,(int)Behaviour.Patrol);
        fsm.ForceState((int)Behaviour.Patrol);
    }

    private void Update()
    {
        fsm.Tick();
    }
}