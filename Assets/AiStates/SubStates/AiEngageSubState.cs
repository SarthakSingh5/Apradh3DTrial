using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AiEngageSubStateId
{
    Follow,
    FlankLeft,
    FlankRight,
    Cover
}

public interface AiEngageSubState
{
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}

