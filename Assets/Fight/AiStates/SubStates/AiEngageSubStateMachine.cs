using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiEngageSubStateMachine
{
    Dog dog;
    Dictionary<AiEngageSubStateId, AiEngageSubState> states;
    AiEngageSubState currentState;

    // float stateDuration = Random.Range(2f, 5f);
    // float timer = 0f;
    bool isStopped; // Track if the sub-state machine is stopped
    private bool isThinking = false;
    private float thinkingDelay = 0f;

    public AiEngageSubStateMachine(Dog dog)
    {
        this.dog = dog;
        states = new Dictionary<AiEngageSubStateId, AiEngageSubState>
        {
            { AiEngageSubStateId.Follow, new AiFollowSubState() },
            { AiEngageSubStateId.FlankLeft, new AiFlankSubState(false) },
            { AiEngageSubStateId.FlankRight, new AiFlankSubState(true) },
        };
        dog.npc.OnTaskComplete += OnTaskCompleteRecieved;
        isStopped = false;
        ChangeState(AiEngageSubStateId.Follow); // Start in Follow state
    }

    public void Update()
    {
        if (isStopped || !dog.targeting.HasTarget)
        {
            Stop();
            return;
        }

        // timer += Time.deltaTime;

        // if (timer >= stateDuration)
        // {
        //     ChangeRandomState();
        // }

        if (isThinking) 
        {
            thinkingDelay -= Time.deltaTime;
            if (thinkingDelay <= 0) 
            {
                isThinking = false;
                ChangeState(GetRandomState());
            }
            return; // Don't run the current state's update while "thinking"
        }

        currentState?.Update(dog);
    }

    public void ChangeState(AiEngageSubStateId newState)
    {
        currentState?.Exit(dog);
        currentState = states[newState];
        currentState.Enter(dog);
        // timer = 0f;
    }


    AiEngageSubStateId GetRandomState()
    {
        // AiEngageSubStateId[] choices = new AiEngageSubStateId[]
        // {
        //     AiEngageSubStateId.Follow,
        //     AiEngageSubStateId.FlankLeft,
        //     AiEngageSubStateId.FlankRight
        // };
        // return choices[Random.Range(0, choices.Length)];
        float roll = Random.value; // 0.0 to 1.0
        if (roll < 0.6f) return AiEngageSubStateId.Follow;    // 60% chance to stay/follow
        if (roll < 0.8f) return AiEngageSubStateId.FlankLeft; // 20% chance
        return AiEngageSubStateId.FlankRight;                // 20% chance
    }

    void OnTaskCompleteRecieved(){
        isThinking = true;
        thinkingDelay = Random.Range(0.5f, 2f);
    }

    public void Stop()
    {
        if (!isStopped && currentState != null)
        {
            currentState.Exit(dog);
            currentState = null;
            isStopped = true;
        }
    }
}