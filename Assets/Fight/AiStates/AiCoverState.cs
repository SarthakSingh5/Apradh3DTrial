using UnityEngine;

public class AiCoverState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Cover;
    }

    public void Enter(Dog dog)
    {
        Debug.Log("Entering Cover State");
        dog.coverMovement.StartHiding(dog);
    }

    public void Update(Dog dog)
    {
        // ESCAPE HATCH: Only leave cover if the player flanked us and broke the math, 
        // OR if the wall itself is no longer safe.

        // 1. TOP-LEVEL FAIL-SAFE: Target died or memory expired while we were in cover!
        if (!dog.targeting.HasTarget)
        {
            dog.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }
        
        if (dog.coverMovement.failedToFindCover)
        {
            if(dog.targeting.HasTarget && dog.targeting.TargetInSight){
                dog.stateMachine.ChangeState(AiStateId.EngageTarget);
                return;
            }
            else if(dog.targeting.HasTarget && !dog.targeting.TargetInSight){
                dog.stateMachine.ChangeState(AiStateId.Investigate);
                return;
            }
            else{
                dog.stateMachine.ChangeState(AiStateId.FindTarget);
                return;
            }
        }

        // We DO NOT check for TargetInSight here anymore. 
        // The NPC is behind a wall, so it naturally won't have line of sight 
        // until the Peek coroutine physically pushes it around the corner.
    }

    public void Exit(Dog dog)
    {
        Debug.Log("Exiting Cover State");
        dog.coverMovement.StopHiding(dog);
    }
}