using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AiStateId
{
    ChasePlayer,
    Idle,
    FindTarget,
    AttackTarget,
    TakeCover

}

public interface AiState
{
    AiStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
    
    

    
}
