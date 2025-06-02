using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiEngageSubStateMachine
{
    AiAgent agent;
    Dictionary<AiEngageSubStateId, AiEngageSubState> states;
    AiEngageSubState currentState;

    float stateDuration = 3f;
    float timer;
    bool isStopped; // Track if the sub-state machine is stopped

    public AiEngageSubStateMachine(AiAgent agent)
    {
        this.agent = agent;
        states = new Dictionary<AiEngageSubStateId, AiEngageSubState>
        {
            { AiEngageSubStateId.Follow, new AiFollowSubState() },
            { AiEngageSubStateId.FlankLeft, new AiFlankSubState(false) },
            { AiEngageSubStateId.FlankRight, new AiFlankSubState(true) },
        };
        isStopped = false;
        ChangeState(AiEngageSubStateId.Follow); // Start in Follow state
    }

    public void Update()
    {
        if (isStopped || !agent.targeting.HasTarget)
        {
            Stop();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= stateDuration)
        {
            ChangeState(GetRandomState());
        }

        currentState?.Update(agent);
    }

    public void ChangeState(AiEngageSubStateId newState)
    {
        currentState?.Exit(agent);
        currentState = states[newState];
        currentState.Enter(agent);
        timer = 0f;
    }

    AiEngageSubStateId GetRandomState()
    {
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
        if (!isStopped && currentState != null)
        {
            currentState.Exit(agent);
            Debug.Log("Stopping SubState: " + currentState + " at " + Time.time);
            currentState = null;
            isStopped = true;
        }
    }
}