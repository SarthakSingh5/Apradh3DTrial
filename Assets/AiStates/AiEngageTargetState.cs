using UnityEngine;

public class AiEngageTargetState : AiState
{
    private AiEngageSubStateMachine subFSM;

    public AiStateId GetId() => AiStateId.EngageTarget;

    public void Enter(AiAgent agent)
    {
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

        // Update the sub-state machine (Follow, FlankLeft, FlankRight)
        subFSM.Update();
    }
}

