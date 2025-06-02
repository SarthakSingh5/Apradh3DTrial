using UnityEngine;

public class AiEngageTargetState : AiState
{
    private AiEngageSubStateMachine subFSM;

    public AiStateId GetId() => AiStateId.EngageTarget;

    public void Enter(AiAgent agent)
    {
        agent.SetAim(true);
        // Initialize the sub-state machine when entering the state
        subFSM = new AiEngageSubStateMachine(agent);
        
    }

    public void Exit(AiAgent agent)
    {
        // Clean up when exiting the state
        subFSM.Stop();
    }

    public void Update(AiAgent agent)
    {
        // If no target, change to FindTarget state
        if (!agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
        else if(agent.targeting.TargetInSight && agent.coverMovement.HasAnyCover(agent.targeting.TargetPosition))
        {
            // If the target is in sight, change to Cover state
            agent.stateMachine.ChangeState(AiStateId.Cover);
        }

        // Update the sub-state machine (Follow, FlankLeft, FlankRight)
        subFSM.Update();
    }
}

