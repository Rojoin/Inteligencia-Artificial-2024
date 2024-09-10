using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


public struct BehaviourActions
{
    public Dictionary<int, List<Action>> mainThreadBehaviours;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> multithreadBehaviours;
    public Action transitionBehaviour;

    public void AddMainThreadBehaviour(int executionOrder, Action behaviour)
    {
        if (mainThreadBehaviours == null)
        {
            mainThreadBehaviours = new Dictionary<int, List<Action>>();
        }

        if (!mainThreadBehaviours.ContainsKey(executionOrder))
        {
            mainThreadBehaviours.Add(executionOrder, new List<Action>());
        }

        mainThreadBehaviours[executionOrder].Add(behaviour);
    }

    public void AddMultiThreadBehaviour(int executionOrder, Action behaviour)
    {
        if (multithreadBehaviours == null)
        {
            multithreadBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
        }

        if (!multithreadBehaviours.ContainsKey(executionOrder))
        {
            multithreadBehaviours.TryAdd(executionOrder, new ConcurrentBag<Action>());
        }

        multithreadBehaviours[executionOrder].Add(behaviour);
    }

    public void SetTransitionBehavior(Action transitionBehaviour)
    {
        this.transitionBehaviour = transitionBehaviour;
    }
}

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaviourActions GetTickBehaviours(params object[] parameters);
    public abstract BehaviourActions GetEnterBehaviours(params object[] parameters);
    public abstract BehaviourActions GetExitBehaviours(params object[] parameters);
}


public sealed class ChaseState : State
{
    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);
        BehaviourActions behaviour = new BehaviourActions();
        behaviour.AddMainThreadBehaviour(0, (() =>
        {
            OwnerTransform.position +=
                (targetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        }));
        behaviour.AddMultiThreadBehaviour(0, (() => { Debug.Log("Whistle"); }));
        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(targetTransform.position, OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }
            else if (Vector3.Distance(targetTransform.position, OwnerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetLost);
            }
        });
        return behaviour;
    }

    public override BehaviourActions GetEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetExitBehaviours(params object[] parameters)
    {
        return default;
    }
}

public sealed class ExplodeState : State
{
    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviour = new BehaviourActions();

        behaviour.AddMultiThreadBehaviour(0, () => Debug.Log("F"));
        return behaviour;
    }

    public override BehaviourActions GetEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviour = new BehaviourActions();

        behaviour.AddMultiThreadBehaviour(0, () => Debug.Log("Cagaste Light"));
        return behaviour;
    }

    public override BehaviourActions GetExitBehaviours(params object[] parameters)
    {
        return default;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);
        BehaviourActions behaviour = new BehaviourActions();

        behaviour.AddMultiThreadBehaviour(0, () => Debug.Log("Patrolling"));

        Vector3 ownerTransformPosition = OwnerTransform.position;
        Vector3 actualTargetPosition = chaseTarget.position;
        behaviour.AddMultiThreadBehaviour(0, (() =>
        {
            if (actualTarget == null)
            {
                actualTarget = wayPoint1;
            }

            if (Vector3.Distance(ownerTransformPosition, actualTargetPosition) < 0.2f)
            {
                if (actualTarget == wayPoint1)
                    actualTarget = wayPoint2;
                else if (actualTarget == wayPoint2)
                    actualTarget = wayPoint1;
            }
        }));
        behaviour.AddMainThreadBehaviour(1, () =>
            {
                OwnerTransform.position +=
                    (actualTargetPosition - ownerTransformPosition).normalized * speed * Time.deltaTime;
            }
        );
        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(ownerTransformPosition, chaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetNear);
            }
        });
        return behaviour;
    }

    public override BehaviourActions GetEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetExitBehaviours(params object[] parameters)
    {
        return default;
    }
}