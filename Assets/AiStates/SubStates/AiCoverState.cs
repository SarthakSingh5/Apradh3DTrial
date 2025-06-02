using UnityEngine;
public class AiCoverState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Cover;
    }
    public void Enter(AiAgent agent)
    {
        agent.coverMovement.StartHiding(agent);
    }

    public void Update(AiAgent agent)
    {
        // Check if the AI is peeking and does not have TargetInsight
        if (agent.coverMovement.isPeeking && !agent.targeting.TargetInSight)
        {
            agent.stateMachine.ChangeState(AiStateId.EngageTarget);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.coverMovement.StopHiding(agent);
        Debug.Log("Exiting Cover State at " + Time.time);
    }

    
}
