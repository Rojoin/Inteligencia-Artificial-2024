using System;
using System.Collections.Generic;

public class FSM
{
    private int stateCount = 0;
    public int currentState = 0;
    private Dictionary<int, State> behaviours;
    private Dictionary<int, Func<object[]>> behaviourOnTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviours = new Dictionary<int, State>();
        transitions = new int[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASSIGNED_TRASNSITION;
            }
        }

        behaviourOnTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void Transition(int flag)
    {
        if (transitions[currentState, flag] != UNNASSIGNED_TRASNSITION)
        {
            foreach (Action behaviour in behaviours[currentState]
                         .GetExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
            currentState = transitions[currentState, flag];
            foreach (Action behaviour in behaviours[currentState]
                         .GetEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTickParametes = null,
        Func<object[]> onEnterParametes = null,
        Func<object[]> onExitParametes = null) where T : State, new()
    {
        if (!behaviours.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            behaviours.Add(stateIndex, newBehaviour);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParametes);
            behaviourOnTickParameters.Add(stateIndex, onTickParametes);
            behaviourOnExitParameters.Add(stateIndex, onExitParametes);
        }
    }

    public void SetTransition(int originState, int flag, int destinationState)
    {
        transitions[originState, flag] = destinationState;
    }

    public void Tick()
    {
        if (behaviours.ContainsKey(currentState))
        {
            foreach (Action behaviour in behaviours[currentState]
                         .GetEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
            // behaviour[currentState].Perform(behaviourOnTickParameters[currentState]?.Invoke());
        }
    }
}