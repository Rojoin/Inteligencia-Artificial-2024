using System;
using System.Collections.Generic;
using UnityEngine;

namespace Miner
{
    public enum MinerFlags
    {
        OnEmptyEnergy,
        OnStartMining,
        OnGoingToMine,
        OnGoingToCenter,
        OnAlarmResume
    }
    public enum MinerStates
    {
        Idle,
        Mining,
        Travel
    }
    public class IdleState : State
    {
        private Mine mine;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            int energy = (int)parameters[0];
            mine = parameters[1] as Mine;

            BehaviourActions behaviour = new BehaviourActions();
            behaviour.SetTransitionBehavior(() =>
            {
                if (mine.TryGetFood())
                {
                    OnFlag.Invoke(MinerFlags.OnStartMining);
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }

    public class MiningState : State
    {
        private int gold;
        private int energy;
        private float timer = 0;
        private Mine mine;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            gold = (int)parameters[0];
            int maxGold = (int)parameters[1];
            energy = (int)parameters[2];
            float deltaTime = (float)parameters[3];
            float timeBetweenGold = (float)parameters[4];


            BehaviourActions behaviour = new BehaviourActions();
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                timer += deltaTime;
                if (timer > timeBetweenGold && energy > 0 && mine.hasGold)
                {
                    timer -= timeBetweenGold;
                    energy--;
                    mine.TryGetGold();
                    gold++;
                }
            });

            behaviour.SetTransitionBehavior(() =>
            {
                if (gold >= maxGold)
                {
                    OnFlag.Invoke(MinerFlags.OnGoingToCenter);
                }

                if (!mine.hasGold)
                {
                    OnFlag.Invoke(MinerFlags.OnGoingToMine);
                }

                if (energy <= 0)
                {
                    OnFlag.Invoke(MinerFlags.OnEmptyEnergy);
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            Mine mine = parameters[0] as Mine;
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }

    public class TravelState : State
    {
        private int pathCounter = 0;

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            Transform OwnerTransform = parameters[0] as Transform;
            List<Node<Vector2>> path = parameters[1] as List<Node<Vector2>>;
            Vector3 direction = (Vector3)parameters[2];
            float speed = Convert.ToSingle(parameters[3]);
            float distanceToNode = Convert.ToSingle(parameters[4]);
            BehaviourActions behaviour = new BehaviourActions();


            Vector3 ownerTransformPosition = OwnerTransform.position;
            Vector3 actualTargetPosition = path[pathCounter].GetCoordinate();
            behaviour.AddMultiThreadBehaviour(0, (() =>
            {
                if (Vector3.Distance(ownerTransformPosition, actualTargetPosition) < distanceToNode)
                {
                    pathCounter++;
                }
            }));
            behaviour.AddMainThreadBehaviour(1, () =>
                {
                    OwnerTransform.forward = Vector3.Lerp(OwnerTransform.forward, direction, 1);
                    OwnerTransform.position += OwnerTransform.forward * speed * Time.deltaTime;
                }
            );
            behaviour.SetTransitionBehavior(() =>
            {
                if (Vector3.Distance(ownerTransformPosition, path[^1].GetCoordinate()) < distanceToNode)
                {
                   OnFlag.Invoke(MinerFlags.OnStartMining);
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            pathCounter = 0;
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
        }
    }
}