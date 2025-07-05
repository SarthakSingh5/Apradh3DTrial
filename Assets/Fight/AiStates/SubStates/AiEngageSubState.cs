using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AiEngageSubStateId
{
    Follow,
    FlankLeft,
    FlankRight
}

public interface AiEngageSubState
{
    void Enter(Dog dog);
    void Update(Dog dog);
    void Exit(Dog dog);
}

