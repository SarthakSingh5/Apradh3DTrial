using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiStateMachine
{
    public AiState[] states;
    public Dog dog;
    public AiStateId currentState;

    public AiStateMachine(Dog dog)
    {
        this.dog = dog;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        states = new AiState[numStates];
    }

    public void RegisterState(AiState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AiState GetState(AiStateId stateId)
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(dog);
    }

    public void ChangeState(AiStateId newState)
    {
        GetState(currentState)?.Exit(dog);
        currentState = newState;
        GetState(currentState)?.Enter(dog);
    }

}

