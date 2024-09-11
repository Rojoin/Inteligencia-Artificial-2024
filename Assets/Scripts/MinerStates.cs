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
        OnWaitingOnCenter,
        OnAlarmSound,
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
            Action<int> energy = parameters[0] as Action<int>;
            var place = parameters[1] as IPlace;

            BehaviourActions behaviour = new BehaviourActions();
            behaviour.SetTransitionBehavior(() =>
            {
                if (place is Mine mine1)
                {
                    mine = mine1;
                    if (mine.TryGetFood())
                    {
                        int energyToGet = 3;
                        energy.Invoke(energyToGet);
                        OnFlag.Invoke(MinerFlags.OnStartMining);
                    }
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            // int energy = (int)parameters[0];
            // var place = parameters[1] as IPlace;
            //
            // BehaviourActions behaviour = new BehaviourActions();
            // behaviour.SetTransitionBehavior(() =>
            // {
            //     if (place is Mine mine1)
            //     {
            //         mine = mine1;
            //         if (mine.TryGetFood())
            //         {
            //             OnFlag.Invoke(MinerFlags.OnStartMining);
            //         }
            //     }
            // });
            // return behaviour;
            return default;
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
            Action<int> setGold = parameters[5] as Action<int>;
            Action<int> setEnergy = parameters[6] as Action<int>;


            BehaviourActions behaviour = new BehaviourActions();
            behaviour.AddMultiThreadBehaviour(0, () =>
            {
                timer += deltaTime;
                if (timer > timeBetweenGold && energy > 0 && mine.hasGold)
                {
                    timer -= timeBetweenGold;
                    energy--;
                    setEnergy.Invoke(energy);
                    mine.TryGetGold();
                    gold++;
                    setGold.Invoke(gold);
                }
            });

            behaviour.SetTransitionBehavior(() =>
            {
                if (gold >= maxGold)
                {
                    OnFlag.Invoke(MinerFlags.OnGoingToCenter);
                }
                else if (!mine.hasGold)
                {
                    OnFlag.Invoke(MinerFlags.OnGoingToMine);
                }
                else if (energy <= 0)
                {
                    OnFlag.Invoke(MinerFlags.OnEmptyEnergy);
                }
            });
            return behaviour;
        }

        public override BehaviourActions GetEnterBehaviours(params object[] parameters)
        {
            mine = parameters[0] as Mine;
            return default;
        }

        public override BehaviourActions GetExitBehaviours(params object[] parameters)
        {
            return default;
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
            actualTargetPosition.z = ownerTransformPosition.z;
            behaviour.AddMultiThreadBehaviour(0, CalculateDistance());


            behaviour.AddMainThreadBehaviour(1, Move
            );
            behaviour.SetTransitionBehavior(() =>
            {
                if (Vector3.Distance(ownerTransformPosition, path[^1].GetCoordinate()) < distanceToNode)
                {
                    if (path[^1].GetPlace() is Mine)
                    {
                        OnFlag.Invoke(MinerFlags.OnStartMining);
                    }

//Todo: Make center work
                    if (path[^1].GetPlace() is HumanCenter)
                    {
                        OnFlag.Invoke(MinerFlags.OnWaitingOnCenter);
                    }
                }
            });
            return behaviour;

            Action CalculateDistance()
            {
                return () =>
                {
                    if (Vector3.Distance(ownerTransformPosition, actualTargetPosition) < distanceToNode && pathCounter+1 < path.Count)
                    {
                        pathCounter++;
                    }
                };
            }

            void Move()
            {
                Vector3 dir = actualTargetPosition - ownerTransformPosition;
                dir.Normalize();
                OwnerTransform.right = dir;
                OwnerTransform.position += dir * speed * Time.deltaTime;
            }
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