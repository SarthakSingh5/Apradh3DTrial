using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AiStateId
{
    ChasePlayer,
    Idle,
    FindTarget,
    AttackTarget,
    EngageTarget,
    Cover

}

public interface AiState
{
    AiStateId GetId();
    void Enter(Dog dog);
    void Update(Dog dog);
    void Exit(Dog dog);
    
    

    
}
