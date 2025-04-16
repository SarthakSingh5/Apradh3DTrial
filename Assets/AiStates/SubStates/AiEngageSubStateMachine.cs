using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiEngageSubStateMachine
{
    AiAgent agent;
    Dictionary<AiEngageSubStateId, AiEngageSubState> states;
    AiEngageSubState currentState;

    float stateDuration = 3f; // Time before switching to another state (3 seconds)
    float timer;

    public AiEngageSubStateMachine(AiAgent agent)
    {
        this.agent = agent;

        // Register the sub-states in the dictionary
        states = new Dictionary<AiEngageSubStateId, AiEngageSubState>
        {
            { AiEngageSubStateId.Follow, new AiFollowSubState() },
            { AiEngageSubStateId.FlankLeft, new AiFlankSubState(false) },
            { AiEngageSubStateId.FlankRight, new AiFlankSubState(true) },
        };

        // Start with a random state
        ChangeState(GetRandomState());
    }

    public void Update()
    {
        // Call the Update method for the current state
        currentState?.Update(agent);

        // Timer logic: switch states after the specified duration
        timer += Time.deltaTime;

        if (timer >= stateDuration)
        {
            ChangeState(GetRandomState());
        }
    }

    public void ChangeState(AiEngageSubStateId newState)
    {
        // Exit the current state and enter the new one
        currentState?.Exit(agent);
        currentState = states[newState];
        currentState.Enter(agent);
        
        // Reset the timer for the next state
        timer = 0f;
    }

    AiEngageSubStateId GetRandomState()
    {
        // Choose a random sub-state (Follow, FlankLeft, or FlankRight)
        AiEngageSubStateId[] choices = new AiEngageSubStateId[]
        {
            AiEngageSubStateId.Follow,
            AiEngageSubStateId.FlankLeft,
            AiEngageSubStateId.FlankRight
        };

        return choices[Random.Range(0, choices.Length)];
    }

    public void Stop()
    {
        // Clean up when stopping the state machine
        currentState?.Exit(agent);
    }
}

