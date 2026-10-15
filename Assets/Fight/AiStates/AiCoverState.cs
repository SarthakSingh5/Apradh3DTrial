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
        if (dog.coverMovement.failedToFindCover)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
            return;
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