using UnityEngine;

public class AiEngageTargetState : AiState
{
    private AiEngageSubStateMachine subFSM;

    public AiStateId GetId() => AiStateId.EngageTarget;

    public void Enter(Dog dog)
    {
        dog.npc.SetAim(true);
        // Initialize the sub-state machine when entering the state
        subFSM = new AiEngageSubStateMachine(dog);
        
    }

    public void Exit(Dog dog)
    {
        // Clean up when exiting the state
        subFSM.Stop();
    }

    public void Update(Dog dog)
    {
        // If no target, change to FindTarget state
        if (!dog.targeting.HasTarget)
        {
            dog.stateMachine.ChangeState(AiStateId.FindTarget);
        }
        else if(dog.targeting.TargetInSight && dog.coverMovement.HasAnyCover(dog.targeting.TargetPosition))
        {
            // If the target is in sight, change to Cover state
            dog.stateMachine.ChangeState(AiStateId.Cover);
        }

        // Update the sub-state machine (Follow, FlankLeft, FlankRight)
        subFSM.Update();
    }
}

