using UnityEngine;

public class AiCoverState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Cover;
    }

    public void Enter(Dog dog)
    {
        dog.coverMovement.StartHiding(dog);
    }

    public void Update(Dog dog)
    {
        // ESCAPE HATCH: If no cover is safe, instantly go back to fighting/flanking
        if (dog.coverMovement.failedToFindCover)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
            return;
        }

        // Check if the AI is peeking and does not have TargetInsight
        if (dog.coverMovement.isPeeking && !dog.targeting.TargetInSight)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
        }
    }

    public void Exit(Dog dog)
    {
        dog.coverMovement.StopHiding(dog);
    }
}