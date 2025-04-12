using UnityEngine;

public class AiTakeCoverState : AiState
{
    public AiStateId GetId() { return AiStateId.TakeCover; }

    public void Enter(AiAgent agent)
    {
        
    }

    public void Update(AiAgent agent)
    {
        // Hiding logic is handled by coroutine in EnemyMovement
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
