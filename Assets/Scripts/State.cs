using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class State
{
    public Action<int> OnFlag;
    public abstract List<Action> GetTickBehaviours(params object[] parameters);
    public abstract List<Action> GetEnterBehaviours(params object[] parameters);
    public abstract List<Action> GetExitBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);
        List<Action> behaviours = new List<Action>();
        behaviours.Add((() =>
        {
            OwnerTransform.position +=
                (targetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        }));
        behaviours.Add((() =>
        {
            Debug.Log("Whistle");
        }));
        behaviours.Add((() =>
        {
            if (Vector3.Distance(targetTransform.position,OwnerTransform.position)<explodeDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetReach);
            }
            else if (Vector3.Distance(targetTransform.position,OwnerTransform.position)>lostDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetLost);
            }
        }));
        return new List<Action>();
    }

    public override List<Action> GetEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }
}

public sealed class ExplodeState : State
{
    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        List<Action> behaviours = new List<Action>();
        behaviours.Add((() => Debug.Log("F")));
        return behaviours;
    }

    public override List<Action> GetEnterBehaviours(params object[] parameters)
    {
        List<Action> behaviours = new List<Action>();
        behaviours.Add((() => Debug.Log("Cagaste Light")));
        return behaviours;
    }

    public override List<Action> GetExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[4] as Transform;
        float speed = Convert.ToSingle(parameters[5]);
        float chaseDistance = Convert.ToSingle(parameters[6]);
        List<Action> behaviours = new List<Action>();
        behaviours.Add((() =>
        {
            if (actualTarget == null)
            {
                actualTarget = wayPoint1;
            }

            if (Vector3.Distance(OwnerTransform.position, actualTarget.position) < 0.2f)
            {
                if (actualTarget == wayPoint1)
                    actualTarget = wayPoint2;
                else if (actualTarget == wayPoint2)
                    actualTarget = wayPoint1;
            }

            OwnerTransform.position +=
                (actualTarget.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        }));
        behaviours.Add(() =>
        {
            if (Vector3.Distance(OwnerTransform.position, chaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetNear);
            }
        });
        return behaviours;
    }

    public override List<Action> GetEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }
}